using System;

namespace ClaimFreeBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Usage: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ClaimFreeBook email password");
            }
            else
            {
                var packt = new Packt();
                packt.ClaimFreeBook(args[0], args[1]).Wait();
            }

            ConfirmExit();
            Console.ResetColor();
        }

        private static void ConfirmExit()
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("*** Press ENTER to Exit ***");
            Console.ReadLine();
#endif
        }
    }
}
