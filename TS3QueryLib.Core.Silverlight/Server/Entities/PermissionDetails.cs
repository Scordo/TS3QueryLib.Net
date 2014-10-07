using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class PermissionDetails : IDump
    {
        #region Properties

        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        #endregion

        #region Constructor

        private PermissionDetails()
        {

        }

        #endregion

        #region Public Methods

        public static PermissionDetails Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new PermissionDetails
            {
                Id = currentParameterGroup.GetParameterValue<uint>("permid"),
                Name = currentParameterGroup.GetParameterValue("permname"),
                Description = currentParameterGroup.GetParameterValue("permdesc"),
            };
        }

        #endregion
    }
}