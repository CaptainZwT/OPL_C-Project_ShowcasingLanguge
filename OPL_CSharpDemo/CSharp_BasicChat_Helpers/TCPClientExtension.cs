using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_BasicChat_Serverside
{
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
