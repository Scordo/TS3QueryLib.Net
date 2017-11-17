using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class HostInfoResponse : ResponseBase<HostInfoResponse>
    {
        #region Properties

        public TimeSpan Uptime { get; protected set; }
        public DateTime UtcTimeStamp { get; protected set; }
        public uint? VirtualServersCount { get; protected set; }
        public ulong FileTransferBandwidthSent { get; protected set; }
        public ulong FileTransferBandwidthReceived { get; protected set; }
        public ulong AmountOfPacketsSendTotal { get; protected set; }
        public ulong AmountOfPacketsReceivedTotal { get; protected set; }
        public ulong AmountOfBytesSendTotal { get; protected set; }
        public ulong AmountOfBytesReceivedTotal { get; protected set; }
        public ulong BandWidthSentLastSecondTotal { get; protected set; }
        public ulong BandWidthReceivedLastSecondTotal { get; protected set; }
        public ulong BandWidthSentLastMinuteTotal { get; protected set; }
        public ulong BandWidthReceivedLastMinuteTotal { get; protected set; }
        public uint TotalMaxClients { get; protected set; }
        public uint TotalClientsOnline { get; protected set; }
        public uint TotalChannelsOnline { get; protected set; }
        public ulong FileTransferBytesSentTotal { get; protected set; }
        public ulong FileTransferBytesReceivedTotal { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            Uptime = TimeSpan.FromSeconds(list.GetParameterValue<ulong>("INSTANCE_UPTIME"));
            UtcTimeStamp = new DateTime(1970, 1, 1).AddSeconds(list.GetParameterValue<ulong>("HOST_TIMESTAMP_UTC"));
            VirtualServersCount = list.GetParameterValue<uint>("VIRTUALSERVERS_RUNNING_TOTAL");
            FileTransferBandwidthSent = list.GetParameterValue<ulong>("CONNECTION_FILETRANSFER_BANDWIDTH_SENT");
            FileTransferBandwidthReceived = list.GetParameterValue<ulong>("CONNECTION_FILETRANSFER_BANDWIDTH_RECEIVED");
            AmountOfPacketsSendTotal = list.GetParameterValue<ulong>("CONNECTION_PACKETS_SENT_TOTAL");
            AmountOfPacketsReceivedTotal = list.GetParameterValue<ulong>("CONNECTION_PACKETS_RECEIVED_TOTAL");
            AmountOfBytesSendTotal = list.GetParameterValue<ulong>("CONNECTION_BYTES_SENT_TOTAL");
            AmountOfBytesReceivedTotal = list.GetParameterValue<ulong>("CONNECTION_BYTES_RECEIVED_TOTAL");
            BandWidthSentLastSecondTotal = list.GetParameterValue<ulong>("CONNECTION_BANDWIDTH_SENT_LAST_SECOND_TOTAL");
            BandWidthReceivedLastSecondTotal = list.GetParameterValue<ulong>("CONNECTION_BANDWIDTH_RECEIVED_LAST_SECOND_TOTAL");
            BandWidthSentLastMinuteTotal = list.GetParameterValue<ulong>("CONNECTION_BANDWIDTH_SENT_LAST_MINUTE_TOTAL");
            BandWidthReceivedLastMinuteTotal = list.GetParameterValue<ulong>("CONNECTION_BANDWIDTH_RECEIVED_LAST_MINUTE_TOTAL");
            TotalMaxClients = list.GetParameterValue<uint>("virtualservers_total_maxclients");
            TotalClientsOnline = list.GetParameterValue<uint>("virtualservers_total_clients_online");
            TotalChannelsOnline = list.GetParameterValue<uint>("virtualservers_total_channels_online");

            FileTransferBytesSentTotal = list.GetParameterValue<ulong>("connection_filetransfer_bytes_sent_total");
            FileTransferBytesReceivedTotal = list.GetParameterValue<ulong>("connection_filetransfer_bytes_received_total");
        }

        #endregion
    }
}