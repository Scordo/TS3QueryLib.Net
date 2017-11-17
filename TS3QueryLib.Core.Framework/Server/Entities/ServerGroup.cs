using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ServerGroup : ServerGroupLight
    {
        #region Properties

        public ushort Type { get; protected set; }
        public uint IconId { get; protected set; }
        public bool SaveDb { get; protected set; }

        #endregion

        #region Constructors

        protected ServerGroup()
        {

        }

        #endregion

        #region Public Methods

        public new static ServerGroup Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ServerGroup
            {
                Id = currentParameterGroup.GetParameterValue<uint>("sgid"),
                Name = currentParameterGroup.GetParameterValue("name"),
                Type = currentParameterGroup.GetParameterValue<ushort>("type"),
                IconId = currentParameterGroup.GetParameterValue<uint>("iconid"),
                SaveDb = currentParameterGroup.GetParameterValue("savedb") == "1",
            };
        }

        #endregion
    }
}