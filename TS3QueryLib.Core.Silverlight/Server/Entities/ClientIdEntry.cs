using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ClientIdEntry : IDump
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public string NickName { get; protected set; }

        #endregion

        #region Constructor

        private ClientIdEntry()
        {

        }

        #endregion

        #region Public Methods

        public static ClientIdEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ClientIdEntry
            {
                ClientId = currentParameterGroup.GetParameterValue<uint>("clid"),
                NickName = currentParameterGroup.GetParameterValue("name"),
            };
        }

        #endregion
    }
}