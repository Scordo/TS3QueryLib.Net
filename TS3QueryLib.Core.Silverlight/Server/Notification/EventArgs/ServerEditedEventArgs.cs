using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ServerEditedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint ReasonId { get; protected set; }
        public uint InvokerId { get; protected set; }
        public string InvokerName { get; protected set; }
        public string InvokerUniqueId { get; protected set; }

        public string Name { get; set; }
        public uint? DefaultServerGroupId { get; set; }
        public uint? DefaultChannelGroupId { get; set; }
        public double? PrioritySpeakerDimmModification { get; set; }
        public string HostBannerUrl { get; set; }
        public string HostBannerGraphicsUrl { get; set; }
        public uint? HostBannerGraphicsInterval { get; set; }
        public string HostButtonGraphicsUrl { get; set; }
        public string HostButtonTooltip { get; set; }
        public string HostButtonUrl { get; set; }
        public HostBannerMode? HostBannerMode { get; set; }

        public string PhoneticName { get; set; }
        public uint? IconId { get; set; }

        #endregion

        #region Constructors


        public ServerEditedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException(nameof(commandParameterGroupList));

            ReasonId = commandParameterGroupList.GetParameterValue<uint>("reasonid");
            InvokerId = commandParameterGroupList.GetParameterValue<uint>("invokerid");
            InvokerName = commandParameterGroupList.GetParameterValue<string>("invokername");
            InvokerUniqueId = commandParameterGroupList.GetParameterValue<string>("invokeruid");

            Name = commandParameterGroupList.GetParameterValue<string>("virtualserver_name");
            DefaultServerGroupId = commandParameterGroupList.GetParameterValue<uint?>("virtualserver_default_server_group");
            DefaultChannelGroupId = commandParameterGroupList.GetParameterValue<uint?>("virtualserver_default_channel_group");
            HostBannerUrl = commandParameterGroupList.GetParameterValue<string>("virtualserver_hostbanner_url");
            HostBannerGraphicsUrl = commandParameterGroupList.GetParameterValue<string>("virtualserver_hostbanner_gfx_url");

            HostBannerGraphicsInterval = commandParameterGroupList.GetParameterValue<uint?>("virtualserver_hostbanner_gfx_interval");
            PrioritySpeakerDimmModification = commandParameterGroupList.GetParameterValue<double?>("virtualserver_priority_speaker_dimm_modificator");
            HostButtonTooltip = commandParameterGroupList.GetParameterValue("virtualserver_hostbutton_tooltip");
            HostButtonUrl = commandParameterGroupList.GetParameterValue("virtualserver_hostbutton_url");
            PhoneticName = commandParameterGroupList.GetParameterValue("virtualserver_name_phonetic");
            IconId = commandParameterGroupList.GetParameterValue<uint?>("virtualserver_icon_id");
            HostButtonGraphicsUrl = commandParameterGroupList.GetParameterValue<string>("virtualserver_hostbutton_gfx_url");
            HostBannerMode = (HostBannerMode?)commandParameterGroupList.GetParameterValue<uint?>("virtualserver_hostbanner_mode");
        }

        #endregion
    }
}