using System;

namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    public class PayeezyBusinessDayStatsAPIResponse : PayeezyBaseAPIResponse
    {
        #region Properties

        private const string _resultKey = "RESULT";
       
        private const string _creditTransCountKey = "FLD_074";
        private const string _creditReversalsCountKey = "FLD_075";
        private const string _debitReversalsCountKey = "FLD_077";

        private const string _creditTransAmountKey = "FLD_086";
        private const string _creditReversalsAmountKey = "FLD_087";
        private const string _debitTransAmountKey = "FLD_088";
        private const string _debitReversalsAmountKey = "FLD_089";

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

        //counts
        public int TotalCreditTransactionCount
        {
            get
            {
                return GetCountValue(_creditTransCountKey);
            }
        }

        public int TotalCreditReversalCount
        {
            get
            {
                return GetCountValue(_creditReversalsCountKey);
            }
        }

        public int TotalDebitTransactionCount
        {
            get
            {
                return GetCountValue(_debitReversalsCountKey);
            }
        }

        public int TotalDebitReversalCount
        {
            get
            {
                return GetCountValue(_debitReversalsCountKey);
            }
        }

        //amounts
        public double TotalCreditTransactionAmount
        {
            get
            {
                return GetAmountValue(_creditTransAmountKey);
            }
        }

        public double TotalCreditReversalAmount
        {
            get
            {
                return GetAmountValue(_creditReversalsAmountKey);
            }
        }

        public double TotalDebitTransactionAmount
        {
            get
            {
                return GetAmountValue(_debitTransAmountKey);
            }
        }

        public double TotalDebitReversalAmount
        {
            get
            {
                return GetAmountValue(_debitReversalsAmountKey);
            }
        }

        public PayeezyBusinessDayStatsAPIResponse(string rawData) : base(rawData) { }

        private int GetCountValue(string key)
        {
            int? val = GetIntValue(key);
            return val == null ? 0 : Convert.ToInt32(val);
        }

        private double GetAmountValue(string key)
        {
            double? val = GetDoubleValue(key);
            return val == null ? 0 : Convert.ToDouble(val);
        }

        #endregion
    }
}