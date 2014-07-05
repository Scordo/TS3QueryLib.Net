using System.ServiceProcess;

namespace TS3ServiceWrapper
{
    public partial class MainService : ServiceBase
    {
        private HostEngine Host { get; set; }

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Host = new HostEngine();
            Host.Start();
        }

        protected override void OnStop()
        {
            Host.Stop();
        }
    }
}