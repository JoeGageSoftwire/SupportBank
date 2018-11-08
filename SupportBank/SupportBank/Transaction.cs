using Newtonsoft.Json;

namespace SupportBank
{
    class Transaction
    {
        public string date;
        [JsonProperty("FromAccount")]
        public string giverName;
        [JsonProperty("ToAccount")]
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
}