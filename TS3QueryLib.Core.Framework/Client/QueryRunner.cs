using System;
using TS3QueryLib.Core.Client.Entities;
using TS3QueryLib.Core.Client.Notification;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Client.Responses;

namespace TS3QueryLib.Core.Client
{
    public class QueryRunner : QueryRunnerBase
    {
        #region Properties

        /// <summary>
        /// Provides access to events raised by notifications
        /// </summary>
        public Notifications Notifications { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of QueryRunner using the provided dispatcher
        /// </summary>
        /// <param name="queryDispatcher">The dispatcher used to send commands</param>
        public QueryRunner(IQueryDispatcher queryDispatcher) : base(queryDispatcher)
        {
            Notifications = new Notifications(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Authenticate against the local query client
        /// </summary>
        /// <param name="apiKey">The api key used to authenticate</param>
        public SimpleResponse Authenticate(string apiKey)
        {
            Command command = CommandName.Auth.CreateCommand();

            if (apiKey == null)
                throw new ArgumentNullException("apiKey");

            command.AddParameter("apikey", apiKey);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// This command allows you to listen to events that the client encounters. Events
        /// are things like people starting or stopping to talk, people joining or leaving,
        /// new channels being created and many more. It registers for client notifications
        /// for the specified serverConnectionHandlerId. If the serverConnectionHandlerID
        /// is set to null it applies to all server connection handlers.
        /// </summary>
        /// <param name="event">The event to register for. Choose ClientNotifyRegisterEvent.Any to register for all events.</param>
        /// <param name="serverConnectionHandlerId">The id of the server connection handler.</param>
        public SimpleResponse RegisterForNotifications(ClientNotifyRegisterEvent @event, uint? serverConnectionHandlerId = null)
        {
            Command command = CommandName.ClientNotifyRegister.CreateCommand();
            command.AddParameter("event", @event.ToString().ToLower());
            command.AddParameter("schandlerid", serverConnectionHandlerId.HasValue ? serverConnectionHandlerId.Value : 0);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Sets one or more values concerning your own client, and makes them available to other clients through the server where applicable
        /// </summary>
        /// <param name="modificationInstance">The modifications as class</param>
        public SimpleResponse UpdateClient(ClientModification modificationInstance)
        {
            if (modificationInstance == null)
                throw new ArgumentNullException("modificationInstance");

            Command command = SharedCommandName.ClientUpdate.CreateCommand();
            modificationInstance.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of channels created on a virtual server including their ID, order, name, etc. The output can be modified using several command options.
        /// </summary>
        /// <param name="includeAll">if set to true all options are set to true.</param>
        /// <param name="includeTopics">if set to true topic is included</param>
        /// <param name="includeFlags">if set to true flag parameters are included</param>
        /// <param name="includeVoiceInfo">if set to true voice parameters are included</param>
        /// <param name="includeLimits">if set to true limit parameters are included</param>
        /// <param name="includeIcon">if set to true icon parameter is included</param>
        public ListResponse<ChannelListEntry> GetChannelList(bool includeAll = false, bool includeTopics = false, bool includeFlags = false, bool includeVoiceInfo = false, bool includeLimits = false, bool includeIcon = false)
        {
            Command command = SharedCommandName.ChannelList.CreateCommand();

            if (includeTopics || includeAll)
                command.AddOption("topic");

            if (includeFlags || includeAll)
                command.AddOption("flags");

            if (includeVoiceInfo || includeAll)
                command.AddOption("voice");

            if (includeLimits || includeAll)
                command.AddOption("limits");

            if (includeIcon || includeAll)
                command.AddOption("icon");

            return ListResponse<ChannelListEntry>.Parse(SendCommand(command), ChannelListEntry.Parse);
        }

        /// <summary>
        /// Selects the current server connection handler.
        /// </summary>
        public SingleValueResponse<uint> SelectServerConnectionHandler()
        {
            return SelectServerConnectionHandler(null);
        }

        /// <summary>
        /// Selects the server connection handler.
        /// </summary>
        /// <param name="serverConnectionHandlerId">The id of the server connection handler to select. If set to null, the currently active connection handler is selected.</param>
        public SingleValueResponse<uint> SelectServerConnectionHandler(uint? serverConnectionHandlerId)
        {
            Command command = SharedCommandName.Use.CreateCommand();

            if (serverConnectionHandlerId.HasValue)
                command.AddParameter("schandlerid", serverConnectionHandlerId.Value);

            return ResponseBase<SingleValueResponse<uint>>.Parse(SendCommand(command), "schandlerid");
        }

        /// <summary>
        /// Gets channel connection information for the current channel from the currently selected server connection handler.
        /// </summary>
        public ChannelConnectionInfoResponse GetChannelConnectionInfo()
        {
            return GetChannelConnectionInfo(null);
        }

        /// <summary>
        /// Gets channel connection information for the provided channel id from the currently selected server connection handler. If the provided id is null, the current channel id is used.
        /// </summary>
        /// <param name="channelId">The channel id to get the info for. Null for the current channel</param>
        public ChannelConnectionInfoResponse GetChannelConnectionInfo(uint? channelId)
        {
            Command command = CommandName.ChannelConnectInfo.CreateCommand();

            if (channelId.HasValue)
                command.AddParameter("cid", channelId.Value);

            return ResponseBase<ChannelConnectionInfoResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Returns a list of currently active server connection handler ids
        /// </summary>
        /// <returns>A list of ids</returns>
        public ListResponse<uint> GetServerConnectionHandlerIdList()
        {
            Command command = CommandName.ServerConnectionHandlerList.CreateCommand();

            return ResponseBase<ListResponse<uint>>.Parse(SendCommand(command), "schandlerid");
        }

        /// <summary>
        /// Displays information about your current ServerQuery connection including the ID of the selected virtual server, your loginname, etc.
        /// </summary>
        public WhoAmIResponse SendWhoAmI()
        {
            return ResponseBase<WhoAmIResponse>.Parse(SendCommand(SharedCommandName.WhoAmI.CreateCommand()));
        }

        #endregion
    }
}