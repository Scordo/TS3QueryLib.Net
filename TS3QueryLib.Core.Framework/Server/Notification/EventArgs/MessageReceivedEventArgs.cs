using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class MessageReceivedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public string Message { get; protected set; }
        public uint InvokerClientId { get; protected set; }
        public string InvokerNickname { get; protected set; }
        public string InvokerUniqueId { get; protected set; }

        #endregion

        #region Constructor

        internal MessageReceivedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            Message = commandParameterGroupList.GetParameterValue("msg");
            InvokerClientId = commandParameterGroupList.GetParameterValue<uint>("invokerid");
            InvokerUniqueId = commandParameterGroupList.GetParameterValue("invokeruid");
            InvokerNickname = commandParameterGroupList.GetParameterValue("invokername");
        }

        #endregion
    }
}
