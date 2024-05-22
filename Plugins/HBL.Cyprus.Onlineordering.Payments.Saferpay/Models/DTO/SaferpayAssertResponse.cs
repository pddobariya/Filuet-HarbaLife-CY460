using System;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    #region Properties

    public class Transaction
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public Amount Amount { get; set; }
        public string AcquirerName { get; set; }
        public string AcquirerReference { get; set; }
        public string SixTransactionReference { get; set; }
        public string ApprovalCode { get; set; }
    }

    public class Brand
    {
        public string PaymentMethod { get; set; }
        public string Name { get; set; }
    }

    public class Card
    {
        public string MaskedNumber { get; set; }
        public int ExpYear { get; set; }
        public int ExpMonth { get; set; }
        public string HolderName { get; set; }
        public string CountryCode { get; set; }
    }

    public class PaymentMeans
    {
        public Brand Brand { get; set; }
        public string DisplayText { get; set; }
        public Card Card { get; set; }
    }

    public class ThreeDs
    {
        public bool Authenticated { get; set; }
        public bool LiabilityShift { get; set; }
        public string Xid { get; set; }
        public string VerificationValue { get; set; }
    }

    public class Liability
    {
        public bool LiabilityShift { get; set; }
        public string LiableEntity { get; set; }
        public ThreeDs ThreeDs { get; set; }
    }

    public class SaferpayAssertResponse
    {
        public SaferpayResponseHeader ResponseHeader { get; set; }
        public Transaction Transaction { get; set; }
        public PaymentMeans PaymentMeans { get; set; }
        public Liability Liability { get; set; }
    }

    #endregion
}
