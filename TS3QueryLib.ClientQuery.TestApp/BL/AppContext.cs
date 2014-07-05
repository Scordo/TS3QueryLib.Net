using TS3QueryLib.Core;
using TS3QueryLib.Core.Client;

namespace TS3QueryLib.ClientQuery.TestApp.BL
{
    public class AppContext
    {
        public AsyncTcpDispatcher Dispatcher { get; set; }
        public QueryRunner QueryRunner { get; set; }
    }
}