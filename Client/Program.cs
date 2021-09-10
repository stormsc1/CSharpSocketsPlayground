using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            UDP udp = new UDP();
            Console.WriteLine("Client...");

            while (true)
            {
                /* This "idling" thing not very optimal... */
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.D1)
                    {
                        udp.SendData(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000), new Package("Testing 123"));
                    }
                }
            }
        }
    }

    // TODO: Implement properly, this is super temporary
    public class Package 
    {
        string data = "";

        public Package(string message)
        {
            data = message;
        }

        public byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(data);
        }
    }

    public class UDP
    {
        private static UdpClient m_UDPListener = null;

        public UDP()
        {
            m_UDPListener = new UdpClient(11000);
            m_UDPListener.BeginReceive(new AsyncCallback(ReceiveData), null);
        }

        // TODO
        public void SendData(IPEndPoint endPoint, Package package)
        {
            try
            {
                if (endPoint != null)
                {
                    byte[] bytes = package.GetBytes();
                    m_UDPListener.BeginSend(bytes, bytes.Length, endPoint, null, null);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error sending data to {endPoint}: {exception}");
            }
        }

        public void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = m_UDPListener.EndReceive(asyncResult, ref remoteEndPoint);
                m_UDPListener.BeginReceive(new AsyncCallback(ReceiveData), null);

                // TODO: Handle data properly
                Console.WriteLine("[REMOTE]: " + Encoding.UTF8.GetString(data));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error receiving data: {exception}");
            }
        }
    }
}
