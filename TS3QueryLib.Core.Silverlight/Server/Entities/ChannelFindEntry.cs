using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ChannelFindEntry : IDump
    {
        #region Properties

        public uint Id { get; protected set; }
        public string Name { get; protected set; }

        #endregion

        #region Constructor

        private ChannelFindEntry()
        {

        }

        #endregion

        #region Public Methods

        public static ChannelFindEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ChannelFindEntry
                   {
                       Id = currentParameterGroup.GetParameterValue<uint>("cid"),
                       Name = currentParameterGroup.GetParameterValue("channel_name"),
                   };
        }

        #endregion
    }
}