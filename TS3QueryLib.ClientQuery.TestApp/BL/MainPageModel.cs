using System.ComponentModel;

namespace TS3QueryLib.ClientQuery.TestApp.BL
{
    public class MainPageModel : INotifyPropertyChanged
    {
        #region Members

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
