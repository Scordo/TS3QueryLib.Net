namespace TS3QueryLib.Core.CommandHandling
{
    public enum GroupDatabaseType
    {
        /// <summary>
        /// Used for new virtual servers
        /// </summary>
        Template = 0,
        /// <summary>
        /// Used for regular clients
        /// </summary>
        Regular = 1,
        /// <summary>
        /// Used for ServerQuery clients
        /// </summary>
        Query = 2
    }
}