using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class Token : IDump
    {
        #region Properties

        public string TokenText { get; protected set; }
        public uint Type { get; protected set; }
        public uint GroupId { get; protected set; }
        public uint ChannelId { get; protected set; }
        public string Description { get; protected set; }

        #endregion

        #region Constructor

        private Token()
        {

        }

        #endregion

        #region Public Methods

        public static Token Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new Token
            {
                TokenText = currentParameterGroup.GetParameterValue("token"),
                Type = currentParameterGroup.GetParameterValue<uint>("token_type"),
                GroupId = currentParameterGroup.GetParameterValue<uint>("token_id1"),
                ChannelId = currentParameterGroup.GetParameterValue<uint>("token_id2"),
                Description = currentParameterGroup.GetParameterValue("token_description")
            };
        }

        #endregion
    }
}