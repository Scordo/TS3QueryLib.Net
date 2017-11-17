using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class ClientGetUidFromClidResponse : ResponseBase<ClientGetUidFromClidResponse>
    {
        #region Properties

        public string ClientUniqueId { get; protected set; }
        public string NickName { get; protected set; }
        public uint ClientId { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            ClientUniqueId = list.GetParameterValue("cluid");
            NickName = list.GetParameterValue("name");
            ClientId = list.GetParameterValue<uint>("clid");
        }

        #endregion
    }
}