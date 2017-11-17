using System;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ClientBanEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint SourceChannelId { get; protected set; }
        public uint TargetChannelId { get; protected set; }
        public uint InvokerClientId { get; protected set; }
        public uint VictimClientId { get; protected set; }
        public string BanReason { get; protected set; }
        public string InvokerNickname { get; protected set; }
        public string InvokerUniqueId { get; protected set; }
        public TimeSpan? BanDuration { get; protected set; }
        public bool IsPermanentBan { get { return !BanDuration.HasValue; } }

        #endregion

        #region Constructors

        internal ClientBanEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            SourceChannelId = commandParameterGroupList.GetParameterValue<uint>("cfid");
            TargetChannelId = commandParameterGroupList.GetParameterValue<uint>("ctid");
            InvokerClientId = commandParameterGroupList.GetParameterValue<uint>("invokerid");
            VictimClientId = commandParameterGroupList.GetParameterValue<uint>("clid");

            BanReason = commandParameterGroupList.GetParameterValue("reasonmsg");
            InvokerUniqueId = commandParameterGroupList.GetParameterValue("invokeruid");
            InvokerNickname = commandParameterGroupList.GetParameterValue("invokername");
            uint banTimeInSeconds = commandParameterGroupList.GetParameterValue<uint>("bantime");

            BanDuration = banTimeInSeconds == 0 ? null : (TimeSpan?) TimeSpan.FromSeconds(banTimeInSeconds);
        }

        #endregion
    }
}
