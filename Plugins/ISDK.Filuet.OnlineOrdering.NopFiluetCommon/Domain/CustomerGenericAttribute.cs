using Nop.Core;
using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class CustomerGenericAttribute : BaseEntity
    {
        #region Properties

        public int CustomerId { get; set; }
        public string DistributorId { get; set; }
        public string Email { get; set; }
        public string Emails { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProcessingCountryCode { get; set; }
        public string ResidenceCountryCode { get; set; }
        public string Phone { get; set; }
        public string Phones { get; set; }
        public string SponsorId { get; set; }
        public string MailingCountryCode { get; set; }
        public string TypeCode { get; set; }
        public string SubTypeCode { get; set; }
        public string ResidenceCountry { get; set; }
        public string CountryOfProcessing { get; set; }
        public string Addresses { get; set; }
        public string DsType { get; set; }
        public string DsSubType { get; set; }
        public string OrderRestrictions { get; set; }
        public int ConsignmentVolumeLimit { get; set; }
        public int ConsignmentVolumeLimitUsed { get; set; }
        public int PersonalVolumeLimit { get; set; }
        public int PersonalVolumeLimitUsed { get; set; }
        public decimal TV { get; set; }
        public decimal PV { get; set; }
        public decimal PPV { get; set; }
        public bool? CantBuy { get; set; }
        public string DistributorStatus { get; set; }
        public bool? IsCustomer { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsTerminated { get; set; }
        public bool? IsLockedByDivorce { get; set; }
        public bool? IsTransitioning { get; set; }
        public bool? IsBCP { get; set; }
        public bool? ForeignSale { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? ApfDueDate { get; set; }
        public bool IsValidInfo { get; set; }
        public int? Discount { get; set; }

        #endregion
    }
}
