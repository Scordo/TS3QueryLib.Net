using Connect.BL;

namespace Connect
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
