using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public partial class DistributorDetailedProfileResponse
    {
        #region Properties

        [JsonProperty("addresses")]
        public Addresses Addresses { get; set; }

        [JsonProperty("apfDueDate")]
        public DateTimeOffset ApfDueDate { get; set; }

        [JsonProperty("birthDate")]
        public DateTimeOffset? BirthDate { get; set; }

        [JsonProperty("dsSubType")]
        public string DsSubType { get; set; }

        [JsonProperty("dsType")]
        public string DsType { get; set; }

        [JsonProperty("emails")]
        public Email[] Emails { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("flags")]
        public Flags Flags { get; set; }

        [JsonProperty("memberId")]
        public string MemberId { get; set; }

        [JsonProperty("names")]
        public Names Names { get; set; }

        [JsonProperty("phones")]
        public string[] Phones { get; set; }

        [JsonProperty("processingCountryCode")]
        public string ProcessingCountryCode { get; set; }

        [JsonProperty("residenceCountryCode")]
        public string ResidenceCountryCode { get; set; }

        [JsonProperty("mailingCountryCode")]
        public string MailingCountryCode { get; set; }

        [JsonProperty("sponsorId")]
        public string SponsorId { get; set; }

        [JsonProperty("teamLevel")]
        public object TeamLevel { get; set; }

        [JsonProperty("tenCustomerFormStatus")]
        public object TenCustomerFormStatus { get; set; }

        [JsonProperty("volumeLimits")]
        public VolumeLimits VolumeLimits { get; set; }

        [JsonProperty("custCategoryType")]
        public object CustCategoryType { get; set; }

        [JsonProperty("orderRestrictions")]
        public object OrderRestrictions { get; set; }

        [JsonProperty("cantBuyReasons")]
        public string[] CantBuyReasons { get; set; }

        [JsonProperty("staticDiscount")]
        public double Discount { get; set; }


        [JsonIgnore]
        public DistributorTypes DistributorType
        {
            get
            {
                if (DsType != "DS")
                {
                    return DistributorTypes.Supervisor;
                }
                return DistributorTypes.Distributor;
            }
        }

        [JsonIgnore]
        public string Email => Emails?.FirstOrDefault()?.ToString();

        [JsonIgnore]
        public string FirstName => Names?.English.First;

        [JsonIgnore]
        public string LastName => Names?.English.Last;

        [JsonIgnore]
        public string Phone => Phones?.FirstOrDefault();
    }

    public partial class Addresses
    {
        [JsonProperty("billingAddress")]
        public Address BillingAddress { get; set; }

        [JsonProperty("fiscalAddress")]
        public Address FiscalAddress { get; set; }

        [JsonProperty("mailingAddress")]
        public Address MailingAddress { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("cloudId")]
        public string CloudId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countyDistrict")]
        public string CountyDistrict { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public object LastUpdatedDate { get; set; }

        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("line2")]
        public string Line2 { get; set; }

        [JsonProperty("line3")]
        public string Line3 { get; set; }

        [JsonProperty("line4")]
        public string Line4 { get; set; }

        [JsonProperty("nickName")]
        public string NickName { get; set; }

        [JsonProperty("personCloudId")]
        public string PersonCloudId { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("stateProvinceTerritory")]
        public string StateProvinceTerritory { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonIgnore]
        public string FullAddress
        {
            get
            {
                return $"{PostalCode ?? string.Empty}, {City ?? string.Empty}, {Country ?? string.Empty}, " +
                    $"{CountyDistrict ?? string.Empty}, {Line1 ?? string.Empty}, {Line2 ?? string.Empty}, " +
                    $"{Line3 ?? string.Empty}, {Line4 ?? string.Empty}";
            }
        }
    }

    public partial class Email
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("type")]
        public object Type { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("cloudId")]
        public object CloudId { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public object LastUpdatedDate { get; set; }

        public override string ToString()
            => Address;
    }

    public partial class Flags
    {
        [JsonProperty("advisoryRequired")]
        public bool AdvisoryRequired { get; set; }

        [JsonProperty("cantBuy")]
        public bool CantBuy { get; set; }

        [JsonProperty("distributorStatus")]
        public string DistributorStatus { get; set; }

        [JsonProperty("hardCashOnly")]
        public bool HardCashOnly { get; set; }

        [JsonProperty("isCustomer")]
        public bool IsCustomer { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("isTerminated")]
        public bool IsTerminated { get; set; }

        [JsonProperty("isLockedByDivorce")]
        public bool IsLockedByDivorce { get; set; }

        [JsonProperty("isTransitioning")]
        public bool IsTransitioning { get; set; }

        [JsonProperty("isBCP")]
        public bool IsBcp { get; set; }

        [JsonProperty("orderRestriction")]
        public bool OrderRestriction { get; set; }

        [JsonProperty("foreignSale")]
        public bool ForeignSale { get; set; }
    }

    public partial class Names
    {
        [JsonProperty("english")]
        public English English { get; set; }

        [JsonProperty("local")]
        public English Local { get; set; }
    }

    public partial class English
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("middle")]
        public string Middle { get; set; }
    }

    public partial class VolumeLimits
    {
        [JsonProperty("consignmentVolumeLimit")]
        public long ConsignmentVolumeLimit { get; set; }

        [JsonProperty("consignmentVolumeLimitUsed")]
        public long ConsignmentVolumeLimitUsed { get; set; }

        [JsonProperty("personalVolumeLimit")]
        public long PersonalVolumeLimit { get; set; }

        [JsonProperty("personalVolumeLimitUsed")]
        public long PersonalVolumeLimitUsed { get; set; }
    }

    public partial class DistributorDetailedProfileResponse
    {
        public static DistributorDetailedProfileResponse FromJson(string json) => JsonConvert.DeserializeObject<DistributorDetailedProfileResponse>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this DistributorDetailedProfileResponse self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    #endregion
}
