using CSharp_BasicChat_Serverside;
using System;
using System.Net.Sockets;
using System.Threading;

namespace CSharp_BasicChat_Clientside
{
    public class ClientModel
    {
        private TcpClient _socket;

        public string message;

        public string messageBoard;

        public bool active;

        private string _userName;

        public ClientModel (string username)
        {
            // initializing variables
            _socket = new TcpClient("127.0.0.1", 8888);
            _userName = username;
            active = true;

            // welcome message and creating the thread to handle input
            Console.WriteLine("Welcome to the conversation. Type in a message when ready.");

            // assign a thread to constantly be running GetMessage
            var thread = new Thread(GetMessage);
            thread.Start();

            // initial connection
            _socket.WriteString(username);
        }
            
        private void GetMessage()
        {
            while (active)
            {

                string msg = _socket.ReadString();

                if (msg[0] == '#') // lobby chat message
                {
                    //MessageBoard += "\r\n" + msg.Substring(2);
                    Console.WriteLine(msg.Substring(2));
                }

            }
        }

        public void Send()
        {
            if ((message.Length > 2) && (message[0] == '/'))
            {
                // Commands

                if (message.Substring(1, 4) == "stop")
                {
                    // end the client
                    active = false;
                }
                else
                {

                }
            }
            else
            {
                // Normal Message
                _socket.WriteString("#(" + message);
                message = "";
            }
        }
    }
}
