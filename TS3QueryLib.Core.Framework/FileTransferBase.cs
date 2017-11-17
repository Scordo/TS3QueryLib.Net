using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace TS3QueryLib.Core
{
    public class FileTransferBase
    {
        protected static EndPoint ResolveEndpoint(string host, ushort port)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(host);

            if (hostEntry.AddressList.Length == 0)
                throw new InvalidOperationException(string.Format("Could not resolve host: {0}", host));

            IPAddress ipV4 = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (ipV4 == null)
                throw new InvalidOperationException("Could not find a network device with an ip-v4-address.");

            return new IPEndPoint(ipV4, port);
        }

        protected static void Validate(string fileTransferKey, string host, ushort filePort, Stream stream)
        {
            if (fileTransferKey == null)
                throw new ArgumentNullException();

            if (fileTransferKey.Length != 32)
                throw new ArgumentOutOfRangeException("fileTransferKey", "fileTransferKey must have a length of 32 characters");

            if (host == null)
                throw new ArgumentNullException("host");

            if (host.Trim().Length == 0)
                throw new ArgumentException("host is empty", "host");

            if (stream == null)
                throw new ArgumentNullException("stream");
        }

        protected static byte[] ReadFromStream(Stream stream, int size, out int readBytes)
        {
            byte[] buffer = new byte[size];
            readBytes = stream.Read(buffer, 0, size);

            if (readBytes == size)
                return buffer;

            byte[] result = new byte[readBytes];
            Array.Copy(buffer, result, readBytes);
            return result;
        }
    }
}
