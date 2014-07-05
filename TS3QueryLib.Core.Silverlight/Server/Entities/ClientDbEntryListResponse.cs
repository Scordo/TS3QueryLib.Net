using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ClientDbEntryListResponse : ListResponse<ClientDbEntry>
    {
        public uint? TotalClientsInDatabase { get; protected set; }

        public static ClientDbEntryListResponse Parse(string responseText)
        {
            ClientDbEntryListResponse response = new ClientDbEntryListResponse();
            response.Assign(Parse(responseText, ClientDbEntry.Parse));

            if (!response.IsErroneous)
            {
                CommandParameterGroupList list = CommandParameterGroupList.Parse(response.BodyText);
                CommandParameterGroup firstCommandParameterGroup = list.Count == 0 ? null : list[0];

                if (firstCommandParameterGroup != null)
                    response.TotalClientsInDatabase = firstCommandParameterGroup.GetParameterValue<uint?>("count");
            }

            return response;
        }
    }
}