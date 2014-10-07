using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ClientKickEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint SourceChannelId { get; protected set; }
        public uint TargetChannelId { get; protected set; }
        public uint InvokerClientId { get; protected set; }
        public uint VictimClientId { get; protected set; }
        public string KickReason { get; protected set; }
        public string InvokerNickname { get; protected set; }
        public string InvokerUniqueId { get; protected set; }

        #endregion

        #region Constructors

        internal ClientKickEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            SourceChannelId = commandParameterGroupList.GetParameterValue<uint>("cfid");
            TargetChannelId = commandParameterGroupList.GetParameterValue<uint>("ctid");
            InvokerClientId = commandParameterGroupList.GetParameterValue<uint>("invokerid");
            VictimClientId = commandParameterGroupList.GetParameterValue<uint>("clid");

            KickReason = commandParameterGroupList.GetParameterValue("reasonmsg");
            InvokerUniqueId = commandParameterGroupList.GetParameterValue("invokeruid");
            InvokerNickname = commandParameterGroupList.GetParameterValue("invokername");
        }

        #endregion
    }
}
