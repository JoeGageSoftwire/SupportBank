using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Program started running.");

            List<string> data = GetTransactions();
            logger.Info("Data read from file.");
            List<Transaction> transactions = new List<Transaction>();

            data.RemoveAt(0);

            logger.Info("Started to parse transactions.");

            for (var index = 0; index < data.Count; index++)
            {
                string line = data[index];
                string[] lineArray = line.Split(",");
                try
                {
                    double amount = double.Parse(lineArray[4]);
                }
                catch
                {
                    logger.Error($"An entry in the 'Amount' column (row number: {index + 2}) could not be parsed as a double.");
                    continue;
                }

                Transaction transaction = new Transaction(lineArray[0], lineArray[1], lineArray[2], lineArray[3], double.Parse(lineArray[4]));
                transactions.Add(transaction);
            }

            logger.Info("Completed parsing transactions.");

            List<string> names = new List<string>();
            foreach (Transaction entry in transactions)
            {
                names.Add(entry.giverName);
                names.Add(entry.receiverName);
            }

            List<string> namesUnique = names.Distinct().ToList();

            logger.Info("Compiled list of account names.");

            List<Account> accounts = new List<Account>();
            foreach (string name in namesUnique)
            {
                var account = new Account(name);
                accounts.Add(account);
            }

            logger.Info("Created empty accounts.");

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

            logger.Info("Account balances calculated.");

            Console.WriteLine("Please type 'List All' to display all accounts and balances or 'List [Account]' to display all transactions associated with the given account. Type 'Exit' to exit.");
            Console.WriteLine();

            logger.Info("User prompted for input.");

            for (;;)
            {
                string input = Console.ReadLine();
                Console.WriteLine();

                logger.Info($"User input '{input}'.");

                if (input.ToLower().Trim() == "list all")
                {
                    logger.Info("Input recognized as 'List All'.");

                    foreach (Account account in accounts)
                    {
                        Console.WriteLine($"{account.ownerName}'s balance is {account.balance}");
                    }
                    Console.WriteLine();

                    logger.Info("All account balances displayed.");
                }
                else if (input.ToLower().StartsWith("list "))
                {
                    logger.Info("Input recognized as 'List [Account]'.");

                    string accountName = input.Remove(0, 4).Trim();
                    List<string> namesLower = namesUnique.ConvertAll(s => s.ToLower());

                    if (namesLower.Contains(accountName))
                    {
                        TransactionFinder finder = new TransactionFinder();
                        List<Transaction> transactionsFrom = finder.FindTransactions(transactions, accountName, true);
                        List<Transaction> transactionsTo = finder.FindTransactions(transactions, accountName, false);

                        ConsoleOutputter outputter = new ConsoleOutputter();
                        outputter.WriteAllTransactions(transactionsFrom, transactionsTo);

                        logger.Info("All transactions for [Account] displayed.");
                    }
                    else
                    {
                        logger.Info("Input [Account] not recognized.");

                        Console.WriteLine("Account name not recognized. Type 'List All' to see all accounts.");
                        Console.WriteLine();
                    }
                }
                else if (input.ToLower() == "exit")
                {
                    logger.Info("Program terminated.");
                    break;
                }
                else
                {
                    logger.Info("Input unrecognized.");

                    Console.WriteLine("Command not recognized. Please type 'List All' to display all accounts and balances or 'List [Account]' to display all transactions associated with the given account. Type 'Exit' to exit.");
                    Console.WriteLine();
                }
            }
        }

        private static List<string> GetTransactions()
        {
            return System.IO.File.ReadAllLines(@"C:\Users\JHG\Work\Training\SupportBank\DodgyTransactions2015.csv").ToList();
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
