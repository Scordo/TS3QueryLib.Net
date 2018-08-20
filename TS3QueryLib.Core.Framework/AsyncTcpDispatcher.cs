using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;

namespace TS3QueryLib.Core
{
    /// <summary>
    ///  Class for sending and receiving data from a teamspeak query port using tcp. This class also supports notifications and is using asynchronous behavior.
    /// </summary>
    public class AsyncTcpDispatcher : TcpDispatcherBase
    {
        #region Non Public members

        private string _lastCommandResponse;
        private readonly object _lastCommandResponseLock = new object();
        private readonly object _sendMessageLockObject = new object();
        private readonly object _dispatchLockObject = new object();
        private StringBuilder _receiveRepository;
        private bool _greetingReceived;

        #endregion

        #region Events

        /// <summary>
        /// Raised when the connection was reset by the server
        /// </summary>
        public event EventHandler ServerClosedConnection;
        /// <summary>
        /// Raised when an error was caused by the socket
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> SocketError;
        /// <summary>
        /// Raised when the socket connected successfully and the greeting (TS3) was received.
        /// </summary>
        public event EventHandler ReadyForSendingCommands;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the current class
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        public AsyncTcpDispatcher(string host, ushort? port) : base(host, port)
        {
            SocketError += AdvancedTcpDispatcher_SocketError;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connects to the remote socket.
        /// </summary>
        /// <returns>Returns SocketError.Success when connection was successful or the socket was already connected or returns another SocketError when something went wrong.</returns>
        public void Connect()
        {
            if (SocketAsyncEventArgs != null)
                return;

            Trace.WriteLine("Starting to connect to: " + Host);

            _receiveRepository = new StringBuilder();
            _lastCommandResponse = null;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {ReceiveBufferSize = RECEIVE_BUFFER_SIZE};

            IPAddress ipV4;

            if (!IPAddress.TryParse(Host, out ipV4))
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(Host);

                if (hostEntry.AddressList.Length == 0)
                    throw new InvalidOperationException(string.Format("Could not resolve host: {0}", Host));

                ipV4 = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                if (ipV4 == null)
                    throw new InvalidOperationException("Could not find a network device with an ip-v4-address.");

                RemoteEndPoint = new IPEndPoint(ipV4, Port);
            }
            else
                RemoteEndPoint = new IPEndPoint(ipV4, Port);

            _greetingReceived = false;
            SocketAsyncEventArgs = new SocketAsyncEventArgs {RemoteEndPoint = RemoteEndPoint, UserToken = new SocketAsyncEventArgsUserToken {Socket = Socket}};
            Socket.InvokeAsyncMethod(Socket.ConnectAsync, Client_Connected, SocketAsyncEventArgs);
        }

        /// <summary>
        /// Disconnects from the connected socket
        /// </summary>
        /// <returns>Returns true if the disconnect was done or false when the socket was already disconnected</returns>
        public bool Disconnect()
        {
            _greetingReceived = false;

            if (SocketAsyncEventArgs == null)
                return false;

            if (SocketAsyncEventArgs.SocketError != System.Net.Sockets.SocketError.Success)
            {
                Socket = null;
                SocketAsyncEventArgs.Dispose();
                SocketAsyncEventArgs = null;
                return false;
            }

            try
            {
                if (Socket != null)
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket = null;
                    SocketAsyncEventArgs.Dispose();
                    SocketAsyncEventArgs = null;
                }
            }
            catch (ObjectDisposedException)
            {

            }

            return true;
        }

        /// <summary>
        /// Detaches all event listeners.
        /// </summary>
        public override void DetachAllEventListeners()
        {
            ServerClosedConnection = null;
            SocketError = null;
            ReadyForSendingCommands = null;

            base.DetachAllEventListeners();
        }
        #endregion

        #region Non Public Methods

        private void Client_Connected(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            socketAsyncEventArgs.Completed -= Client_Connected;

            EnsureSocketSuccess(socketAsyncEventArgs, () =>
            {
                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;

                byte[] sizeBuffer = new byte[RECEIVE_BUFFER_SIZE];
                socketAsyncEventArgs.SetBuffer(sizeBuffer, 0, sizeBuffer.Length);

                userToken.Socket.InvokeAsyncMethod(userToken.Socket.ReceiveAsync, MessageReceived, socketAsyncEventArgs);
            });
        }

        private void ReceiveMessage(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            EnsureSocketSuccess(socketAsyncEventArgs, () =>
            {
                try
                {
                    SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;

                    byte[] sizeBuffer = new byte[RECEIVE_BUFFER_SIZE];
                    socketAsyncEventArgs.SetBuffer(sizeBuffer, 0, sizeBuffer.Length);

                    userToken.Socket.InvokeAsyncMethod(userToken.Socket.ReceiveAsync, MessageReceived, socketAsyncEventArgs);
                }
                catch (ObjectDisposedException)
                {

                }
            });
        }

        private void MessageReceived(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            socketAsyncEventArgs.Completed -= MessageReceived;

            EnsureSocketSuccess(socketAsyncEventArgs, () =>
            {
                string message = Encoding.UTF8.GetString(socketAsyncEventArgs.Buffer, socketAsyncEventArgs.Offset, socketAsyncEventArgs.BytesTransferred);

                if (message.Length == 0)
                {
                    Disconnect();
                    OnSocketError(System.Net.Sockets.SocketError.ConnectionReset);
                    return;
                }

                _receiveRepository.Append(message);

                if (_greetingReceived)
                {
                    while (true)
                    {
                        Match notifyMatch = NotifyResponseMatch(_receiveRepository.ToString());

                        if (notifyMatch.Success)
                        {
                            ThreadPool.QueueUserWorkItem(OnNotificationReceived, notifyMatch.Value);

                            _receiveRepository.Remove(0, notifyMatch.Length);

                            if (_receiveRepository.Length == 0)
                                break;

                            continue;
                        }

                        Match statusLineMatch = StatusLineMatch(_receiveRepository.ToString());

                        if (statusLineMatch.Success)
                        {
                            string responseText = statusLineMatch.Value;

                            // happens when there is a notification between the body and statusline of a command response --> I think this is a bug of ts3
                            if (responseText.IndexOf("\n\rnotify", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // extract the notification data and raise notification
                                notifyMatch = GetNotifyResponseMatchBetweenCommandResponse(responseText);
                                ThreadPool.QueueUserWorkItem(OnNotificationReceived, notifyMatch.Groups["event"].Value);
                                
                                // modify the response thext used for the command response
                                responseText = notifyMatch.Groups["part1"].Value + notifyMatch.Groups["part2"].Value;
                            }

                            ModifyLastCommandResponse(responseText);
                            _receiveRepository.Remove(0, statusLineMatch.Length);

                            SimpleResponse response = SimpleResponse.Parse(responseText);

                            if (response.IsBanned)
                            {
                                ThreadPool.QueueUserWorkItem(OnBanDetected, response);
                                Disconnect();
                                OnSocketError(System.Net.Sockets.SocketError.ConnectionReset);
                                return;
                            }

                            if (_receiveRepository.Length == 0)
                                break;

                            continue;
                        }

                        break;
                    }
                }
                else
                {
                    string greeting = _receiveRepository.ToString();
                    if (!IsValidGreetingPart(greeting))
                        GreetingFailed();

                    QueryType? queryType = GetQueryTypeFromGreeting(greeting);

                    if (queryType.HasValue)
                    {
                        int requiredGreetingLength = queryType == QueryType.Client ? CLIENT_GREETING.Length : SERVER_GREETING.Length;

                        if (greeting.Length >= requiredGreetingLength)
                        {
                            bool greetingCorrect;
                            switch (queryType.Value)
                            {
                                case QueryType.Client:
                                    greetingCorrect = HandleClientQueryGreeting(greeting);
                                    break;
                                case QueryType.Server:
                                    greetingCorrect = HandleServerQueryGreeting(greeting);
                                    break;
                                default:
                                    throw new InvalidOperationException("Forgott to implement query type: " + queryType);
                            }

                            if (_greetingReceived && !greetingCorrect)
                                GreetingFailed();
                        }
                    }
                }

                ReceiveMessage(socketAsyncEventArgs);
            });
        }

        private bool HandleClientQueryGreeting(string greeting)
        {
            string originalGreeting = greeting;
            if (!greeting.StartsWith(CLIENT_GREETING, StringComparison.InvariantCultureIgnoreCase))
            {
                _greetingReceived = true;
                return false;
            }

            greeting = greeting.Substring(CLIENT_GREETING.Length);

            const string PATTERN_STATIC_PART = "selected schandlerid=";
            const string PATTERN = PATTERN_STATIC_PART + @"(?<id>\d+)" + Ts3Util.QUERY_REGEX_LINE_BREAK;

            if (greeting.IndexOf(PATTERN_STATIC_PART, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                _greetingReceived = true;
                return false;
            }

            if (!greeting.Contains(Ts3Util.QUERY_LINE_BREAK))
                return false;

            _greetingReceived = true;
            Match match = Regex.Match(greeting, PATTERN, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (!match.Success)
                return false;

            LastServerConnectionHandlerId = Convert.ToInt32(match.Groups["id"].Value);
            // greeting was correct!
            _receiveRepository.Remove(0, originalGreeting.Length);
            ThreadPool.QueueUserWorkItem(x => OnReadyForSendingCommands());

            return true;
        }

        private bool HandleServerQueryGreeting(string greeting)
        {
            _greetingReceived = true;
            if (!greeting.StartsWith(SERVER_GREETING, StringComparison.InvariantCultureIgnoreCase))
                return false;

            // greeting was correct!
            _receiveRepository.Remove(0, SERVER_GREETING.Length);
            ThreadPool.QueueUserWorkItem(x => OnReadyForSendingCommands());

            return true;
        }

        private void GreetingFailed()
        {
            Trace.WriteLine("Greeting was wrong! Greeting was: " + _receiveRepository);

            OnSocketError(System.Net.Sockets.SocketError.ProtocolNotSupported);
        }

        private static Match NotifyResponseMatch(string text)
        {
            const string pattern = @"^notify.+?"+Ts3Util.QUERY_REGEX_LINE_BREAK;

            return text.StartsWith("notify", StringComparison.OrdinalIgnoreCase) ? Regex.Match(text, pattern, RegexOptions.Singleline) : Match.Empty;
        }
        private static Match GetNotifyResponseMatchBetweenCommandResponse(string text)
        {
            const string PATTERN = @"^(?<part1>.*?\x0A\x0D)(?<event>notify.+?\x0A\x0D)(?<part2>.*)$";

            return Regex.Match(text, PATTERN, RegexOptions.Singleline);
        }

        protected static Match StatusLineMatch(string responseText)
        {
            const string pattern = @"((^)|(.*?" + Ts3Util.QUERY_REGEX_LINE_BREAK + "))error id=.+?" + Ts3Util.QUERY_REGEX_LINE_BREAK;

            return responseText.IndexOf("error id=", StringComparison.OrdinalIgnoreCase) != -1 ? Regex.Match(responseText, pattern, RegexOptions.Singleline) : Match.Empty;
        }

        private void Send(string messageToSend)
        {
            lock (_sendMessageLockObject)
            {
                using (SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = RemoteEndPoint, UserToken = new SocketAsyncEventArgsUserToken { Socket = Socket } })
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(messageToSend);
                    socketAsyncEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);

                    ManualResetEvent sendLock = new ManualResetEvent(false);
                    EventHandler<SocketAsyncEventArgs> sendCallback = (sender, args) => EnsureSocketSuccess(args, () => sendLock.Set());

                    if (Socket != null)
                    {
                        Socket.InvokeAsyncMethod(Socket.SendAsync, sendCallback, socketAsyncEventArgs);
                        sendLock.WaitOne();
                        socketAsyncEventArgs.Completed -= sendCallback;
                    }
                }
            }
        }

        private void ModifyLastCommandResponse(string newValue)
        {
            lock (_lastCommandResponseLock)
            {
                _lastCommandResponse = newValue;
            }
        }

        protected void EnsureSocketSuccess(SocketAsyncEventArgs socketAsyncEventArgs, Action action)
        {
            switch (socketAsyncEventArgs.SocketError)
            {
                case System.Net.Sockets.SocketError.Success:
                    action();
                    break;
                    // more handling here later
                default:
                    OnSocketError(socketAsyncEventArgs.SocketError);
                    break;
            }
        }

        private void AdvancedTcpDispatcher_SocketError(object sender, SocketErrorEventArgs e)
        {
            _greetingReceived = false;

            if (SocketAsyncEventArgs != null)
            {
                SocketAsyncEventArgs.Dispose();
                SocketAsyncEventArgs = null;
            }

            if (e.SocketError == System.Net.Sockets.SocketError.ConnectionReset && ServerClosedConnection != null)
                ThreadPool.QueueUserWorkItem(OnServerClosedConnection);
        }

        private void OnServerClosedConnection(object state)
        {
            if (ServerClosedConnection != null)
                SyncContext.PostEx(sender => ServerClosedConnection(sender, EventArgs.Empty), this);
        }

        private void OnReadyForSendingCommands()
        {
            if (ReadyForSendingCommands != null)
                SyncContext.PostEx(sender => ReadyForSendingCommands(sender, EventArgs.Empty), this);
        }

        protected void OnSocketError(SocketError socketError)
        {
            ThreadPool.QueueUserWorkItem(OnSocketErrorInternal, socketError);
        }

        private void OnSocketErrorInternal(object state)
        {
            SocketError socketError = (SocketError) state;

            if (SocketError != null)
                SyncContext.PostEx(p => SocketError(((object[])p)[0], new SocketErrorEventArgs((SocketError)((object[])p)[1])), new object[] { this, socketError });
        }

        protected override void DisposeInternal()
        {
            Disconnect();
        }

        protected override string DispatchInternal(string commandText)
        {
            lock (_dispatchLockObject)
            {
                Send(string.Concat(commandText, "\n"));

                do
                {
                    if (_lastCommandResponse != null)
                    {
                        string lastCommandResponse = _lastCommandResponse;
                        ModifyLastCommandResponse(null);

                        return lastCommandResponse;
                    }

                    Thread.Sleep(10);
                }
                while (IsConnected);

                return null;
            }
        }

        #endregion
    }
}
