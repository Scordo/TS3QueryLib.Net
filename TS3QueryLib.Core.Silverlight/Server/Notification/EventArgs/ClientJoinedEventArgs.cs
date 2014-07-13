using System;
using System.Collections.Generic;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;


namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ClientJoinedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public uint ClientId { get; protected set; }
        public uint ChannelId { get; protected set; }
        public string ClientUniqueId { get; protected set; }
        public string Nickname { get; protected set; }
        public bool IsClientInputMuted { get; protected set; }
        public bool IsClientOutputMuted { get; protected set; }
        public bool HasClientInputHardware { get; protected set; }
        public bool HasClientOutputHardware { get; protected set; }
        public string MetaData { get; protected set; }
        public bool IsRecording { get; protected set; }
        public uint ClientDatabaseId { get; protected set; }
        public uint ClientChannelGroupId { get; protected set; }
        public List<uint> ServerGroups { get; protected set; }
        public bool IsClientAway { get; protected set; }
        public string ClientAwayMessage { get; protected set; }
        public ushort ClientType { get; protected set; }
        public string Avatar { get; protected set; }
        public uint ClientTalkPower { get; protected set; }
        public bool IsTalkRequested { get; protected set; }
        public string TalkRequestMessage { get; protected set; }
        public string Description { get; protected set; }
        public bool IsClientTalker { get; protected set; }
        public bool IsPrioritySpeaker { get; protected set; }
        public uint UnreadMessages { get; protected set; }
        public string NicknamePhonetic { get; protected set; }
        public uint NeededServerQueryViewPower { get; protected set; }
        public uint IconId { get; protected set; }

        #endregion

        #region Constructor

        internal ClientJoinedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException("commandParameterGroupList");

            ClientId = commandParameterGroupList.GetParameterValue<uint>("clid");
            ChannelId = commandParameterGroupList.GetParameterValue<uint>("ctid");
            ClientUniqueId = commandParameterGroupList.GetParameterValue("client_unique_identifier");
            Nickname = commandParameterGroupList.GetParameterValue("client_nickname");
            IsClientInputMuted = commandParameterGroupList.GetParameterValue("client_input_muted").ToBool();
            IsClientOutputMuted = commandParameterGroupList.GetParameterValue("client_output_muted").ToBool();
            HasClientInputHardware = commandParameterGroupList.GetParameterValue("client_input_hardware").ToBool();
            HasClientOutputHardware = commandParameterGroupList.GetParameterValue("client_output_hardware").ToBool();
            MetaData = commandParameterGroupList.GetParameterValue("client_meta_data");
            IsRecording = commandParameterGroupList.GetParameterValue("client_is_recording").ToBool();
            ClientDatabaseId = commandParameterGroupList.GetParameterValue<uint>("client_database_id");
            ClientChannelGroupId = commandParameterGroupList.GetParameterValue<uint>("client_channel_group_id");
            ServerGroups = commandParameterGroupList.GetParameterValue("client_servergroups").ToIdList();
            IsClientAway = commandParameterGroupList.GetParameterValue("client_away").ToBool();
            ClientAwayMessage = commandParameterGroupList.GetParameterValue("client_away_message");
            ClientType = commandParameterGroupList.GetParameterValue<ushort>("client_type");
            Avatar = commandParameterGroupList.GetParameterValue("client_flag_avatar");
            ClientTalkPower = commandParameterGroupList.GetParameterValue<uint>("client_talk_power");
            IsTalkRequested = commandParameterGroupList.GetParameterValue("client_talk_request").ToBool();
            TalkRequestMessage = commandParameterGroupList.GetParameterValue("client_talk_request_msg");
            Description = commandParameterGroupList.GetParameterValue("client_description");
            IsClientTalker = commandParameterGroupList.GetParameterValue("client_is_talker").ToBool();
            IsPrioritySpeaker = commandParameterGroupList.GetParameterValue("client_is_priority_speaker").ToBool();
            UnreadMessages = commandParameterGroupList.GetParameterValue<uint>("client_unread_messages");
            NicknamePhonetic = commandParameterGroupList.GetParameterValue("client_nickname_phonetic");
            NeededServerQueryViewPower = commandParameterGroupList.GetParameterValue<uint>("client_needed_serverquery_view_power");
            IconId = commandParameterGroupList.GetParameterValue<uint>("client_icon_id");
        }

        #endregion
    }
}