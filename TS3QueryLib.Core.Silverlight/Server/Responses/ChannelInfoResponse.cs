using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class ChannelInfoResponse : ResponseBase<ChannelInfoResponse>
    {
        #region Properties

		public uint ParentChannelId { get; protected set; }
        public string Name { get; protected set; }
        public string Topic { get; protected set; }
        public string Description { get; protected set; }
        public string PasswordHash { get; protected set; }
        public ushort Codec { get; protected set; }
        public double CodecQuality { get; protected set; }
        public int MaxClients { get; protected set; }
        public int MaxFamilyClients { get; protected set; }
        public uint Order { get; protected set; }
        public bool IsPermanent { get; protected set; }
        public bool IsSemiPermanent { get; protected set; }
        public bool IsDefaultChannel { get; protected set; }
        public bool IsPasswordProtected { get; protected set; }
        public bool IsMaxClientsUnlimited { get; protected set; }
        public bool IsMaxFamilyClientsUnlimited { get; protected set; }
        public bool IsMaxFamilyClientsInherited { get; protected set; }
        public string FilePath { get; protected set; }
        public uint NeededTalkPower { get; protected set; }
        public bool ForcedSilence { get; protected set; }
        public string PhoneticName { get; protected set; }
        public uint IconId { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

			ParentChannelId = list.GetParameterValue<uint>("pid");
            Name = list.GetParameterValue("channel_name");
            Topic = list.GetParameterValue("channel_topic");
            Description = list.GetParameterValue("channel_description");
            PasswordHash = list.GetParameterValue("channel_password");
            Codec = list.GetParameterValue<ushort>("channel_codec");
            CodecQuality = list.GetParameterValue<double>("channel_codec_quality");
            MaxClients = list.GetParameterValue<int>("channel_maxclients");
            MaxFamilyClients = list.GetParameterValue<int>("channel_maxfamilyclients");
            Order = list.GetParameterValue<uint>("channel_order");
            IsDefaultChannel = list.GetParameterValue("channel_flag_default").ToBool();
            IsPasswordProtected = list.GetParameterValue("channel_flag_password").ToBool();
            IsPermanent = list.GetParameterValue("channel_flag_permanent").ToBool();
            IsSemiPermanent = list.GetParameterValue("channel_flag_semi_permanent").ToBool();
            IsMaxClientsUnlimited = list.GetParameterValue("channel_flag_maxclients_unlimited").ToBool();
            IsMaxFamilyClientsUnlimited = list.GetParameterValue("channel_flag_maxfamilyclients_unlimited").ToBool();
            IsMaxFamilyClientsInherited = list.GetParameterValue("channel_flag_maxfamilyclients_inherited").ToBool();
            FilePath = list.GetParameterValue("channel_filepath");
            NeededTalkPower = list.GetParameterValue<uint>("channel_needed_talk_power");
            ForcedSilence = list.GetParameterValue("channel_forced_silence").ToBool();
            PhoneticName = list.GetParameterValue("channel_name_phonetic");
            IconId = list.GetParameterValue<uint>("channel_icon_id");
        }

        #endregion
    }
}