using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> data = GetTransactions();
            List<Transaction> transactions = new List<Transaction>();

            data.RemoveAt(0);

            foreach (string line in data)
            {
                string[] lineArray = line.Split(",");
                Transaction transaction = new Transaction(lineArray[0], lineArray[1], lineArray[2], lineArray[3], double.Parse(lineArray[4]));
                transactions.Add(transaction);
            }

            List<string> names = new List<string>();
            foreach (Transaction entry in transactions)
            {
                names.Add(entry.giverName);
                names.Add(entry.receiverName);
            }

            List<string> namesUnique = names.Distinct().ToList();

            List<Account> accounts = new List<Account>();
            foreach (string name in namesUnique)
            {
                var account = new Account(name);
                accounts.Add(account);
            }

            foreach (Transaction entry in transactions)
            {
                Account giverAccount = null;
                Account receiverAccount = null;

                foreach (var account in accounts)
                {
                    if (account.ownerName == entry.giverName)
                    {
                        giverAccount = account;
                    }

                    if (account.ownerName == entry.receiverName)
                    {
                        receiverAccount = account;
                    }
                }

                if (giverAccount == null || receiverAccount == null)
                {
                    return;
                }

                double transactionAmount = entry.amount;

                giverAccount.balance -= transactionAmount;
                receiverAccount.balance += transactionAmount;
            }

            Console.WriteLine("Please type 'List All' to display all accounts and balances or 'List [Account]' to display all transactions associated with the given account. Type 'Exit' to exit.");
            Console.WriteLine();

            for (;;)
            {
                string input = Console.ReadLine();
                Console.WriteLine();

                if (input.ToLower().Trim() == "list all")
                {
                    for (int i = 0; i < accounts.Count; i++)
                    {
                        Console.WriteLine($"{accounts[i].ownerName}'s balance is {accounts[i].balance}");
                    }
                    Console.WriteLine();
                }
                else if (input.ToLower().StartsWith("list "))
                {
                    string accountName = input.Remove(0, 4).Trim();
                    List<string> namesLower = namesUnique.ConvertAll(s => s.ToLower());

                    if (namesLower.Contains(accountName))
                    {
                        TransactionFinder finder = new TransactionFinder();
                        List<Transaction> transactionsFrom = finder.FindTransactions(transactions, accountName, true);
                        List<Transaction> transactionsTo = finder.FindTransactions(transactions, accountName, false);

                        ConsoleOutputter outputter = new ConsoleOutputter();
                        outputter.WriteAllTransactions(transactionsFrom, transactionsTo);
                    }
                    else
                    {
                        Console.WriteLine("Account name not recognized.");
                        Console.WriteLine();
                    }
                }
                else if (input.ToLower() == "exit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Command not recognized. Please type 'List All' to display all accounts and balances or 'List [Account]' to display all transactions associated with the given account. Type 'Exit' to exit.");
                    Console.WriteLine();
                }
            }
        }

        private static List<string> GetTransactions()
        {
            return System.IO.File.ReadAllLines(@"C:\Users\JHG\Work\Training\SupportBank\Transactions2014.csv").ToList();
        }
    }

    class Account
    {
        public string ownerName;
        public double balance;

        public Account(string ownerName)
        {
            this.ownerName = ownerName;
            this.balance = 0;
        }
    }

    class Transaction
    {
        public string date;
        public string giverName;
        public string receiverName;
        public string narrative;
        public double amount;

        public Transaction(string date, string giverName, string receiverName, string narrative, double amount)
        {
            this.date = date;
            this.giverName = giverName;
            this.receiverName = receiverName;
            this.narrative = narrative;
            this.amount = amount;
        }
    }

    class ConsoleOutputter
    {
        public void WriteFromTransaction(Transaction transaction)
        {
            Console.WriteLine($"{transaction.date.PadRight(15,' ')} {transaction.giverName.PadRight(14,' ')} {transaction.receiverName.PadRight(14, ' ')} {transaction.narrative.PadRight(39, ' ')} {transaction.amount}");
        }

        public void WriteAllTransactions(List<Transaction> transactionsFrom, List<Transaction> transactionsTo)
        {
            Console.WriteLine("Transactions from:");
            Console.WriteLine("Date            From           To             Narrative                               Amount");
            foreach (Transaction transaction in transactionsFrom)
            {
                WriteFromTransaction(transaction);
            }

            Console.WriteLine();

            Console.WriteLine("Transactions to:");
            Console.WriteLine("Date            From           To             Narrative                               Amount");
            foreach (Transaction transaction in transactionsTo)
            {
                WriteFromTransaction(transaction);
            }

            Console.WriteLine();
        }
    }

    class TransactionFinder
    {
        public List<Transaction> FindTransactions(List<Transaction> transactions, string accountName, bool isFrom)
        {
            List<Transaction> transactionsNew = new List<Transaction>();

            foreach (Transaction transaction in transactions)
            {
                string nameToCompare = isFrom ? transaction.giverName : transaction.receiverName;
                if (nameToCompare.ToLower() == accountName)
                {
                    transactionsNew.Add(transaction);
                }
            }

            return transactionsNew;
        }
    }
}
