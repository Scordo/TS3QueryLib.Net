using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS3QueryLib.Core.Communication;

namespace TS3QueryLib.Core
{
    public class AsyncFileTransfer : FileTransferBase
    {
        #region Public Methods

        public static void DownloadFile(string fileTransferKey, ulong size, string host, ushort filePort, string targetFileName, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
        {
            FileStream fileStream = File.Create(targetFileName);

            DownloadFile
            (
                fileTransferKey, size, host, filePort, fileStream,
                connectedMethod,
                (a, b, c) =>
                {
                    fileStream.Close();

                    if (errorMethod != null)
                        errorMethod(a, b, c);
                },
                progressMethod,
                a =>
                {
                    fileStream.Close();

                    if (finishedMethod != null)
                        finishedMethod(a);
                },
                s =>
                {
                    if (abortFunction != null && abortFunction(s))
                    {
                        fileStream.Close();
                        return true;
                    }

                    return false;
                }
            );
        }

        public static void DownloadFile(string fileTransferKey, ulong size, string host, ushort filePort, Stream downloadStream, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
        {
            Validate(fileTransferKey, host, filePort, downloadStream);
            AsyncFileTranserHelper fileTransferHelper = new AsyncFileTranserHelper(host, filePort, fileTransferKey, size, downloadStream);
            fileTransferHelper.DownloadFile(connectedMethod, errorMethod, progressMethod, finishedMethod, abortFunction);
        }

        public static void UploadFile(string fileTransferKey, string host, ushort filePort, string sourceFileName, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
        {
            FileInfo fileInfo = new FileInfo(sourceFileName);

            UploadFile(fileTransferKey, host, filePort, 0, (ulong)fileInfo.Length, sourceFileName, connectedMethod, errorMethod, progressMethod, finishedMethod, abortFunction);
        }

        public static void UploadFile(string fileTransferKey, string host, ushort filePort, ulong numberOfBytesToSkip, string sourceFileName, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
        {
            FileInfo fileInfo = new FileInfo(sourceFileName);
            ulong numberOfBytesToSend = (ulong)fileInfo.Length - numberOfBytesToSkip;

            UploadFile(fileTransferKey, host, filePort, numberOfBytesToSkip, numberOfBytesToSend, sourceFileName, connectedMethod, errorMethod, progressMethod, finishedMethod, abortFunction);
        }

        public static void UploadFile(string fileTransferKey, string host, ushort filePort, ulong numberOfBytesToSkip, ulong numberOfBytesToSend, string sourceFileName, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
        {
            FileStream fileStream = File.OpenRead(sourceFileName);

            UploadData
            (
                fileTransferKey, host, filePort, numberOfBytesToSkip, numberOfBytesToSend, fileStream,
                connectedMethod,
                (a, b, c) =>
                {
                    fileStream.Close();

                    if (errorMethod != null)
                        errorMethod(a, b, c);
                },
                progressMethod,
                a =>
                {
                    fileStream.Close();

                    if (finishedMethod != null)
                        finishedMethod(a);
                },
                s =>
                {
                    if (abortFunction != null && abortFunction(s))
                    {
                        fileStream.Close();
                        return true;
                    }

                    return false;
                }
            );
        }

        public static void UploadData(string fileTransferKey, string host, ushort filePort, ulong numberOfBytesToSkip, ulong numberOfBytesToSend, Stream sourceStream, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
        {
            Validate(fileTransferKey, host, filePort, sourceStream);

            if (numberOfBytesToSkip > 0)
                sourceStream.Seek((long) numberOfBytesToSkip, SeekOrigin.Current);

            AsyncFileTranserHelper fileTransferHelper = new AsyncFileTranserHelper(host, filePort, fileTransferKey, numberOfBytesToSend, sourceStream);
            fileTransferHelper.UploadFile(connectedMethod, errorMethod, progressMethod, finishedMethod, abortFunction);
        }

        #endregion

        private class AsyncFileTranserHelper
        {
            #region Constants

            private const int RECEIVE_BUFFER_SIZE = 4 * 1024; // 1 MB
            private const int SEND_BUFFER_SIZE = 4 * 1024; // 1 MB

            #endregion

            #region Properties

            private string Host { get; set; }
            private ushort Port { get; set; }
            private ulong Size { get; set; }
            private ulong Processed { get; set; }
            private Stream DataStream { get; set; }
            private string FileTransferKey { get; set; }
            private Socket Socket { get; set; }
            private SocketAsyncEventArgs SocketAsyncEventArgs { get; set; }

            #endregion

            #region Delegates

            private Action<string> ConnectedMethod { get; set; }
            private Action<string, Exception, SocketError?> ErrorMethod { get; set; }
            private Action<string, ulong, ulong> ProgressMethod { get; set; }
            private Action<string> FinishedMethod { get; set; }
            private Func<string, bool> AbortFunction { get; set; }

            #endregion

            #region Constructor

            public AsyncFileTranserHelper(string host, ushort port, string fileTransferKey, ulong size, Stream dataStream)
            {
                if (dataStream == null)
                    throw new ArgumentNullException("dataStream");

                if (size == 0)
                    throw new ArgumentOutOfRangeException("size", "size must be larger than 0.");

                Host = host;
                Port = port;
                FileTransferKey = fileTransferKey;
                Size = size;
                DataStream = dataStream;
            }

            #endregion

            #region Public Methods

            public void DownloadFile(Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
            {
                HandleFileTransfer(Download_Connected, connectedMethod, errorMethod, progressMethod, finishedMethod, abortFunction);
            }




            public void UploadFile(Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
            {
                HandleFileTransfer(Upload_Connected, connectedMethod, errorMethod, progressMethod, finishedMethod, abortFunction);
            }

            #endregion

            #region Download Part

            private void Download_Connected(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
            {
                socketAsyncEventArgs.Completed -= Download_Connected;

                if (!CheckOperationState(socketAsyncEventArgs))
                    return;

                OnConnected();

                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
                byte[] messageBytes = Encoding.UTF8.GetBytes(FileTransferKey);
                socketAsyncEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);

                userToken.Socket.InvokeAsyncMethod(userToken.Socket.SendAsync, Download_FileTransferKeySent, socketAsyncEventArgs);
            }

            private void Download_FileTransferKeySent(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
            {
                socketAsyncEventArgs.Completed -= Download_FileTransferKeySent;

                if (!CheckOperationState(socketAsyncEventArgs))
                    return;

                OnProgress();

                Download_StartReadingData(socketAsyncEventArgs);
            }

            private void Download_StartReadingData(SocketAsyncEventArgs socketAsyncEventArgs)
            {
                if (AbortFunction != null && AbortFunction(FileTransferKey))
                {
                    CloseConnection();
                    return;
                }

                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
                byte[] sizeBuffer = new byte[RECEIVE_BUFFER_SIZE];
                socketAsyncEventArgs.SetBuffer(sizeBuffer, 0, sizeBuffer.Length);
                userToken.Socket.InvokeAsyncMethod(userToken.Socket.ReceiveAsync, Download_FinishedReadingData, socketAsyncEventArgs);
            }

            private void Download_FinishedReadingData(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
            {
                socketAsyncEventArgs.Completed -= Download_FinishedReadingData;

                if (!CheckOperationState(socketAsyncEventArgs))
                    return;

                ulong bytesTransfered = (ulong)socketAsyncEventArgs.BytesTransferred;
                if (bytesTransfered != 0)
                    DataStream.Write(socketAsyncEventArgs.Buffer, socketAsyncEventArgs.Offset, socketAsyncEventArgs.BytesTransferred);

                Processed += bytesTransfered;
                OnProgress();

                if (bytesTransfered == 0 && Processed < Size)
                {
                    OnError(new InvalidOperationException(string.Format("File download failed and was aborted after reading {0} of {1} bytes.", Processed, Size)));
                    return;
                }

                if (Processed == Size)
                {
                    OnFinished();
                    return;
                }

                Download_StartReadingData(socketAsyncEventArgs);
            }


            #endregion

            #region Upload Part

            private void HandleFileTransfer(EventHandler<SocketAsyncEventArgs> callback, Action<string> connectedMethod, Action<string, Exception, SocketError?> errorMethod, Action<string, ulong, ulong> progressMethod, Action<string> finishedMethod, Func<string, bool> abortFunction)
            {
                ConnectedMethod = connectedMethod;
                ErrorMethod = errorMethod;
                ProgressMethod = progressMethod;
                FinishedMethod = finishedMethod;
                AbortFunction = abortFunction;

                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveBufferSize = RECEIVE_BUFFER_SIZE, SendBufferSize = SEND_BUFFER_SIZE };
                SocketAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = ResolveEndpoint(Host, Port), UserToken = new SocketAsyncEventArgsUserToken { Socket = Socket } };
                Socket.InvokeAsyncMethod(Socket.ConnectAsync, callback, SocketAsyncEventArgs);
            }

            private void Upload_Connected(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
            {
                socketAsyncEventArgs.Completed -= Upload_Connected;

                if (!CheckOperationState(socketAsyncEventArgs))
                    return;

                OnConnected();

                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
                byte[] messageBytes = Encoding.UTF8.GetBytes(FileTransferKey);
                socketAsyncEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);

                userToken.Socket.InvokeAsyncMethod(userToken.Socket.SendAsync, Upload_FileTransferKeySent, socketAsyncEventArgs);
            }

            private void Upload_FileTransferKeySent(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
            {
                socketAsyncEventArgs.Completed -= Upload_FileTransferKeySent;

                if (!CheckOperationState(socketAsyncEventArgs))
                    return;

                OnProgress();

                Upload_StartSendingData(socketAsyncEventArgs);
            }

            private void Upload_StartSendingData(SocketAsyncEventArgs socketAsyncEventArgs)
            {
                if (AbortFunction != null && AbortFunction(FileTransferKey))
                {
                    CloseConnection();
                    return;
                }

                SocketAsyncEventArgsUserToken userToken = (SocketAsyncEventArgsUserToken)socketAsyncEventArgs.UserToken;
                int bytesRead;
                byte[] bytesToSend = ReadFromStream(DataStream, RECEIVE_BUFFER_SIZE, out bytesRead);

                if (bytesRead == 0)
                {
                    OnError(new InvalidOperationException(string.Format("File upload failed and was aborted after sending {0} of {1} bytes.", Processed, Size)));
                    return;
                }

                socketAsyncEventArgs.SetBuffer(bytesToSend, 0, bytesToSend.Length);
                userToken.Socket.InvokeAsyncMethod(userToken.Socket.SendAsync, Upload_FinishedSendingData, socketAsyncEventArgs);
            }

            private void Upload_FinishedSendingData(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
            {
                socketAsyncEventArgs.Completed -= Upload_FinishedSendingData;

                if (!CheckOperationState(socketAsyncEventArgs))
                    return;

                ulong bytesTransfered = (ulong)socketAsyncEventArgs.BytesTransferred;
                Processed += bytesTransfered;
                OnProgress();

                if (bytesTransfered == 0 && Processed < Size)
                {
                    OnError(new InvalidOperationException(string.Format("File upload failed and was aborted after sending {0} of {1} bytes.", Processed, Size)));
                    return;
                }

                if (Processed == Size)
                {
                    OnFinished();

                    return;
                }

                Upload_StartSendingData(socketAsyncEventArgs);
            }

            #endregion

            #region Common Methods

            private bool CheckOperationState(SocketAsyncEventArgs socketAsyncEventArgs)
            {
                if (socketAsyncEventArgs.SocketError != SocketError.Success)
                {
                    OnError(socketAsyncEventArgs.SocketError);
                    return false;
                }

                return true;
            }

            private void CloseConnection()
            {
                if (Socket != null)
                    Socket.Close();

                if (SocketAsyncEventArgs != null)
                {
                    SocketAsyncEventArgs.Dispose();
                    SocketAsyncEventArgs = null;
                }
            }

            private void OnError(SocketError error)
            {
                CloseConnection();

                if (ErrorMethod != null)
                    ThreadPool.QueueUserWorkItem(x => ErrorMethod(FileTransferKey, null, error));
            }

            private void OnError(Exception exception)
            {
                CloseConnection();

                if (ErrorMethod != null)
                    ThreadPool.QueueUserWorkItem(x => ErrorMethod(FileTransferKey, exception, null));
            }

            private void OnConnected()
            {
                if (ConnectedMethod != null)
                    ThreadPool.QueueUserWorkItem(x => ConnectedMethod(FileTransferKey));
            }

            private void OnProgress()
            {
                if (ProgressMethod != null)
                    ThreadPool.QueueUserWorkItem(x => ProgressMethod(FileTransferKey, Processed, Size));
            }

            private void OnFinished()
            {
                CloseConnection();

                if (FinishedMethod != null)
                    ThreadPool.QueueUserWorkItem(x => FinishedMethod(FileTransferKey));
            }

            #endregion
        }
    }
}