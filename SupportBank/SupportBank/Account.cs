namespace SupportBank
{
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
}