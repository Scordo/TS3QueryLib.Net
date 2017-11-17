using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class UnknownNotificationEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public string Name { get; protected set; }
        public CommandParameterGroupList CommandParameterGroupList { get; protected set; }
        
        #endregion

        #region Constructors

        public UnknownNotificationEventArgs(string name, CommandParameterGroupList commandParameterGroupList)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (commandParameterGroupList == null)
                throw new ArgumentNullException(nameof(commandParameterGroupList));

            Name = name;
            CommandParameterGroupList = commandParameterGroupList;
        }

        #endregion
    }
}