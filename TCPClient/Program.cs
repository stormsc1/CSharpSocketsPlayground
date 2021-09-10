using System;
using System.Net;
using System.Net.Sockets;

namespace TCPClient
{
    public class Client
    {
        public static int DataBufferSize = 4096;
        public TCP Tcp;

        public Client()
        {
            Tcp = new TCP();
        }

        public void ConnectToServer()
        {
            Tcp.Connect("127.0.0.1", 29650);
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream m_NetworkStream;
            private byte[] m_ReceiveBuffer;

            public void Connect(string serverIP, int port)
            {
                socket = new TcpClient();
                socket.ReceiveBufferSize = DataBufferSize;
                socket.SendBufferSize = DataBufferSize;

                m_ReceiveBuffer = new byte[DataBufferSize];
                socket.BeginConnect(serverIP, port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult asyncResult)
            {
                socket.EndConnect(asyncResult);

                if (!socket.Connected)
                {
                    return;
                }

                m_NetworkStream = socket.GetStream();
                m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult asyncResult)
            {
                try
                {
                    int byteLength = m_NetworkStream.EndRead(asyncResult);
                    if (byteLength <= 0)
                    {
                        // TODO: disconnect
                        return;
                    }

                    byte[] data = new byte[DataBufferSize];
                    Array.Copy(m_ReceiveBuffer, data, byteLength);

                    // TODO: Handle data

                    m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error receiving TCP data: {exception}");
                    // TODO: disconnect
                }
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.ConnectToServer();

            Console.ReadKey();
        }
    }
}
