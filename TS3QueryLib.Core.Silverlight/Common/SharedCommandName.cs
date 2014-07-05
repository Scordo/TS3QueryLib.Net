using TS3QueryLib.Core.CommandHandling;

namespace TS3QueryLib.Core.Common
{
    public enum SharedCommandName
    {
        ChannelList,
        ClientUpdate,
        Quit,
        Use,
        WhoAmI,
    }

    public static class SharedCommandNameExtensions
    {
        public static Command CreateCommand(this SharedCommandName commandName)
        {
            return new Command(commandName);
        }
    }
}