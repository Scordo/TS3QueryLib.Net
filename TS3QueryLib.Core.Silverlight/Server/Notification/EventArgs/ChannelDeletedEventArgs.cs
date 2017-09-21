using System;
using System.Collections.Generic;
using System.Linq;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ChannelDeletedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public int? ChannelId { get; protected set; }
        public List<int> SubChannelIdList { get; protected set; }
        public int? InvokerId { get; protected set; }
        public string InvokerName { get; protected set; }

        #endregion

        #region Constructors


        public ChannelDeletedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException(nameof(commandParameterGroupList));

            List<int> channelIds = commandParameterGroupList.Select(pg => pg.GetParameterValue<int>("cid")).ToList();

            ChannelId = channelIds.Count > 0 ? (int?)channelIds.Last() : null;
            InvokerId = commandParameterGroupList.GetParameterValue<int?>("invokerid");
            InvokerName = commandParameterGroupList.GetParameterValue<string>("invokername");

            SubChannelIdList = channelIds.GetRange(0, Math.Max(0, channelIds.Count - 1));
        }

        #endregion
    }
}