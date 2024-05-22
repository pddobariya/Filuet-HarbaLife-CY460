using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class UserInformationModel
    {
        #region Properties

        /// <summary>
        /// DistributorId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// ZipCode
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string MailingCountry { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string ResidenceCountry { get; set; }

        /// <summary>
        /// Contact number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Personally paid volume / Лично выкупленный объем (PPV)
        /// </summary>
        public double Ppv { get; set; }

        /// <summary>
        /// Personal volume / Личный объем (PV)
        /// </summary>
        public double Pv { get; set; }

        /// <summary>
        /// Total volume / Общий объем (TV)
        /// </summary>
        public double Tv { get; set; }

        /// <summary>
        /// Debts on service payment
        /// </summary>
        public bool IsDebtor { get; set; }

        /// <summary>
        /// Possible to choose the month of an order (the previous / the current / the following one)
        /// </summary>
        public bool IsDualMonthAllowed { get; set; }

        /// <summary>
        /// Distributor type (SP / DS)
        /// </summary>
        public DistributorTypeEnum DistributorType { get; set; }

        #endregion
    }
}
