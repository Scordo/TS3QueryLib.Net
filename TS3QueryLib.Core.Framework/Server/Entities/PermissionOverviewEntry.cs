using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class PermissionOverviewEntry: IDump
    {
        #region Properties

        public uint T { get; set; }
        public uint Id1 { get; set; }
        public uint Id2 { get; set; }
        public int Value { get; set; }
        public uint PermissionId { get; set; }
        public bool Negated { get; set; }
        public bool Skip { get; set; }

        #endregion

        #region Constructor

        private PermissionOverviewEntry()
        {

        }

        #endregion

        #region Public Methods

        public static PermissionOverviewEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new PermissionOverviewEntry
            {
                T = currentParameterGroup.GetParameterValue<uint>("t"),
                Id1 = currentParameterGroup.GetParameterValue<uint>("id1"),
                Id2 = currentParameterGroup.GetParameterValue<uint>("id2"),
                Value = currentParameterGroup.GetParameterValue<int>("v"),
                PermissionId = currentParameterGroup.GetParameterValue<uint>("p"),
                Negated = currentParameterGroup.GetParameterValue("n") == "1",
                Skip = currentParameterGroup.GetParameterValue("s") == "1"
            };
        }

        #endregion
    }
}
