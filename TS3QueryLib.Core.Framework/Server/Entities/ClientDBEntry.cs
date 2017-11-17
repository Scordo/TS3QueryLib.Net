using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ClientDbEntry : IDump
    {
        #region Properties

        public uint DatabaseId { get; protected set; }
        public string NickName { get; protected set; }
        public string UniqueId { get; protected set; }
        public string Description { get; protected set; }
        public DateTime LastConnected { get; protected set; }
        public string LastIP { get; protected set; }
        public uint TotalConnections { get; protected set; }

        #endregion

        #region Constructor

        private ClientDbEntry()
        {

        }

        #endregion

        #region Public Methods

        public static ClientDbEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ClientDbEntry
            {
                DatabaseId = currentParameterGroup.GetParameterValue<uint>("cldbid"),
                NickName = currentParameterGroup.GetParameterValue("client_nickname"),
                UniqueId = currentParameterGroup.GetParameterValue("client_unique_identifier"),
                Description = currentParameterGroup.GetParameterValue("client_description"),
                LastConnected = new DateTime(1970, 1, 1).AddSeconds(currentParameterGroup.GetParameterValue<ulong>("client_lastconnected")),
                TotalConnections = currentParameterGroup.GetParameterValue<uint>("client_totalconnections"),
                LastIP = currentParameterGroup.GetParameterValue("client_lastip"),
            };
        }

        #endregion
    }
}