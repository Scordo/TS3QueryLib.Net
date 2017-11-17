using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Notification.EventArgs
{
    public class ChannelCreatedEventArgs : System.EventArgs, IDump
    {
        #region Properties

        public int? ChannelId { get; protected set; }
        public int? ParentId { get; protected set; }
        public string Name { get; protected set; }
        public string Topic { get; protected set; }
        public string NamePhonetic { get; protected set; }
        public int? Codec { get; protected set; }
        public int? CodecQuality { get; protected set; }
        public int? Order { get; protected set; }
        public int? FlagPassword { get; protected set; }
        public int? FlagSemiPermanent { get; protected set; }
        public int? DeleteDelay { get; protected set; }
        public int? FlagPermanent { get; protected set; }
        public int? FlagDefault { get; protected set; }
        public int? CodecIsUnencrypted { get; protected set; }
        public int? NeededTalkPower { get; protected set; }
        public int? MaxClients { get; protected set; }
        public int? MaxFamilyClients { get; protected set; }
        public int? FlagMaxClientsUnlimited { get; protected set; }
        public int? FlagMaxFamilyClientsUnlimited { get; protected set; }
        public int? FlagMaxFamilyClientsInherited { get; protected set; }
        public int? InvokerId { get; protected set; }
        public string InvokerName { get; protected set; }
        public string InvokerUniqueId { get; protected set; }

        #endregion

        #region Constructors


        public ChannelCreatedEventArgs(CommandParameterGroupList commandParameterGroupList)
        {
            if (commandParameterGroupList == null)
                throw new ArgumentNullException(nameof(commandParameterGroupList));

            ChannelId = commandParameterGroupList.GetParameterValue<int?>("cid");
            ParentId = commandParameterGroupList.GetParameterValue<int?>("cpid");
            Name = commandParameterGroupList.GetParameterValue<string>("channel_name");
            Topic = commandParameterGroupList.GetParameterValue<string>("channel_topic");
            NamePhonetic = commandParameterGroupList.GetParameterValue<string>("channel_name_phonetic");
            Codec = commandParameterGroupList.GetParameterValue<int?>("channel_codec");
            CodecQuality = commandParameterGroupList.GetParameterValue<int?>("channel_codec_quality");
            FlagPermanent = commandParameterGroupList.GetParameterValue<int?>("channel_flag_permanent");
            FlagDefault = commandParameterGroupList.GetParameterValue<int?>("channel_flag_default");
            NeededTalkPower = commandParameterGroupList.GetParameterValue<int?>("channel_needed_talk_power");
            Order = commandParameterGroupList.GetParameterValue<int?>("channel_order");
            CodecIsUnencrypted = commandParameterGroupList.GetParameterValue<int?>("channel_codec_is_unencrypted");
            FlagMaxFamilyClientsUnlimited = commandParameterGroupList.GetParameterValue<int?>("channel_flag_maxfamilyclients_unlimited");
            FlagMaxFamilyClientsInherited = commandParameterGroupList.GetParameterValue<int?>("channel_flag_maxfamilyclients_inherited");
            InvokerId = commandParameterGroupList.GetParameterValue<int?>("invokerid");
            InvokerName = commandParameterGroupList.GetParameterValue<string>("invokername");
            InvokerUniqueId = commandParameterGroupList.GetParameterValue<string>("invokeruid");
            DeleteDelay = commandParameterGroupList.GetParameterValue<int?>("channel_delete_delay");
            FlagPassword = commandParameterGroupList.GetParameterValue<int?>("channel_flag_password");
            FlagPermanent = commandParameterGroupList.GetParameterValue<int?>("channel_flag_permanent");
            FlagSemiPermanent = commandParameterGroupList.GetParameterValue<int?>("channel_flag_semi_permanent");
            MaxClients = commandParameterGroupList.GetParameterValue<int?>("channel_maxclients");
            MaxFamilyClients = commandParameterGroupList.GetParameterValue<int?>("channel_maxfamilyclients");
            FlagMaxClientsUnlimited = commandParameterGroupList.GetParameterValue<int?>("channel_flag_maxclients_unlimited");
        }

        #endregion
    }
}
