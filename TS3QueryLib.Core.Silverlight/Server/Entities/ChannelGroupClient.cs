using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ChannelGroupClient : IDump
    {
        #region Properties

        public uint ChannelId { get; protected set; }
        public uint ClientDatabaseId { get; protected set; }
        public uint ChannelGroupId { get; protected set; }


        #endregion

        #region Constructor

        private ChannelGroupClient()
        {

        }

        #endregion

        #region Public Methods

        public static ChannelGroupClient Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ChannelGroupClient
            {
                ChannelId = currentParameterGroup.GetParameterValue<uint>("cid"),
                ClientDatabaseId = currentParameterGroup.GetParameterValue<uint>("cldbid"),
                ChannelGroupId = currentParameterGroup.GetParameterValue<uint>("cgid"),
            };
        }

        #endregion
    }
}