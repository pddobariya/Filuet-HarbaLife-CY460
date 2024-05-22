using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.CountryCode")]
        public string CountryCode { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.DefaultWarehouse")]
        public string Warehouse { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.ProcessingLocationCode")]
        public string ProcessingLocationCode { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.CurrencyCode")]
        public CurrencyCodes CurrencyCode { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.HerbalifeEnvironment")]
        public string HerbalifeEnvironment { get; set; }
        
        public bool ExcludeDeliveryPriceFromTotalForHerbalife { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.NumberOfDigitsInn")]
        public int NumberOfDigitsInn { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.OrderInfoDeliveryEmail")]
        public string OrderInfoDeliveryEmail { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.CheckPartnerUrlAddress")]
        public string CheckPartnerUrlAddress { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.MainConditionMessageBoxEnabled")]
        public bool MainConditionMessageBoxEnabled { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.IsDeptorEnabled")]
        public bool IsDeptorEnabled { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.IsCheckPartnerContract")]
        public bool IsCheckPartnerContract { get; set; }
        
        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.HoursShift")]
        public int HoursShift { get; set; }
        
        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.ShowOrderStatuses")]
        public bool ShowOrderStatuses { get; set; }
        
        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.Check1SStockBalance")]
        public bool Check1SStockBalance { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.MonthVpLimit")]
        public double MonthVpLimit { get; set; } 

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.CorePlugin.OneOrderVpLimit")]
        public double OneOrderVpLimit { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.Settings.VipCustomers")]
        public string VipCustomers { get; set; }

        #endregion
    }
}