using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace TCPServer
{
    class Client
    {
        public static int DataBufferSize = 4096;
        public int ID;
        public TCP Tcp;

        public Client(int clientId)
        {
            ID = clientId;
            Tcp = new TCP(ID);
        }

        public class TCP
        {
            public TcpClient TcpSocket;

            private readonly int m_ID;
            private NetworkStream m_NetworkStream;
            private byte[] m_ReceiveBuffer;

            public TCP(int id)
            {
                m_ID = id;
            }

            public void Connect(TcpClient socket)
            {
                TcpSocket = socket;
                TcpSocket.ReceiveBufferSize = DataBufferSize;
                TcpSocket.SendBufferSize = DataBufferSize;

                m_NetworkStream = TcpSocket.GetStream();
                m_ReceiveBuffer = new byte[DataBufferSize];

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

    class Server
    {
        public int MaxPlayers { get; private set; }
        public int Port { get; private set; }
        public Dictionary<int, Client> Clients = new Dictionary<int, Client>();
        public TcpListener Tcp;

        public void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("Starting server");

            // Initialize server data
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }

            Tcp = new TcpListener(IPAddress.Any, Port);
            Tcp.Start();
            Tcp.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        }

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = Tcp.EndAcceptTcpClient(result);
            Tcp.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine("Incomming connection");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if(Clients[i].Tcp.TcpSocket == null)
                {
                    Clients[i].Tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine("Server is full");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start(8, 29650);

            Console.ReadKey();
        }
    }
}
