using System;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Server;
using TS3QueryLib.Core.Server.Responses;

namespace Connect
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ConnectButton_OnClick(object sender, EventArgs e)
        {
            ResultLabel.Text = null;
            ErrorLabel.Text = null;

            string host = ServerAddressTextBox.Text.Trim();
            ushort port = Convert.ToUInt16(ServerPortTextBox.Text);

            try
            {
                using (QueryRunner queryRunner = new QueryRunner(new SyncTcpDispatcher(host, port)))  // host and port
                {
                    // connection to the TS3-Server is established with the first query command

                    VersionResponse versionResponse = queryRunner.GetVersion();

                    if (versionResponse.IsErroneous)
                    {
                        ErrorLabel.Text = "Could not get server version: " + versionResponse.ErrorMessage;
                        return;
                    }

                    ResultLabel.Text = string.Format("Server version:<br>Platform: {0}<br>Version: {1}<br>Build: {2}", versionResponse.Platform, versionResponse.Version, versionResponse.Build);
                }

            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
            }
        }
    }
}