using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class BanListEntry: IDump
    {
        #region Properties

        public uint Id { get; protected set; }
        public string Ip { get; protected set; }
        public DateTime Created { get; protected set; }
        public string InvokerNickname { get; protected set; }
        public uint InvokerClientDatabaseId { get; protected set; }
        public string InvokerUniqueId { get; protected set; }
        public string Reason { get; protected set; }
        public uint Enforcements { get; protected set; }

        #endregion

        #region Constructor

        private BanListEntry()
        {

        }

        #endregion

        #region Public Methods

        public static BanListEntry Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            return new BanListEntry
            {
                Id = currentParameterGroup.GetParameterValue<uint>("banid"),
                Ip = currentParameterGroup.GetParameterValue("ip"),
                Created = new DateTime(1970, 1, 1).AddSeconds(currentParameterGroup.GetParameterValue<ulong>("created")),
                InvokerNickname = currentParameterGroup.GetParameterValue("invokername"),
                InvokerClientDatabaseId = currentParameterGroup.GetParameterValue<uint>("invokercldbid"),
                InvokerUniqueId = currentParameterGroup.GetParameterValue("invokeruid"),
                Reason = currentParameterGroup.GetParameterValue("reason"),
                Enforcements = currentParameterGroup.GetParameterValue<uint>("enforcements"),
            };
        }

        #endregion
    }
}