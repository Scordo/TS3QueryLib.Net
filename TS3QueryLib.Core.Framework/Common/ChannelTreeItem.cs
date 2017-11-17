using System.Collections.Generic;
using TS3QueryLib.Core.Server.Entities;

namespace TS3QueryLib.Core.Common
{
    public class ChannelTreeItem
    {
        public ChannelListEntry Channel { get; set; }
        public ChannelListEntry ParentChannel { get; set; }
        public List<ChannelTreeItem> Children { get; protected set; }
        public List<ClientListEntry> Clients { get; protected set; }

        public ChannelTreeItem(ChannelListEntry value, ChannelListEntry parent)
        {
            Channel = value;
            ParentChannel = ParentChannel;
            Children = new List<ChannelTreeItem>();
            Clients = new List<ClientListEntry>();
        }
    }
}