using System.ComponentModel;

namespace Connect.BL
{
    public class MainPageModel : INotifyPropertyChanged
    {
        #region Members

        private bool _canConnect;
        private string _serverAddress;
        private ushort? _serverPort;
        private ConnectionState _connectionState = ConnectionState.Disconnected;
        #endregion

        #region Properties

        

        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set
            {
                if (_connectionState == value)
                    return;

                _connectionState = value;
                OnPropertyChanged("ConnectionState");
                OnPropertyChanged("CanConnect");
                OnPropertyChanged("CanDisconnect");
            }
        }

        public bool CanConnect
        {
            get { return ConnectionState == ConnectionState.Disconnected; }
        }

        public bool CanDisconnect
        {
            get { return ConnectionState == ConnectionState.Connected; }
        }

        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                if (_serverAddress == value)
                    return;

                _serverAddress = value;

                OnPropertyChanged("ServerAddress");
            }
        }

        public ushort? ServerPort
        {
            get { return _serverPort; }
            set
            {
                if (_serverPort == value)
                    return;

                _serverPort = value;

                OnPropertyChanged("ServerPort");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
