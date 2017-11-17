using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ClientMovedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public uint TargetChannelId { get; protected set; }

        #endregion

        internal ClientMovedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            ClientId = commandParameterGroupList.GetParameterValue<uint>("clid");
            TargetChannelId = commandParameterGroupList.GetParameterValue<uint>("ctid");
        }
    }
}
