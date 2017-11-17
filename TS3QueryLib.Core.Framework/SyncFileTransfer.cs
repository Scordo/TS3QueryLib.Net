using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS3QueryLib.Core.Communication;

namespace TS3QueryLib.Core
{
    public class SyncFileTransfer : FileTransferBase
    {
        public static void UploadFile(string fileTransferKey, string host, ushort filePort, string sourceFileName)
        {
            FileInfo fileInfo = new FileInfo(sourceFileName);

            UploadFile(fileTransferKey, host, filePort, 0, (ulong)fileInfo.Length, sourceFileName);
        }

        public static void UploadFile(string fileTransferKey, string host, ushort filePort, ulong numberOfBytesToSkip, string sourceFileName)
        {
            FileInfo fileInfo = new FileInfo(sourceFileName);
            ulong numberOfBytesToSend = (ulong)fileInfo.Length - numberOfBytesToSkip;

            UploadFile(fileTransferKey, host, filePort, numberOfBytesToSkip, numberOfBytesToSend, sourceFileName);
        }

        public static void UploadFile(string fileTransferKey, string host, ushort filePort, ulong numberOfBytesToSkip, ulong numberOfBytesToSend, string sourceFileName)
        {
            using (FileStream fileStream = File.OpenRead(sourceFileName))
            {
                UploadData(fileTransferKey, host, filePort, numberOfBytesToSkip, numberOfBytesToSend, fileStream);
            }
        }

        public static void UploadData(string fileTransferKey, string host, ushort filePort, ulong numberOfBytesToSkip, ulong numberOfBytesToSend, Stream sourceStream)
        {
            Validate(fileTransferKey, host, filePort, sourceStream);

            if (numberOfBytesToSkip > 0)
                sourceStream.Seek((long)numberOfBytesToSkip, SeekOrigin.Current);

            using (SocketAsyncEventArgs socketAsyncEventArgs = OpenConnection(host, filePort))
            {
                SendFileTransferKey(socketAsyncEventArgs, fileTransferKey);
                ReadFromStreamWriteToSocket(socketAsyncEventArgs, numberOfBytesToSend, sourceStream);

                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken) socketAsyncEventArgs.UserToken;
                userToken.Socket.Close();
            }
        }

        public static void DownloadFile(string fileTransferKey, ulong size, string host, ushort filePort, string targetFileName)
        {
            using (FileStream fileStream = File.Create(targetFileName))
            {
                DownloadFile(fileTransferKey, size, host, filePort, fileStream);
            }
        }

        public static void DownloadFile(string fileTransferKey, ulong size, string host, ushort filePort, Stream downloadStream)
        {
            Validate(fileTransferKey, host, filePort, downloadStream);

            using (SocketAsyncEventArgs socketAsyncEventArgs = OpenConnection(host, filePort))
            {
                SendFileTransferKey(socketAsyncEventArgs, fileTransferKey);
                ReadFromSocketWriteToStream(socketAsyncEventArgs, size, downloadStream);

                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken) socketAsyncEventArgs.UserToken;
                userToken.Socket.Close();
            }
        }

        protected static SocketAsyncEventArgs OpenConnection(string host, ushort port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveBufferSize = 4096 };
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = ResolveEndpoint(host, port), UserToken = new SocketAsyncEventArgsUserToken { Socket = socket } };

            ManualResetEvent connectLock = new ManualResetEvent(false);
            SocketError socketError = SocketError.Success;
            EventHandler<SocketAsyncEventArgs> connectCallback = (sender, args) => { socketError = SocketError.Success; connectLock.Set(); };
            socket.InvokeAsyncMethod(socket.ConnectAsync, connectCallback, socketAsyncEventArgs);
            connectLock.WaitOne();
            socketAsyncEventArgs.Completed -= connectCallback;

            if (socketError != SocketError.Success)
                throw new SocketException((int)socketError);

            return socketAsyncEventArgs;
        }

        protected static void SendFileTransferKey(SocketAsyncEventArgs socketAsyncEventArgs, string fileTransferKey)
        {
            SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
            byte[] messageBytes = Encoding.UTF8.GetBytes(fileTransferKey);
            socketAsyncEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);

            SocketError resultError = SocketError.Success;
            ManualResetEvent sendLock = new ManualResetEvent(false);
            EventHandler<SocketAsyncEventArgs> sendCallback = (sender, args) => { resultError = args.SocketError; sendLock.Set(); };

            userToken.Socket.InvokeAsyncMethod(userToken.Socket.SendAsync, sendCallback, socketAsyncEventArgs);
            sendLock.WaitOne();
            socketAsyncEventArgs.Completed -= sendCallback;

            if (resultError != SocketError.Success)
                throw new SocketException((int)resultError);
        }

        protected static void ReadFromStreamWriteToSocket(SocketAsyncEventArgs socketAsyncEventArgs, ulong numberOfBytesToSend, Stream sourceStream)
        {
            const int BUFFER_SIZE = 4096;
            ulong bytesSent = 0;

            do
            {
                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
                int bytesRead, bytesTransfered = 0;
                byte[] bytesToSend = ReadFromStream(sourceStream, BUFFER_SIZE, out bytesRead);

                if (bytesRead == 0)
                    break;

                socketAsyncEventArgs.SetBuffer(bytesToSend, 0, bytesToSend.Length);

                SocketError resultError = SocketError.Success;
                ManualResetEvent sendLock = new ManualResetEvent(false);
                EventHandler<SocketAsyncEventArgs> sendCallback = (sender, args) => { resultError = args.SocketError; bytesTransfered = args.BytesTransferred; sendLock.Set(); };

                userToken.Socket.InvokeAsyncMethod(userToken.Socket.SendAsync, sendCallback, socketAsyncEventArgs);
                sendLock.WaitOne();
                socketAsyncEventArgs.Completed -= sendCallback;

                if (resultError != SocketError.Success)
                    throw new SocketException((int)resultError);

                bytesSent += (ulong)bytesTransfered;

                if (bytesRead < BUFFER_SIZE)
                    break;
            }
            while (bytesSent < numberOfBytesToSend);

            if (bytesSent < numberOfBytesToSend)
                throw new InvalidOperationException(string.Format("File upload failed and was aborted after sending {0} of {1} bytes.", bytesSent, numberOfBytesToSend));
        }

        protected static void ReadFromSocketWriteToStream(SocketAsyncEventArgs socketAsyncEventArgs, ulong size, Stream downloadStream)
        {
            SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
            ulong bytesRead = 0;

            do
            {
                byte[] sizeBuffer = new byte[4096];
                socketAsyncEventArgs.SetBuffer(sizeBuffer, 0, sizeBuffer.Length);
                SocketError lastError = SocketError.Success;

                ManualResetEvent receiveLock = new ManualResetEvent(false);
                int bytesTransfered = 0;
                EventHandler<SocketAsyncEventArgs> receiveCallback = (sender, args) =>
                {
                    if (args.SocketError != SocketError.Success)
                    {
                        lastError = args.SocketError;
                        receiveLock.Set();
                        return;
                    }

                    bytesTransfered = socketAsyncEventArgs.BytesTransferred;
                    if (bytesTransfered != 0)
                        downloadStream.Write(socketAsyncEventArgs.Buffer, socketAsyncEventArgs.Offset, socketAsyncEventArgs.BytesTransferred);

                    receiveLock.Set();
                };

                userToken.Socket.InvokeAsyncMethod(userToken.Socket.ReceiveAsync, receiveCallback, socketAsyncEventArgs);
                receiveLock.WaitOne();
                socketAsyncEventArgs.Completed -= receiveCallback;

                if (lastError != SocketError.Success)
                    throw new SocketException((int)lastError);

                if (bytesTransfered == 0)
                    break;

                bytesRead += (ulong)bytesTransfered;
            }
            while (bytesRead < size);

            if (bytesRead < size)
                throw new InvalidOperationException(string.Format("File download failed and was aborted after reading {0} of {1} bytes.", bytesRead, size));
        }
    }
}