using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class ConnectionInfoResponse : ResponseBase<ConnectionInfoResponse>
    {
        #region Properties

        public uint FileTransferBandwidthSent { get; protected set; }
        public uint FileTransferBandwidthReceived { get; protected set; }
        public ulong PacketsSentTotal { get; protected set; }
        public ulong PacketsReceivedTotal { get; protected set; }
        public ulong BytesSentTotal { get; protected set; }
        public ulong BytesReceivedTotal { get; protected set; }
        public uint BandwidthSentLastSecond { get; protected set; }
        public uint BandwidthSentLastMinute { get; protected set; }
        public uint BandwidthReceivedLastSecond { get; protected set; }
        public uint BandwidthReceivedLastMinute { get; protected set; }
        public TimeSpan ConnectionDuration { get; protected set; }
        public ulong FileTransferBytesSentTotal { get; protected set; }
        public ulong FileTransferBytesReceivedTotal { get; protected set; }
        public double PacketLossTotal { get; protected set; }
        public double Ping { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            FileTransferBandwidthSent = list.GetParameterValue<uint>("connection_filetransfer_bandwidth_sent");
            FileTransferBandwidthReceived = list.GetParameterValue<uint>("connection_filetransfer_bandwidth_received");
            PacketsSentTotal = list.GetParameterValue<ulong>("connection_packets_sent_total");
            PacketsReceivedTotal = list.GetParameterValue<ulong>("connection_bytes_sent_total");
            BytesSentTotal = list.GetParameterValue<ulong>("connection_packets_received_total");
            BytesReceivedTotal = list.GetParameterValue<ulong>("connection_bytes_received_total");
            BandwidthSentLastSecond = list.GetParameterValue<uint>("connection_bandwidth_sent_last_second_total");
            BandwidthSentLastMinute = list.GetParameterValue<uint>("connection_bandwidth_sent_last_minute_total");
            BandwidthReceivedLastSecond = list.GetParameterValue<uint>("connection_bandwidth_received_last_second_total");
            BandwidthReceivedLastMinute = list.GetParameterValue<uint>("connection_bandwidth_received_last_minute_total");
            ConnectionDuration = TimeSpan.FromMilliseconds(list.GetParameterValue<uint>("connection_connected_time"));

            FileTransferBytesSentTotal = list.GetParameterValue<ulong>("connection_filetransfer_bytes_sent_total");
            FileTransferBytesReceivedTotal = list.GetParameterValue<ulong>("connection_filetransfer_bytes_received_total");
            PacketLossTotal = list.GetParameterValue<double>("connection_packetloss_total");
            Ping = list.GetParameterValue<double>("connection_ping");
        }

        #endregion
    }
}