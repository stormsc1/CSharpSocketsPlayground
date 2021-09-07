using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;



namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            UDP udp = new UDP(11000);
            Console.WriteLine("Client...");


            while (true) { /* This "idling" thing not very optimal... */ }
        }
    }

    public class Package { }

    public class UDP
    {
        private static UdpClient m_UDPListener = null;

        public UDP(int port)
        {
            m_UDPListener = new UdpClient(port);
            m_UDPListener.BeginReceive(new AsyncCallback(ReceiveData), null);
        }

        public void SendData(IPEndPoint endPoint)
        {
            string tmpData = "data";

            try
            {
                if (endPoint != null)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(tmpData);
                    m_UDPListener.BeginSend(bytes, bytes.Length, endPoint, null, null);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error sending data to {endPoint} via UDP: {exception}");
            }
        }

        public void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = m_UDPListener.EndReceive(asyncResult, ref remoteEndPoint);

                // Start listening for new packages
                m_UDPListener.BeginReceive(new AsyncCallback(ReceiveData), null);

                // Handle data
                Console.WriteLine(Encoding.UTF8.GetString(data));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error receiving UDP data: {exception}");
            }
        }
    }
}
