using System;
using System.Collections.Generic;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] data = GetTransactions();
            List<string[]> transactions = new List<string[]>();

            foreach (string line in data)
            {
                string[] lineArray = line.Split(",");
                transactions.Add(lineArray);
            }

            Console.ReadLine();
        }

        private static string[] GetTransactions()
        {
            return System.IO.File.ReadAllLines(@"C:\Users\JHG\Work\Training\SupportBank\Transactions2014.csv");
        }
    }
}
