using System.Collections.Generic;

namespace SupportBank
{
    class TransactionFinder
    {
        public List<Transaction> FindTransactions(List<Transaction> transactions, string accountName, bool isFrom)
        {
            var transactionsNew = new List<Transaction>();

            foreach (Transaction transaction in transactions)
            {
                var nameToCompare = isFrom ? transaction.giverName : transaction.receiverName;
                if (nameToCompare.ToLower() == accountName)
                {
                    transactionsNew.Add(transaction);
                }
            }

            return transactionsNew;
        }
    }
}