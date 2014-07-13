using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Responses
{
    public class ClientDbInfoResponse : ClientInfoBaseResponse<ClientDbInfoResponse>
    {
        public string LastIP { get; protected set; }

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            base.FillFrom(responseText, additionalStates);

            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);
            LastIP = list.GetParameterValue("client_lastip");
        }
    }
}
