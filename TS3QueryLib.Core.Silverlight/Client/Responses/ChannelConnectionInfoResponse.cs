using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Client.Responses
{
    public class ChannelConnectionInfoResponse : ResponseBase<ChannelConnectionInfoResponse>
    {
        public string Path { get; protected set; }
        public string Password { get; protected set; }

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            Path = list.GetParameterValue("path");
            Password = list.GetParameterValue("password");
        }
    }
}
