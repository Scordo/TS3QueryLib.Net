using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Server.Entities;

namespace TS3QueryLib.Core.Server.Responses
{
    public class WhoAmIResponse : WhoAmIResponseBase<WhoAmIResponse>
    {
        #region Properties

        public string ClientUniqueId { get; protected set; }
        public string ClientNickName { get; protected set; }
        public string ClientLoginName { get; protected set; }
        public uint ClientDatabaseId { get; protected set; }
        public uint VirtualServerId { get; protected set; }
        public string VirtualServerUniqueId { get; protected set; }
        public VirtualServerStatus VirtualServerStatus { get; protected set; }
        public ushort ServerPort { get; protected set; }
        public uint OriginServerId { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            string statusString = list.GetParameterValue("virtualserver_status");
            VirtualServerStatus = VirtualServerStatusHelper.Parse(statusString);
            ClientId = list.GetParameterValue<uint>("client_id");
            VirtualServerUniqueId = list.GetParameterValue("virtualserver_unique_identifier");
            VirtualServerId = list.GetParameterValue<uint>("virtualserver_id");
            ChannelId = list.GetParameterValue<uint>("client_channel_id");
            ClientDatabaseId = list.GetParameterValue<uint>("client_database_id");
            ClientNickName = list.GetParameterValue("client_nickname");
            ClientLoginName = list.GetParameterValue("client_login_name");
            ClientUniqueId = list.GetParameterValue("client_unique_identifier");
            ServerPort = list.GetParameterValue<ushort>("virtualserver_port");
            OriginServerId = list.GetParameterValue<uint>("client_origin_server_id");
        }

        #endregion
    }
}