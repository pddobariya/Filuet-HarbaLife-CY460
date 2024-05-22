using Filuet.Hrbl.Ordering.Abstractions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Configuration;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Services.Common;
using ProxyServiceReference;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    public class ShoppingCartProxyService : AuthenticationProxyClientBase<IFusionServiceProxy>, IShoppingCartProxyService
    {
        #region Fields

        private static readonly IHrblOrderingAdapter _hrblOrderingAdapter;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        static ShoppingCartProxyService()
        {
            _hrblOrderingAdapter = ConnectionBuilder.GetRestApiAdapter();
        }
      
        public ShoppingCartProxyService(FiluetConfig filuetConfig,IWorkContext workContext, IGenericAttributeService genericAttributeService)
            : base(ConnectionBuilder.GetBinding(filuetConfig.FusionProxyConnectionProtocol),
                  ConnectionBuilder.GetEndpointAddress(filuetConfig.FusionProxyConnectionAddress))
        {
            this.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = ConnectionBuilder.GetSslCertificateAuthentication(false);
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public DistributorFopLimitsModel GetDistributorLimits(string distributorId, string countryCode, string orderMonth)
        {
            DistributorFopLimitsModel distributorLimits = null;
            ExecuteInContext(async () =>
            {
                FOPPurchasingLimitsResult rest = null;
                DSPurchasingLimit dsPurchasingLimit = null;
                DSFOPLimit fopLimit = null;
                try
                {
                     rest =await _hrblOrderingAdapter.GetDSFOPPurchasingLimits(distributorId, countryCode);
                    dsPurchasingLimit = rest.DSPurchasingLimits?.FirstOrDefault(pl => pl.PPVOrderMonth == $"{DateTime.Now:yyyy}/{DateTime.Now:MM}");
                }
                catch
                {
                }
                try
                {
                    fopLimit = rest?.FopLimit;
                }
                catch
                {
                }
                distributorLimits = new DistributorFopLimitsModel()
                {
                    InFopPeriod = rest?.FopLimit?.FOPFirstOrderDate?.AddDays(rest?.FopLimit.FOPThresholdPeriod ?? 0) > DateTime.Now,
                    FopLimit = (double)(fopLimit?.AvailableFOPLimit ?? 0),
                    PcLimit = (double)(dsPurchasingLimit?.AvailablePCLimit ?? 0)
                };
            });
            return distributorLimits;
        }

        public async Task<ShoppingCartTotalModel> GetShoppingCartTotalAsync(string distributorId, string processingLocation,
            string warehouseCode, string orderMonth, string freightCode, string countryCode, string postalCode,
            string address, string city, IEnumerable<OrderItemModel> orderItems, string orderNumber, string orderCategory,
            string orderType, string currencyCode)
        {
            ShoppingCartTotalModel shoppingCartTotal = null;
            var customer = await _workContext.GetCurrentCustomerAsync();
            var deliveryPrice = await _genericAttributeService.GetAttributeAsync<decimal>(customer, CustomerAttributeNames.DeliveryPrice);
            var pricingResponse = await _hrblOrderingAdapter.GetPriceDetails(builder => //new PricingRequest()
            {
                builder.AddHeader(header =>
                {
                    header.OrderSource = "Internet";
                    header.ProcessingLocation = processingLocation;
                    header.Warehouse = warehouseCode;
                    header.DistributorId = distributorId;
                    header.FreightCode = freightCode;
                    header.OrderType = orderType;
                    header.OrderCategory = orderCategory;
                    header.OrderMonth = DateTime.ParseExact(orderMonth, "yy.MM", CultureInfo.CurrentCulture);
                    header.CountryCode = countryCode;
                    header.PostalCode = postalCode;
                    header.State = "";
                    header.Address1 = address;
                    header.Address2 = "";
                    header.City = city;
                    header.ExternalOrderNumber = string.IsNullOrEmpty(orderNumber) ? null : orderNumber;
                    header.CurrencyCode = currencyCode;
                });
                builder.AddItems(() =>
                {
                    return orderItems.Select(oi =>
                    {
                        string productType = "P";
                        if (string.Equals("ETO", orderCategory, StringComparison.InvariantCultureIgnoreCase))
                            productType = "E";
                        return new PricingRequestLine
                        {
                            ProcessingLocation = processingLocation,
                            ProductType = productType,
                            Sku = oi.Sku.Name,
                            Quantity = oi.Count
                        };
                    }).ToArray();
                });
                builder.Build();
            });
            shoppingCartTotal = new ShoppingCartTotalModel
            {
                AmountBase = pricingResponse.Header.TotalRetailAmount ?? 0,
                CurrentOrderVolumePoints = (double)(pricingResponse.Header.VolumePoints ?? 0),
                VolumePoints = (double)(pricingResponse.Header.VolumePoints ?? 0),
                Discount = (double)(pricingResponse.Header.TotalDiscountAmount ?? 0),
                DiscountPercent = pricingResponse.Header.DiscountPercent ?? 0,
                DiscountedBasePrice = pricingResponse.Header.TotalRetailAmount.HasValue
                    ? pricingResponse.Header.TotalRetailAmount - pricingResponse.Header.TotalDiscountAmount ?? 0
                    : 0,
                FreightCharge = pricingResponse.Header.TotalFreightCharges ?? 0,
                OrderMonth = pricingResponse.Header.OrderMonth.Month,
                OrderYear = pricingResponse.Header.OrderMonth.Year,
                OrderNumber = pricingResponse.Header.ExternalOrderNumber,
                RewardAmountBase = pricingResponse.Header.TotalRetailAmount ?? 0,
                TotalAmount = pricingResponse.Header.TotalOrderAmount ?? 0,
                TotalDue = pricingResponse.Header.TotalDue ?? 0,
                TotalTaxAmount = pricingResponse.Header.TotalTaxAmount ?? 0,
                ShoppingCartLines = pricingResponse.Lines.Select(prl => new ShoppingCartLineModel
                {
                    Count = (int)prl.Quantity,
                    Earnbase = prl.Earnbase,
                    LineAmount = prl.LineDueAmount,
                    Sku = new SkuItemModel { Name = prl.Sku },
                    TotalDiscountedPrice = prl.LineDiscountAmount,
                    TotalEarnbase = prl.TotalEarnBase,
                    TotalRetailPrice = prl.TotalRetailPrice,
                    UnitPrice = prl.UnitRetailPrice,
                    UnitVolume = prl.UnitVolumePoints
                })
            };

            return shoppingCartTotal;
        }

        public async Task<SubmitOrderResultModel> SubmitOrderAsync(SubmitRequestPayment submitRequestPayment, SubmitRequestHeader submitRequestHeader, SubmitRequestOrderLine[] submitRequestOrderLines)
        {
            var submitResponse = await _hrblOrderingAdapter.SubmitOrder(builder =>
            {
                builder.AddHeader(header =>
                {
                    AutoMapperConfiguration.Mapper.Map(submitRequestHeader, header);
                });
                builder.AddPayments(requestPayment =>
                {
                    AutoMapperConfiguration.Mapper.Map(submitRequestPayment, requestPayment);
                });
                builder.AddItems(() => submitRequestOrderLines);
            });

            return new SubmitOrderResultModel
            {
                IsSuccess = submitResponse.IsSuccess,
                OrderNumber = submitResponse.OrderNumber,
                Errors = string.IsNullOrEmpty(submitResponse.ErrorMessage) ? null : new string[] { submitResponse.ErrorMessage }
            };
        }

        #endregion
    }
}
