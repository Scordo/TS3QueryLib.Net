using System;
using System.Net.Sockets;

namespace TS3QueryLib.Core.Communication
{
    public class SocketErrorEventArgs : EventArgs
    {
        public SocketError SocketError
        {
            get; private set;
        }

        public SocketErrorEventArgs(SocketError socketError)
        {
            SocketError = socketError;
        }
    }
}