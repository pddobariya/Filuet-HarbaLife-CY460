using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Core.Domain.Common;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class ShippingMethodMappingExtensions
    {
        #region Methods

        public static Address ToAddressEntity(this ShippingMethodModel shippingMethodModel, Address existingAddress = null)
        {
            if (existingAddress == null)
            {
                return new Address
                {
                    Address1 = shippingMethodModel.Address,
                    City = shippingMethodModel.City,
                    CountryId = shippingMethodModel.CountryId,
                    FirstName = shippingMethodModel.FirstName,
                    LastName = shippingMethodModel.LastName,
                    PhoneNumber = shippingMethodModel.PhoneNumber,
                    ZipPostalCode = shippingMethodModel.ZipPostalCode
                };
            }
            else
            {
                existingAddress.Address1 = shippingMethodModel.Address;
                existingAddress.City = shippingMethodModel.City;
                existingAddress.CountryId = shippingMethodModel.CountryId;
                existingAddress.FirstName = shippingMethodModel.FirstName;
                existingAddress.LastName = shippingMethodModel.LastName;
                existingAddress.PhoneNumber = shippingMethodModel.PhoneNumber;
                existingAddress.ZipPostalCode = shippingMethodModel.ZipPostalCode;

                return existingAddress;
            }
        }

        #endregion
    }
}
