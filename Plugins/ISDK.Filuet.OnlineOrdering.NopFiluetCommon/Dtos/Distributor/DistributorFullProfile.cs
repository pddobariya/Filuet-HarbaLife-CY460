namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class DistributorFullProfile
    {
        #region Properties

        public DistributorProfileResponse DistributorProfileResponse { get; set; }

        public DistributorDetailedProfileResponse DistributorDetailedProfileResponse { get; set; }

        public DistributorVolumeResponse DistributorVolumeResponse { get; set; }

        public int? Discount { get; set; }

        #endregion
    }
}
