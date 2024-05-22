using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Customer;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer
{
    /// <summary>
    /// Extended Customer Info Model class
    /// </summary>
    public record FiluetCustomerInfoModel : CustomerInfoModel
    {
        public FiluetCustomerInfoModel()
        {
            VolumeLimits = new List<VolumeLimits>();
        }
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public IList<VolumeLimits> VolumeLimits { get; set; }
        /// <summary>
        /// Distributor Id
        /// </summary>
        [NopResourceDisplayName("Account.Fields.DistributorId")]
        public string DistributorId { get; set; }

        /// <summary>
        /// Country, a string value
        /// </summary>
        [NopResourceDisplayName("Account.Fields.Country")]
        public string Country { get; set; }

        /// <summary>
        /// Personally paid volume / Лично выкупленный объем (PPV)
        /// </summary>
        [NopResourceDisplayName("Account.Fields.PPV")]
        public decimal? Ppv { get; set; }

        /// <summary>
        /// Personal volume / Личный объем (PV)
        /// </summary>
        [NopResourceDisplayName("Account.Fields.PV")]
        public decimal? Pv { get; set; }

        /// <summary>
        /// Total volume / Общий объем (TV)
        /// </summary>
        [NopResourceDisplayName("Account.Fields.TV")]
        public decimal? Tv { get; set; }

        [NopResourceDisplayName("Account.Fields.StreetAddress")]
        public List<string> AdditionalStreetAddresses { get; } = new List<string>();


        [NopResourceDisplayName("Account.Fields.Phone")]
        public List<string> AdditionalPhones { get; } = new List<string>();

        #endregion
    }
}
