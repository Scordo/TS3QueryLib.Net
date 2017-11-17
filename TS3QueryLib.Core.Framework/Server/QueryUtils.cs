using System;
using System.Collections.Generic;
using TS3QueryLib.Core.Common;
using System.Linq;
using TS3QueryLib.Core.Server.Entities;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Server
{
    public class QueryUtils
    {
        #region Properties

        private QueryRunner QueryRunner { get; set; }
        private List<ChannelListEntry> CachedChannelList { get; set; }
        private List<ClientListEntry> CachedClientList { get; set; }

        #endregion

        #region Constructor

        internal QueryUtils(QueryRunner queryRunner)
        {
            if (queryRunner == null)
                throw new ArgumentNullException("queryRunner");

            QueryRunner = queryRunner;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a tree of channels and contained clients by calling GetChannelList() and GetClientList() on QueryRunner.
        /// </summary>
        public List<ChannelTreeItem> GetChannelTree()
        {
            return GetChannelTree(false);
        }

        /// <summary>
        /// Returns a tree of channels and contained clients by calling GetChannelList() and GetClientList() on QueryRunner when useCachedData is set to false. Otherwise the data retrieved by previous calls is used.
        /// </summary>
        /// <param name="useCachedData">whether to use cached data from previous calls or to query the data again to get fresh data</param>
        /// <returns></returns>
        public List<ChannelTreeItem> GetChannelTree(bool useCachedData)
        {
            if (!useCachedData || CachedChannelList == null || CachedClientList == null)
            {
                CachedChannelList = QueryRunner.GetChannelList(true).Values;
                CachedClientList = QueryRunner.GetClientList(true).Values;
            }

            return GetChannelTree(CachedChannelList, CachedClientList);
        }

        public static List<ChannelTreeItem> GetChannelTree(IEnumerable<ChannelListEntry> channelListEntries)
        {
            return GetChannelTree(channelListEntries, null);
        }

        /// <summary>
        /// Returns a list of channels in a tree like fashion. if a clientlist is provided, the clients of the appropriate channel will eb added to the channeltreeitem.
        /// You cna use this method to display the structure of a virtual server with channels and users
        /// </summary>
        /// <param name="channelListEntries">The list of channels from GetChannelList-Method</param>
        /// <param name="clientListEntries">The optional list of clients from GetClientList-Method</param>
        public static List<ChannelTreeItem> GetChannelTree(IEnumerable<ChannelListEntry> channelListEntries, IEnumerable<ClientListEntry> clientListEntries)
        {
            List<ChannelListEntry> allChannels = new List<ChannelListEntry>(channelListEntries);
            List<ClientListEntry> allClients = new List<ClientListEntry>(clientListEntries ?? new ClientListEntry[] { });
            List<ChannelTreeItem> result = new List<ChannelTreeItem>();
            AddChildren(null, result, allChannels, allClients);

            return result;
        }

        /// <summary>
        ///  Get the filename of the avatar of the client with the provided clientId
        /// </summary>
        /// <param name="clientId">the id of the client</param>
        public string GetAvatarFilename(uint clientId)
        {
            return GetAvatarFilename(QueryRunner.GetClientInfo(clientId));
        }

        /// <summary>
        /// Get the filename of the avatar using the provided ClientInfoResponse
        /// </summary>
        /// <param name="clientInfoResponse">The ClientInfoResponse which you can get by calling QueryRunner.GetClientInfo(..)</param>
        public static string GetAvatarFilename(ClientInfoResponse clientInfoResponse)
        {
            if (clientInfoResponse == null)
                throw new ArgumentNullException("clientInfoResponse");

            if (clientInfoResponse.Avatar.IsNullOrTrimmedEmpty())
                return null;

            return GetAvatarFilename(clientInfoResponse.HashedUniqueId);
        }

        /// <summary>
        /// Get the filename of the avatar using the clients hashed unique id
        /// </summary>
        /// <param name="hashedUniqueClientId">The hashed unique id of the client which you can get by calling QueryRunner.GetClientInfo(..).HashedUniqueId</param>
        public static string GetAvatarFilename(string hashedUniqueClientId)
        {
            return hashedUniqueClientId.IsNullOrTrimmedEmpty() ? null : string.Format("/avatar_{0}", hashedUniqueClientId);
        }

        #endregion

        #region Non Public Methods

        private static void AddChildren(ChannelTreeItem parentChannelTreeItem, List<ChannelTreeItem> channelTree, List<ChannelListEntry> remainingChannels, List<ClientListEntry> remainingClients)
        {
            uint parentChannelId = parentChannelTreeItem == null ? 0 : parentChannelTreeItem.Channel.ChannelId;
            List<ChannelTreeItem> targetChannelList = parentChannelTreeItem == null ? channelTree : parentChannelTreeItem.Children;

            List<ChannelListEntry> children = new List<ChannelListEntry>();

            for (int i = remainingChannels.Count - 1; i >= 0; i--)
            {
                ChannelListEntry channel = remainingChannels[i];

                if (channel.ParentChannelId != parentChannelId)
                    continue;

                children.Add(channel);
                remainingChannels.RemoveAt(i);
            }

            SortChannels(children);

            foreach (ChannelListEntry channel in children)
            {
                ChannelTreeItem channelTreeItem = new ChannelTreeItem(channel, parentChannelTreeItem == null ? null : parentChannelTreeItem.ParentChannel);
                targetChannelList.Add(channelTreeItem);

                if (remainingClients.Count > 0)
                {
                    List<ClientListEntry> clients = remainingClients.Where(c => c.ChannelId == channelTreeItem.Channel.ChannelId).ToList();
                    clients.Sort(SortUser);
                    channelTreeItem.Clients.AddRange(clients);
                }

                AddChildren(channelTreeItem, null, remainingChannels, remainingClients);
            }
        }

        private static void SortChannels(List<ChannelListEntry> channels)
        {
            if (channels.Count < 2)
                return;

            Dictionary<uint, ChannelListEntry> channelDict = new Dictionary<uint, ChannelListEntry>();
            channels.ForEach(c => channelDict[c.Order] = c);

            if (!channelDict.ContainsKey(0))
                return;

            ChannelListEntry lastAddedChannelEntry = channelDict[0];
            List<ChannelListEntry> temporaryChannels = new List<ChannelListEntry> { lastAddedChannelEntry };

            do
            {
                ChannelListEntry nextSiblingChannelEntry;

                if (!channelDict.TryGetValue(lastAddedChannelEntry.ChannelId, out nextSiblingChannelEntry))
                    break;

                temporaryChannels.Add(nextSiblingChannelEntry);
                lastAddedChannelEntry = nextSiblingChannelEntry;
            }
            while (true);

            if (temporaryChannels.Count != channels.Count)
                return;

            channels.Clear();
            channels.AddRange(temporaryChannels);
        }

        private static int SortUser(ClientListEntry client1, ClientListEntry client2)
        {
            if (client1.ClientType != client2.ClientType)
                return client1.ClientType == 1 ? 1 : -1;

            if (client1.ClientTalkPower != client2.ClientTalkPower)
                return (int)client2.ClientTalkPower - (int)client1.ClientTalkPower;

            if (client1.IsClientTalker != client2.IsClientTalker)
                return client1.IsClientTalker.Value ? 1 : -1;

            if (client1.IsClientInputMuted != client2.IsClientInputMuted)
                return client1.IsClientInputMuted.Value ? 1 : -1;

            return string.Compare(client1.Nickname, client2.Nickname);
        }

        #endregion
    }
}