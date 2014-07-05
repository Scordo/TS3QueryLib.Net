using TS3QueryLib.Core;
using TS3QueryLib.Core.Server;

namespace Connect.BL
{
    public class AppContext
    {
        public AsyncTcpDispatcher Dispatcher { get; set; }
        public QueryRunner QueryRunner { get; set; }
    }
}