using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
#if !SILVERLIGHT
using System.Linq;
#endif
using System.Text.RegularExpressions;
using System.Threading;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core
{
    /// <summary>
    /// Class for sending and receiving data from a teamspeak query port using tcp
    /// </summary>
    public class SyncTcpDispatcher : TcpDispatcherBase
    {
        #region Non Public Members

        private readonly object _sendMessageLockObject = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of SyncTcpDispatcher
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        public SyncTcpDispatcher(string host, ushort? port) : base(host?? "localhost", port ?? 11001)
        {

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

            #if !SILVERLIGHT
                Trace.WriteLine("Starting to connect to: " + Host);
            #endif

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveBufferSize = RECEIVE_BUFFER_SIZE };

            IPAddress ipV4;

            if (!IPAddress.TryParse(Host, out ipV4))
            {
            #if SILVERLIGHT
                RemoteEndPoint = new DnsEndPoint(Host, Port);
            #else
                IPHostEntry hostEntry = Dns.GetHostEntry(Host);

                if (hostEntry.AddressList.Length == 0)
                    throw new InvalidOperationException(string.Format("Could not resolve host: {0}", Host));

                ipV4 = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                if (ipV4 == null)
                    throw new InvalidOperationException("Could not find a network device with an ip-v4-address.");

                RemoteEndPoint = new IPEndPoint(ipV4, Port);
            #endif
            }
            else
                RemoteEndPoint = new IPEndPoint(ipV4, Port);

            SocketAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = RemoteEndPoint, UserToken = new SocketAsyncEventArgsUserToken { Socket = Socket } };
            ManualResetEvent connectLock = new ManualResetEvent(false);
            SocketError result = SocketError.Success;
            EventHandler<SocketAsyncEventArgs> connectCallback = (sender, args) => { result = args.SocketError; connectLock.Set(); };
            Socket.InvokeAsyncMethod(Socket.ConnectAsync, connectCallback, SocketAsyncEventArgs);
            connectLock.WaitOne();
            SocketAsyncEventArgs.Completed -= connectCallback;

            if (result != SocketError.Success)
            {
                Disconnect();
                throw new SocketException((int) result);
            }

            string greeting = string.Empty;

            while (true)
            {
                KeyValuePair<SocketError, string> receiveResult = ReceiveMessage(SocketAsyncEventArgs);

                if (receiveResult.Key != SocketError.Success)
                {
                    Disconnect();
                    throw new SocketException((int) receiveResult.Key);
                }

                greeting = string.Concat(greeting, receiveResult.Value);
                QueryType? queryType = GetQueryTypeFromGreeting(greeting);

                if (!queryType.HasValue)
                {
                    GreetingFailed();
                    return;
                }

                switch (queryType.Value)
                {
                    case QueryType.Client:
                        HandleClientQueryGreeting(greeting);
                        break;
                    case QueryType.Server:
                        HandleServerQueryGreeting(greeting);
                        break;
                    default:
                        throw new InvalidOperationException("Forgott to implement query type: " + queryType);
                }

                break;
            }
        }

        private void HandleClientQueryGreeting(string greeting)
        {
            string[] greetingLines = greeting.Split(new[] { Ts3Util.QUERY_LINE_BREAK }, StringSplitOptions.RemoveEmptyEntries);
            const string PATTERN = @"selected schandlerid=(?<id>\d+)";

            Match match = greetingLines.Select(l => Regex.Match(l, PATTERN, RegexOptions.IgnoreCase | RegexOptions.Singleline)).FirstOrDefault(m => m.Success);

            if (match?.Success == true)
                LastServerConnectionHandlerId = Convert.ToInt32(match.Groups["id"].Value);
        }

        private static void HandleServerQueryGreeting(string greeting)
        {
            string lastGreetingLine = greeting.Split(new[] { Ts3Util.QUERY_LINE_BREAK }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            if (lastGreetingLine == null || !lastGreetingLine.Contains("="))
                return;

            SimpleResponse response = SimpleResponse.Parse(lastGreetingLine);

            if (response.IsBanned)
                throw new InvalidOperationException("You are banned from the server: " + response.BanExtraMessage);
        }

        private void GreetingFailed()
        {
            Disconnect();

            #if !SILVERLIGHT
                Trace.WriteLine("Greeting was wrong! ");
            #endif

            throw new SocketException((int)SocketError.ProtocolNotSupported);
        }

        /// <summary>
        /// Disconnects from the connected socket
        /// </summary>
        /// <returns>Returns true if the disconnect was done or false when the socket was already disconnected</returns>
        public bool Disconnect()
        {
            if (SocketAsyncEventArgs == null)
                return false;

            if (SocketAsyncEventArgs.SocketError != SocketError.Success)
            {
                Socket.Close();
                Socket = null;

                SocketAsyncEventArgs.Dispose();
                SocketAsyncEventArgs = null;
                return false;
            }

            try
            {
                Socket.Close();
                Socket = null;

                SocketAsyncEventArgs.Dispose();
                SocketAsyncEventArgs = null;
            }
            catch (ObjectDisposedException)
            {

            }

            return true;
        }

        #endregion

        #region Non Public Methods

        private string Send(string messageToSend)
        {
            lock (_sendMessageLockObject)
            {
                using (SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = RemoteEndPoint, UserToken = new SocketAsyncEventArgsUserToken { Socket = Socket } })
                {

                    byte[] messageBytes = Encoding.UTF8.GetBytes(messageToSend);
                    socketAsyncEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);

                    SocketError resultError = SocketError.Success;
                    ManualResetEvent sendLock = new ManualResetEvent(false);
                    EventHandler<SocketAsyncEventArgs> sendCallback = (sender, args) => { resultError = args.SocketError; sendLock.Set(); };

                    Socket.InvokeAsyncMethod(Socket.SendAsync, sendCallback, socketAsyncEventArgs);
                    sendLock.WaitOne();
                    socketAsyncEventArgs.Completed -= sendCallback;

                    if (resultError != SocketError.Success)
                    {
                        Disconnect();
                        throw new SocketException((int)resultError);
                    }

                    StringBuilder receivedMessage = new StringBuilder();

                    do
                    {
                        KeyValuePair<SocketError, string> receiveResult = ReceiveMessage(socketAsyncEventArgs);
                        receivedMessage.Append(receiveResult.Value);
                        resultError = receiveResult.Key;
                    }
                    while (resultError == SocketError.Success && !NoMoreReads(receivedMessage.ToString()));

                    if (resultError != SocketError.Success)
                    {
                        Disconnect();
                        throw new SocketException((int)resultError);
                    }

                    return receivedMessage.ToString();
                }
            }
        }

        private KeyValuePair<SocketError, string> ReceiveMessage(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Disconnect();
                return new KeyValuePair<SocketError, string>(e.SocketError, null);
            }

            SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)e.UserToken;
            byte[] sizeBuffer = new byte[RECEIVE_BUFFER_SIZE];
            e.SetBuffer(sizeBuffer, 0, sizeBuffer.Length);
            SocketError resultError = SocketError.Success;

            ManualResetEvent receiveLock = new ManualResetEvent(false);
            string resultMessage = null;
            EventHandler<SocketAsyncEventArgs> receiveCallback = (sender, args) =>
                                                                 {
                                                                     if (args.SocketError != SocketError.Success)
                                                                     {
                                                                         resultError = args.SocketError;
                                                                         receiveLock.Set();
                                                                         return;
                                                                     }

                                                                     byte[] buffer = new byte[e.BytesTransferred];
                                                                     Array.Copy(e.Buffer, e.Offset, buffer, 0, e.BytesTransferred);
                                                                     resultMessage = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                                                                     receiveLock.Set();

                                                                     if (resultMessage.Length == 0)
                                                                         Disconnect();
                                                                 };

            userToken.Socket.InvokeAsyncMethod(userToken.Socket.ReceiveAsync, receiveCallback, e);
            receiveLock.WaitOne();
            e.Completed -= receiveCallback;

            return new KeyValuePair<SocketError, string>(resultError, resultMessage);
        }

        private static bool NoMoreReads(string responseText)
        {
            if (responseText == null)
                throw new ArgumentNullException("responseText");

            return responseText.Length == 0 || ContainsStatusLine(responseText);
        }

        protected override void DisposeInternal()
        {
            Disconnect();
        }

        protected override string DispatchInternal(string commandText)
        {
            Connect();
            return Send(string.Concat(commandText, "\n"));
        }

        #endregion
    }
}