using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public class OwnPermissionResponse : ResponseBase<OwnPermissionResponse>
    {
        public uint PermissionId { get; protected set; }
        public string PermissionName { get; protected set; }
        public int PermissionValue { get; protected set; }

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            PermissionId = list.GetParameterValue<uint>("permid");
            PermissionName = list.GetParameterValue("permsid");
            PermissionValue = list.GetParameterValue<int>("permvalue");
        }
    }
}