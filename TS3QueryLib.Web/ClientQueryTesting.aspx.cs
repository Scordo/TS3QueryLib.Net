using System;
using System.Text;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Client;
using TS3QueryLib.Core.Common;

namespace TS3QueryLib.Web
{
    public partial class ClientQueryTesting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            RunQueries(queryRunner =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<pre>");
                sb.Append(queryRunner.SendWhoAmI().GetDumpString(true));
                sb.Append(queryRunner.GetChannelList(true).GetDumpString(true));
                sb.Append("</pre>");

                AppendToOutput(sb.ToString());
            });
            Response.Write((DateTime.Now - start));
        }

        protected void RunQueries(Action<QueryRunner> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");


            using (QueryRunner queryRunner = new QueryRunner(new SyncTcpDispatcher("localhost", 25639)))  // host and port
            {
                // connection to the TS3-Server is established with the first query command

                action(queryRunner);
            }
        }

        private void AppendToOutput(IDump dump)
        {
            AppendToOutput(dump.GetDumpString());
        }

        private void AppendToOutput(string message)
        {
            Response.Write(string.Format("<pre>{0}</pre><br>", message));
        }
    }
}