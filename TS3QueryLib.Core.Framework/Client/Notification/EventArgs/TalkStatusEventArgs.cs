using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Client.Notification.EventArgs
{
    public class TalkStatusEventArgs : TalkStatusEventArgsBase
    {
        #region Properties

        public bool IsWisper { get; protected set; }

        #endregion

        #region Constructor

        internal TalkStatusEventArgs(CommandParameterGroupList commandParameterGroupList) : base(commandParameterGroupList)
        {
            IsWisper = commandParameterGroupList.GetParameterValue<byte>("isreceivedwhisper").ToBool();
        }

        #endregion
    }
}