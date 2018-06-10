using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Entities;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ChannelModification : ModificationBase
    {
        #region Properties

        public string Name { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public Codec? Codec { get; set; }
        public double? CodecQuality { get; set; }
        public int? MaxClients { get; set; }
        public int? MaxFamilyClients { get; set; }
        public uint? ChannelOrder { get; set; }
        public bool? IsPermanent { get; set; }
        public bool? IsSemiPermanent { get; set; }
        public bool? IsTemporary { get; set; }
        public bool? IsDefault { get; set; }
        public bool? HasUnlimitedMaxClients { get; set; }
        public bool? HasUnlimitedMaxFamilyClients { get; set; }
        public bool? ArUnlimitedMaxFamilyClientsInherited { get; set; }
        public uint? NeededTalkPower { get; set; }
        public string NamePhonetic { get; set; }
        public uint? IconId { get; set; }
        public uint? ParentChannelId { get; set; }

        #endregion

        #region Public Methods

        public void AddToCommand(Command command)
        {
            AddToCommand(command, "channel_name", Name);
            AddToCommand(command, "channel_topic", Topic);
            AddToCommand(command, "channel_description", Description);
            AddToCommand(command, "channel_password", Password);

            if (Codec.HasValue)
                AddToCommand(command, "channel_codec", (uint)Codec.Value);

            AddToCommand(command, "channel_codec_quality", CodecQuality);
            AddToCommand(command, "channel_maxclients", MaxClients);
            AddToCommand(command, "channel_maxfamilyclients", MaxFamilyClients);
            AddToCommand(command, "channel_order", ChannelOrder);

            AddToCommand(command, "channel_flag_permanent", IsPermanent);
            AddToCommand(command, "channel_flag_semi_permanent", IsSemiPermanent);
            AddToCommand(command, "channel_flag_temporary", IsTemporary);
            AddToCommand(command, "channel_flag_default", IsDefault);
            AddToCommand(command, "channel_flag_maxclients_unlimited", HasUnlimitedMaxClients);
            AddToCommand(command, "channel_flag_maxfamilyclients_unlimited", HasUnlimitedMaxFamilyClients);
            AddToCommand(command, "channel_flag_maxfamilyclients_inherited", ArUnlimitedMaxFamilyClientsInherited);
            AddToCommand(command, "channel_needed_talk_power", NeededTalkPower);
            AddToCommand(command, "channel_name_phonetic", NamePhonetic);
            AddToCommand(command, "channel_icon_id", IconId);
            AddToCommand(command, "cpid", ParentChannelId);
        }

        #endregion
    }

    public enum Codec
    {
        /// <summary>
        /// speex narrowband mono, 16bit, 8kHz)
        /// </summary>
        SpeexNarrowband = 0,

        /// <summary>
        /// speex wideband (mono, 16bit, 16kHz)
        /// </summary>
        SpeexWideband = 1,

        /// <summary>
        /// speex ultra-wideband (mono, 16bit, 32kHz)
        /// </summary>
        SpeexUltrawideband = 2,

        /// <summary>
        /// celt mono (mono, 16bit, 48kHz)
        /// </summary>
        CeltMono = 3,

        /// <summary>
        /// opus voice
        /// </summary>
        OpusVoice = 4,

        /// <summary>
        /// opus music
        /// </summary>
        OpusMusic = 5
    }
}