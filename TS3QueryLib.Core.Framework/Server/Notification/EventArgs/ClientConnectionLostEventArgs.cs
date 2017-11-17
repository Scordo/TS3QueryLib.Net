using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;


namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ClientConnectionLostEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint SourceChannelId { get; protected set; }
        public uint TargetChannelId { get; protected set; }
        public uint ClientId { get; protected set; }
        public string ReasonMessage { get; protected set; }

        #endregion

        #region Constructors

        internal ClientConnectionLostEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            SourceChannelId = commandParameterGroupList.GetParameterValue<uint>("cfid");
            TargetChannelId = commandParameterGroupList.GetParameterValue<uint>("ctid");
            ClientId = commandParameterGroupList.GetParameterValue<uint>("clid");
            ReasonMessage = commandParameterGroupList.GetParameterValue("reasonmsg");
        }

        #endregion
    }
}
