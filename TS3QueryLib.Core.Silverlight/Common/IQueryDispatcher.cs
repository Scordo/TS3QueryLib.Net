using System;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Common
{
    public interface IQueryDispatcher: IDisposable
    {
        bool IsDisposed { get; }
        string Dispatch(string commandText);
        int? LastServerConnectionHandlerId { get; }
        event EventHandler<EventArgs<string>> NotificationReceived;
        event EventHandler<EventArgs<SimpleResponse>> BanDetected;
    }
}