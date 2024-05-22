using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class ExtendedMessageTokenProvider: MessageTokenProvider
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public ExtendedMessageTokenProvider(
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            IBlogService blogService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerAttributeFormatter customerAttributeFormatter,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            IOrderService orderService, 
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter, 
            IProductService productService, 
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService, 
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService, 
            IVendorAttributeFormatter vendorAttributeFormatter,
            IWorkContext workContext,
            MessageTemplatesSettings templatesSettings,
            PaymentSettings paymentSettings,
            StoreInformationSettings storeInformationSettings, 
            TaxSettings taxSettings)
            : base(catalogSettings,
                  currencySettings,
                  actionContextAccessor, 
                  addressAttributeFormatter,
                  addressService,
                  blogService,
                  countryService, 
                  currencyService,
                  customerAttributeFormatter, 
                  customerService, 
                  dateTimeHelper, 
                  eventPublisher, 
                  genericAttributeService, 
                  giftCardService,
                  htmlFormatter, 
                  languageService,
                  localizationService,
                  newsService,
                  orderService,
                  paymentPluginManager,
                  paymentService, 
                  priceFormatter, 
                  productService,
                  rewardPointService,
                  shipmentService,
                  stateProvinceService,
                  storeContext, 
                  storeService,
                  urlHelperFactory,
                  urlRecordService, 
                  vendorAttributeFormatter,
                  workContext,
                  templatesSettings,
                  paymentSettings,
                  storeInformationSettings,
                  taxSettings)
        {
            _catalogSettings = catalogSettings;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _storeContext = storeContext;
            _workContext = workContext;
            _templatesSettings = templatesSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier (used to limit products by vendor</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the hTML table of products
        /// </returns>
        protected override async Task<string> ProductListToHtmlTableAsync(Order order, int languageId, int vendorId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Quantity", languageId)}</th>");
            sb.AppendLine("</tr>");

            var table = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendorId);
            for (var i = 0; i <= table.Count - 1; i++)
            {
                var orderItem = table[i];

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                if (product == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //product name
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(productName));

                //add download link
                if (await _orderService.IsDownloadAllowedAsync(orderItem))
                {
                    var downloadUrl = await RouteUrlAsync(order.StoreId, "GetDownload", new { orderItemId = orderItem.OrderItemGuid });
                    var downloadLink = $"<a class=\"link\" href=\"{downloadUrl}\">{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Download", languageId)}</a>";
                    sb.AppendLine("<br />");
                    sb.AppendLine(downloadLink);
                }
                //add download link
                if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                {
                    var licenseUrl = await RouteUrlAsync(order.StoreId, "GetLicense", new { orderItemId = orderItem.OrderItemGuid });
                    var licenseLink = $"<a class=\"link\" href=\"{licenseUrl}\">{await _localizationService.GetResourceAsync("Messages.Order.Product(s).License", languageId)}</a>";
                    sb.AppendLine("<br />");
                    sb.AppendLine(licenseLink);
                }
                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(orderItem.AttributeDescription);
                }
                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }
                //SKU
                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                {
                    var sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
                    if (!string.IsNullOrEmpty(sku))
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine(string.Format(await _localizationService.GetResourceAsync("Messages.Order.Product(s).SKU", languageId), WebUtility.HtmlEncode(sku)));
                    }
                }

                sb.AppendLine("</td>");
                
                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: center;\">{orderItem.Quantity}</td>");
                
                sb.AppendLine("</tr>");
            }

            if (vendorId == 0)
            {
                //we render checkout attributes and totals only for store owners (hide for vendors)

                if (!string.IsNullOrEmpty(order.CheckoutAttributeDescription))
                {
                    sb.AppendLine("<tr><td style=\"text-align:right;\" colspan=\"1\">&nbsp;</td><td colspan=\"3\" style=\"text-align:right\">");
                    sb.AppendLine(order.CheckoutAttributeDescription);
                    sb.AppendLine("</td></tr>");
                }

                //totals
                await WriteTotalsAsync(order, language, sb);
            }

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Write order totals
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="language">Language</param>
        /// <param name="sb">StringBuilder</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task WriteTotalsAsync(Order order, Language language, StringBuilder sb)
        {
            //subtotal
            string cusSubTotal;
            var displaySubTotalDiscount = false;
            var cusSubTotalDiscount = string.Empty;
            var languageId = language.Id;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                {
                    cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                    displaySubTotalDiscount = true;
                }
            }
            else
            {
                //excluding tax

                //subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                {
                    cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                    displaySubTotalDiscount = true;
                }
            }

            //shipping, payment method fee
            string cusShipTotal;
            string cusPaymentMethodAdditionalFee;
            var taxRates = new SortedDictionary<decimal, decimal>();
            var cusTaxTotal = string.Empty;
            var cusDiscount = string.Empty;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            //shipping
            var displayShipping = order.ShippingStatus != ShippingStatus.ShippingNotRequired;

            //payment method fee
            var displayPaymentMethodFee = order.PaymentMethodAdditionalFeeExclTax > decimal.Zero;

            //tax
            bool displayTax;
            bool displayTaxRates;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = new SortedDictionary<decimal, decimal>();
                    foreach (var tr in _orderService.ParseTaxRates(order, order.TaxRates))
                        taxRates.Add(tr.Key, _currencyService.ConvertCurrency(tr.Value, order.CurrencyRate));

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    var taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        false, languageId);
                    cusTaxTotal = taxStr;
                }
            }

            //discount
            var displayDiscount = false;
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                cusDiscount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
                displayDiscount = true;
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            var cusTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            //discount (applied to order subtotal)
            if (displaySubTotalDiscount)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotalDiscount}</strong></td></tr>");
            }

            //shipping
            if (displayShipping)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Shipping", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusShipTotal}</strong></td></tr>");
            }

            //payment method fee
            if (displayPaymentMethodFee)
            {
                var paymentMethodFeeTitle = await _localizationService.GetResourceAsync("Messages.Order.PaymentMethodAdditionalFee", languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{paymentMethodFeeTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusPaymentMethodAdditionalFee}</strong></td></tr>");
            }

            //tax
            if (displayTax)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Tax", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTaxTotal}</strong></td></tr>");
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(await _localizationService.GetResourceAsync("Messages.Order.TaxRateLine"),
                        _priceFormatter.FormatTaxRate(item.Key));
                    var taxValue = await _priceFormatter.FormatPriceAsync(item.Value, true, order.CustomerCurrencyCode, false, languageId);
                    sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxRate}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxValue}</strong></td></tr>");
                }
            }

            //discount
            if (displayDiscount)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.TotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusDiscount}</strong></td></tr>");
            }

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                var giftCardText = string.Format(await _localizationService.GetResourceAsync("Messages.Order.GiftCardInfo", languageId),
                    WebUtility.HtmlEncode((await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode));
                var giftCardAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true, order.CustomerCurrencyCode,
                    false, languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardText}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardAmount}</strong></td></tr>");
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(await _localizationService.GetResourceAsync("Messages.Order.RewardPoints", languageId),
                    -redeemedRewardPointsEntry.Points);
                var rpAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate), true,
                    order.CustomerCurrencyCode, false, languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpAmount}</strong></td></tr>");
            }

            //total
            sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.OrderTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTotal}</strong></td></tr>");
        }

        #endregion

        #region Methods

        public override async Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId, int vendorId = 0)
        {
            Order dbOrder = await _orderService.GetOrderByIdAsync(order.Id);
            tokens.Add(new Token("Order.FusionOrderNumber",await dbOrder.GetFusionOrderNumberAsync(), true));
            tokens.Add(new Token("Order.OrderTotal", dbOrder.OrderTotal, true));
            var commentJson = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), ShippingDetailsAttributes.ShippingDetailsCommentAttribute, (await _storeContext.GetCurrentStoreAsync()).Id);
            tokens.Add(new Token("Order.ShippingDetailsComment",
                !string.IsNullOrWhiteSpace(commentJson) ? JsonConvert.DeserializeObject<Dictionary<int, string>>(commentJson)?[languageId] : String.Empty, true));
            await base.AddOrderTokensAsync(tokens, order, languageId, vendorId);
        }

        #endregion
    }
}
