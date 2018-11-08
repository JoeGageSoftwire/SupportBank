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

            var data = GetTransactions();
            logger.Info("Data read from file.");
            var transactions = new List<Transaction>();

            data.RemoveAt(0);

            logger.Info("Started to parse transactions.");

            for (var index = 0; index < data.Count; index++)
            {
                var line = data[index];
                var lineArray = line.Split(",");
                try
                {
                    var amount = double.Parse(lineArray[4]);
                }
                catch
                {
                    logger.Error($"An entry in the 'Amount' column (row number: {index + 2}) could not be parsed as a double.");
                    continue;
                }

                var transaction = new Transaction(lineArray[0], lineArray[1], lineArray[2], lineArray[3], double.Parse(lineArray[4]));
                transactions.Add(transaction);
            }

            logger.Info("Completed parsing transactions.");

            var names = new List<string>();
            foreach (Transaction entry in transactions)
            {
                names.Add(entry.giverName);
                names.Add(entry.receiverName);
            }

            var namesUnique = names.Distinct().ToList();

            logger.Info("Compiled list of account names.");

            var accounts = new List<Account>();
            foreach (var name in namesUnique)
            {
                var account = new Account(name);
                accounts.Add(account);
            }

            logger.Info("Created empty accounts.");

            foreach (var entry in transactions)
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

                var transactionAmount = entry.amount;

                giverAccount.balance -= transactionAmount;
                receiverAccount.balance += transactionAmount;
            }

            logger.Info("Account balances calculated.");

            var ui = new UserInterface(accounts, transactions);
            ui.Start();
        }

        private static List<string> GetTransactions()
        {
            return System.IO.File.ReadAllLines(@"C:\Users\JHG\Work\Training\SupportBank\DodgyTransactions2015.csv").ToList();
        }
    }
}
