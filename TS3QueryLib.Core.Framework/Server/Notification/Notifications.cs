using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Notification;
using TS3QueryLib.Core.Server.Notification.EventArgs;

namespace TS3QueryLib.Core.Server.Notification
{
    /// <summary>
    /// This class handles the notifications sent by the teamspeak-server and raises type safe events for each notification
    /// </summary>
    public class Notifications : NotificationsBase
    {
        #region Events

        /// <summary>
        /// Raised, when a client was banned from the server
        /// </summary>
        public event EventHandler<ClientBanEventArgs> ClientBan;
        /// <summary>
        /// Raised, when a client was kicked from the server
        /// </summary>
        public event EventHandler<ClientKickEventArgs> ClientKick;
        /// <summary>
        /// Raised, when a client disconnected from the server
        /// </summary>
        public event EventHandler<ClientDisconnectEventArgs> ClientDisconnect;
        /// <summary>
        /// Raised, when a client lost the connection to the server (for example when the client teamspeak app crashed)
        /// </summary>
        public event EventHandler<ClientConnectionLostEventArgs> ClientConnectionLost;
        /// <summary>
        /// Raised, when a private message was sent to the current query client.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ClientMessageReceived;
        /// <summary>
        /// Raised, when a message was sent to the channel the current query client has joined.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ChannelMessageReceived;
        /// <summary>
        /// Raised, when a message was sent to the server the current query client is connected to.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ServerMessageReceived;
        /// <summary>
        /// Raised, when a client moved himself to another channel
        /// </summary>
        public event EventHandler<ClientMovedEventArgs> ClientMovedByTemporaryChannelCreate;
        /// <summary>
        /// Raised, when a client moved himself to another channel
        /// </summary>
        public event EventHandler<ClientMovedEventArgs> ClientMoved;
        /// <summary>
        /// Raised, when a client was moved to another channel by another client
        /// </summary>
        public event EventHandler<ClientMovedByClientEventArgs> ClientMoveForced;
        /// <summary>
        /// Raised, when a client joined the server
        /// </summary>
        public event EventHandler<ClientJoinedEventArgs> ClientJoined;
        /// <summary>
        /// Raised, when a client has used a token
        /// </summary>
        public event EventHandler<TokenUsedEventArgs> TokenUsed;

        /// <summary>
        /// Raised, when a channel is edited
        /// </summary>
        public event EventHandler<ChannelEditedEventArgs> ChannelEdited;

        /// <summary>
        /// Raised, when a channel is moved
        /// </summary>
        public event EventHandler<ChannelMovedEventArgs> ChannelMoved;
        
        /// <summary>
        /// Raised, when the description of a channel has changed
        /// </summary>
        public event EventHandler<ChannelDescriptionChangedEventArgs> ChannelDescriptionChanged;

        /// <summary>
        /// Raised, when the password of a channel has changed
        /// </summary>
        public event EventHandler<ChannelPasswordChangedEventArgs> ChannelPasswordChanged;

        /// <summary>
        /// Raised, when a channel was created
        /// </summary>
        public event EventHandler<ChannelCreatedEventArgs> ChannelCreated;

        /// <summary>
        /// Raised, when a channel was deleted
        /// </summary>
        public event EventHandler<ChannelDeletedEventArgs> ChannelDeleted;

        /// <summary>
        /// Raised, when the current server was edited
        /// </summary>
        public event EventHandler<ServerEditedEventArgs> ServerEdited;

        /// <summary>
        /// Raised, when a unknown notification was received
        /// </summary>
        public event EventHandler<UnknownNotificationEventArgs> UnknownNotificationReceived;

        #endregion

        #region Constructor

        internal Notifications(QueryRunner queryRunner) : base(queryRunner)
        {

        }

        #endregion

        #region Non Public Methods

        protected override Dictionary<string, Action<CommandParameterGroupList>> GetNotificationHandlers()
        {
            return new Dictionary<string, Action<CommandParameterGroupList>>
            {
                { "notifyclientleftview", HandleClientLeave },
                { "notifytextmessage", HandleMessages },
                { "notifyclientmoved", HandleClientMove },
                { "notifycliententerview", HandleClientJoin },
                { "notifytokenused", HandleTokenUsed },
                { "notifychanneledited", HandleChannelEdited },
                { "notifychannelmoved", HandleChannelMoved },
                { "notifychanneldescriptionchanged", HandleChannelDescriptionChanged },
                { "notifychannelpasswordchanged", HandleChannelPasswordChanged },
                { "notifychannelcreated", HandleChannelCreated },
                { "notifychanneldeleted", HandleChannelDeleted },
                { "notifyserveredited", HandleServerEdited },
                { "*", HandleUnknownNotificationReceived },
            };
        }

        private void HandleTokenUsed(CommandParameterGroupList parameterGroupList)
        {
            if (TokenUsed != null)
                ThreadPool.QueueUserWorkItem(x => TokenUsed(this, new TokenUsedEventArgs(parameterGroupList)), null);
        }

        private void HandleClientLeave(CommandParameterGroupList parameterGroupList)
        {
            int? reasonId = parameterGroupList.GetParameterValue<int?>("reasonid");

            if (!reasonId.HasValue)
            {
                // do something here later ;)
                return;
            }

            switch ((ClientLeftReason)reasonId.Value)
            {
                case ClientLeftReason.Kicked:
                    if (ClientKick != null)
                        ThreadPool.QueueUserWorkItem(x => ClientKick(this, new ClientKickEventArgs(parameterGroupList)), null);
                    break;
                case ClientLeftReason.Banned:
                    if (ClientBan != null)
                        ThreadPool.QueueUserWorkItem(x => ClientBan(this, new ClientBanEventArgs(parameterGroupList)), null);
                    break;
                case ClientLeftReason.ConnectionLost:
                    if (ClientConnectionLost != null)
                        ThreadPool.QueueUserWorkItem(x => ClientConnectionLost(this, new ClientConnectionLostEventArgs(parameterGroupList)), null);
                    break;
                case ClientLeftReason.Disconnect:
                    if (ClientDisconnect != null)
                        ThreadPool.QueueUserWorkItem(x => ClientDisconnect(this, new ClientDisconnectEventArgs(parameterGroupList)), null);
                    break;
            }
        }

        private void HandleMessages(CommandParameterGroupList parameterGroupList)
        {
            MessageTarget messageTarget = (MessageTarget)parameterGroupList.GetParameterValue<uint>("targetmode");

            switch (messageTarget)
            {
                case MessageTarget.Client:
                    if (ClientMessageReceived != null)
                        ThreadPool.QueueUserWorkItem(x => ClientMessageReceived(this, new MessageReceivedEventArgs(parameterGroupList)), null);
                    break;
                case MessageTarget.Channel:
                    if (ChannelMessageReceived != null)
                        ThreadPool.QueueUserWorkItem(x => ChannelMessageReceived(this, new MessageReceivedEventArgs(parameterGroupList)), null);
                    break;
                case MessageTarget.Server:
                    if (ServerMessageReceived != null)
                        ThreadPool.QueueUserWorkItem(x => ServerMessageReceived(this, new MessageReceivedEventArgs(parameterGroupList)), null);
                    break;
            }
        }

        private void HandleClientMove(CommandParameterGroupList parameterGroupList)
        {
            int? invokerId = parameterGroupList.GetParameterValue<int?>("invokerid");

            if (!invokerId.HasValue)
            {
                if (ClientMoved != null)
                    ThreadPool.QueueUserWorkItem(x => ClientMoved(this, new ClientMovedEventArgs(parameterGroupList)), null);

                return;
            }

            if (invokerId == 0)
            {
                if (ClientMovedByTemporaryChannelCreate != null)
                    ThreadPool.QueueUserWorkItem(x => ClientMovedByTemporaryChannelCreate(this, new ClientMovedEventArgs(parameterGroupList)), null);
            }
            else
            {
                if (ClientMoveForced != null)
                    ThreadPool.QueueUserWorkItem(x => ClientMoveForced(this, new ClientMovedByClientEventArgs(parameterGroupList)), null);
            }
        }

        private void HandleClientJoin(CommandParameterGroupList parameterGroupList)
        {
            if (ClientJoined != null)
                ThreadPool.QueueUserWorkItem(x => ClientJoined(this, new ClientJoinedEventArgs(parameterGroupList)), null);
        }

        private void HandleChannelEdited(CommandParameterGroupList parameterGroupList)
        {
            if (ChannelEdited != null)
                ThreadPool.QueueUserWorkItem(x => ChannelEdited(this, new ChannelEditedEventArgs(parameterGroupList)), null);
        }

        private void HandleChannelMoved(CommandParameterGroupList parameterGroupList)
        {
            if (ChannelMoved != null)
                ThreadPool.QueueUserWorkItem(x => ChannelMoved(this, new ChannelMovedEventArgs(parameterGroupList)), null);
        }

        private void HandleChannelDescriptionChanged(CommandParameterGroupList parameterGroupList)
        {
            if (ChannelDescriptionChanged != null)
                ThreadPool.QueueUserWorkItem(x => ChannelDescriptionChanged(this, new ChannelDescriptionChangedEventArgs(parameterGroupList)), null);
        }

        private void HandleChannelPasswordChanged(CommandParameterGroupList parameterGroupList)
        {
            if (ChannelPasswordChanged != null)
                ThreadPool.QueueUserWorkItem(x => ChannelPasswordChanged(this, new ChannelPasswordChangedEventArgs(parameterGroupList)), null);
        }

        private void HandleChannelCreated(CommandParameterGroupList parameterGroupList)
        {
            if (ChannelCreated != null)
                ThreadPool.QueueUserWorkItem(x => ChannelCreated(this, new ChannelCreatedEventArgs(parameterGroupList)), null);
        }

        private void HandleChannelDeleted(CommandParameterGroupList parameterGroupList)
        {
            if (ChannelDeleted != null)
                ThreadPool.QueueUserWorkItem(x => ChannelDeleted(this, new ChannelDeletedEventArgs(parameterGroupList)), null);
        }

        private void HandleServerEdited(CommandParameterGroupList parameterGroupList)
        {
            if (ServerEdited != null)
                ThreadPool.QueueUserWorkItem(x => ServerEdited(this, new ServerEditedEventArgs(parameterGroupList)), null);
        }

        private void HandleUnknownNotificationReceived(CommandParameterGroupList parameterGroupList)
        {
            if (UnknownNotificationReceived != null)
                ThreadPool.QueueUserWorkItem(x => UnknownNotificationReceived(this, new UnknownNotificationEventArgs(parameterGroupList.First()?.First()?.Name, parameterGroupList)), null);
        }


        #endregion
    }
}