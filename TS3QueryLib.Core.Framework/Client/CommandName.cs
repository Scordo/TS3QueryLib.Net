using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Client
{
    public enum CommandName
    {
        Auth,
        ChannelConnectInfo,
        ClientNotifyRegister,
        ServerConnectionHandlerList,
    }

    public static class CommandNameExtensions
    {
        public static Command CreateCommand(this CommandName commandName)
        {
            return new Command(commandName);
        }
    }
}