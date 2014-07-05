using TS3QueryLib.ClientQuery.TestApp.BL;

namespace TS3QueryLib.ClientQuery.TestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly AppContext _appContext = new AppContext();

        public static App Currennt
        {
            get { return (App)Current; }
        }

        public AppContext Context
        {
            get { return _appContext; }
        }
    }
}