using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Entities;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Server.Entities
{
    public class VirtualServerModification : ModificationBase
    {
        #region Properties

        public string Name { get; set; }
        public string WelcomeMessage { get; set; }
        public int? MaxClients { get; set; }
        public string Password { get; set; }
        public string HostMessage { get; set; }
        public HostMessageMode? HostMessageMode { get; set; }
        public uint? DefaultServerGroupId { get; set; }
        public uint? DefaultChannelGroupId { get; set; }
        public uint? DefaultChannelAdminGroupId { get; set; }
        public ulong? MaxDownloadTotalBandwidth { get; set; }
        public ulong? MaxUploadTotalBandwidth { get; set; }
        public string HostBannerUrl { get; set; }
        public string HostBannerGraphicsUrl { get; set; }
        public uint? HostBannerGraphicsInterval { get; set; }
        public int? ComplainAutoBanCount { get; set; }
        public int? ComplainAutoBanTime { get; set; }
        public int? ComplainRemoveTime { get; set; }
        public int? MinClientsInChannelBeforeForcedSilence { get; set; }
        public double? PrioritySpeakerDimmModification { get; set; }
        public int? AntiFloodPointsTickReduce { get; set; }
        public int? AntiFloodPointsNeededWarning { get; set; }
        public int? AntiFloodPointsNeededKick { get; set; }
        public int? AntiFloodPointsNeededBan { get; set; }
        public int? AntiFloodBanTime { get; set; }
        public string HostButtonTooltip { get; set; }
        public string HostButtonUrl { get; set; }
        public ulong? DownloadQuota { get; set; }
        public ulong? UploadQuota { get; set; }
        public string MachineId { get; set; }
        public ushort? Port { get; set; }
        public ushort? ReservedSlots { get; set; }
        public bool? AutoStart { get; set; }
        public int? NeededIdentitySecurityLevel { get; set; }
        public VirtualServerStatus? ServerStatus { get; set; }
        public string MinClientVersion { get; set; }

        public bool? LogClient { get; set; }
        public bool? LogQuery { get; set; }
        public bool? LogChannel { get; set; }
        public bool? LogPermission { get; set; }
        public bool? LogServer { get; set; }
        public bool? LogFiletransfer { get; set; }

        public string PhoneticName { get; set; }
        public uint? IconId { get; set; }
        public string HostButtonGraphicsUrl { get; set; }
        public HostBannerMode? HostBannerMode { get; set; }
        public bool? WeblistEnabled { get; set; }

        #endregion

        #region Public Methods

        public void AddToCommand(Command command)
        {
            AddToCommand(command, "virtualserver_name", Name);
            AddToCommand(command, "virtualserver_welcomemessage", WelcomeMessage);
            AddToCommand(command, "virtualserver_maxclients", MaxClients);
            AddToCommand(command, "virtualserver_password", Password);
            AddToCommand(command, "virtualserver_hostmessage", HostMessage);

            if (HostMessageMode.HasValue)
                AddToCommand(command, "virtualserver_hostmessage_mode", (uint)HostMessageMode.Value);

            AddToCommand(command, "virtualserver_default_server_group", DefaultServerGroupId);
            AddToCommand(command, "virtualserver_default_channel_group", DefaultChannelGroupId);
            AddToCommand(command, "virtualserver_default_channel_admin_group", DefaultChannelAdminGroupId);
            AddToCommand(command, "virtualserver_max_download_total_bandwidth", MaxDownloadTotalBandwidth);
            AddToCommand(command, "virtualserver_max_upload_total_bandwidth", MaxUploadTotalBandwidth);
            AddToCommand(command, "virtualserver_hostbanner_url", HostBannerUrl);
            AddToCommand(command, "virtualserver_hostbanner_gfx_url", HostBannerGraphicsUrl);
            AddToCommand(command, "virtualserver_hostbanner_gfx_interval", HostBannerGraphicsInterval);
            AddToCommand(command, "virtualserver_complain_autoban_count", ComplainAutoBanCount);
            AddToCommand(command, "virtualserver_complain_autoban_time", ComplainAutoBanTime);
            AddToCommand(command, "virtualserver_complain_remove_time", ComplainRemoveTime);
            AddToCommand(command, "virtualserver_min_clients_in_channel_before_forced_silence", MinClientsInChannelBeforeForcedSilence);
            AddToCommand(command, "virtualserver_priority_speaker_dimm_modificator", PrioritySpeakerDimmModification);
            AddToCommand(command, "virtualserver_antiflood_points_tick_reduce", AntiFloodPointsTickReduce);
            AddToCommand(command, "virtualserver_antiflood_points_needed_warning", AntiFloodPointsNeededWarning);
            AddToCommand(command, "virtualserver_antiflood_points_needed_kick", AntiFloodPointsNeededKick);
            AddToCommand(command, "virtualserver_antiflood_points_needed_ban", AntiFloodPointsNeededBan);
            AddToCommand(command, "virtualserver_antiflood_points_ban_time", AntiFloodBanTime);
            AddToCommand(command, "virtualserver_hostbutton_tooltip", HostButtonTooltip);
            AddToCommand(command, "virtualserver_hostbutton_url", HostButtonUrl);
            AddToCommand(command, "virtualserver_download_quota", DownloadQuota);
            AddToCommand(command, "virtualserver_upload_quota", UploadQuota);
            AddToCommand(command, "virtualserver_machine_id", MachineId);
            AddToCommand(command, "virtualserver_port", Port);
            AddToCommand(command, "virtualserver_reserved_slots", ReservedSlots);
            AddToCommand(command, "virtualserver_autostart", AutoStart);
            AddToCommand(command, "virtualserver_needed_identity_security_level", NeededIdentitySecurityLevel);
            AddToCommand(command, "virtualserver_min_client_version", MinClientVersion);

            if (ServerStatus.HasValue)
                AddToCommand(command, "virtualserver_status", (uint)ServerStatus.Value);

            AddToCommand(command, "virtualserver_log_client", LogClient);
            AddToCommand(command, "virtualserver_log_query", LogQuery);
            AddToCommand(command, "virtualserver_log_permissions", LogPermission);
            AddToCommand(command, "virtualserver_log_channel", LogChannel);
            AddToCommand(command, "virtualserver_log_server", LogServer);
            AddToCommand(command, "virtualserver_log_filetransfer", LogFiletransfer);
            AddToCommand(command, "virtualserver_name_phonetic", PhoneticName);
            AddToCommand(command, "virtualserver_icon_id", IconId);
            AddToCommand(command, "virtualserver_hostbutton_gfx_url", HostButtonGraphicsUrl);

            if (HostBannerMode.HasValue)
                AddToCommand(command, "virtualserver_hostbanner_mode", (uint) HostBannerMode.Value);

            AddToCommand(command, "virtualserver_weblist_enabled", WeblistEnabled);
        }

        #endregion
    }
}