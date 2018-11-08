using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace SupportBank
{
    class UserInterface
    {
        private readonly List<Account> _accounts;
        private readonly List<Transaction> _transactions;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public UserInterface(List<Account> accounts, List<Transaction> transactions)
        {
            _accounts = accounts;
            _transactions = transactions;
        }

        public void Start()
        {
            Console.WriteLine("Please type 'List All' to display all accounts and balances or 'List [Account]' to display all transactions associated with the given account. Type 'Exit' to exit.");
            Console.WriteLine();

            logger.Info("User prompted for input.");

            for (; ; )
            {
                var input = Console.ReadLine();
                Console.WriteLine();
                logger.Info($"User input '{input}'.");

                var command = input.Trim().ToLower();
                logger.Info($"Input parsed as '{command}'.");

                if (command == "list all")
                {
                    ListAll();
                }
                else if (command.StartsWith("list "))
                {
                    var accountName = command.Remove(0, 4).Trim();

                    ListAccount(accountName);
                }
                else if (command == "exit")
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

        public void ListAll()
        {
            logger.Info("Input recognized as 'List All'.");

            foreach (Account account in _accounts)
            {
                Console.WriteLine($"{account.ownerName}'s balance is {account.balance}");
            }
            Console.WriteLine();

            logger.Info("All account balances displayed.");
        }

        public void ListAccount(string accountName)
        {
            logger.Info("Input recognized as 'List [Account]'.");

            var namesLower = _accounts.Select(account => account.ownerName.ToLower());

            if (namesLower.Contains(accountName))
            {
                var finder = new TransactionFinder();
                var transactionsFrom = finder.FindTransactions(_transactions, accountName, true);
                var transactionsTo = finder.FindTransactions(_transactions, accountName, false);

                var outputter = new TransactionWriter();
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
    }
}