using TS3QueryLib.Core;
using TS3QueryLib.Core.Server;

namespace Connect.BL
{
    public class AppContext
    {
        private static AppContext _instance;

        public AsyncTcpDispatcher Dispatcher { get; set; }
        public QueryRunner QueryRunner { get; set; }

        public static AppContext Instance
        {
            get { return _instance ?? (_instance = new AppContext()); }
        }
    }
}