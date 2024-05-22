using Nop.Plugin.Payments.Payeezy.Dto;

namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    public class PayeezyTransactionResultAPIResponse : PayeezyBaseAPIResponse
    {
        #region Properties

        private const string _resultKey = "RESULT";
        private const string _resultCodeKey = "RESULT_CODE";
        private const string _3dSecureKey = "3DSECURE";
        private const string _aavKey = "AAV";
        private const string _rrnKey = "RRN";
        private const string _approvalCodeKey = "APPROVAL_CODE";
        private const string _cardNumber = "CARD_NUMBER";

        #endregion

        #region Methods

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
                return GetValue(_resultKey);
            }
        }

        public string Transaction3DSecureStatus
        {
            get
            {
                return GetValue(_3dSecureKey);
            }
        }

        public string Aav
        {
            get
            {
                return GetValue(_aavKey);
            }
        }

        public string Rrn
        {
            get
            {
                return GetValue(_rrnKey);
            }
        }

        public string ApprovalCode
        {
            get
            {
                return GetValue(_approvalCodeKey);
            }
        }

        public string CardNumber
        {
            get
            {
                return GetValue(_cardNumber);
            }
        }

        public PayeezyTransactionResultAPIResponse(string rawData) : base(rawData) { }

        public override string ToString()
        {
            string baseResponse = base.ToString();
            string result = Result != null ? Result.GetResultMessage() : "";
            string resultDescription = Result != null ? Result.GetResultDescription() : "";
            return string.Format("Result: {0}; Result description: {1}; {2}", result == null ? "" : result, resultDescription == null ? "" : resultDescription, baseResponse);
        }

        #endregion
    }
}