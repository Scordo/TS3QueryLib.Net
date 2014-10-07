using System;
using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Server.Entities
{
    public class ServerListItem : ServerListItemBase
    {
        #region Properties

        public string UniqueId { get; protected set; }
        public uint? ServerNumberOfClientsOnline { get; protected set; }
        public uint? ServerNumberOfQueryClientsOnline { get; protected set; }
        public uint? ServerMaximumClientsAllowed { get; protected set; }
        public TimeSpan? ServerUptime { get; protected set; }
        public string ServerName { get; protected set; }
        public bool ServerAutoStart { get; protected set; }
        public string ServerMachineId { get; protected set; }


        #endregion

        #region Constructors

        private ServerListItem()
        {

        }

        #endregion

        #region Public Methods

        public static ServerListItem Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            string statusString = currentParameterGroup.GetParameterValue("virtualserver_status");
            VirtualServerStatus status = VirtualServerStatusHelper.Parse(statusString);
            uint? uptime = currentParameterGroup.GetParameterValue<uint?>("virtualserver_uptime");

            return new ServerListItem
            {
                UniqueId = currentParameterGroup.GetParameterValue("virtualserver_unique_identifier"),
                ServerId = currentParameterGroup.GetParameterValue<uint>("virtualserver_id"),
                ServerPort = currentParameterGroup.GetParameterValue<ushort>("virtualserver_port"),
                ServerStatus = status,
                ServerNumberOfClientsOnline = currentParameterGroup.GetParameterValue<uint?>("virtualserver_clientsonline"),
                ServerNumberOfQueryClientsOnline = currentParameterGroup.GetParameterValue<uint?>("virtualserver_queryclientsonline"),
                ServerMaximumClientsAllowed = currentParameterGroup.GetParameterValue<uint?>("virtualserver_maxclients"),
                ServerUptime = uptime.HasValue ? (TimeSpan?) TimeSpan.FromSeconds(uptime.Value) : null,
                ServerName = currentParameterGroup.GetParameterValue("virtualserver_name"),
                ServerAutoStart = currentParameterGroup.GetParameterValue("virtualserver_autostart") == "1",
                ServerMachineId = currentParameterGroup.GetParameterValue("virtualserver_machine_id"),
            };
        }

        #endregion
    }
}