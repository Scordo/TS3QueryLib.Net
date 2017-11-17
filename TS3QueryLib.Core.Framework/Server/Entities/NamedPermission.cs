//using System;
//using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class NamedPermission : NamedClientPermission
    {
        #region Properties

        public bool Negated { get; set; }

        #endregion

        #region Public Methods

        //public static Permission Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        //{
        //    if (currentParameterGroup == null)
        //        throw new ArgumentNullException("currentParameterGroup");

        //    return new Permission
        //    {
        //        Id = currentParameterGroup.GetParameterValue("permsid").ChangeTypeInvariant<uint>(),
        //        Value = currentParameterGroup.GetParameterValue("permvalue").ChangeTypeInvariant<int>(),
        //        Negated = currentParameterGroup.GetParameterValue("permnegated") == "1",
        //        Skip = currentParameterGroup.GetParameterValue("permskip") == "1"
        //    };
        //}

        #endregion
    }
}