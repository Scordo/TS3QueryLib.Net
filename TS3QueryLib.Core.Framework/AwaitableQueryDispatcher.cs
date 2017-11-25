using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core
{
    public class AwaitableQueryDispatcher : IQueryDispatcher
    {
        #region Events

        /// <summary>
        /// Raised when the connection to the server was closed
        /// </summary>
        public event EventHandler<EventArgs<string>> ConnectionClosed;

        public bool IsDisposed { get; private set; }
        public int? LastServerConnectionHandlerId { get; private set; }

        /// <summary>
        /// Raised when a notification was received
        /// </summary>
        public event EventHandler<EventArgs<string>> NotificationReceived;

        /// <summary>
        /// Raised when a ban was detected
        /// </summary>
        public event EventHandler<EventArgs<SimpleResponse>> BanDetected;

        #endregion

        #region Properties

        public string Host { get; }
        public int Port { get; }
        protected TimeSpan? KeepAliveInterval { get; }
        public bool Connected { get; protected set; }

        private List<string> ReceivedLines { get; } = new List<string>();
        private bool AtLeastOneResponseReceived { get; set; }
        private ConcurrentQueue<string> MessageResponses { get; } = new ConcurrentQueue<string>();
        private Task ReadLoopTask { get; set; }
        private Task KeepAliveTask { get; set; }

        private TcpClient Client { get; set; }
        private StreamReader ClientReader { get; set; }
        private StreamWriter ClientWriter { get; set; }
        private NetworkStream ClientStream { get; set; }
        protected SynchronizationContext SyncContext { get; set; }
        private SemaphoreSlim SendLock { get; } = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the current class
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="keepAliveInterval">The keep alive interval used to send heart beats in a specific interval to the server to not get timed out (disconnected)</param>
        /// <param name="synchronizationContext">The synchronization context on which to raise events.</param>
        public AwaitableQueryDispatcher(string host = null, ushort? port = null, TimeSpan? keepAliveInterval = null, SynchronizationContext synchronizationContext = null)
        {
            Host = host ?? "localhost";
            Port = port ?? 10011;
            KeepAliveInterval = keepAliveInterval;
            SyncContext = synchronizationContext ?? SynchronizationContext.Current;
        }
        
        #endregion

        #region Public Methods

        public ConnectResponse Connect()
        {
            return AsyncHelper.RunSync(ConnectAsync);
        }

        public async Task<ConnectResponse> ConnectAsync()
        {
            if (Client != null)
                return new ConnectResponse(message: "Already connected!");

            Client = new TcpClient();
            await Client.ConnectAsync(Host, Port).ConfigureAwait(false);

            if (!Client.Connected)
                throw new IOException($"Could not connect to {Host} on port {Port}.");

            ReceivedLines.Clear();
            AtLeastOneResponseReceived = false;
            ClientStream = Client.GetStream();
            ClientReader = new StreamReader(ClientStream);
            ClientWriter = new StreamWriter(ClientStream) { NewLine = "\n" };

            string message = await ReadLineAsync().ConfigureAwait(false);

            QueryType queryType;

            if (message.StartsWith("TS3", StringComparison.OrdinalIgnoreCase))
            {
                queryType = QueryType.Server;
            }
            else if (message.StartsWith("TS3 Client", StringComparison.OrdinalIgnoreCase))
            {
                queryType = QueryType.Client;
            }
            else
            {
                string statusMessage = $"Invalid greeting received: {message}";
                DisconnectForced(statusMessage);
                return new ConnectResponse(statusMessage);
            }

            Connected = true;
            ReadLoopTask = Task.Factory.StartNew(ReadLoop, TaskCreationOptions.LongRunning);
            KeepAliveTask = Task.Factory.StartNew(KeepAliveLoop, TaskCreationOptions.LongRunning);
            return new ConnectResponse(message, queryType, true);
        }

        public string Send(string messageToSend)
        {
            return AsyncHelper.RunSync(() => SendAsync(messageToSend));
        }

        public async Task<string> SendAsync(string messageToSend)
        {
            await SendLock.WaitAsync();

            try
            {
                await SendAsync(ClientWriter, messageToSend);

                do
                {
                    if (MessageResponses.TryDequeue(out var result))
                        return result;

                    await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
                } while (Connected);

                return null;
            }
            finally
            {
                SendLock.Release();
            }
        }

        protected static async Task SendAsync(StreamWriter writer, string messageToSend)
        {
            ConfiguredTaskAwaitable? writeLineAwaitable = writer?.WriteLineAsync(messageToSend).ConfigureAwait(false);

            if (writeLineAwaitable.HasValue)
                await writeLineAwaitable.Value;

            ConfiguredTaskAwaitable? flushAwaitable = writer?.FlushAsync().ConfigureAwait(false);
            if (flushAwaitable.HasValue)
                await flushAwaitable.Value;
        }

        public void Disconnect()
        {
            DisconnectForced();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string Dispatch(string commandText)
        {
            return Send(commandText);
        }

        #endregion

        #region Non Public Methods

        protected async void KeepAliveLoop()
        {
            while (Client != null && KeepAliveInterval.HasValue)
            {
                await Task.Delay(KeepAliveInterval.Value);
                await SendAsync("version");
            }
        }

        protected async void ReadLoop()
        {
            while (Client != null && Client.Connected)
            {
                string message = await ReadLineAsync(false).ConfigureAwait(false);

                if (message == null)
                    continue;

                if (message.StartsWith("error", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!AtLeastOneResponseReceived)
                    {
                        AtLeastOneResponseReceived = true;
                        // Remove welcome messages after connect
                        ReceivedLines.Clear();
                    }

                    string responseText = string.Join("\n\r", ReceivedLines.Concat(new[] { message }));
                    MessageResponses.Enqueue(responseText);
                    ReceivedLines.Clear();

                    SimpleResponse response = SimpleResponse.Parse(responseText);

                    if (response.IsBanned)
                    {
                        BanDetected?.Invoke(this, new EventArgs<SimpleResponse>(response));
                        DisconnectForced("Banned!");
                        return;
                    }
                }
                else if (message.StartsWith("notify", StringComparison.CurrentCultureIgnoreCase))
                {
                    ThreadPool.QueueUserWorkItem(OnNotificationReceived, message);
                }
                else
                {
                    if (!AtLeastOneResponseReceived)
                    {
                        const string LastServerConnectionHandlerIdText = "selected schandlerid=";

                        if (message.StartsWith(LastServerConnectionHandlerIdText, StringComparison.InvariantCultureIgnoreCase) && int.TryParse(message.Substring(LastServerConnectionHandlerIdText.Length).Trim(), out int handlerId))
                            LastServerConnectionHandlerId = handlerId;
                    }

                    ReceivedLines.Add(message);
                }
            }
        }

        protected async Task<string> ReadLineAsync(bool throwOnEmptyMessage = true)
        {
            ConfiguredTaskAwaitable<string>? readLineAwaitable = ClientReader?.ReadLineAsync().ConfigureAwait(false);
            string message = readLineAwaitable.HasValue ? await readLineAwaitable.Value : null;

            if (message != null)
                return message;

            DisconnectForced("Empty message received from server.");

            if (throwOnEmptyMessage)
                throw new InvalidOperationException("Received no message. Socket got disconnected.");

            return null;
        }

        protected void OnNotificationReceived(object notificationText)
        {
            if (NotificationReceived != null)
                SyncContext.PostEx(p => NotificationReceived(((object[])p)[0], new EventArgs<string>(Convert.ToString(((object[])p)[1]))), new[] { this, notificationText });
        }

        private void DisconnectForced(string reason = null)
        {
            bool clientWasConnected = Client?.Connected == true;
            ReceivedLines.Clear();
            Client?.Close();
            ClientStream?.Dispose();
            ClientReader?.Dispose();
            ClientWriter?.Dispose();

            Client = null;
            ClientStream = null;
            ClientReader = null;
            ClientWriter = null;

            Connected = false;
            ReadLoopTask = null;
            KeepAliveTask = null;

            if (clientWasConnected)
                ConnectionClosed?.Invoke(this, new EventArgs<string>(reason));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                    DisconnectForced();
            }

            IsDisposed = true;
        }
        
        #endregion

        #region Embedded Types
        
        public class ConnectResponse
        {
            public string Greeting { get; }
            public QueryType? QueryType { get; }

            public bool Success { get; }
            public string Message { get; set; }

            public ConnectResponse(string greeting = null, QueryType? queryType = null, bool success = false, string message = null)
            {
                Greeting = greeting;
                QueryType = queryType;
                Success = success;
                Message = message;
            }
        }

        #endregion
    }
}