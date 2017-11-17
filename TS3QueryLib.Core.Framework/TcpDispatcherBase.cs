using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core
{
    public enum QueryType
    {
        Server,
        Client
    }

    /// <summary>
    /// Base class for tcp dispatchers
    /// </summary>
    public abstract class TcpDispatcherBase : IQueryDispatcher
    {
        #region Constants

        protected const string CLIENT_GREETING_FIRST_LINE = "TS3 Client" + Ts3Util.QUERY_LINE_BREAK;
        protected const string SERVER_GREETING_FIRST_LINE = "TS3" + Ts3Util.QUERY_LINE_BREAK;
        protected const string SERVER_GREETING = SERVER_GREETING_FIRST_LINE + "Welcome to the TeamSpeak 3 ServerQuery interface, type \"help\" for a list of commands and \"help <command>\" for information on a specific command." + Ts3Util.QUERY_LINE_BREAK;
        protected const string CLIENT_GREETING = CLIENT_GREETING_FIRST_LINE + "Welcome to the TeamSpeak 3 ClientQuery interface, type \"help\" for a list of commands and \"help <command>\" for information on a specific command." + Ts3Util.QUERY_LINE_BREAK;
        protected const int RECEIVE_BUFFER_SIZE = 4 * 1024;

        #endregion

        #region Properties

        public string Host
        {
            get;
            protected set;
        }

        public int Port
        {
            get;
            protected set;
        }

        public bool IsConnected
        {
            get { return SocketAsyncEventArgs != null; }
        }

        public SocketAsyncEventArgs SocketAsyncEventArgs
        {
            get;
            protected set;
        }

        public Socket Socket
        {
            get;
            protected set;
        }

        public EndPoint RemoteEndPoint
        {
            get;
            protected set;
        }

        public bool IsDisposed { get { return _disposed; } }
        public int? LastServerConnectionHandlerId { get; protected set; }

        protected SynchronizationContext SyncContext { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Raised when a notification was received
        /// </summary>
        public event EventHandler<EventArgs<string>> NotificationReceived;

        /// <summary>
        /// Raised when a ban was detected
        /// </summary>
        public event EventHandler<EventArgs<SimpleResponse>> BanDetected;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the current class  using localhost on port 10011
        /// </summary>
        protected TcpDispatcherBase() : this(null, null)
        {

        }

        /// <summary>
        /// Creates an instance of the current class
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="synchronizationContext">The synchronization context on which to raise events.</param>
        protected TcpDispatcherBase(string host, ushort? port, SynchronizationContext synchronizationContext = null)
        {
            Host = host ?? "localhost";
            Port = port ?? 10011;

            SyncContext = synchronizationContext ?? SynchronizationContext.Current;
        }

        #endregion

        #region Destructor

        ~TcpDispatcherBase()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Detaches all event listeners.
        /// </summary>
        public virtual void DetachAllEventListeners()
        {
            NotificationReceived = null;
            BanDetected = null;
        }

        #endregion

        #region Non Public Methods

        protected bool IsValidGreetingPart(string greetingPart)
        {
            if (greetingPart.IsNullOrTrimmedEmpty())
                return false;

            return SERVER_GREETING_FIRST_LINE.StartsWith(greetingPart, StringComparison.InvariantCultureIgnoreCase) ||
                   greetingPart.StartsWith(SERVER_GREETING_FIRST_LINE, StringComparison.InvariantCultureIgnoreCase) ||
                   CLIENT_GREETING_FIRST_LINE.StartsWith(greetingPart, StringComparison.InvariantCultureIgnoreCase) ||
                   greetingPart.StartsWith(CLIENT_GREETING_FIRST_LINE, StringComparison.InvariantCultureIgnoreCase);
        }

        protected QueryType? GetQueryTypeFromGreeting(string greetingPart)
        {
            if (greetingPart != null)
            {
                if (greetingPart.StartsWith(SERVER_GREETING_FIRST_LINE, StringComparison.InvariantCultureIgnoreCase))
                    return QueryType.Server;

                if (greetingPart.StartsWith(CLIENT_GREETING_FIRST_LINE, StringComparison.InvariantCultureIgnoreCase))
                    return QueryType.Client;
            }

            return null;
        }

        protected static bool ContainsStatusLine(string responseText)
        {
            const string pattern = @"((^)|(" + Ts3Util.QUERY_REGEX_LINE_BREAK + "))error id=.+?" + Ts3Util.QUERY_REGEX_LINE_BREAK;

            return Regex.IsMatch(responseText, pattern, RegexOptions.Singleline);
        }

        protected void OnNotificationReceived(object notificationText)
        {
            if (NotificationReceived != null)
                SyncContext.PostEx(p => NotificationReceived(((object[])p)[0], new EventArgs<string>(Convert.ToString(((object[])p)[1]))), new[] { this, notificationText });
        }

        protected void OnBanDetected(object banResponse)
        {
            if (BanDetected != null)
                SyncContext.PostEx(p => BanDetected(((object[])p)[0], new EventArgs<SimpleResponse>((SimpleResponse)((object[])p)[1])), new [] { this, banResponse });
        }

        #endregion

        #region IQueryDispatcher Members

        string IQueryDispatcher.Dispatch(string commandText)
        {
            CheckForDisposed();
            return DispatchInternal(commandText);
        }

        protected abstract string DispatchInternal(string commandText);

        #endregion

        #region IDisposable Members

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void CheckForDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("This object has alread been disposed!");
        }

        protected virtual void DisposeInternal()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    DisposeInternal();

                Socket = null;
            }

            _disposed = true;
        }

        #endregion
    }
}