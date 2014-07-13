using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;


namespace TS3QueryLib.Core.Server.Entities
{
    public class CustomSearchEntry: IDump
    {
        #region Properties

        public uint ClientDatabaseId { get; protected set; }
        public string Ident { get; protected set; }
        public string Value { get; protected set; }

        #endregion

        #region Constructor

        protected CustomSearchEntry()
        {

        }

        #endregion

        #region Public Methods

        public static CustomSearchEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new CustomSearchEntry
            {
                ClientDatabaseId = currentParameterGroup.GetParameterValue<uint>("cldbid"),
                Ident = currentParameterGroup.GetParameterValue("ident"),
                Value = currentParameterGroup.GetParameterValue("value"),
            };
        }

        #endregion
    }
}
