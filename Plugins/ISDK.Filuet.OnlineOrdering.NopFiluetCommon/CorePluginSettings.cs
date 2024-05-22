﻿using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core.Configuration;
using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon
{
    public class CorePluginSettings : ISettings
    {
        #region Properties

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CountryCode { get; set; }
        public string Warehouse { get; set; }
        public string ProcessingLocationCode { get; set; }
        public CurrencyCodes CurrencyCode { get; set; }
        public string HerbalifeEnvironment { get; set; }
        public int NumberOfDigitsInn { get; set; }
        public string OrderInfoDeliveryEmail { get; set; }
        public string CheckPartnerUrlAddress { get; set; }
        public bool MainConditionMessageBoxEnabled { get; set; }
        public bool IsDeptorEnabled { get; set; }
        public int HoursShift { get; set; }
        public bool IsCheckPartnerContract { get; set; }
        public bool ShowOrderStatuses { get; set; }
        public bool Check1SStockBalance { get; set; }

        public double MonthVpLimit { get; set; }

        public double OneOrderVpLimit { get; set; }

        public string FreightCodeForDiscountAMB { get; set; }

        public (double month, double oneOrder) GetLimits()
        {
            return (MonthVpLimit, OneOrderVpLimit);
        }

        //inherited from SSOAuthPluginSetting
        public bool IsEnableCountryRestrictions { get; set; }
        public bool IsProhibitedForNotResident { get; set; }
        public string WhitelistedCountries { get; set; }

        #endregion
    }
}