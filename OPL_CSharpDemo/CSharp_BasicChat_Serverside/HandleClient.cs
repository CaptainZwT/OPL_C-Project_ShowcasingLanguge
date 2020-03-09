using System;
using System.Net.Sockets;
using System.Threading;

namespace CSharp_BasicChat_Serverside
{
    class HandleClient
    {
        //Socket associated with this remote host / client
        private TcpClient _clientSocket;
        //Name/number of the client
        private string _clientName;

        /// <summary>
        /// Initializes the object and starts the new thread.
        /// </summary>
        /// <param name="clientSocket">TcpClient to talk with client</param>
        /// <param name="clientName">Name of client</param>
        public void StartClient(TcpClient clientSocket, string clientName)
        {
            _clientName = clientName;
            _clientSocket = clientSocket;
            var thread = new Thread(DoChat);
            thread.Start();
        }

        /// <summary>
        /// Executed on a new thread - services messages sent by remote host/client
        /// </summary>
        private void DoChat()
        {
            while (true)
            {
                try
                {
                    string dataFromClient = _clientSocket.ReadString();
                    Program.Broadcast(dataFromClient, _clientName, true);
                    Console.WriteLine(dataFromClient);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
