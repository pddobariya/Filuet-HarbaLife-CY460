using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ
{
    public class FaqEventTickets : BaseFaq
    {
        #region Ctor

        public FaqEventTickets(
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
            return await FaqModelFactory("Events Tickets", FiluetThemePluginDefaults.FaqEventTickets, 156, body);
        }

        #endregion

        #region GetEnLocales

        protected override void GetEnLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Events Tickets";
            locale.Body = GetQuestionAndAnswer(1, "Can I order a ticket to an event at the Baltic Online Ordering System Website?",
                              "A: Yes. Tickets available for sale can be found in section «Order event tickets».");
        }

        #endregion

        #region GetRuLocales

        protected override void GetRuLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Билеты на мероприятия";
            locale.Body = GetQuestionAndAnswer(1, "Могу ли я заказать билеты на мероприятия онлайн?",
                "О: Да. Доступные для продажи билеты вы можете найти в разделе «Заказать билеты на мероприятие».");
        }

        #endregion

        #region GetLTLocales

        protected override void GetLTLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Bilietai į renginius";
            locale.Body = GetQuestionAndAnswer(1, "Ar Baltijos šalims skirtoje užsakymo internetu svetainėje galiu užsisakyti bilietus į renginius?",
                "A: Taip. Parduodamus bilietus galite rasti dalyje „Užsisakyti bilietus į renginius“.");
        }

        #endregion

        #region GetLVLocales

        protected override void GetLVLocales(TopicLocalizedModel locale)
        {
            locale.Title = "BIĻETES UZ PASĀKUMIEM";
            locale.Body = GetQuestionAndAnswer(1, "Vai es varu pasūtīt biļetes uz pasākumiem Baltijas valstu tiešsaistes pasūtījumu vietnē?",
                "A: Jā. Pārdošanā pieejamās biļetes Jūs varat atrast sadaļā “Pasūtīt biļetes uz pasākumu”.");
        }

        #endregion

        #region GetEELocales

        protected override void GetEELocales(TopicLocalizedModel locale)
        {
            locale.Title = "Piletid üritustele";
            locale.Body = GetQuestionAndAnswer(1, "Kas ma saan Balti riikide interneti tellimuste veebilehe kaudu osta pileteid üritustele?",
                "V: Ja. Müügil olevad piletid leiate alajaotusest «Osta piletid üritusele».");
        }

        #endregion
    }
}
