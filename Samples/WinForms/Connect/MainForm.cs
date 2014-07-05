using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Connect.BL;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;
using TS3QueryLib.Core.Server;
using TS3QueryLib.Core.Server.Responses;

namespace Connect
{
    public partial class MainForm : Form
    {

        private static AsyncTcpDispatcher QueryDispatcher
        {
            get { return AppContext.Instance.Dispatcher; }
            set { AppContext.Instance.Dispatcher = value; } 
        } 

        private static QueryRunner QueryRunner
        {
            get { return AppContext.Instance.QueryRunner; }
            set { AppContext.Instance.QueryRunner = value; }
        } 

        // Constructor
        public MainForm()
        {
            InitializeComponent();
            UpdateUI(ConnectionState.Disconnected);
        }


        private void ConnectButton_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        public void Connect()
        {
            // do not connect when already connected or during connection establishing
            if (QueryDispatcher != null)
                return;

            if (string.IsNullOrWhiteSpace(ServerAddressTextBox.Text))
            {
                MessageBox.Show("Please provide a server address!");
                return;
            }

            ushort port;


            if (!ushort.TryParse(ServerPort.Text, out port))
            {
                MessageBox.Show("Please provide a valid query port!");
                return;
            }

            UpdateUI(ConnectionState.Connecting);
            QueryDispatcher = new AsyncTcpDispatcher(ServerAddressTextBox.Text.Trim(), port);
            QueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
            QueryDispatcher.ReadyForSendingCommands += QueryDispatcher_ReadyForSendingCommands;
            QueryDispatcher.ServerClosedConnection += QueryDispatcher_ServerClosedConnection;
            QueryDispatcher.SocketError += QueryDispatcher_SocketError;
            QueryDispatcher.Connect();
        }

        private void QueryDispatcher_ReadyForSendingCommands(object sender, EventArgs e)
        {
            UpdateUI(ConnectionState.Connected);
            // you can only run commands on the queryrunner when this event has been raised first!
            QueryRunner = new QueryRunner(QueryDispatcher);

            VersionResponse versionResponse = QueryRunner.GetVersion();

            if (versionResponse.IsErroneous)
            {
                MessageBox.Show("Could not get server version: " + versionResponse.ErrorMessage);
                return;
            }

            MessageBox.Show(string.Format("Server version:\n\nPlatform: {0}\nVersion: {1}\nBuild: {2}", versionResponse.Platform, versionResponse.Version, versionResponse.Build));
        }

        private void QueryDispatcher_ServerClosedConnection(object sender, EventArgs e)
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

        public void Disconnect()
        {
            // QueryRunner disposes the Dispatcher too
            if (QueryRunner != null)
                QueryRunner.Dispose();

            QueryDispatcher = null;
            QueryRunner = null;
            UpdateUI(ConnectionState.Disconnected);
        }

        private void UpdateUI(ConnectionState state)
        {
            ConnectButton.Enabled = state == ConnectionState.Disconnected;
            DisconnectButton.Enabled = state == ConnectionState.Connected;
        }
    }
}
