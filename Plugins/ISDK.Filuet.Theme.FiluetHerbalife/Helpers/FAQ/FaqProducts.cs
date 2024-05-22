using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ
{
    public class FaqProducts : BaseFaq
    {
        #region Ctor

        public FaqProducts(ILocalizedModelFactory localizedModelFactory,
            ILanguageService languageService)
            : base(localizedModelFactory, languageService)
        {
        }

        #endregion

        #region Get

        public async Task<TopicModel> Get()
        {
            var body = "Put your FAQ information here. You can edit this in the admin panel.";
            return await FaqModelFactory("Products", FiluetThemePluginDefaults.FaqProducts, 153, body);
        }

        #endregion

        #region GetEnLocales

        protected override void GetEnLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Products";
            locale.Body = GetQuestionAndAnswer(1,
                              "What Product Directories (from what countries) can I search at the Baltic Online Ordering System Website?",
                              "A: The Baltic Online Ordering System Website shows the Product Directory for the Baltic countries (Estonia, Latvia and Lithuania).") +
                          GetQuestionAndAnswer(2, "How do I add products to my Basket?",
                              "A: There are two simple ways to make an order: Product Directory and Order by Series Number.",
                              "<ul>" +
                              "<li>In the Product Directory mark the products that you want to order, specify the amounts and press Add to Basket.</li>" +
                              "<li>Making an order by Series Number, fill in the series number and the amount in the top right corner, then press Add to Basket.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(3, "How do I know if the product is available for ordering?",
                              "A: If you try to select an unavailable product you will get a special notification and will not be able to continue ordering. You will have to remove the unavailable product from the basket to continue ordering.");
        }

        #endregion

        #region GetRuLocales

        protected override void GetRuLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Вопросы по продукции";
            locale.Body = GetQuestionAndAnswer(1, "Продуктовые каталоги каких стран я могу посмотреть на балтийском сайте для заказов онлайн?",
                              "О: Для заказов онлайн отображается продуктовый каталог, актуальный для стран Балтии.") +
                          GetQuestionAndAnswer(2, "Как я могу добавить продукты в мою корзину?",
                              "О: Существуют 2 простых способа заказа: Каталог Продуктов и Заказ по Номеру Серии.",
                              "<ul>" +
                              "<li>В Каталоге Продуктов отметьте позиции, которые Вы хотите заказать, укажите количество и нажмите \"Добавить в Корзину\".</li>" +
                              "<li>При составлении заказа по Номеру Серии в правом верхнем углу введите номер серии и количество, далее нажмите \"Добавить в Корзину\".</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(3, "Как я могу узнать, доступен ли продукт для заказа?",
                              "О: При выборе продукта, которого нет в наличии, Вы получите сообщение. При этом продукты автоматически удалятся из корзины или их количество будет уменьшено до допустимого уровня.");
        }

        #endregion

        #region GetLTLocales

        protected override void GetLTLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Produkcija";
            locale.Body = GetQuestionAndAnswer(1, "Kokius produktų žinynus (kokių šalių) galiu peržiūrėti Baltijos šalims skirtoje užsakymo internetu svetainėje?",
                              "A. Baltijos šalims skirtoje užsakymo internetu svetainėje rodomas Baltijos šalių (Estija, Latvija ir Lietuva) produktų vadovas.") +
                          GetQuestionAndAnswer(2, "Kaip galiu pridėti produktų į savo krepšelį?",
                              "A. Yra 2 paprasti užsakymo būdai: produktų katalogas ir užsakymas pagal serijos numerį.",
                              "<ul>" +
                              "<li>Produktų kataloge pažymėkite pozicijas, kurias norite užsakyti, nurodykite kiekį ir spustelėkite „Pridėti į krepšelį“.</li>" +
                              "<li>Kai užsakymą sudarote pagal serijos numerį, viršutiniame dešiniajame kampe įveskite serijos numerį ir kiekį, tada spustelėkite „Pridėti į krepšelį“.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(3, "Kaip galiu sužinoti, ar produktą galima užsakyti?",
                              "A. Pasirinkę produktą, kurio neturime, gausite specialų pranešimą ir negalėsite toliau forminti užsakymo. Norėdami toliau forminti užsakymą, turėsite pašalinti negalimą poziciją.");
        }

        #endregion

        #region GetLVLocales

        protected override void GetLVLocales(TopicLocalizedModel locale)
        {
            locale.Title = "JAUTĀJUMI PAR PRODUKCIJU";
            locale.Body = GetQuestionAndAnswer(1, "Kuru valstu produktu katalogus es varu apskatīt Baltijas valstu tīmekļa vietnē pasūtīšanai tiešsaistē?",
                              "A: Tiešsaistes pasūtījumiem tiek parādīts Baltijas valstīm aktuāls produktu katalogs.") +
                          GetQuestionAndAnswer(2, "Kā es varu pievienot produktus savam grozam?",
                              "A: Ir divi vienkārši pasūtīšanas veidi: Produktu katalogs un pasūtījums pēc sērijas/partijas numura.",
                              "<ul>" +
                              "<li>Produktu katalogā atzīmējiet tās vienības, kuras vēlaties pasūtīt, norādiet daudzumu un spiediet - Pievienot grozam.</li>" +
                              "<li>Kad veidosiet pasūtījumu pēc sērijas numura, labā augšējā stūrī ievadiet sērijas numuru un daudzumu, tad spiediet - Pievienot grozam.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(3, "Kā es varu uzzināt vai produkts ir pieejams pasūtīšanai?",
                              "A: Ja izvēlaties produktu, kura nav noliktavā, jūs saņemsiet ziņojumu. Šajā gadījumā produkti tiks automātiski izņemti no groza vai arī to daudzums tiks samazināts līdz pieejamajam līmenim.");
        }

        #endregion

        #region GetEELocales

        protected override void GetEELocales(TopicLocalizedModel locale)
        {
            locale.Title = "Toodete kohta";
            locale.Body = GetQuestionAndAnswer(1, "Milliseid tootekatalooge (milliste riikide) saan ma näha Balti riikide interneti tellimuste veebilehel?",
                              "V: Balti riikide interneti tellimuste veebilehel kuvatakse Balti riikide (Eesti, Läti ja Leedu) tootekataloog.") +
                          GetQuestionAndAnswer(2, "Kuidas ma saan lisada tooteid oma ostukorvi?",
                              "V: On 2 lihtsat tellimisviisi: Tootekataloog ja tellimine seerianumbri järgi.",
                              "<ul>" +
                              "<li>Tootekataloogis tuleb märkida tooted, mida soovite tellida, näidata ära nende kogus ja vajutada Lisa ostukorvi.</li>" +
                              "<li>Kui esitate tellimuse seerianumbri järgi, siis sisestage paremal ülal seerianumber ja kogus, edasi vajutage Lisa ostukorvi.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(3, "Kuidas ma saan teada, kas toode on tellimiseks saadaval?",
                              "V: Toote valimisel, mida ei ole saadaval, saate vastavasisulise teate ega saa tellimuse vormistamist jätkata. Selleks, et jätkata tellimuse vormistamist, peate mittesaadavaloleva toote kustutama.");
        }
        #endregion
    }
}
