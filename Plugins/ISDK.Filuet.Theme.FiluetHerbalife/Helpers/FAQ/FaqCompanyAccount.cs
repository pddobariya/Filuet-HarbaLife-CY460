using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ
{
    public class FaqCompanyAccount : BaseFaq
    {
        #region Ctor

        public FaqCompanyAccount(
            ILocalizedModelFactory localizedModelFactory,
            ILanguageService languageService)
            : base(localizedModelFactory, languageService)
        {

        }

        #endregion

        #region Get

        public async Task<TopicModel> Get()
        {
            var body = "Put your FAQ information here. You can edit this in the admin panel.";
            return await FaqModelFactory("Company Account",FiluetThemePluginDefaults.FaqCompanyAccount, 162, body);
        }

        #endregion

        #region GetEnLocales

        protected override void GetEnLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Company Account";
            locale.Body = "";
        }

        #endregion

        #region GetRuLocales

        protected override void GetRuLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Аккаунт компании";
            locale.Body = "";
        }

        #endregion

        #region GetLTLocales

        protected override void GetLTLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Company Account";
            locale.Body = "";
        }

        #endregion

        #region GetLVLocales

        protected override void GetLVLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Company Account";
            locale.Body = "";
        }

        #endregion

        #region GetEELocales

        protected override void GetEELocales(TopicLocalizedModel locale)
        {
            locale.Title = "Company Account";
            locale.Body = "";
        }
        
        #endregion

    }
}
