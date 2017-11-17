using System;
using System.Collections.Generic;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Responses
{
    public class ClientInfoResponse : ClientInfoBaseResponse<ClientInfoResponse>
    {
        #region Properties

        public string Version { get; set; }
        public string Platform { get; set; }

        public bool InputMuted { get; protected set; }
        public bool OutputMuted { get; protected set; }
        public bool OuputOnlyMuted { get; protected set; }
        public bool HasInputHardware { get; protected set; }
        public bool HasOutputHardware { get; protected set; }
        public bool IsRecording { get; protected set; }
        public bool IsAway { get; protected set; }
        public bool IsTalker { get; protected set; }
        public bool IsPrioritySpeaker { get; protected set; }
        public string AwayMessage { get; protected set; }

        public string DefaultChannel { get; protected set; }
        public string MetaData { get; protected set; }
        public string LoginName { get; protected set; }
        public string TalkRequestMessage { get; protected set; }
        public string NicknamePhonetic { get; protected set; }

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

        public uint ChannelId { get; protected set; }
        public uint ChannelGroupId { get; protected set; }
        public List<uint> ServerGroups { get; protected set; }
        public uint Type { get; protected set; }
        public uint TalkPower { get; protected set; }
        public uint TalkRequests { get; protected set; }
        public uint NeededServerQueryViewPower { get; protected set; }
        public string Avatar { get; protected set; }
        public TimeSpan IdleTime { get; protected set; }
        public TimeSpan ConnectedTime { get; protected set; }
        public string ClientIP { get; protected set; }
        public string ClientCountry { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            base.FillFrom(responseText, additionalStates);

            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            Version = list.GetParameterValue("client_version");
            Platform = list.GetParameterValue("client_platform");
            InputMuted = list.GetParameterValue("client_input_muted").ToBool();
            OutputMuted = list.GetParameterValue("client_output_muted").ToBool();
            OuputOnlyMuted = list.GetParameterValue("client_outputonly_muted").ToBool();
            HasInputHardware = list.GetParameterValue("client_input_hardware").ToBool();
            HasOutputHardware = list.GetParameterValue("client_output_hardware").ToBool();
            IsRecording = list.GetParameterValue("client_is_recording").ToBool();
            IsAway = list.GetParameterValue("client_away").ToBool();
            IsTalker = list.GetParameterValue("client_is_talker").ToBool();
            IsPrioritySpeaker = list.GetParameterValue("client_is_priority_speaker").ToBool();
            AwayMessage = list.GetParameterValue("client_away_message");

            DefaultChannel = list.GetParameterValue("client_default_channel");
            MetaData = list.GetParameterValue("client_meta_data");
            LoginName = list.GetParameterValue("client_login_name");
            TalkRequestMessage = list.GetParameterValue("client_talk_request_msg");
            NicknamePhonetic = list.GetParameterValue("client_nickname_phonetic");

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

            DatabaseId = list.GetParameterValue<uint>("client_database_id");
            ChannelId = list.GetParameterValue<uint>("cid");
            ChannelGroupId = list.GetParameterValue<uint>("client_channel_group_id");

            ServerGroups = list.GetParameterValue("client_servergroups").ToIdList();
            Type = list.GetParameterValue<uint>("client_type");
            TalkPower = list.GetParameterValue<uint>("client_talk_power");
            TalkRequests = list.GetParameterValue<uint>("client_talk_request");
            NeededServerQueryViewPower = list.GetParameterValue<uint>("client_needed_serverquery_view_power");

            Avatar = list.GetParameterValue("client_flag_avatar");
            IdleTime = TimeSpan.FromMilliseconds(list.GetParameterValue<uint>("client_idle_time"));
            ConnectedTime = TimeSpan.FromMilliseconds(list.GetParameterValue<uint>("connection_connected_time"));
            ClientIP = list.GetParameterValue("connection_client_ip");
            ClientCountry = list.GetParameterValue("client_country");
        }

        #endregion
    }
}