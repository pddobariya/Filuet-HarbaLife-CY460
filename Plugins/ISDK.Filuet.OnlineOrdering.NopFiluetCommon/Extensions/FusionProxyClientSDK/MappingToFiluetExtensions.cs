using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ProxyServiceReference;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.FusionProxyClientSDK
{
    public static class MappingToFiluetExtensions
    {
        #region Methods

        public static DistributorLimitsWithBalanceModel ToFiluetModel(this DistributorLimitsWithBalance distributorLimitsWithBalance)
        {
            return new DistributorLimitsWithBalanceModel
            {
                DistributorLimit = (Enums.DistributorLimitEnum)distributorLimitsWithBalance.DistributorLimit,
                ExceedanceAmount = distributorLimitsWithBalance.ExceedanceAmount,
                ExceedItems = distributorLimitsWithBalance.ExceedItems.Select(p => p.ToFiluetModel())
            };
        }

        public static DistributorFopLimitsModel ToFiluetModel(this DistributorFopLimits distributorFopLimits)
        {
            return new DistributorFopLimitsModel
            {
                FopLimit = distributorFopLimits.FopLimit,
                InFopPeriod = distributorFopLimits.InFopPeriod,
                PcLimit = distributorFopLimits.PcLimit
            };
        }

        public static ShoppingCartTotalModel ToFiluetModel(this ShoppingCartTotal shoppingCartTotal)
        {
            return new ShoppingCartTotalModel
            {
                AmountBase = shoppingCartTotal.AmountBase,
                CurrentOrderVolumePoints = shoppingCartTotal.CurrentOrderVolumePoints,
                Discount = shoppingCartTotal.Discount,
                DiscountedBasePrice = shoppingCartTotal.DiscountedBasePrice,
                DiscountPercent = shoppingCartTotal.DiscountPercent,
                Errors = shoppingCartTotal.Errors,
                FreightCharge = shoppingCartTotal.FreightCharge,
                OrderMonth = shoppingCartTotal.OrderMonth,
                OrderNumber = shoppingCartTotal.OrderNumber,
                OrderYear = shoppingCartTotal.OrderYear,
                RewardAmountBase = shoppingCartTotal.RewardAmountBase,
                ShoppingCartLines = shoppingCartTotal.ShoppingCartLines.Select(p => p.ToFiluetModel()),
                TotalAmount = shoppingCartTotal.TotalAmount,
                TotalDue = shoppingCartTotal.TotalDue,
                TotalTaxAmount = shoppingCartTotal.TotalTaxAmount,
                VolumePoints = shoppingCartTotal.VolumePoints,
            };
        }

        public static ShoppingCartLineModel ToFiluetModel(this ShoppingCartLine shoppingCartLine)
        {
            return new ShoppingCartLineModel
            {
                Count = shoppingCartLine.Count,
                Earnbase = shoppingCartLine.Earnbase,
                LineAmount = shoppingCartLine.LineAmount,
                Sku = shoppingCartLine.Sku.ToFiluetModel(),
                TotalDiscountedPrice = shoppingCartLine.TotalDiscountedPrice,
                TotalEarnbase = shoppingCartLine.TotalEarnbase,
                TotalRetailPrice = shoppingCartLine.TotalRetailPrice,
                UnitPrice = shoppingCartLine.UnitPrice,
                UnitVolume = shoppingCartLine.UnitVolume
            };
        }

        public static SkuItemModel ToFiluetModel(this SkuItem skuItem)
        {
            return new SkuItemModel
            {
                Name = skuItem.Name,
                Warehouse = skuItem.Warehouse
            };
        }

        public static SubmitOrderResultModel ToFiluetModel(this SubmitOrderResult submitOrderResult)
        {
            return new SubmitOrderResultModel
            {
                OrderNumber = submitOrderResult.OrderNumber,
                IsSuccess = submitOrderResult.IsSuccess,
                Errors = submitOrderResult.Errors
            };
        }

        public static StockBalanceModel ToFiluetModel(this StockBalance stockBalance)
        {
            return new StockBalanceModel
            {
                StockBalances = stockBalance.StockBalances?.Select(p => p.ToFiluetModel())
            };
        }

        public static StockBalanceItemModel ToFiluetModel(this StockBalanceItem stockBalanceItem)
        {
            return new StockBalanceItemModel
            {
                IsAvailable = stockBalanceItem.IsAvailable,
                IsBlocked = stockBalanceItem.IsBlocked,
                IsValid = stockBalanceItem.IsValid,
                Sku = stockBalanceItem.Sku.ToFiluetModel(),
                StockBalanceItemAvailability = (Enums.StockBalanceItemAvailabilityEnum)stockBalanceItem.StockBalanceItemAvailability,
                StockQty = stockBalanceItem.StockQty
            };
        }

        #endregion
    }
}
