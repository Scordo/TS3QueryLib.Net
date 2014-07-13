using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ServerGroupLight : IDump
    {
        #region Properties

        public uint Id { get; set; }
        public string Name { get; set; }

        #endregion

        #region Public Methods

        public static ServerGroupLight Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ServerGroupLight
                   {
                       Id = currentParameterGroup.GetParameterValue<uint>("sgid"),
                       Name = currentParameterGroup.GetParameterValue("name")
                   };
        }

        #endregion
    }
}