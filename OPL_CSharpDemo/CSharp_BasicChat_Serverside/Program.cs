using CSharp_BasicChat_Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CSharp_BasicChat_Serverside
{
    class Program
    {
        //This is a dictionary of key,value pairs used to store the names and sockets of each client.
        public static Dictionary<string, TcpClient> ClientList = new Dictionary<string, TcpClient>();

        private static bool active;

        private static TcpListener serverSocket;

        /// <summary>
        /// Listens for new connections and handles them.
        /// </summary>
        static void Main()
        {
            serverSocket = new TcpListener(IPAddress.Any, 8888);
            serverSocket.Start();
            Console.WriteLine("OPL Chat server started...");

            active = true;

            var thread0 = new Thread(GetConnection);
            thread0.Start();

            var thread1 = new Thread(GetCommand);
            thread1.Start();
        }

        private static void GetCommand()
        {
            while (active)
            {
                string inp = Console.ReadLine();

                if ((inp.Length > 5) && (inp.Substring(0, 4) == "/say"))
                    // transmits a message to all connected users
                {
                    Broadcast(inp.Substring(4), "Server", true);
                    Console.WriteLine("[" + DateTime.Now + "] (Server):" + inp.Substring(5));
                }
                else if ((inp.Substring(0, 5) == "/list"))
                    // lists the users connected
                {
                    string msg = "";
                    if (ClientList.Count > 0)
                    {
                        foreach (string name in ClientList.Keys)
                        {
                            msg += name + "\n";
                        }
                    }
                    else
                    {
                        msg = "No clients currently connected.";
                    }

                    Console.WriteLine(msg);
                }
                else if ((inp.Substring(0, 6) == "/clear"))
                {
                    Console.Clear();
                }
            }
        }

        private static void GetConnection()
        {
            while (active)
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
                var m = flag ? DateTime.Now + " [" + uname + "]: " + msg : msg;
                item.Value.WriteString("#(" + m);
            }
        }
    }
}
