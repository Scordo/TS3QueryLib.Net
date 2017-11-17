using TS3QueryLib.Core.CommandHandling;
using System;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class InstanceInfoResponse : ResponseBase<InstanceInfoResponse>
    {
        #region Properties

        public uint DatabaseVersion { get; protected set; }
        public uint GuestServerQueryGroupId { get; protected set; }
        public uint TemplateServerAdminGroupId { get; protected set; }
        public uint TemplateServerDefaultGroupId { get; protected set; }
        public uint TemplateChannelAdminGroupId { get; protected set; }
        public uint TemplateChannelDefaultGroupId { get; protected set; }
        public ushort FileTransferPort { get; protected set; }
        public ulong MaxDownloadTotalBandWidth { get; protected set; }
        public ulong MaxUploadTotalBandWidth { get; protected set; }

        public uint ServerQueryFloodCommandsCount { get; protected set; }
        public TimeSpan ServerQueryFloodRatingDuration { get; protected set; }
        public TimeSpan ServerQueryBanDuration { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            DatabaseVersion = list.GetParameterValue<uint>("SERVERINSTANCE_DATABASE_VERSION");
            GuestServerQueryGroupId = list.GetParameterValue<uint>("SERVERINSTANCE_GUEST_SERVERQUERY_GROUP");
            TemplateServerAdminGroupId = list.GetParameterValue<uint>("SERVERINSTANCE_TEMPLATE_SERVERADMIN_GROUP");
            TemplateServerDefaultGroupId = list.GetParameterValue<uint>("serverinstance_template_serverdefault_group");
            TemplateChannelAdminGroupId = list.GetParameterValue<uint>("serverinstance_template_channeladmin_group");
            TemplateChannelDefaultGroupId = list.GetParameterValue<uint>("serverinstance_template_channeldefault_group");
            FileTransferPort = list.GetParameterValue<ushort>("SERVERINSTANCE_FILETRANSFER_PORT");
            MaxDownloadTotalBandWidth = list.GetParameterValue<ulong>("SERVERINSTANCE_MAX_DOWNLOAD_TOTAL_BANDWIDTH");
            MaxUploadTotalBandWidth = list.GetParameterValue<ulong>("SERVERINSTANCE_MAX_UPLOAD_TOTAL_BANDWIDTH");

            ServerQueryFloodCommandsCount = list.GetParameterValue<uint>("serverinstance_serverquery_flood_commands");
            ServerQueryFloodRatingDuration = TimeSpan.FromSeconds(list.GetParameterValue<double>("serverinstance_serverquery_flood_time"));
            ServerQueryBanDuration = TimeSpan.FromSeconds(list.GetParameterValue<double>("serverinstance_serverquery_ban_time"));
        }

        #endregion
    }
}