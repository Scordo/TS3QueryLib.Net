using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class FileTransferListEntry: IDump
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public ulong Size { get; protected set; }
        public ulong SizeDone { get; protected set; }
        public uint ClientFtFileId { get; protected set; }
        public uint ServerFtFileId { get; protected set; }
        public uint Sender { get; protected set; }
        public uint Status { get; protected set; }
        public double CurrentSpeed { get; protected set; }

        #endregion

        #region Constructor

        private FileTransferListEntry()
        {

        }

        #endregion

        #region Public Methods

        public static FileTransferListEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new FileTransferListEntry
            {
                ClientId = currentParameterGroup.GetParameterValue<uint>("clid"),
                Path = currentParameterGroup.GetParameterValue("path"),
                Name = currentParameterGroup.GetParameterValue("name"),
                Size = currentParameterGroup.GetParameterValue<ulong>("size"),
                SizeDone = currentParameterGroup.GetParameterValue<ulong>("sizedone"),
                ClientFtFileId = currentParameterGroup.GetParameterValue<uint>("clientftfid"),
                ServerFtFileId = currentParameterGroup.GetParameterValue<uint>("serverftfid"),
                Sender = currentParameterGroup.GetParameterValue<uint>("sender"),
                Status = currentParameterGroup.GetParameterValue<uint>("status"),
                CurrentSpeed = currentParameterGroup.GetParameterValue<double>("current_speed"),
            };
        }

        #endregion
    }
}