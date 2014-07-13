using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ChannelGroup : IDump
    {
        #region Properties

        public uint Id { get; set; }
        public string Name { get; set; }
        public ushort Type { get; protected set; }
        public uint IconId { get; protected set; }
        public bool SaveDb { get; protected set; }
        public uint SortId { get; protected set; }
        public uint NameMode { get; protected set; }

        #endregion

        #region Constructor

        private ChannelGroup()
        {

        }

        #endregion

        #region Public Methods

        public static ChannelGroup Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new ChannelGroup
            {
                Id = currentParameterGroup.GetParameterValue<uint>("cgid"),
                Name = currentParameterGroup.GetParameterValue("name"),
                Type = currentParameterGroup.GetParameterValue<ushort>("type"),
                IconId = currentParameterGroup.GetParameterValue<uint>("iconid"),
                SaveDb = currentParameterGroup.GetParameterValue("savedb").ToBool(),
                SortId = currentParameterGroup.GetParameterValue<uint>("sortid"),
                NameMode = currentParameterGroup.GetParameterValue<uint>("namemode"),
            };
        }

        #endregion
    }
}