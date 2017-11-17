using System;

using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class FileTransferFileEntry: IDump
    {
        #region Properties

        public uint ChannelId { get; protected set; }
        public string Name { get; protected set; }
        public ulong Size { get; protected set; }
        public DateTime Created { get; protected set; }
        public uint? Type { get; protected set; }

        #endregion

        #region Constructor

        private FileTransferFileEntry()
        {

        }

        #endregion

        #region Public Methods

        public static FileTransferFileEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new FileTransferFileEntry
            {
                ChannelId = firstParameterGroup.GetParameterValue<uint>("cid"),
                Name = currentParameterGroup.GetParameterValue("name"),
                Size = currentParameterGroup.GetParameterValue<ulong>("size"),
                Created = new DateTime(1970, 1, 1).AddSeconds(currentParameterGroup.GetParameterValue<ulong>("datetime")),
                Type = currentParameterGroup.GetParameterValue<uint?>("type"),
            };
        }

        #endregion
    }
}