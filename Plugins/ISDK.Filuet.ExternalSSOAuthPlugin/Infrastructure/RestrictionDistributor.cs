using System;
using System.Text;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure
{
    public class RestrictionDistributor
    {
        #region Methods

        public string DsTypes { get; set; }
        public bool AccessAllowed { get; set; }
        public string ResidenceCountryCodes { get; set; }

        public string[] GetDsTypes()
        {
            return DsTypes.Split(new char[] { ',', ';', ' ' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public void SetDsTypes(string[] dsTypes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var dsType in dsTypes)
                sb.Append(dsType + ",");

            DsTypes = sb.ToString();
        }

        public string[] GetResidenceCountryCodes()
        {
            return ResidenceCountryCodes.Split(new char[] { ',', ';', ' ' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public void SetResidenceCountryCodes(string[] residenceCountryCodes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var residenceCountry in residenceCountryCodes)
                sb.Append(residenceCountry + ",");

            ResidenceCountryCodes = sb.ToString();
        }

        #endregion
    }
}
