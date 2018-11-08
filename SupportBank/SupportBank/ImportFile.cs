using System;
using System.Collections.Generic;
using NLog;

namespace SupportBank
{
    class ImportFile
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public List<Transaction> AskForFile()
        {
            Console.WriteLine("Please type the path of the file containing your transaction data (must be in CSV or JSON format).");
            Console.WriteLine();

            logger.Info("User prompted for data file path.");

            var input = Console.ReadLine();
            Console.WriteLine();
            logger.Info($"User input '{input}'.");
            var filepath = input.Trim();

            var getData = new DataReader();
            var transactions = new List<Transaction>();

            if (filepath.EndsWith(".csv"))
            {
                transactions = getData.GetTransactionsCsv(filepath);
            }
            else if (filepath.EndsWith(".json"))
            {
                transactions = getData.GetTransactionsJson(filepath);
            }
            else
            {
                logger.Error("The given file was not in CSV or JSON format.");
                Console.WriteLine("This file is not in the right format.");
            }
            return transactions;
        }
    }
}