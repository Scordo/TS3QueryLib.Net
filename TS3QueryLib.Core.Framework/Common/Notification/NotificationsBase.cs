using System;
using System.Collections.Generic;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Common.Notification
{
    public abstract class NotificationsBase
    {
        #region Properties

        protected QueryRunnerBase QueryRunner { get; set; }
        private Dictionary<string, Action<CommandParameterGroupList>> NotificationHandlers { get; set; }

        #endregion

        #region Constructor

        protected NotificationsBase(QueryRunnerBase queryRunner)
        {
            if (queryRunner == null)
                throw new ArgumentNullException("queryRunner");

            QueryRunner = queryRunner;
            QueryRunner.UnknownNotificationReceived += QueryRunner_UnknownNotificationReceived;

            NotificationHandlers = GetNotificationHandlers();
        }

        #endregion

        #region Non Public Methods

        protected abstract Dictionary<string, Action<CommandParameterGroupList>> GetNotificationHandlers();

        private void QueryRunner_UnknownNotificationReceived(object sender, EventArgs<string> e)
        {
            CommandParameterGroupList parameterGroupList = CommandParameterGroupList.Parse(e.Value);

            if (parameterGroupList.Count == 0)
                return;

            Action<CommandParameterGroupList> handler;
            string notificationName = parameterGroupList[0][0].Name;
            if (NotificationHandlers.TryGetValue(notificationName, out handler) || NotificationHandlers.TryGetValue("*", out handler))
                handler(parameterGroupList);
        }

        #endregion
    }
}