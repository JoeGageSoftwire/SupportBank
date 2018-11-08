using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NLog;

namespace SupportBank
{
    class DataReader
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public List<Transaction> GetTransactionsCsv(string filepath)
        {
            var data = GetDataCsv(filepath);

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

        private static List<string> GetDataCsv(string filepath)
        {
            return System.IO.File.ReadAllLines(filepath).ToList();
        }

        public List<Transaction> GetTransactionsJson(string filepath)
        {
            var data = GetDataJson(filepath);
            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(data);
            return transactions;
        }

        private static string GetDataJson(string filepath)
        {
            return System.IO.File.ReadAllText(filepath);
        }
    }
}