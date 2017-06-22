using System;
using System.Net.Sockets;
using System.Windows;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Client;
using TS3QueryLib.Core.Client.Entities;
using TS3QueryLib.Core.Client.Notification.EventArgs;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;
using TS3QueryLib.ClientQuery.TestApp.BL;

namespace TS3QueryLib.ClientQuery.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static AsyncTcpDispatcher QueryDispatcher
        {
            get { return App.Currennt.Context.Dispatcher; }
            set { App.Currennt.Context.Dispatcher = value; }
        }

        private static QueryRunner QueryRunner
        {
            get { return App.Currennt.Context.QueryRunner; }
            set { App.Currennt.Context.QueryRunner = value; }
        }

        private MainPageModel Model { get; set; }

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            Model = new MainPageModel();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;
            
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        public void Connect()
        {
            // do not connect when already connected or during connection establishing
            if (QueryDispatcher != null)
                return;

            Model.ConnectionState = ConnectionState.Connecting;
            QueryDispatcher = new AsyncTcpDispatcher("localhost", 25639);
            QueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
            QueryDispatcher.ReadyForSendingCommands += QueryDispatcher_ReadyForSendingCommands;
            QueryDispatcher.ServerClosedConnection += QueryDispatcher_ServerClosedConnection;
            QueryDispatcher.SocketError += QueryDispatcher_SocketError;
            QueryDispatcher.NotificationReceived += QueryDispatcher_NotificationReceived;
            QueryDispatcher.Connect();
        }

        private void QueryDispatcher_ReadyForSendingCommands(object sender, System.EventArgs e)
        {
            Model.ConnectionState = ConnectionState.Connected;

            string apiKey = Microsoft.VisualBasic.Interaction.InputBox("Api-Key:", "Please provide your API-Key");
            
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Disconnect();
                return;
            }
            
            // you can only run commands on the queryrunner when this event has been raised first!
            QueryRunner = new QueryRunner(QueryDispatcher);
            SimpleResponse authResponse = QueryRunner.Authenticate(apiKey);

            if (authResponse.IsErroneous)
            {
                MessageBox.Show("Api-Key was wrong!");
                Disconnect();
                return;
            }

            QueryRunner.Notifications.ChannelTalkStatusChanged += Notifications_ChannelTalkStatusChanged;
            QueryRunner.RegisterForNotifications(ClientNotifyRegisterEvent.Any);

            MessageBox.Show("Ready");
        }

        private void QueryDispatcher_ServerClosedConnection(object sender, System.EventArgs e)
        {
            // this event is raised when the connection to the server is lost.
            MessageBox.Show("Connection to server closed/lost.");

            // dispose
            Disconnect();
        }

        private void QueryDispatcher_BanDetected(object sender, EventArgs<SimpleResponse> e)
        {
            MessageBox.Show(string.Format("You're account was banned!\nError-Message: {0}\nBan-Message:{1}", e.Value.ErrorMessage, e.Value.BanExtraMessage));

            // force disconnect
            Disconnect();
        }

        private void QueryDispatcher_SocketError(object sender, SocketErrorEventArgs e)
        {
            // do not handle connection lost errors because they are already handled by QueryDispatcher_ServerClosedConnection
            if (e.SocketError == SocketError.ConnectionReset)
                return;

            // this event is raised when a socket exception has occured
            MessageBox.Show("Socket error!! Error Code: " + e.SocketError);

            // force disconnect
            Disconnect();
        }

        private void QueryDispatcher_NotificationReceived(object sender, EventArgs<string> e)
        {
            LogTextBox.AppendText(e.Value);
            LogTextBox.ScrollToEnd();
        }

        private void Notifications_ChannelTalkStatusChanged(object sender, TalkStatusEventArgsBase e)
        {
            LogTextBox.AppendText(e.GetDumpString());
            LogTextBox.ScrollToEnd();
        }

        public void Disconnect()
        {
            // QueryRunner disposes the Dispatcher too
            if (QueryRunner != null)
                QueryRunner.Dispose();

            QueryDispatcher = null;
            QueryRunner = null;
            Model.ConnectionState = ConnectionState.Disconnected;
        }
    }
}
