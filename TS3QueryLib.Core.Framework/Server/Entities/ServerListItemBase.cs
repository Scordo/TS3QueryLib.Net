using System;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public enum VirtualServerStatus { Online, Virtual, OtherInstance,  None }

    public static class VirtualServerStatusHelper
    {
        public static VirtualServerStatus Parse(string value)
        {
            switch (value)
            {
                case "online":
                    return VirtualServerStatus.Online;
                case "virtual":
                    return VirtualServerStatus.Virtual;
                case "other_instance":
                    return VirtualServerStatus.OtherInstance;
                default:
                    return VirtualServerStatus.None;
            }
        }
    }

    public class ServerListItemBase : IDump
    {
        #region Properties

        public uint ServerId { get; protected set; }
        public ushort ServerPort { get; protected set; }
        public VirtualServerStatus ServerStatus { get; protected set; }

        #endregion

        #region Constructors

        protected ServerListItemBase()
        {

        }

        #endregion

        #region Public Methods

        public static ServerListItemBase Parse(CommandParameterGroup currentParameterGroup, CommandParameterGroup firstParameterGroup)
        {
            if (currentParameterGroup == null)
                throw new ArgumentNullException("currentParameterGroup");

            string statusString = currentParameterGroup.GetParameterValue("virtualserver_status");
            VirtualServerStatus status = VirtualServerStatusHelper.Parse(statusString);

            return new ServerListItemBase
            {
                ServerId = currentParameterGroup.GetParameterValue<uint>("virtualserver_id"),
                ServerPort = currentParameterGroup.GetParameterValue<ushort>("virtualserver_port"),
                ServerStatus = status,
            };
        }

        #endregion
    }
}