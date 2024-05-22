namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    public class PayeezyStartTransactionAPIResponse: PayeezyBaseAPIResponse
    {
        #region Methods

        private const string _transactionKey = "TRANSACTION_ID";

        public string TransactionId
        {
            get
            {
                return GetValue(_transactionKey);
            }
        }

        public PayeezyStartTransactionAPIResponse(string rawData) : base(rawData) { }

        public override string ToString()
        {
            string baseResponse = base.ToString();
            string transId = TransactionId;
            return string.Format("Transaction ID: {0}; {1}", transId == null ? "" : transId, baseResponse);
        }

        #endregion
    }
}