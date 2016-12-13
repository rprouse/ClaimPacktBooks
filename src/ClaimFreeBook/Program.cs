using System;

namespace ClaimFreeBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var packt = new Packt();
            packt.ClaimFreeBook().Wait();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("*** Press ENTER to Exit ***");
            Console.ReadLine();
            Console.ResetColor();
        }
    }
}
