using System;
using TS3QueryLib.Core.CommandHandling;


namespace TS3QueryLib.Core.Server.Entities
{
    public class CustomInfoEntry : CustomSearchEntry
    {
        #region Constructor

        private CustomInfoEntry()
        {

        }

        #endregion

        #region Public Methods

        public static CustomInfoEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new CustomInfoEntry
            {
                ClientDatabaseId = firstParameterGroup.GetParameterValue<uint>("cldbid"),
                Ident = currentParameterGroup.GetParameterValue("ident"),
                Value = currentParameterGroup.GetParameterValue("value"),
            };
        }

        #endregion
    }
}