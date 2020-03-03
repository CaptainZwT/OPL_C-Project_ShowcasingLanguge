using System;

namespace Tru
{
    class Program
    {

        /// Launches a REPL where you can evaluate Tru statements.
        /// Type "quit" to exit.
        static void Main(string[] args)
        {
            Environment global = TruLibrary.Library;

            while (true) {
                Console.Write( ">>> ");
                string input = Console.ReadLine();
                if (input == "quit")  return;

                try {
                    TruStatement[] statements = TruStatement.ParseAll(input);
                    foreach (TruStatement statement in statements) {
                        TruVal result = statement.Interpret(global);
                        if (result != null)
                            Console.WriteLine(result);
                    }
                } catch (System.ArgumentException e) {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }
    }
}
