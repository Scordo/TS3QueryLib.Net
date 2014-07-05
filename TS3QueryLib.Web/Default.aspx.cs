using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Text;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Server;
using TS3QueryLib.Core.Server.Entities;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        /* things to check for RC1:
+ serveredit and serverinfo command can be used with use 0 to modify and display
  virtualserver template values (serverinfo display is different from normal output!)
         */

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            RunQueries(queryRunner =>
            {
                queryRunner.SelectVirtualServerById(1); // select server with id 1 and show a dump-output of the response in a textbox

                List<ChannelTreeItem> channelTree = queryRunner.Utils.GetChannelTree();
                StringBuilder sb = new StringBuilder();
                sb.Append("<pre>");
                AddChannelTree(channelTree, 0, sb);
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

            string host = ConfigurationManager.AppSettings["host"];

            if (host == null)
                throw new ConfigurationErrorsException("host could not be found in web.config");

            string portString = ConfigurationManager.AppSettings["port"];

            if (portString == null)
                throw new ConfigurationErrorsException("port could not be found in web.config");

            ushort port;

            if (!ushort.TryParse(portString, NumberStyles.Integer, CultureInfo.InvariantCulture, out port))
                throw new ConfigurationErrorsException("port in web.config has invalid an invalid format.");

            string login = ConfigurationManager.AppSettings["login"];
            string password = ConfigurationManager.AppSettings["password"];


            using (QueryRunner queryRunner = new QueryRunner(new SyncTcpDispatcher(host, port)))  // host and port
            {
                // connection to the TS3-Server is established with the first query command

                if (!login.IsNullOrTrimmedEmpty() && !password.IsNullOrTrimmedEmpty())
                {
                    SimpleResponse loginResponse = queryRunner.Login(login, password); // login using the provided username and password and show a dump-output of the response in a textbox

                    if (loginResponse.IsErroneous)
                        throw new NotSupportedException("Login failed for the given username and password!");
                }

                action(queryRunner);
            }
        }

        private static void AddChannelTree(IEnumerable<ChannelTreeItem> channelTree, int depth, StringBuilder result)
        {
            foreach (ChannelTreeItem channelTreeItem in channelTree)
            {
                result.Append(string.Empty.PadLeft(depth, '\t') + "@" + channelTreeItem.Channel.Name +"\n");
                AddChannelTree(channelTreeItem.Children, depth + 1, result);

                foreach (ClientListEntry clientListEntry in channelTreeItem.Clients)
                {
                    result.Append(string.Empty.PadLeft(depth + 1, '\t') + "-" + clientListEntry.Nickname + "\n");    
                }
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