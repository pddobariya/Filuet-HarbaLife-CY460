namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    public class PayeezyReversalAPIResponse : PayeezyBaseAPIResponse
    {
        #region Methods

        private const string _resultKey = "RESULT";
        private const string _resultCodeKey = "RESULT_CODE";

        public string Result
        {
            get
            {
                return GetValue(_resultKey);
            }
        }

        public string ResultCode
        {
            get
            {
                return GetValue(_resultCodeKey);
            }
        }

        public PayeezyReversalAPIResponse(string rawData) : base(rawData) { }

        #endregion
    }
}