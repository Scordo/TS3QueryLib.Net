using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Entities;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ServerInstanceModification : ModificationBase
    {
        #region Properties

        public uint? GuestServerQueryGroupId { get; set; }
        public ushort? FileTransferPort { get; set; }
        public ulong? MaxDownloadTotalBandwidth { get; set; }
        public ulong? MaxUploadTotalBandwidth { get; set; }

        public uint? TemplateServerAdminGroupId { get; set; }
        public uint? TemplateServerDefaultGroupId { get; set; }
        public uint? TemplateChannelAdminGroupId { get; set; }
        public uint? TemplateChannelDefaultGroupId { get; set; }

        public uint? ServerQueryFloodCommandsCount { get; set; }
        public TimeSpan? ServerQueryFloodRatingDuration { get; set; }
        public TimeSpan? ServerQueryBanDuration { get; set; }

        #endregion

        #region Public Methods

        public void AddToCommand(Command command)
        {
            AddToCommand(command, "serverinstance_guest_serverquery_group", GuestServerQueryGroupId);
            AddToCommand(command, "serverinstance_filetransfer_port", FileTransferPort);
            AddToCommand(command, "serverinstance_max_download_total_bandwidth", MaxDownloadTotalBandwidth);
            AddToCommand(command, "serverinstance_max_upload_total_bandwidth", MaxUploadTotalBandwidth);

            AddToCommand(command, "serverinstance_template_serveradmin_group", TemplateServerAdminGroupId);
            AddToCommand(command, "serverinstance_template_serverdefault_group", TemplateServerDefaultGroupId);
            AddToCommand(command, "serverinstance_template_channeladmin_group", TemplateChannelAdminGroupId);
            AddToCommand(command, "serverinstance_template_channeldefault_group", TemplateChannelDefaultGroupId);

            AddToCommand(command, "serverinstance_serverquery_flood_commands", ServerQueryFloodCommandsCount);
            AddToCommand(command, "serverinstance_serverquery_flood_time", ServerQueryFloodRatingDuration);
            AddToCommand(command, "serverinstance_serverquery_ban_time", ServerQueryBanDuration);
        }

        #endregion
    }
}