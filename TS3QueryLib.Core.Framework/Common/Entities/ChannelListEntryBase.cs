using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Common.Entities
{
    public abstract class ChannelListEntryBase : IDump
    {
        #region Non Public Members

        private bool _spacerInfoChecked;
        private SpacerInfo _spacerInfo;

        #endregion

        #region Properties

        #region Always returned Properties

        public uint ChannelId { get; protected set; }
        public uint ParentChannelId { get; protected set; }
        public uint Order { get; protected set; }
        public string Name { get; protected set; }
        public int TotalClients { get; protected set; }

        public bool IsSpacer { get { return SpacerInfo != null; } }
        public SpacerInfo SpacerInfo
        {
            get
            {
                if (_spacerInfoChecked)
                    return _spacerInfo;

                _spacerInfoChecked = true;
                _spacerInfo = SpacerInfo.Parse(Name, ParentChannelId);

                return _spacerInfo;
            }
        }

        #endregion

        #region Topic-Properties

        public string Topic { get; protected set; }

        #endregion

        #region Flags-Properties

        public bool? IsDefaultChannel { get; protected set; }
        public bool? IsPasswordProtected { get; protected set; }
        public bool? IsPermanent { get; protected set; }
        public bool? IsSemiPermanent { get; protected set; }

        #endregion

        #region Voice-Properties

        public ushort? Codec { get; protected set; }
        public double? CodecQuality { get; protected set; }
        public uint? NeededTalkPower { get; protected set; }

        #endregion

        #region Limits-Properties

        public int? MaxClients { get; protected set; }
        public int? MaxFamilyClients { get; protected set; }

        #endregion

        #region Icon-Properties

        public uint? ChannelIconId { get; protected set; }

        #endregion

        #region "SecondsEmpty-Properties"

        public TimeSpan? ChannelEmptyDuration { get; protected set; }

        #endregion

        #endregion

        #region Public Methods

        protected void FillFrom(CommandParameterGroup currrentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currrentParameterGroup == null)
                throw new ArgumentNullException("currrentParameterGroup");


            ChannelId = currrentParameterGroup.GetParameterValue<uint>("cid");
            ParentChannelId = currrentParameterGroup.GetParameterValue<uint>("pid");
            Order = currrentParameterGroup.GetParameterValue<uint>("channel_order");
            Name = currrentParameterGroup.GetParameterValue("channel_name");
            TotalClients = currrentParameterGroup.GetParameterValue<int>("total_clients");

            Topic = currrentParameterGroup.GetParameterValue("channel_topic");

            IsDefaultChannel = currrentParameterGroup.GetParameterValue<byte?>("channel_flag_default").ToNullableBool();
            IsPasswordProtected = currrentParameterGroup.GetParameterValue<byte?>("channel_flag_password").ToNullableBool();
            IsPermanent = currrentParameterGroup.GetParameterValue<byte?>("channel_flag_permanent").ToNullableBool();
            IsSemiPermanent = currrentParameterGroup.GetParameterValue<byte?>("channel_flag_semi_permanent").ToNullableBool();

            Codec = currrentParameterGroup.GetParameterValue<ushort?>("channel_codec");
            CodecQuality = currrentParameterGroup.GetParameterValue<double?>("channel_codec_quality");
            NeededTalkPower = currrentParameterGroup.GetParameterValue<uint?>("channel_needed_talk_power");

            MaxClients = currrentParameterGroup.GetParameterValue<int?>("channel_maxclients");
            MaxFamilyClients = currrentParameterGroup.GetParameterValue<int?>("channel_maxfamilyclients");
            ChannelIconId = currrentParameterGroup.GetParameterValue<uint?>("channel_icon_id");

            // Seconds empty
            int? emptySeconds = currrentParameterGroup.GetParameterValue<int?>("seconds_empty");
            ChannelEmptyDuration = emptySeconds.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(emptySeconds.Value) : null;
        }

        #endregion
    }
}