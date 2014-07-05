using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class Permission : ClientPermission
    {
        #region Properties

        public bool Negated { get; set; }
        public string Name { get; protected set; }

        #endregion

        #region Public Methods

        public static Permission Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new Permission
            {
                Id = currentParameterGroup.GetParameter("permid") == null ? 0 : currentParameterGroup.GetParameterValue<uint>("permid"),
                Name = currentParameterGroup.GetParameterValue("permsid"),
                Value = currentParameterGroup.GetParameterValue<int>("permvalue"),
                Negated = currentParameterGroup.GetParameterValue("permnegated") == "1",
                Skip = currentParameterGroup.GetParameterValue("permskip") == "1"
            };
        }

        #endregion
    }
}