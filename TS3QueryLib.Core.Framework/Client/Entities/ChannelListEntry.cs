using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Entities;

namespace TS3QueryLib.Core.Client.Entities
{
    public class ChannelListEntry : ChannelListEntryBase
    {
        #region Properties

        #region Always returned Properties

        public bool IsSubscribed { get; protected set; }

        #endregion

        #endregion

        #region Public Methods

        public static ChannelListEntry Parse(CommandParameterGroup currrentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currrentParameterGroup == null)
                throw new ArgumentNullException("currrentParameterGroup");

            ChannelListEntry result = new ChannelListEntry();
            result.FillFrom(currrentParameterGroup, firstParameterGroup);
            result.IsSubscribed = currrentParameterGroup.GetParameterValue<byte>("channel_flag_are_subscribed").ToBool();

            return result;
        }

        #endregion
    }
}