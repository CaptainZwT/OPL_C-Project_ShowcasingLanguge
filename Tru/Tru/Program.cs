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
                    TruVal result = TruExpr.Parse(input).Interpret();
                    Console.WriteLine( result.ToString() );
                } catch (System.ArgumentException e) {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
