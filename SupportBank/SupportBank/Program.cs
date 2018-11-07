using System;
using System.Collections.Generic;
using System.Linq;

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

            transactions.RemoveAt(0);                           //Removes column headings

            List<string> names = new List<string>();
            foreach (string[] entry in transactions)
            {
                string giver = entry[1];
                string receiver = entry[2];
                names.Add(giver);
                names.Add(receiver);
            }
            
            List<string> namesUnique = names.Distinct().ToList();
            List<double> balances = new List<double>(new double[namesUnique.Count]);

            foreach (string[] entry in transactions)
            {
                int giverIndex = namesUnique.IndexOf(entry[1]);
                int receiverIndex = namesUnique.IndexOf(entry[2]);
                double transactionAmount = double.Parse(entry[4]);
                balances[giverIndex] = balances[giverIndex] - transactionAmount;
                balances[receiverIndex] = balances[receiverIndex] + transactionAmount;
            }

            Console.ReadLine();
        }

        private static string[] GetTransactions()
        {
            return System.IO.File.ReadAllLines(@"C:\Users\JHG\Work\Training\SupportBank\Transactions2014.csv");
        }
    }
}
