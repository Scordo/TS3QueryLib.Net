using System;
using System.Net.Sockets;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;
using TS3QueryLib.Core.Server;
using TS3QueryLib.Core.Server.Entities;
using TS3QueryLib.Core.Server.Responses;

namespace Connect
{
    class Program
  {
    #region Program Start

    static void Main()
    {
      const string serverAddress = "localhost";
      const ushort port = 10011;

      new Program(serverAddress, port).Run();
    }

    #endregion

    private AsyncTcpDispatcher QueryDispatcher { get; set; }
    private QueryRunner QueryRunner { get; set; }
    public string ServerAddress { get; set; }
    public ushort Port { get; set; }

    public Program(string serverAddress, ushort port)
    {
      ServerAddress = serverAddress;
      Port = port;
    }

    private void Run()
    {
      Connect();
      Console.ReadLine();
    }

    private void Connect()
    {
      QueryDispatcher = new AsyncTcpDispatcher(ServerAddress, Port);
      QueryDispatcher.BanDetected += QueryDispatcher_BanDetected;
      QueryDispatcher.ReadyForSendingCommands += QueryDispatcher_ReadyForSendingCommands;
      QueryDispatcher.ServerClosedConnection += QueryDispatcher_ServerClosedConnection;
      QueryDispatcher.SocketError += QueryDispatcher_SocketError;
      QueryDispatcher.Connect();
    }

    private void QueryDispatcher_ReadyForSendingCommands(object sender, System.EventArgs e)
    {
      // you can only run commands on the queryrunner when this event has been raised first!
      QueryRunner = new QueryRunner(QueryDispatcher);
        SimpleResponse loginResponse = QueryRunner.Login("serveradmin", "RWkzzXu9");
        SimpleResponse selectVirtualServerById = QueryRunner.SelectVirtualServerById(1);
        ChannelModification cm = new ChannelModification
            {
                Name = "randomName",
                Description = "desc-test",
                IsPermanent = true
            };
            var response = QueryRunner.CreateChannel(cm);

            VersionResponse versionResponse = QueryRunner.GetVersion();

      if (versionResponse.IsErroneous)
      {
        Console.WriteLine("Could not get server version: " + versionResponse.ErrorMessage);
        return;
      }

      Console.WriteLine("Server version:\n\nPlatform: {0}\nVersion: {1}\nBuild: {2}", versionResponse.Platform, versionResponse.Version, versionResponse.Build);
    }

    private void QueryDispatcher_ServerClosedConnection(object sender, System.EventArgs e)
    {
      // this event is raised when the connection to the server is lost.
      Console.WriteLine("Connection to server closed/lost.");

      // dispose
      Disconnect();
    }

    private void QueryDispatcher_BanDetected(object sender, EventArgs<SimpleResponse> e)
    {
      Console.WriteLine("You're account was banned!\nError-Message: {0}\nBan-Message:{1}", e.Value.ErrorMessage, e.Value.BanExtraMessage);

      // force disconnect
      Disconnect();
    }

    private void QueryDispatcher_SocketError(object sender, SocketErrorEventArgs e)
    {
      // do not handle connection lost errors because they are already handled by QueryDispatcher_ServerClosedConnection
      if (e.SocketError == SocketError.ConnectionReset)
        return;

      // this event is raised when a socket exception has occured
      Console.WriteLine("Socket error!! Error Code: " + e.SocketError);

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
    }
  }
}
