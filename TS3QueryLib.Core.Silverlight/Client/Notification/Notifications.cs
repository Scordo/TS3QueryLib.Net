using System;
using System.Collections.Generic;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Client.Notification.EventArgs;
using TS3QueryLib.Core.Common.Notification;

namespace TS3QueryLib.Core.Client.Notification
{
    /// <summary>
    /// This class handles the notifications sent by the teamspeak-client and raises type safe events for each notification
    /// </summary>
    public class Notifications : NotificationsBase
    {
        #region Events

        public event EventHandler<TalkStatusEventArgs> TalkStatusChanged;
        public event EventHandler<TalkStatusEventArgsBase> ChannelTalkStatusChanged;
        public event EventHandler<TalkStatusEventArgsBase> WisperTalkStatusChanged;

        #endregion

        #region Constructor

        internal Notifications(QueryRunner queryRunner) : base(queryRunner)
        {

        }

        #endregion

        #region Non Public Methods

        protected override Dictionary<string, Action<CommandParameterGroupList>> GetNotificationHandlers()
        {
            return new  Dictionary<string, Action<CommandParameterGroupList>>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "notifytalkstatuschange", HandleTalkStatusChange },
            };
        }

        private void HandleTalkStatusChange(CommandParameterGroupList parameterGroupList)
        {
            TalkStatusEventArgs eventArgs = new TalkStatusEventArgs(parameterGroupList);

            OnTalkStatusChanged(eventArgs);

            if (eventArgs.IsWisper)
                OnWisperTalkStatusChanged(eventArgs);
            else
                OnChannelTalkStatusChanged(eventArgs);
        }

        protected void OnTalkStatusChanged(TalkStatusEventArgs args)
        {
            if (TalkStatusChanged != null)
                TalkStatusChanged(this, args);
        }

        protected void OnChannelTalkStatusChanged(TalkStatusEventArgsBase args)
        {
            if (ChannelTalkStatusChanged != null)
                ChannelTalkStatusChanged(this, args);
        }

        protected void OnWisperTalkStatusChanged(TalkStatusEventArgsBase args)
        {
            if (WisperTalkStatusChanged != null)
                WisperTalkStatusChanged(this, args);
        }

        #endregion
    }
}