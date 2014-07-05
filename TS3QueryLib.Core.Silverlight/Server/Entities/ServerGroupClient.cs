using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ServerGroupClient : IDump
    {
        #region Properties

        public uint DatabaseId { get; protected set; }
        public string Nickname { get; protected set; }
        public string UniqueId { get; protected set; }

        #endregion

        #region Public Methods

        public static ServerGroupClient Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            return new ServerGroupClient
            {
                DatabaseId = currentParameterGroup.GetParameterValue<uint>("cldbid"),
                Nickname = currentParameterGroup.GetParameterValue("client_nickname"),
                UniqueId = currentParameterGroup.GetParameterValue("client_unique_identifier"),
            };
        }

        #endregion

    }
}
