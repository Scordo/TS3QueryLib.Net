using System.Net.Sockets;

namespace TS3QueryLib.Core.Communication
{
    public class SocketAsyncEventArgsUserToken
    {
        public Socket Socket { get; set; }
        public string Message { get; set; }

        public SocketAsyncEventArgsUserToken()
        {
            Reset();
        }

        public void Reset()
        {
            Message = string.Empty;
        }

        public void AppenToMessage(string text)
        {
            Message = string.Concat(Message, text);
        }
    }
}