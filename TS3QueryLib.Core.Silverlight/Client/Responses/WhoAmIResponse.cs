using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Client.Responses
{
    public class WhoAmIResponse : WhoAmIResponseBase<WhoAmIResponse>
    {
        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            ClientId = list.GetParameterValue<uint>("clid");
            ChannelId = list.GetParameterValue<uint>("cid");
        }

        #endregion
    }
}