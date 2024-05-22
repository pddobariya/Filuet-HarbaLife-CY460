namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos
{
    public class PickupPointsFilterDto
    {
        #region Properties

        public int? CountryId { get; }
        public string City { get; }
        public string NameOrAddress { get; }

        public PickupPointsFilterDto(int? countryId, string city, string nameOrAddress)
        {
            CountryId = countryId;
            City = city;
            NameOrAddress = nameOrAddress;
        }

        #endregion
    }
}