using System;

namespace Tru
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true) {
                Console.Write( ">>> ");
                string input = Console.ReadLine();

                if (input == "exit")  return;

                try {
                    bool result = TruExpr.Parse(input).Interpret();
                    Console.WriteLine( result ? "true" : "false" );
                } catch (System.ArgumentException e) {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
