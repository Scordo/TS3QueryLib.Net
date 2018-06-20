using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common.Responses;

namespace TS3QueryLib.Core.Server.Responses
{
    public abstract class ClientInfoBaseResponse<T> : ResponseBase<T> where T : ResponseBase<T>
    {
        public string UniqueId { get; protected set; }
        public string Nickname { get; protected set; }
        public uint DatabaseId { get; protected set; }
        public DateTime LastConnected { get; protected set; }
        public DateTime Created { get; protected set; }
        public uint TotalConnections { get; protected set; }
        public string Description { get; protected set; }
        public ulong MonthBytesUploaded { get; protected set; }
        public ulong MonthBytesDonwloaded { get; protected set; }
        public ulong TotalBytesUploaded { get; protected set; }
        public ulong TotalBytesDownloaded { get; protected set; }
        public uint? IconId { get; protected set; }
        public string HashedUniqueId { get; protected set; }

        protected override void FillFrom(string responseText, params object[] additionalStates)
        {
            CommandParameterGroupList list = CommandParameterGroupList.Parse(BodyText);

            if (list.Count == 0)
                return;

            UniqueId = list.GetParameterValue("client_unique_identifier");
            Nickname = list.GetParameterValue("client_nickname");
            DatabaseId = list.GetParameterValue<uint>("client_database_id");
            Created = new DateTime(1970, 1, 1).AddSeconds(list.GetParameterValue<ulong>("client_created"));
            LastConnected = new DateTime(1970, 1, 1).AddSeconds(list.GetParameterValue<ulong>("client_lastconnected"));
            TotalConnections = list.GetParameterValue<uint>("client_totalconnections");
            Description = list.GetParameterValue("client_description");
            MonthBytesUploaded = list.GetParameterValue<ulong>("client_month_bytes_uploaded");
            MonthBytesDonwloaded = list.GetParameterValue<ulong>("client_month_bytes_downloaded");
            TotalBytesUploaded = list.GetParameterValue<ulong>("client_total_bytes_uploaded");
            TotalBytesDownloaded = list.GetParameterValue<ulong>("client_total_bytes_downloaded");
            IconId = list.GetParameterValue<uint?>("client_icon_id");
            HashedUniqueId = list.GetParameterValue("client_base64HashClientUID");
        }
    }
}
