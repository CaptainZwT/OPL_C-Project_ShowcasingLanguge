using System;
using System.Net.Sockets;
using System.Text;

namespace CSharp_BasicChat_Helpers
{
    // This Extension code is taken from Principles of Networking class
    // I am abstracting it away even though I understand it.
    // It was Written by Scot Anderson, whose website is 
    // https://dra.cs.southern.edu/ at SAU
    // http://www.scotnpatti.com/ personal

    public static class TCPClientExtension
    {
        
        public static void WriteString(this TcpClient tcpClient, string msg)
        {
            msg += '\0';
            byte[] bytes = Encoding.ASCII.GetBytes(msg);
            var stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }

        public static string ReadString(this TcpClient tcpClient)
        {
            var bytes = new byte[tcpClient.ReceiveBufferSize];
            var stream = tcpClient.GetStream();
            stream.Read(bytes, 0, tcpClient.ReceiveBufferSize);
            var msg = Encoding.ASCII.GetString(bytes);
            return msg.Substring(0, msg.IndexOf("\0", StringComparison.Ordinal));
        }
    }
}
