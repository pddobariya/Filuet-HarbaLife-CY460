using Nop.Plugin.Payments.Payeezy.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    #region Methods

    public enum PayeezyPaymentTransactionTypes
    {
        SMS = 1,
        DMS = 2
    }

    //according to ISO 4217
    public enum TransactionCurrencyCodes
    {
        USD = 840,
        EUR = 978,
        RUB = 643
    }


    public static class PayeezyBusinessDayClosureResults
    {
        public static string GetResultMessage(string resultCode)
        {
            return resultCode.GetResultMessage();
        }

        [Display(Description = "Business day is closed")]
        public const string OK = "OK";

        [Display(Description = "Business day closing failed")]
        public const string FAILED = "FAILED";

    }

    public static class PayeezyTransactionResults
    {
        public static string GetResultMessage(string resultCode)
        {
            return resultCode.GetResultMessage();
        }

        [Display(Description = "Unknown error")]
        public const string Default = "Default";

        [Display(Description = "Successful transaction")]
        public const string OK = "OK";

        [Display(Description = "Transaction failed")]
        public const string FAILED = "FAILED";

        [Display(Description = "Transaction just registered in the system")]
        public const string CREATED = "CREATED";

        [Display(Description = "Transaction not yet performed")]
        public const string PENDING = "PENDING";

        [Display(Name = "Transaction has been declined", Description = "Transaction declined because its ECI value is included in the list of blocked ECI values (Payeezy server configuration)")]
        public const string DECLINED = "DECLINED";

        [Display(Name = "Transaction has been reversed", Description = "Transaction already reversed")]
        public const string REVERSED = "REVERSED";

        [Display(Name = "Transaction is expired and has been reversed", Description = "Transaction is automatically reversed if the result was not requested within specified time (3 min.)")]
        public const string AUTOREVERSED = "AUTOREVERSED";

        [Display(Description = "Transaction declined due to timeout")]
        public const string TIMEOUT = "TIMEOUT";

    }

    public static class PayeezyTransaction3DSecureStatuses
    {
        public static string GetStatusMessage(string statusCode)
        {
            return statusCode.Get3DSecureMessage();
        }

        [Display(Name = "Transaction is authorized", Description = "Successful 3D Secure authorization")]
        public const string AUTHENTICATED = "AUTHENTICATED";

        [Display(Name = "Payment has been declined due to failed 3D Secure authorization", Description = "3D Secure authorization is unsuccessful")]
        public const string DECLINED = "DECLINED";

        [Display(Name = "3D Secure authorization failed", Description = "Non-participation on 3D scheme")]
        public const string NOTPARTICIPATED = "NOTPARTICIPATED";

        [Display(Name = "Transaction is not authenticated", Description = "Not Enrolled Transactions")]
        public const string NO_RANGE = "NO_RANGE";

        [Display(Name = "Transaction is not authenticated", Description = "Valid authentication attempt")]
        public const string ATTEMPTED = "ATTEMPTED";

        [Display(Name = "Transaction is not authenticated", Description = "Authentication Unavailable")]
        public const string UNAVAILABLE = "UNAVAILABLE";

        [Display(Name = "Error occurred during 3D Secure authorization", Description = "3-D Secure Errors")]
        public const string ERROR = "ERROR";

        [Display(Name = "System error", Description = "System Errors")]
        public const string SYSERROR = "SYSERROR";

        [Display(Name = "Unsupported card provided", Description = "Unknown Card Schemes")]
        public const string UNKNOWNSCHEME = "UNKNOWNSCHEME";

        [Display(Name = "3D Secure authorization error due to timeout", Description = "Status after timeout")]
        public const string FAILED = "FAILED";
    }

    public class PayeezyManager
    {
        private PayeezyClient _payeezyClient;

        public PayeezyManager(string payeezyBankUrl, string payeezyCertFile, string payeezyCertStoreKey)
        {
            _payeezyClient = new PayeezyClient(payeezyBankUrl, payeezyCertFile, payeezyCertStoreKey);
        }

        public PayeezyBusinessDayStatsAPIResponse CloseBusinessDay()
        {
            return new PayeezyBusinessDayStatsAPIResponse(_payeezyClient.CloseBusinessDay());
        }

        public PayeezyReversalAPIResponse ReverseTransaction(string transactionId, decimal amount)
        {
            return new PayeezyReversalAPIResponse(_payeezyClient.Reverse(transactionId, ConvertAmount(amount)));
        }

        public PayeezyStartTransactionAPIResponse InitializeTransaction(PayeezyPaymentTransactionTypes type, string clientIp, decimal amount, 
            string description = null, string lang = null, TransactionCurrencyCodes currency = TransactionCurrencyCodes.EUR)
        {
             
            PayeezyStartTransactionAPIResponse response = null;

            string amountStr = ConvertAmount(amount);
            string currencyStr = Convert.ToString((int)currency);

            switch (type)
            {
                case PayeezyPaymentTransactionTypes.SMS:
                    response = new PayeezyStartTransactionAPIResponse(_payeezyClient.StartSMSTrans(amountStr, currencyStr, clientIp, description, lang));
                    break;
                case PayeezyPaymentTransactionTypes.DMS:
                    response = new PayeezyStartTransactionAPIResponse(_payeezyClient.StartDMSAuth(amountStr, currencyStr, clientIp, description, lang));
                    break;
            }
            return response;
        }

        public PayeezyTransactionResultAPIResponse GetTransactionResult(string transactionId, string clientIp)
        {
            PayeezyTransactionResultAPIResponse response = new PayeezyTransactionResultAPIResponse(_payeezyClient.GetTransResult(transactionId, clientIp));
            return response;
        }

        public PayeezyTransactionResultAPIResponse ProcessDMSTransaction(string transactionId, string clientIp, decimal amount,
            string description = null, TransactionCurrencyCodes currency = TransactionCurrencyCodes.EUR)
        {
            string amountStr = ConvertAmount(amount);
            string currencyStr = Convert.ToString((int)currency);
            PayeezyTransactionResultAPIResponse response = new PayeezyTransactionResultAPIResponse(_payeezyClient.MakeDMSTrans(transactionId, amountStr, currencyStr, clientIp, description));
            return response;
        }

        

        private string ConvertAmount(decimal amount)
        {
            //convert amount to minor values according to Payeezy docs
            string amountStr = Convert.ToString(amount * 100);
            if (amountStr.Contains(".") || amountStr.Contains(","))
            {
                amountStr = amountStr.Split(new string[] { ".", "," }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            return amountStr;
        }
    }

    #endregion
}