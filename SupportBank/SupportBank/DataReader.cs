using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using NLog;

namespace SupportBank
{
    class DataReader
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public List<Transaction> GetTransactionsCsv()
        {
            var data = GetDataCsv();

            logger.Info("Read in data from CSV file.");
            logger.Info("Started parsing transaction data.");

            var transactions = new List<Transaction>();
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

            logger.Info("Finished parsing transaction data.");
            return transactions;
        }

        private static List<string> GetDataCsv()
        {
            return System.IO.File.ReadAllLines(@"C:\Users\JHG\Work\Training\SupportBank\Transactions2014.csv").ToList();
        }

        public List<Transaction> GetTransactionsJson()
        {
            var data = GetDataJson();
            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(data);
            return transactions;
        }

        private static string GetDataJson()
        {
            return System.IO.File.ReadAllText(@"C:\Users\JHG\Work\Training\SupportBank\Transactions2013.json");
        }
    }
}