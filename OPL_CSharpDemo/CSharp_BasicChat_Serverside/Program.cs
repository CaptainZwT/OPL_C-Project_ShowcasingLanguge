using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CSharp_BasicChat_Serverside
{
    class Program
    {
        //This is a dictionary of key,value pairs used to store the names and sockets of each client.
        public static Dictionary<string, TcpClient> ClientList = new Dictionary<string, TcpClient>();

        /// <summary>
        /// Listens for new connections and handles them.
        /// </summary>
        static void Main()
        {
            var serverSocket = new TcpListener(IPAddress.Any, 8888);
            serverSocket.Start();
            Console.WriteLine("JLM OPL C# Chat server started...");
            while (true)
            {
                //This next line of code actually blocks
                var clientSocket = serverSocket.AcceptTcpClient();
                //Somebody connected and sent us data... and no clientSocket doesn't have a method called ReadString: See TcpClientExtension.cs
                string dataFromClient = clientSocket.ReadString();
                //Add the name and StringSocket to the Dictionary object
                ClientList.Add(dataFromClient, clientSocket);
                //Tell everyone that someone new joined!
                Broadcast(dataFromClient + " joined.", dataFromClient, false);
                //Log the fact to the server console
                Console.WriteLine(dataFromClient + " joined the sharp C.");
                //Create a new object to Handle all future incoming messages from this client
                var client = new HandleClient();
                //Start that thread running
                client.StartClient(clientSocket, dataFromClient);
            }
        }

        /// <summary>
        /// A simple broadcast message function that resides here to allow the clients to broadcast
        /// incomming messages to everyone. 
        /// </summary>
        /// <param name="msg">The message to broadcast</param>
        /// <param name="uname">The user's name who sent it</param>
        /// <param name="flag"></param>
        public static void Broadcast(string msg, string uname, bool flag)
        {
            foreach (var item in ClientList)
            {
                var broadcastSocket = item.Value;
                var m = flag ? uname + " says: " + msg : msg;
                item.Value.WriteString(m);
            }
        }
    }
}
