using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Entities;

namespace TS3QueryLib.Core.Client.Entities
{
    public class ClientModification : ClientModificationBase
    {
        #region Properties

        /// <summary>
        ///  Set us away or back available
        /// </summary>
        public bool? IsAway { get; set; }
        /// <summary>
        /// What away message to display when away
        /// </summary>
        public string AwayMessage { get; set; }
        /// <summary>
        /// Mutes or unmutes microphone
        /// </summary>
        public bool? IsInputMuted { get; set; }
        /// <summary>
        /// Mutes or unmutes speakers/headphones
        /// </summary>
        public bool? IsOutputMuted { get; set; }
        /// <summary>
        /// Same as IsInputMuted, but invisible to other clients
        /// </summary>
        public bool? IsInputDeactivated { get; set; }
        /// <summary>
        /// Sets or removes channel commander
        /// </summary>
        public bool? IsChannelCommander { get; set; }
        /// <summary>
        /// Set your phonetic nickname
        /// </summary>
        public string PhoneticNickname { get; set; }
        /// <summary>
        /// Set your avatar
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// Any string that is passed to all clients that have vision of you.
        /// </summary>
        public string Metadata { get; set; }
        /// <summary>
        /// Privilege key to be used for the next server connect
        /// </summary>
        public string DefaultToken { get; set; }

        #endregion

        #region Public Methods

        public override void AddToCommand(Command command)
        {
            base.AddToCommand(command);

            AddToCommand(command, "client_away", IsAway);
            AddToCommand(command, "client_away_message", AwayMessage);
            AddToCommand(command, "client_input_muted", IsInputMuted);
            AddToCommand(command, "client_output_muted", IsOutputMuted);
            AddToCommand(command, "client_input_deactivated", IsInputDeactivated);
            AddToCommand(command, "client_is_channel_commander", IsChannelCommander);
            AddToCommand(command, "client_nickname_phonetic", PhoneticNickname);
            AddToCommand(command, "client_flag_avatar", Avatar);
            AddToCommand(command, "client_meta_data", Metadata);
            AddToCommand(command, "client_default_token", Metadata);
        }

        #endregion
    }
}