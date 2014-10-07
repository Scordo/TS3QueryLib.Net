using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ClientFindEntry : IDump
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public string NickName { get; protected set; }

        #endregion

        #region Constructor

        private ClientFindEntry()
        {

        }

        #endregion

        #region Public Methods

        public static ClientFindEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ClientFindEntry
            {
                ClientId = currentParameterGroup.GetParameterValue<uint>("clid"),
                NickName = currentParameterGroup.GetParameterValue("client_nickname"),
            };
        }

        #endregion
    }
}