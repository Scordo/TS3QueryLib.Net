using System;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class PermissionInfo : IDump
    {
        #region Properties

        public uint PermissionId { get; protected set; }
        public string PermissionName { get; protected set; }

        #endregion

        #region Constructor

        private PermissionInfo()
        {

        }

        #endregion

        #region Public Methods

        public static PermissionInfo Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new PermissionInfo
            {
                PermissionId = currentParameterGroup.GetParameterValue<uint>("permid"),
                PermissionName = currentParameterGroup.GetParameterValue("permsid"),
            };
        }

        #endregion
    }
}