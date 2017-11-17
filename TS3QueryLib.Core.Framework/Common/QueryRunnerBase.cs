using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Common
{
    public abstract class QueryRunnerBase : IDisposable
    {
        #region Properties

        /// <summary>
        /// The dispatcher used to send commands
        /// </summary>
        public IQueryDispatcher Dispatcher { get; protected set; }
        /// <summary>
        /// Returns true if this instance was disposed
        /// </summary>
        public bool IsDisposed { get { return _disposed || Dispatcher == null || Dispatcher.IsDisposed; } }

        /// <summary>
        /// Gets or sets an optional predicate action which is executed before a command is sent. If the predicate action returns <value>true</value>, the command is sent, otherwise not.
        /// </summary>
        public Func<Command, QueryRunnerBase, bool> SendCommandValidationPredicate { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// A notification was received that is not handled in a typesafe manner
        /// </summary>
        public event EventHandler<EventArgs<string>> UnknownNotificationReceived;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Creates an instance of QueryRunner using the provided dispatcher
        /// </summary>
        /// <param name="queryDispatcher">The dispatcher used to send commands</param>
        protected QueryRunnerBase(IQueryDispatcher queryDispatcher)
        {
            if (queryDispatcher == null)
                throw new ArgumentNullException("queryDispatcher");

            Dispatcher = queryDispatcher;
            Dispatcher.NotificationReceived += Dispatcher_NotificationReceived;
        }

        ~QueryRunnerBase()
        {
            Dispose();
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Closes the ServerQuery connection to the TeamSpeak 3 Server instance.
        /// </summary>
        public SimpleResponse Quit()
        {
            return ResponseBase<SimpleResponse>.Parse(SendCommand(SharedCommandName.Quit.CreateCommand()));
        }

        /// <summary>
        /// Send the command to the undlerying socket
        /// </summary>
        /// <param name="command">The command to send</param>
        public string SendCommand(Command command)
        {
            CheckForDisposed();

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return SendCommandValidationPredicate?.Invoke(command, this) == false ? @"error id=256 msg=command\snot\ssent" : Dispatcher.Dispatch(command);
        }

        /// <summary>
        /// Send the message in plain to the socket
        /// </summary>
        /// <param name="rawCommand">The raw messag</param>
        public string SendRaw(string rawCommand)
        {
            CheckForDisposed();

            if (rawCommand == null)
                throw new ArgumentNullException("rawCommand");

            return Dispatcher.Dispatch(rawCommand);
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        private void CheckForDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("This object has alread been disposed!");
        }

        public void Dispose()
        {
            Dispatcher.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Non Public Methods

        private void Dispatcher_NotificationReceived(object sender, EventArgs<string> e)
        {
            OnUnknownNotificationReceived(e.Value);
        }

        protected void OnUnknownNotificationReceived(string notificationText)
        {
            if (UnknownNotificationReceived != null)
                UnknownNotificationReceived(this, new EventArgs<string>(notificationText));
        }

        #endregion
    }
}
