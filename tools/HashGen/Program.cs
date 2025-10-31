// tools/HashGen/Program.cs
using System;

namespace HashGen
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Gebruik: dotnet run --project tools/HashGen -- <plainPassword>");
                return 1;
            }

            string plain = args[0];

            // Volledige kwalificatie om zeker te zijn dat we de juiste API gebruiken
            string hash = BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 12);

            Console.WriteLine(hash);
            return 0;
        }
    }
}