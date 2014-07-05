using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Core.Server.Entities
{
    public class NamedPermissionLight : IDump
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}