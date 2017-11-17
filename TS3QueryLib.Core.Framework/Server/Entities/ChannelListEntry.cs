using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Entities;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ChannelListEntry : ChannelListEntryBase
    {
        #region Properties

        #region Always returned Properties

        public uint NeededSubscribePower { get; protected set; }

        #endregion

        #region Limits-Properties

        public int? TotalClientsFamily { get; protected set; }

        #endregion

        #endregion

        #region Public Methods

        public static ChannelListEntry Parse(CommandParameterGroup currrentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currrentParameterGroup == null)
                throw new ArgumentNullException("currrentParameterGroup");

            ChannelListEntry result = new ChannelListEntry();
            result.FillFrom(currrentParameterGroup, firstParameterGroup);
            result.NeededSubscribePower = currrentParameterGroup.GetParameterValue<uint>("channel_needed_subscribe_power");
            result.TotalClientsFamily = currrentParameterGroup.GetParameterValue<int?>("total_clients_family");

            return result;
        }

        #endregion
    }
}