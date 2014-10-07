using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Server.Entities;

namespace TS3QueryLib.Core.Server.Responses
{
    public enum HostMessageMode
    {
        HostMessageModeNone = 0,        // dont display anything
        HostMessageModeLog = 1,         // display message in chatlog
        HostMessageModeModal = 2,       // display message in modal dialog
        HostMessageModeModalQuit = 3    // display message in modal dialog and close connection
    }

    public class ServerInfoResponse : ResponseBase<ServerInfoResponse>
    {
        #region Properties

        public string UniqueId {get; protected set;}
        public string Name { get; protected set; }
        public string WelcomeMessage { get; protected set; }
        public string Platform { get; protected set; }
        public string Version { get; protected set; }
        public string MinClientVersion { get; protected set; }
        public string PasswordHash { get; protected set; }
        public string HostMessage { get; protected set; }
        public string FileBase { get; protected set; }
        public string HostButtonTooltip { get; protected set; }
        public string HostButtonUrl { get; protected set; }
        public string HostBannerUrl { get; protected set; }
        public string HostBannerGraphicsUrl { get; protected set; }
        public DateTime DateCreatedUtc { get; protected set; }

        public uint Id { get; protected set; }
        public ushort Port { get; protected set; }
        public ushort ReservedSlots { get; protected set; }
        public uint NumberOfClientsOnline { get; protected set; }
        public uint NumberOfQueryClientsOnline { get; protected set; }
        public uint MaximumClientsAllowed { get; protected set; }
        public TimeSpan Uptime { get; protected set; }
        public bool? AutoStart { get; protected set; }
        public string MachineId { get; protected set; }

        public int? AntiFloodPointsTickReduce { get; protected set; }
        public int AntiFloodPointsNeededIPBlock { get; protected set; }
        public int AntiFloodPointsNeededCommandBlock { get; protected set; }

        public bool LogClient { get; protected set; }
        public bool LogQuery { get; protected set; }
        public bool LogChannel { get; protected set; }
        public bool LogPermission { get; protected set; }
        public bool LogServer { get; protected set; }
        public bool LogFiletransfer { get; protected set; }
        public VirtualServerStatus ServerStatus { get; protected set; }
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
        public double PrioritySpeakerDimmModification { get; protected set; }

        public int ClientConnections { get; protected set; }
        public int QueryClientConnections { get; protected set; }
        public int DefaultChannelAdminGroupId { get; protected set; }
        public int DefaultServerGroupId { get; protected set; }
        public int DefaultChannelGroupId { get; protected set; }
        public int HostBannerGraphicsInterval { get; protected set; }
        public int ChannelsOnline { get; protected set; }
        public int ComplainAutoBanCount { get; protected set; }
        public int ComplainAutoBanTime { get; protected set; }
        public int ComplainRemoveTime { get; protected set; }
        public int MinClientsBeforeForcedSilence { get; protected set; }
        public int NeededIdentitySecurityLevel { get; protected set; }

        public ulong DownloadQuota { get; protected set; }
        public ulong UploadQuota { get; protected set; }
        public ulong MaxDownloadTotalBandwidth { get; protected set; }
        public ulong MaxUploadTotalBandwidth { get; protected set; }
        public ulong MonthBytesDownloaded { get; protected set; }
        public ulong MonthBytesUploaded { get; protected set; }
        public ulong TotalBytesDownloaded { get; protected set; }
        public ulong TotalBytesUploaded { get; protected set; }

        public bool IsPasswordProtected { get; protected set; }
        public HostMessageMode HostMessageMode { get; protected set; }
        public string PhoneticName { get; protected set; }
        public uint IconId { get; protected set; }
        public string HostButtonGraphicsUrl { get; protected set; }

        public HostBannerMode HostBannerMode { get; protected set; }
        public double TotalPacketLossSpeech { get; protected set; }
        public double TotalPacketLossKeepalive { get; protected set; }
        public double TotalPacketLossControl { get; protected set; }
        public double TotalPacketLossTotal { get; protected set; }
        public double TotalPing { get; protected set; }
        public string IP { get; protected set; }
        public bool WeblistEnabled { get; protected set; }
        public bool AskForPrivilegKey { get; protected set; }
        public ulong FileTransferBytesSentTotal { get; protected set; }
        public ulong FileTransferBytesReceivedTotal { get; protected set; }
        public ulong PacketsSentSpeech { get; protected set; }
        public ulong BytesSentSpeech { get; protected set; }
        public ulong PacketsReceivedSpeech { get; protected set; }
        public ulong BytesReceivedSpeech { get; protected set; }
        public ulong PacketsSentKeepAlive { get; protected set; }
        public ulong BytesSentKeepAlive { get; protected set; }
        public ulong PacketsReceivedKeepAlive { get; protected set; }
        public ulong BytesReceivedKeepAlive { get; protected set; }
        public ulong PacketsSentControl { get; protected set; }
        public ulong BytesSentControl { get; protected set; }
        public ulong PacketsReceivedControl { get; protected set; }
        public ulong BytesReceivedControl { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            UniqueId = list.GetParameterValue("virtualserver_unique_identifier");
            Name = list.GetParameterValue("virtualserver_name");
            WelcomeMessage = list.GetParameterValue("virtualserver_welcomemessage");
            Platform = list.GetParameterValue("virtualserver_platform");
            Version = list.GetParameterValue("virtualserver_version");
            MinClientVersion = list.GetParameterValue("virtualserver_min_client_version");
            PasswordHash = list.GetParameterValue("virtualserver_password");
            HostMessage = list.GetParameterValue("virtualserver_hostmessage");
            FileBase = list.GetParameterValue("virtualserver_filebase");
            HostButtonTooltip = list.GetParameterValue("virtualserver_hostbutton_tooltip");
            HostButtonUrl = list.GetParameterValue("virtualserver_hostbutton_url");
            HostBannerUrl = list.GetParameterValue("virtualserver_hostbanner_url");
            HostBannerGraphicsUrl = list.GetParameterValue("virtualserver_hostbanner_gfx_url");
            DateCreatedUtc = new DateTime(1970, 1, 1).AddSeconds(list.GetParameterValue<ulong>("virtualserver_created"));

            Id = list.GetParameterValue<uint>("virtualserver_id");
            Port = list.GetParameterValue<ushort>("virtualserver_port");
            ReservedSlots = list.GetParameterValue<ushort>("VIRTUALSERVER_RESERVED_SLOTS");
            NumberOfClientsOnline = list.GetParameterValue<uint>("virtualserver_clientsonline");
            NumberOfQueryClientsOnline = list.GetParameterValue<uint>("virtualserver_queryclientsonline");
            MaximumClientsAllowed = list.GetParameterValue<uint>("virtualserver_maxclients");
            Uptime = TimeSpan.FromSeconds(list.GetParameterValue<uint>("virtualserver_uptime"));
            AutoStart = list.GetParameterValue("virtualserver_autostart") == "1";
            MachineId = list.GetParameterValue("virtualserver_machine_id");

            AntiFloodPointsTickReduce = list.GetParameterValue<int>("virtualserver_antiflood_points_tick_reduce");
            AntiFloodPointsNeededIPBlock = list.GetParameterValue<int>("virtualserver_antiflood_points_needed_ip_block");
            AntiFloodPointsNeededCommandBlock = list.GetParameterValue<int>("virtualserver_antiflood_points_needed_command_block");

            LogClient = list.GetParameterValue("virtualserver_log_client") == "1";
            LogQuery = list.GetParameterValue("virtualserver_log_query") == "1";
            LogChannel = list.GetParameterValue("virtualserver_log_channel") == "1";
            LogPermission = list.GetParameterValue("virtualserver_log_permissions") == "1";
            LogServer = list.GetParameterValue("virtualserver_log_server") == "1";
            LogFiletransfer = list.GetParameterValue("virtualserver_log_filetransfer") == "1";

            string statusString = list.GetParameterValue("virtualserver_status");
            ServerStatus = VirtualServerStatusHelper.Parse(statusString);
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
            PrioritySpeakerDimmModification = list.GetParameterValue<double>("virtualserver_priority_speaker_dimm_modificator");

            ClientConnections = list.GetParameterValue<int>("virtualserver_client_connections");
            QueryClientConnections = list.GetParameterValue<int>("virtualserver_query_client_connections");
            DefaultChannelAdminGroupId = list.GetParameterValue<int>("virtualserver_default_channel_admin_group");
            DefaultServerGroupId = list.GetParameterValue<int>("virtualserver_default_server_group");
            DefaultChannelGroupId = list.GetParameterValue<int>("virtualserver_default_channel_group");
            HostBannerGraphicsInterval = list.GetParameterValue<int>("virtualserver_hostbanner_gfx_interval");
            ChannelsOnline = list.GetParameterValue<int>("virtualserver_channelsonline");
            ComplainAutoBanCount = list.GetParameterValue<int>("virtualserver_complain_autoban_count");
            ComplainAutoBanTime = list.GetParameterValue<int>("virtualserver_complain_autoban_time");
            ComplainRemoveTime = list.GetParameterValue<int>("virtualserver_complain_remove_time");
            MinClientsBeforeForcedSilence = list.GetParameterValue<int>("virtualserver_min_clients_in_channel_before_forced_silence");
            NeededIdentitySecurityLevel = list.GetParameterValue<int>("virtualserver_needed_identity_security_level");

            DownloadQuota = list.GetParameterValue<ulong>("virtualserver_download_quota");
            UploadQuota = list.GetParameterValue<ulong>("virtualserver_upload_quota");
            MaxDownloadTotalBandwidth = list.GetParameterValue<ulong>("virtualserver_max_download_total_bandwidth");
            MaxUploadTotalBandwidth = list.GetParameterValue<ulong>("virtualserver_max_upload_total_bandwidth");
            MonthBytesDownloaded = list.GetParameterValue<ulong>("virtualserver_month_bytes_downloaded");
            MonthBytesUploaded = list.GetParameterValue<ulong>("virtualserver_month_bytes_uploaded");
            TotalBytesDownloaded = list.GetParameterValue<ulong>("virtualserver_total_bytes_downloaded");
            TotalBytesUploaded = list.GetParameterValue<ulong>("virtualserver_total_bytes_uploaded");

            IsPasswordProtected = list.GetParameterValue("virtualserver_flag_password") == "1";
            HostMessageMode = (HostMessageMode)list.GetParameterValue<uint>("virtualserver_hostmessage_mode");
            PhoneticName = list.GetParameterValue("virtualserver_name_phonetic");
            IconId = list.GetParameterValue<uint>("virtualserver_icon_id");
            HostButtonGraphicsUrl = list.GetParameterValue("virtualserver_hostbutton_gfx_url");

            TotalPacketLossSpeech = list.GetParameterValue<double>("virtualserver_total_packetloss_speech");
            TotalPacketLossKeepalive = list.GetParameterValue<double>("virtualserver_total_packetloss_keepalive");
            TotalPacketLossControl = list.GetParameterValue<double>("virtualserver_total_packetloss_control");
            TotalPacketLossTotal = list.GetParameterValue<double>("virtualserver_total_packetloss_total");
            TotalPing = list.GetParameterValue<double>("virtualserver_total_ping");
            IP = list.GetParameterValue("virtualserver_ip");
            WeblistEnabled = list.GetParameterValue("virtualserver_weblist_enabled") == "1";
            AskForPrivilegKey = list.GetParameterValue("virtualserver_ask_for_privilegekey") == "1";
            HostBannerMode = (HostBannerMode)list.GetParameterValue<uint>("virtualserver_hostbanner_mode");
            FileTransferBytesSentTotal = list.GetParameterValue<ulong>("connection_filetransfer_bytes_sent_total");
            FileTransferBytesReceivedTotal = list.GetParameterValue<ulong>("connection_filetransfer_bytes_received_total");
            PacketsSentSpeech = list.GetParameterValue<ulong>("connection_packets_sent_speech");
            BytesSentSpeech = list.GetParameterValue<ulong>("connection_bytes_sent_speech");
            PacketsReceivedSpeech = list.GetParameterValue<ulong>("connection_packets_received_speech");
            BytesReceivedSpeech = list.GetParameterValue<ulong>("connection_bytes_received_speech");
            PacketsSentKeepAlive = list.GetParameterValue<ulong>("connection_packets_sent_keepalive");
            BytesSentKeepAlive = list.GetParameterValue<ulong>("connection_bytes_sent_keepalive");
            PacketsReceivedKeepAlive = list.GetParameterValue<ulong>("connection_packets_received_keepalive");
            BytesReceivedKeepAlive = list.GetParameterValue<ulong>("connection_bytes_received_keepalive");
            PacketsSentControl = list.GetParameterValue<ulong>("connection_packets_sent_control");
            BytesSentControl = list.GetParameterValue<ulong>("connection_bytes_sent_control");
            PacketsReceivedControl = list.GetParameterValue<ulong>("connection_packets_received_control");
            BytesReceivedControl = list.GetParameterValue<ulong>("connection_bytes_received_control");
        }

        #endregion
    }
}