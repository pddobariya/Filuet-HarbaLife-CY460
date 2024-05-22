using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class OrderPriceHeader
    {
        #region Properties

        public string DistributorId { get; set; }

        public string WarehouseCode { get; set; }

        public string OrderMonth { get; set; }

        public string FreightCode { get; set; }

        public string CountryCode { get; set; }

        public string PostalCode { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string OrderNumber { get; set; }

        public string OrderCategory { get; set; }

        public string OrderType { get; set; }

        public DateTime? PriceDate { get; set; }

        public DateTime? OrderDate { get; set; }

        public double Discount { get; set; }

        public double DiscountPercent { get; set; }

        public decimal TotalDue { get; set; }

        public double VolumePoints { get; set; }

        public decimal AmountBase { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public decimal DiscountBasePrice { get; set; }

        public decimal FreightCharge { get; set; }

        #endregion
    }
}