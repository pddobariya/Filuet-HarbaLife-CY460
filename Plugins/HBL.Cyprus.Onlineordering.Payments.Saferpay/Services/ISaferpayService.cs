using System;
using System.Threading.Tasks;
using HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Services
{
    public partial interface ISaferpayService
    {
        #region Methods

        /// <summary>
        /// Method to begin a payment. Returns the token and SaferPay page URL for a customer to perform the payment
        /// </summary>
        /// <param name="orderTotal">Order total to pay</param>
        /// <param name="orderNumber">Order number</param>
        /// <param name="returnSuccessUrl">URL for a customer to proceed back to the merchant site in case of success</param>
        /// <param name="returnFailUrl">URL for a customer to proceed back to the merchant site in case of failure</param>
        /// <param name="notifyUrl">URL of the merchant's back-end method to be called from SaferPay server during customer payment</param>
        /// <returns></returns>
        Task<SaferpayInitializeResponse> InitializeAsync(decimal orderTotal, string orderNumber, string returnSuccessUrl, string returnFailUrl, string notifyUrl, Guid orderGuid);

        /// <summary>
        /// Method to assert a payment authorization and liability shift
        /// </summary>
        /// <param name="token">Token from Initialize response</param>
        /// <returns></returns>
        Task<SaferpayAssertResponse> AssertAsync(string token);

        /// <summary>
        /// Method to capture a payment
        /// </summary>
        /// <param name="transactionId">Transaction Id from Assert method</param>
        /// <returns></returns>
        Task<SaferpayCaptureResponse> CaptureAsync(string transactionId);

        /// <summary>
        /// Method to cancel a payment
        /// </summary>
        /// <param name="transactionId">Transaction Id from Assert method</param>
        /// <returns></returns>
        Task<SaferpayCancelResponse> CancelAsync(string transactionId);

        #endregion
    }
}
