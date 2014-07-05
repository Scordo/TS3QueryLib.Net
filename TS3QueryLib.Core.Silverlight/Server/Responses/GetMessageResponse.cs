using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class GetMessageResponse : ResponseBase<GetMessageResponse>
    {
        #region Properties

        public uint MessageId { get; protected set; }
        public string SenderUniqueId { get; protected set; }
        public string Subject { get; protected set; }
        public DateTime Created { get; protected set; }
        public bool WasRead { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            MessageId = list.GetParameterValue<uint>("msgid");
            SenderUniqueId = list.GetParameterValue("cluid");
            Subject = list.GetParameterValue("subject");
            Created = new DateTime(1970, 1, 1).AddSeconds(list.GetParameterValue<ulong>("timestamp"));
            WasRead = list.GetParameterValue("flag_read").ToBool();
        }

        #endregion
    }
}