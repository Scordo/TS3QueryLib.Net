using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ChannelMovedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint? ChannelId { get; protected set; }
        public uint? ParentChannelId { get; protected set; }
        public uint? Order { get; protected set; }
        public uint? ReasonId { get; protected set; }
        public uint? InvokerId { get; protected set; }
        public string InvokerName { get; protected set; }
        public string InvokerUniqueId { get; protected set; }
     

        #endregion

        #region Constructors

        public ChannelMovedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException(nameof(commandParameterGroupList));

            ChannelId = commandParameterGroupList.GetParameterValue<uint>("cid");
            ParentChannelId = commandParameterGroupList.GetParameterValue<uint>("cpid");
            Order = commandParameterGroupList.GetParameterValue<uint>("order");
            ReasonId = commandParameterGroupList.GetParameterValue<uint>("reasonid");
            InvokerId = commandParameterGroupList.GetParameterValue<uint>("invokerid");
            InvokerName = commandParameterGroupList.GetParameterValue<string>("invokername");
            InvokerUniqueId = commandParameterGroupList.GetParameterValue<string>("invokeruid");
        }

        #endregion
    }
}
