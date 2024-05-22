using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ProxyServiceReference;
using System;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.FusionProxyClientSDK
{
    public static class MappingToProxyExtensions
    {
        #region Properties

        public static OrderItem ToProxyModel(this OrderItemModel orderItemModel)
        {
            return new OrderItem
            {
                Sku = orderItemModel.Sku.ToProxyModel(),
                Count = orderItemModel.Count
            };
        }

        public static SkuItem ToProxyModel(this SkuItemModel skuItemModel)
        {
            return new SkuItem
            {
                Name = skuItemModel.Name,
                Warehouse = skuItemModel.Warehouse
            };
        }

        public static ShoppingCartTotal ToProxyModel(this ShoppingCartTotalModel shoppingCartTotalModel)
        {
            return new ShoppingCartTotal
            {
                AmountBase = shoppingCartTotalModel.AmountBase,
                CurrentOrderVolumePoints = shoppingCartTotalModel.CurrentOrderVolumePoints,
                Discount = shoppingCartTotalModel.Discount,
                DiscountedBasePrice = shoppingCartTotalModel.DiscountedBasePrice,
                DiscountPercent = shoppingCartTotalModel.DiscountPercent,
                Errors = shoppingCartTotalModel.Errors?.ToArray(),
                FreightCharge = shoppingCartTotalModel.FreightCharge,
                OrderMonth = shoppingCartTotalModel.OrderMonth,
                OrderNumber = shoppingCartTotalModel.OrderNumber,
                OrderYear = shoppingCartTotalModel.OrderYear,
                RewardAmountBase = shoppingCartTotalModel.RewardAmountBase,
                ShoppingCartLines = shoppingCartTotalModel.ShoppingCartLines.Select(p => p.ToProxyModel()).ToArray(),
                TotalAmount = shoppingCartTotalModel.TotalAmount,
                TotalDue = shoppingCartTotalModel.TotalDue,
                TotalTaxAmount = shoppingCartTotalModel.TotalTaxAmount,
                VolumePoints = shoppingCartTotalModel.VolumePoints,
            };
        }

        public static OrderPayment ToProxyModel(this OrderPaymentModel orderPaymentModel)
        {
            var flag = orderPaymentModel.Paycode == "WIRE";

            return new OrderPayment
            {
                AppliedDate = flag ? null : orderPaymentModel.AppliedDate,
                ApprovalNumber = flag ? null : orderPaymentModel.ApprovalNumber,
                CurrencyCode = orderPaymentModel.CurrencyCode,
                PaymentAmount = orderPaymentModel.PaymentAmount,
                PaymentDate = flag ? null : orderPaymentModel.PaymentDate,
                PaymentReceived = flag ? null : orderPaymentModel.PaymentReceived,
                PaymentMethodName = orderPaymentModel.PaymentMethodName,
                Paycode = orderPaymentModel.Paycode,
                PaymentType = orderPaymentModel.PaymentType,
                CardExpiryDate = flag ? null : DateTime.Now.AddYears(1),
                CardHolderName = flag ? null : "CARD HOLDER",
                CardNumber = flag ? null : "0B11074741560000",
                CardType = flag ? null : "VI"
            };
        }

        public static ShoppingCartLine ToProxyModel(this ShoppingCartLineModel shoppingCartLineModel)
        {
            return new ShoppingCartLine
            {
                Count = shoppingCartLineModel.Count,
                Earnbase = shoppingCartLineModel.Earnbase,
                LineAmount = shoppingCartLineModel.LineAmount,
                Sku = shoppingCartLineModel.Sku.ToProxyModel(),
                TotalDiscountedPrice = shoppingCartLineModel.TotalDiscountedPrice,
                TotalEarnbase = shoppingCartLineModel.TotalEarnbase,
                TotalRetailPrice = shoppingCartLineModel.TotalRetailPrice,
                UnitPrice = shoppingCartLineModel.UnitPrice,
                UnitVolume = shoppingCartLineModel.UnitVolume
            };
        }

        #endregion
    }
}
