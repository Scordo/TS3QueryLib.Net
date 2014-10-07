using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class MessageEntry: IDump
    {
        #region Properties

        public uint MessageId { get; protected set; }
        public string SenderUniqueId { get; protected set; }
        public string Subject { get; protected set; }
        public DateTime Created { get; protected set; }
        public bool WasRead { get; protected set; }

        #endregion

        #region Constructor

        private MessageEntry()
        {

        }

        #endregion

        #region Public Methods

        public static MessageEntry Parse(CommandParameterGroup parameterGroup)
        {
            if (parameterGroup == null)
                throw new ArgumentNullException("parameterGroup");

            return new MessageEntry
            {
                MessageId = parameterGroup.GetParameterValue<uint>("msgid"),
                SenderUniqueId = parameterGroup.GetParameterValue("cluid"),
                Subject = parameterGroup.GetParameterValue("subject"),
                Created = new DateTime(1970, 1, 1).AddSeconds(parameterGroup.GetParameterValue<ulong>("timestamp")),
                WasRead = parameterGroup.GetParameterValue("flag_read").ToBool(),
            };
        }

        #endregion
    }
}