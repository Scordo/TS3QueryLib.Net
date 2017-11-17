using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class InitializeFileUploadResponse : ResponseBase<InitializeFileUploadResponse>
    {
        #region Properties

        public uint? ClientFileTransferId { get; protected set; }
        public uint? ServerFileTransferId { get; protected set; }
        public string FileTransferKey { get; protected set; }
        public ushort? FileTransferPort { get; protected set; }
        public ulong? SeekPosition { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            ClientFileTransferId = list.GetParameterValue<uint?>("clientftfid");
            ServerFileTransferId = list.GetParameterValue<uint?>("serverftfid");
            FileTransferKey = list.GetParameterValue("ftkey");
            FileTransferPort = list.GetParameterValue<ushort?>("port");
            SeekPosition = list.GetParameterValue<ulong?>("seekpos");
        }

        #endregion
    }
}