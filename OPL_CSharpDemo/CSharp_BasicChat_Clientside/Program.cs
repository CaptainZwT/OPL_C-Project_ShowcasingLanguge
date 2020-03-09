using System;

namespace CSharp_BasicChat_Clientside
{
    class Program
    {
        static void Main()
        { 
            // Type your username and press enter
            Console.WriteLine("Enter username:");

            // Create a string variable and get user input from the keyboard and store it in the variable
            string userNameInp = Console.ReadLine();

            // Initialize ClientModel
            ClientModel model = new ClientModel(userNameInp);

            while (model.active)
            {
                // Get input
                string textInp = Console.ReadLine();

                // give it to the ClientModel
                model.message = textInp;

                // Send the message out
                model.Send();
            }
        }
    }
}
