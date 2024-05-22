using AutoMapper;
using Filuet.Hrbl.Ordering.Abstractions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using Newtonsoft.Json;
using Nop.Core.Infrastructure.Mapper;
using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Infrastructure
{
    class DistributorInfoMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Methods

        public DistributorInfoMapperConfiguration()
        {
            CreateMap<DistributorInfoResponse, CustomerGenericAttribute>()
                .ForMember(model => model.Email, options => options.MapFrom(s => s.Email))
                .ForMember(model => model.FirstName, options => options.MapFrom(s => s.FirstName))
                .ForMember(model => model.LastName, options => options.MapFrom(s => s.LastName))
                .ForMember(model => model.Phone, options => options.MapFrom(s => s.Phone))
                .ForMember(model => model.ProcessingCountryCode, options => options.MapFrom(s => s.ProcessingCountryCode))
                .ForMember(model => model.ResidenceCountryCode, options => options.MapFrom(s => s.ResidenceCountryCode))
                .ForMember(model => model.SponsorId, options => options.MapFrom(s => s.SponsorId))
                .ForMember(model => model.DistributorId, options => options.MapFrom(s => s.Id))
                .ForMember(model => model.Id, options => options.Ignore());

            CreateMap<CustomerGenericAttribute, DistributorDetailedProfileResponse>()
                .ForMember(model => model.Phones,
                    options => options.MapFrom(s => JsonConvert.DeserializeObject<string[]>(s.Phones, Converter.Settings)))
                .ForMember(model => model.Flags,
                    options => options.MapFrom(s => s))
                .ForMember(model => model.MemberId,
                    options => options.MapFrom(s => s.DistributorId))
                .ForMember(model => model.Emails,
                    options => options.MapFrom(s => JsonConvert.DeserializeObject<Email[]>(s.Emails, Converter.Settings)))
                .ForMember(model => model.Addresses,
                    options => options.MapFrom(s => JsonConvert.DeserializeObject<Addresses>(s.Addresses, Converter.Settings)))
                .ForMember(model => model.DsType,
                    options => options.MapFrom(s => s.DsType))
                .ForMember(model => model.DsSubType,
                    options => options.MapFrom(s => s.DsSubType))
                .ForMember(model => model.OrderRestrictions,
                    options => options.MapFrom(s => s.OrderRestrictions))
                .ForMember(model => model.ApfDueDate,
                    options => options.MapFrom(s => s.ApfDueDate == null ? DateTimeOffset.Now : new DateTimeOffset(s.ApfDueDate.Value)));

            CreateMap<CustomerGenericAttribute, VolumeLimits>()
                .ForMember(model => model.ConsignmentVolumeLimit,
                    options => options.MapFrom(s => s.ConsignmentVolumeLimit))
                .ForMember(model => model.ConsignmentVolumeLimitUsed,
                    options => options.MapFrom(s => s.ConsignmentVolumeLimitUsed))
                .ForMember(model => model.PersonalVolumeLimit,
                    options => options.MapFrom(s => s.PersonalVolumeLimit))
                .ForMember(model => model.PersonalVolumeLimitUsed,
                    options => options.MapFrom(s => s.PersonalVolumeLimitUsed));

            CreateMap<CustomerGenericAttribute, Flags>()
                .ForMember(model => model.CantBuy,
                    options => options.MapFrom(s => s.CantBuy))
                .ForMember(model => model.DistributorStatus,
                    options => options.MapFrom(s => s.DistributorStatus))
                .ForMember(model => model.IsCustomer,
                    options => options.MapFrom(s => s.IsCustomer))
                .ForMember(model => model.IsDeleted,
                    options => options.MapFrom(s => s.IsDeleted))
                .ForMember(model => model.IsTerminated,
                    options => options.MapFrom(s => s.IsTerminated))
                .ForMember(model => model.IsLockedByDivorce,
                    options => options.MapFrom(s => s.IsLockedByDivorce))
                .ForMember(model => model.IsTransitioning,
                    options => options.MapFrom(s => s.IsTransitioning))
                .ForMember(model => model.IsBcp,
                    options => options.MapFrom(s => s.IsBCP))
                .ForMember(model => model.ForeignSale,
                    options => options.MapFrom(s => s.ForeignSale));

            CreateMap<CustomerGenericAttribute, DistributorProfileResponse>()
                .ForMember(model => model.Email,
                    options => options.MapFrom(s => s.Email))
                .ForMember(model => model.FirstName,
                    options => options.MapFrom(s => s.FirstName))
                .ForMember(model => model.LastName,
                    options => options.MapFrom(s => s.LastName))
                .ForMember(model => model.ProcessingCountryCode,
                    options => options.MapFrom(s => s.ProcessingCountryCode))
                .ForMember(model => model.ResidenceCountryCode,
                    options => options.MapFrom(s => s.ResidenceCountryCode))
                .ForMember(model => model.Phone,
                    options => options.MapFrom(s => s.Phone))
                .ForMember(model => model.SponsorId,
                    options => options.MapFrom(s => s.SponsorId))
                .ForMember(model => model.MailingCountryCode,
                    options => options.MapFrom(s => s.MailingCountryCode))
                .ForMember(model => model.Type,
                    options => options.MapFrom(s => s.TypeCode))
                .ForMember(model => model.SubType,
                    options => options.MapFrom(s => s.SubTypeCode))
                .ForMember(model => model.CountryOfResidence,
                    options => options.MapFrom(s => s.ResidenceCountry))
                .ForMember(model => model.CountryOfProcessing,
                    options => options.MapFrom(s => s.CountryOfProcessing));

            CreateMap<CustomerGenericAttribute, DistributorVolumeResponse>()
                .ForMember(model => model.TvValue,
                    options => options.MapFrom(s => s.TV))
                .ForMember(model => model.PvValue,
                    options => options.MapFrom(s => s.PV))
                .ForMember(model => model.PpvValue,
                    options => options.MapFrom(s => s.PPV));

            CreateMap<CustomerGenericAttribute, DistributorFullProfile>()
                .ForMember(model => model.DistributorDetailedProfileResponse,
                    options => options.MapFrom(s => s))
                .ForMember(model => model.DistributorProfileResponse, options => options.MapFrom(s => s))
                .ForMember(model => model.DistributorVolumeResponse,
                    options => options.MapFrom(
                        s => s));

            CreateMap<DistributorFullProfile, CustomerGenericAttribute>()
                .ForMember(model => model.CustomerId, options => options.Ignore())
                .ForMember(model => model.DistributorId, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.MemberId))
                .ForMember(model => model.Email, options => options.MapFrom(s => s.DistributorProfileResponse.Email))
                .ForMember(model => model.Emails, options => options.MapFrom(s => JsonConvert.SerializeObject(s.DistributorDetailedProfileResponse.Emails, Converter.Settings)))
                .ForMember(model => model.Phones, options => options.MapFrom(s => JsonConvert.SerializeObject(s.DistributorDetailedProfileResponse.Phones, Converter.Settings)))
                .ForMember(model => model.FirstName, options => options.MapFrom(s => s.DistributorProfileResponse.FirstName))
                .ForMember(model => model.LastName, options => options.MapFrom(s => s.DistributorProfileResponse.LastName))
                .ForMember(model => model.ProcessingCountryCode, options => options.MapFrom(s => s.DistributorProfileResponse.ProcessingCountryCode))
                .ForMember(model => model.ResidenceCountryCode, options => options.MapFrom(s => s.DistributorProfileResponse.ResidenceCountryCode))
                .ForMember(model => model.Phone, options => options.MapFrom(s => s.DistributorProfileResponse.Phone))
                .ForMember(model => model.SponsorId, options => options.MapFrom(s => s.DistributorProfileResponse.SponsorId))
                .ForMember(model => model.MailingCountryCode, options => options.MapFrom(s => s.DistributorProfileResponse.MailingCountryCode))
                .ForMember(model => model.TypeCode, options => options.MapFrom(s => s.DistributorProfileResponse.Type))
                .ForMember(model => model.SubTypeCode, options => options.MapFrom(s => s.DistributorProfileResponse.SubType))
                .ForMember(model => model.ResidenceCountry, options => options.MapFrom(s => s.DistributorProfileResponse.CountryOfResidence))
                .ForMember(model => model.CountryOfProcessing, options => options.MapFrom(s => s.DistributorProfileResponse.CountryOfProcessing))
                .ForMember(model => model.Addresses, options => options.MapFrom(s => JsonConvert.SerializeObject(s.DistributorDetailedProfileResponse.Addresses, Converter.Settings)))
                .ForMember(model => model.DsType, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.DsType))
                .ForMember(model => model.DsSubType, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.DsSubType))
                .ForMember(model => model.OrderRestrictions, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.OrderRestrictions))
                .ForMember(model => model.ConsignmentVolumeLimit, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.VolumeLimits.ConsignmentVolumeLimit))
                .ForMember(model => model.ConsignmentVolumeLimitUsed, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.VolumeLimits.ConsignmentVolumeLimitUsed))
                .ForMember(model => model.PersonalVolumeLimit, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.VolumeLimits.PersonalVolumeLimit))
                .ForMember(model => model.PersonalVolumeLimitUsed, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.VolumeLimits.PersonalVolumeLimitUsed))
                .ForMember(model => model.TV, options => options.MapFrom(s => s.DistributorVolumeResponse.TvValue))
                .ForMember(model => model.PV, options => options.MapFrom(s => s.DistributorVolumeResponse.PvValue))
                .ForMember(model => model.PPV, options => options.MapFrom(s => s.DistributorVolumeResponse.PpvValue))
                .ForMember(model => model.CantBuy, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.CantBuy))
                .ForMember(model => model.DistributorStatus, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.DistributorStatus))
                .ForMember(model => model.IsCustomer, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.IsCustomer))
                .ForMember(model => model.IsDeleted, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.IsDeleted))
                .ForMember(model => model.IsTerminated, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.IsTerminated))
                .ForMember(model => model.IsLockedByDivorce, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.IsLockedByDivorce))
                .ForMember(model => model.IsTransitioning, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.IsTransitioning))
                .ForMember(model => model.IsBCP, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.IsBcp))
                .ForMember(model => model.ForeignSale, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.Flags.ForeignSale))
                .ForMember(model => model.SubmitTime, options => options.MapFrom(s => DateTime.Now))
                .ForMember(model => model.ApfDueDate, options => options.MapFrom(s => s.DistributorDetailedProfileResponse.ApfDueDate.Date))
                .ForMember(model => model.IsValidInfo, options => options.MapFrom(s => true));

                CreateMap<SubmitRequestPayment, SubmitRequestPayment>();
                CreateMap<SubmitRequestHeader, SubmitRequestHeader>();

        }

        public int Order => 0;

        #endregion
    }
}
