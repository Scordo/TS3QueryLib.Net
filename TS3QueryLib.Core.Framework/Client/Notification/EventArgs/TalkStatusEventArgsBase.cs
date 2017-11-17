using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Client.Notification.Enums;

namespace TS3QueryLib.Core.Client.Notification.EventArgs
{
    public class TalkStatusEventArgsBase : System.EventArgs, IDump
    {
        #region Properties

        public uint ServerConnectionHandlerId { get; protected set; }
        public TalkStatus TalkStatus { get; protected set; }
        public uint ClientId { get; protected set; }

        #endregion

        #region Constructor

        protected TalkStatusEventArgsBase(CommandParameterGroupList commandParameterGroupList)
        {
            ServerConnectionHandlerId = commandParameterGroupList.GetParameterValue<uint>("schandlerid");
            TalkStatus = (TalkStatus) commandParameterGroupList.GetParameterValue<byte>("status");
            ClientId = commandParameterGroupList.GetParameterValue<uint>("clid");
        }

        #endregion
    }
}