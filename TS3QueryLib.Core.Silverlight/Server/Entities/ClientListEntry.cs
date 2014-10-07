using System;
using System.Collections.Generic;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ClientListEntry : IDump
    {
        #region Properties

        public uint ClientId { get; set; }
        public uint ChannelId { get; set; }
        public uint ClientDatabaseId { get; set; }
        public string Nickname { get; set; }
        public ushort ClientType { get; protected set; }
        public string ClientUniqueId { get; set; }
        public bool? IsClientAway { get; set; }
        public string ClientAwayMessage { get; set; }
        public List<uint> ServerGroups { get; set; }
        public uint? ClientChannelGroupId { get; set; }
        public bool? IsClientTalking { get; set; }
        public bool? IsClientTalker { get; set; }
        public bool? IsClientInputMuted { get; set; }
        public bool? IsClientOutputMuted { get; set; }
        public bool? HasClientInputHardware { get; set; }
        public bool? HasClientOutputHardware { get; set; }
        public bool? IsClientInputDeactivated { get; set; }
        public uint? ClientTalkPower { get; set; }
        public string ClientVersion { get; set; }
        public string ClientPlatform { get; set; }
        public TimeSpan? ClientIdleDuration { get; set; }
        public bool? IsPrioritySpeaker { get; set; }
        public bool? IsClientRecording { get; set; }
        public bool? IsChannelCommander { get; set; }
        public uint? ClientIconId { get; set; }
        public string ClientCountry { get; set; }
        public DateTime? ClientCreated { get; set; }
        public DateTime? ClientLastConnected { get; set; }
        public string ClientIP { get; set; }

        #endregion

        #region Constructor

        private ClientListEntry()
        {

        }

        #endregion

        #region Public Methods

        public static ClientListEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            uint? idleSeconds = currentParameterGroup.GetParameterValue<uint?>("client_idle_time");
            ulong? created = currentParameterGroup.GetParameterValue<ulong?>("client_created");
            ulong? lastConnected = currentParameterGroup.GetParameterValue<ulong?>("client_lastconnected");

            return new ClientListEntry
            {
                ClientId = currentParameterGroup.GetParameterValue<uint>("clid"),
                ChannelId = currentParameterGroup.GetParameterValue<uint>("cid"),
                ClientDatabaseId = currentParameterGroup.GetParameterValue<uint>("client_database_id"),
                Nickname = currentParameterGroup.GetParameterValue("client_nickname"),
                ClientType = currentParameterGroup.GetParameterValue<ushort>("client_type"),
                ClientUniqueId = currentParameterGroup.GetParameterValue("client_unique_identifier"),
                IsClientAway = currentParameterGroup.GetParameterValue("client_away").ToNullableBool(),
                ClientAwayMessage = currentParameterGroup.GetParameterValue("client_away_message"),
                ServerGroups = currentParameterGroup.GetParameterValue("client_servergroups").ToIdList(),
                ClientChannelGroupId = currentParameterGroup.GetParameterValue<uint?>("client_channel_group_id"),
                IsClientTalking = currentParameterGroup.GetParameterValue("client_flag_talking").ToNullableBool(),
                IsClientTalker = currentParameterGroup.GetParameterValue("client_is_talker").ToNullableBool(),
                IsClientInputMuted = currentParameterGroup.GetParameterValue("client_input_muted").ToNullableBool(),
                IsClientOutputMuted = currentParameterGroup.GetParameterValue("client_output_muted").ToNullableBool(),
                HasClientInputHardware = currentParameterGroup.GetParameterValue("client_input_hardware").ToNullableBool(),
                HasClientOutputHardware = currentParameterGroup.GetParameterValue("client_output_hardware").ToNullableBool(),
                IsClientInputDeactivated = currentParameterGroup.GetParameterValue("client_input_deactivated").ToNullableBool(),
                ClientTalkPower = currentParameterGroup.GetParameterValue<uint?>("client_talk_power"),
                ClientVersion = currentParameterGroup.GetParameterValue("client_version"),
                ClientPlatform = currentParameterGroup.GetParameterValue("client_platform"),
                ClientIdleDuration = idleSeconds.HasValue ? (TimeSpan?)TimeSpan.FromMilliseconds(idleSeconds.Value) : null,
                IsPrioritySpeaker = currentParameterGroup.GetParameterValue("client_is_priority_speaker").ToNullableBool(),
                IsClientRecording = currentParameterGroup.GetParameterValue("client_is_recording").ToNullableBool(),
                ClientIconId = currentParameterGroup.GetParameterValue<uint?>("client_icon_id"),
                IsChannelCommander = currentParameterGroup.GetParameterValue("CLIENT_IS_CHANNEL_COMMANDER").ToNullableBool(),
                ClientCountry = currentParameterGroup.GetParameterValue("client_country"),
                ClientCreated = created.HasValue ? (DateTime?) new DateTime(1970, 1, 1).AddSeconds(created.Value) : null,
                ClientLastConnected = lastConnected.HasValue ? (DateTime?)new DateTime(1970, 1, 1).AddSeconds(lastConnected.Value) : null,
                ClientIP = currentParameterGroup.GetParameterValue("connection_client_ip"),
            };
        }

        #endregion
    }
}
