using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class CreateServerResponse : ResponseBase<CreateServerResponse>
    {
        #region Properties

        public uint ServerId { get; protected set; }
        public ushort ServerPort { get; protected set; }
        public string Token { get; protected set; }

        #endregion

        #region Non Public Methods

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            ServerId = list.GetParameterValue<uint>("sid");
            ServerPort = list.GetParameterValue<ushort>("virtualserver_port");
            Token = list.GetParameterValue("token");
        }

        #endregion
    }
}