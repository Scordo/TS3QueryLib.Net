using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class PermissionFindEntry: IDump
    {
        #region Properties

        public uint T { get; set; }
        public uint Id1 { get; set; }
        public uint Id2 { get; set; }
        public int PermissionId { get; set; }

        #endregion

        #region Constructor

        private PermissionFindEntry()
        {

        }

        #endregion

        #region Public Methods

        public static PermissionFindEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new PermissionFindEntry
            {
                T = currentParameterGroup.GetParameterValue<uint>("t"),
                Id1 = currentParameterGroup.GetParameterValue<uint>("id1"),
                Id2 = currentParameterGroup.GetParameterValue<uint>("id2"),
                PermissionId = currentParameterGroup.GetParameterValue<int>("p"),
            };
        }

        #endregion
    }
}
