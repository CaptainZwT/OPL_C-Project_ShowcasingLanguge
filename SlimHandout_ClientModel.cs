/*
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
*/