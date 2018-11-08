using System;
using System.Collections.Generic;

namespace SupportBank
{
    class TransactionWriter
    {
        public void WriteFromTransaction(Transaction transaction)
        {
            Console.WriteLine($"{transaction.date.PadRight(15,' ')} {transaction.giverName.PadRight(14,' ')} {transaction.receiverName.PadRight(14,' ')} {transaction.narrative.PadRight(39,' ')} {transaction.amount}");
        }

        public void WriteAllTransactions(List<Transaction> transactionsFrom, List<Transaction> transactionsTo)
        {
            Console.WriteLine("Transactions from:");
            Console.WriteLine($"{"Date".PadRight(16,' ')}{"From".PadRight(15,' ')}{"To".PadRight(15,' ')}{"Narrative".PadRight(40,' ')}Amount");
            foreach (Transaction transaction in transactionsFrom)
            {
                WriteFromTransaction(transaction);
            }

            Console.WriteLine();

            Console.WriteLine("Transactions to:");
            Console.WriteLine($"{"Date".PadRight(16, ' ')}{"From".PadRight(15, ' ')}{"To".PadRight(15, ' ')}{"Narrative".PadRight(40, ' ')}Amount");
            foreach (Transaction transaction in transactionsTo)
            {
                WriteFromTransaction(transaction);
            }

            Console.WriteLine();
        }
    }
}