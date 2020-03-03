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
                } catch (TruSyntaxError e) {
                    Console.WriteLine($"Syntax Error: {e.Message}");
                } catch (TruRuntimeException e) {
                    Console.WriteLine($"Runtime Exception: {e.Message}");
                }
            }
        }
    }
}
