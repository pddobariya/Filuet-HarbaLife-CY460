using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ
{
    public class FaqGeneralQuestions : BaseFaq
    {
        #region Ctor

        public FaqGeneralQuestions(
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
            return await FaqModelFactory("General questions", 
                FiluetThemePluginDefaults.FaqGeneralQuestions, 150, body);
        }

        #endregion

        #region GetEnLocales

        protected override void GetEnLocales(TopicLocalizedModel locale)
        {
            locale.Title = "General questions";
            locale.Body = GetQuestionAndAnswer(1, "Can I order Herbalife products from a country that is not in the Baltic Region?",
                              "A: Using the Baltic Online Ordering System you can place an order from any of the Baltic countries (Estonia, Latvia and Lithuania). You can order delivery to Estonia and Lithuania and you can order both delivery and collection from the Sales Centre in Latvia.") +
                          GetQuestionAndAnswer(2, "Who can place an online order?",
                              "A: Only Independent Herbalife Partners can place orders through the Baltic Online Ordering System.") +
                          GetQuestionAndAnswer(3, "Can I pay the Annual Service Payment(APF) online?",
                              "A: Yes. You can pay the Annual service payment online on the Annual service payment due day or later. The Annual service payment will be automatically added to your shopping basket.") +
                          GetQuestionAndAnswer(4, "It is a double month day today.May I choose the month of order when making an online order?",
                              "A: Yes. The field for choosing the month of order is available in the Basket on any double month day. Please use the pop-up list to choose the month for your order.") +
                          GetQuestionAndAnswer(5, "What are the advantages of ordering online?",
                              "A: Making an order online never was as easy as now!",
                              "<ul>" +
                                  "<li>The website is available 24 / 7.</li>" +
                                  "<li>The website is fully compatible with the internal Herbalife ordering system.Therefore the information about the number of points and availability of products is shown in real time.</li>" +
                                  "<li>No additional registration! To enter the site use your ID number of the Independent Partner and PIN code.</li>" +
                                  "<li>Immediate confirmation of the order and the information of the order status.</li>" +
                                  "<li>Detailed catalogue of the products and materials.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(6, "When will my order be shipped?",
                              "A: Once the money has come to the Company’s bank account, the Company has two business days to arrange the order and pass it over for the delivery. Please note that delivery of the orders placed in the end of the month can be delayed.");
        }

        #endregion

        #region GetRuLocales

        protected override void GetRuLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Общие вопросы";
            locale.Body = GetQuestionAndAnswer(1, "Могу ли я заказать продукцию Herbalife из страны, не входящей в Балтийский регион?",
                              "О: Используя балтийский сайт для онлайн заказов, Вы можете оформить заказ с доставкой/получением на территории Балтии (Латвия, Литва и Эстония).") + 
                          GetQuestionAndAnswer(2, "Кто может разместить заказ онлайн?",
                              "О: Размещение заказов на балтийском сайте доступно только для Независимых Партнеров Herbalife.") +
                          GetQuestionAndAnswer(3, "Могу ли я оплатить Ежегодный взнос за обслуживание онлайн?",
                              "О: Да. Вы можете оплатить Ежегодный взнос за обслуживание в тот день, когда у Вас возникает задолженность по оплате Ежегодного взноса, или позднее. Ежегодный взнос за обслуживание будет добавлен в Вашу корзину автоматически.") +
                          GetQuestionAndAnswer(4, "Сегодня день двойного месяца. Могу ли я выбрать месяц заказа при размещении заказа онлайн?",
                              "О: Да. В день двойного месяца поле «месяц заказа» будет доступно в Корзине. Воспользуйтесь имеющимся списком для выбора месяца, на который хотите разместить заказ.") +
                          GetQuestionAndAnswer(5, "Каковы преимущества при размещении заказа онлайн?",
                              "О: Никогда не было легче разместить заказ онлайн!",
                              "<ul>" +
                                  "<li>Сайт доступен 24 часа 7 дней в неделю.</li>" +
                                  "<li>Сайт полностью совмещен с внутренней системой заказов Herbalife. Таким образом, информация о количестве очков и наличии продуктов отображается в режиме реального времени.</li>" +
                                  "<li>Никакой дополнительной регистрации! Для входа на сайт используйте данные Вашего Личного кабинета myherbalife.com.</li>" +
                                  "<li>Моментальное подтверждение о размещении заказа и получение информации о статусе заказа.</li>" +
                                  "<li>Подробный детальный каталог по продуктам и материалам.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(6, "Когда мой заказ будет отправлен?",
                              "О: Все заказы, размещенные и оплаченные до 13:00, обрабатываются и доставляются по рабочим дням в течение 48 часов по указанному в заказе адресу.");
        }

        #endregion

        #region GetLTLocales

        protected override void GetLTLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Bendro pobūdžio klausimai";
            locale.Body = GetQuestionAndAnswer(1, "Ar galiu užsisakyti produktų iš kitų šalių?",
                              "A. Naudodamiesi Baltijos šalims skirta užsakymo internetu svetaine, užsakymus galite teikti tik Baltijos šalyse (Estija, Latvija, Lietuva). Į Estiją ir Lietuvą galite užsisakyti pristatymo paslaugą, o Latvijoje galite arba užsisakyti pristatymą, arba užsakymą atsiimti pardavimo centre.") +
                          GetQuestionAndAnswer(2, "Kas gali užsakymą pateikti internetu?",
                              "A. Baltijos šalims skirtoje užsakymo internetu svetainėje užsakymus gali teikti tik nepriklausomi Herbalife Nutrition partneriai.") +
                          GetQuestionAndAnswer(3, "Ar galiu sumokėti metinį mokestį už aptarnavimą internetu?",
                              "A. Taip. Metinį mokestį už aptarnavimą galite sumokėti internetu tą dieną, kai atsiranda metinio mokesčio už aptarnavimą įsiskolinimas arba vėliau. Metinis mokestis už aptarnavimą į jūs prekių krepšelį bus įtrauktas automatiškai.") +
                          GetQuestionAndAnswer(4, "Šiandien dvigubo mėnesio diena. Ar galiu pasirinkti užsakymo mėnesį, užsakymą teikdamas internetu?",
                              "A. Taip. Dvigubo mėnesio dieną prekių krepšelyje bus pateiktas užsakymo mėnesio laukas. Pasinaudokite sąrašu, kad pasirinktumėte užsakymo mėnesį") +
                          GetQuestionAndAnswer(5, "Kokie yra užsakymų teikimo internetu privalumai?",
                              "A. Užsakymą pateikti internetu dar niekada nebuvo lengviau!",
                              "<ul>" +
                                  "<li>Svetainė veikia 24 val. per parą, 7 dienas per savaitę.</li>" +
                                  "<li>Svetainė yra visiškai suderinta su vidine Herbalife Nutrition užsakymų sistema. Taip informacija apie taškų skaičių ir turimą produktų kiekį rodoma tikruoju laiku.</li>" +
                                  "<li>Jokios papildomos registracijos!   Norėdami patekti į svetainę, naudokite turimą ID numerį ir PIN kodą.</li>" +
                                  "<li>Apie užsakymo pateikimą patvirtinama iškart, teikiama informacija apie užsakymo būseną.</li>" +
                                  "<li>Išsamus produktų ir medžiagos katalogas.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(6, "Kada mano užsakymas bus išsiųstas?",
                              "A. Per 2 darbo dienas nuo tada, kai pinigai bus bendrovės sąskaitoje, bendrovė surinks užsakymą ir perduos jį atitinkamam pristatymo paslaugos teikėjui. Atkreipkite dėmesį, kad mėnesio gale pateikti užsakymai gali vėluoti.");
        }

        #endregion

        #region GetLTLocales

        protected override void GetLVLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Vispārēji";
            locale.Body = GetQuestionAndAnswer(1, "Vai es varu pasūtīt produkciju no citām valstīm?",
                              "A: Izmantojot Baltijas valstu tīmekļa vietni tiešsaistes pasūtījumiem, varat veikt pasūtījumu ar piegādi/saņemšanu Baltijas valstīs (Latvijā, Lietuvā un Igaunijā).") +
                          GetQuestionAndAnswer(2, "Kurš var veikt tiešsaistes pasūtījumu?",
                              "A: Pasūtījumu veikšana Baltijas valstu tīmekļa vietnē ir pieejama tikai Herbalife Nutrition neatkarīgajiem partneriem.") +
                          GetQuestionAndAnswer(3, "Vai es varu samaksāt Ikgadējo maksu par datu apstrādi?",
                              "A: Jā. Jūs varat samaksāt Ikgadējo maksu par datu apstrādi tajā pašā dienā, kad beidzas tās darbības termiņš vai vēlāk. Ikgadējā maksa par datu apstrādi tiks automātiski pievienota jūsu grozam.") +
                          GetQuestionAndAnswer(4, "Šodien ir dubultā mēneša diena. Vai es varu izvēlēties pasūtījuma mēnesi veicot pasūtījumu tiešsaistē?",
                              "A: Jā. Dubultā mēneša dienā iepirkumu grozā būs pieejams lauks \"pasūtījuma mēnesis\". Izmantojiet pieejamo sarakstu, lai izvēlētos mēnesi, par kuru vēlaties veikt pasūtījumu.") +
                          GetQuestionAndAnswer(5, "Kādas ir priekšrocības veicot pasūtījumu tiešsaistē?",
                              "A: Nekad vēl nav bijis vienkāršāk veikt pasūtījumu tiešsaistē!",
                              "<ul>" +
                                  "<li>Vietne ir pieejama 24 stundas 7 dienas nedēļā.</li>" +
                                  "<li>Vietne ir pilnībā savienota ar iekšējo Herbalife pasūtījumu sistēmu. Tādejādi, informācija par punktu skaitu un produktu pieejamību tiek atspoguļota reāllaika režīmā.</li>" +
                                  "<li>Nav nepieciešama papildu reģistrācija! Lai autorizētos tīmekļa vietnē, izmantojiet sava personīgā konta myherbalife.com datus.</li>" +
                                  "<li>Tūlītējsveiktā pasūtījumaapstiprinājumsuninformācijassaņemšanaparpasūtījumastatusu.</li>" +
                                  "<li>Plašs detalizēts katalogs par produktiem un materiāliem.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(6, "Kad tiks nosūtīts mans pasūtījums?",
                              "A: Visi pasūtījumi, kas veikti un apmaksāti pirms pulksten 13:00, tiek apstrādāti un piegādāti darba dienās 48 stundu laikā uz pasūtījumā norādīto adresi.");

        }
        #endregion

        #region GetLTLocales

        protected override void GetEELocales(TopicLocalizedModel locale)
        {
            locale.Title = "ÜLDIST";
            locale.Body = GetQuestionAndAnswer(1, "Kas ma saan tellida tooteid teistest riikidest?",
                              "V: Tellimuste esitamiseks interneti teel Balti riikidele mõeldud veebilehte kasutades võite tellida tooteid Balti riikide raames (Eesti, Läti ja Leedu). Eestis ja Leedus on võimalik tellimine koos kohaletoimetamisega, Lätis on võimalik valida kohaletoimetamise või Müügikeskusesse järeleminemise vahel.") +
                          GetQuestionAndAnswer(2, "Kes saab interneti teel tellimusi esitada?",
                              "V: Tellimuste esitamine interneti teel Balti riikide veebilehel on saadaval üksnes Herbalife’i Sõltumatutele Partneritele.") +
                          GetQuestionAndAnswer(3, "Kas ma saan maksta aastamaksu internetis?",
                              "V: Ja. Te saate interneti teel aastamaksu tasuda alates sellest päevast, kui teil tekib kohustus selle tasumiseks, või hiljem. Aastamaks lisatakse teie ostukorvi automaatselt.") +
                          GetQuestionAndAnswer(4, "Täna on topletkuu päev. Kas ma saan interneti teel tellimuse esitamisel valida tellimuse kuu?",
                              "V: Ja. Topeltkuu päeval on ostukorvis näidatud tellimise kuu väli. Kasutage nimekirja, et valida teie poolt soovitud tellimuse kuu.") +
                          GetQuestionAndAnswer(5, "Millised on interneti teel esitatud tellimuse eelised?",
                              "V: Mitte kunagi ei ole interneti teel tellimuse esitamine olnud lihtsam!",
                              "<ul>" +
                                  "<li>Veebileht on saadaval 24 tundi 7 päeva nädalas.</li>" +
                                  "<li>Veebileht on täielikult ühendatud Herbalife’i sisemise tellimissüsteemiga. Seega kuvatakse teave punktide hulga ja toodete olemasolu kohta reaalajas.</li>" +
                                  "<li>Ei mingit lisaregistreerimist! Veebilehele sisenemiseks kasutage oma Sõltumatu Partneri ID-numbrit ja PIN-koodi.</li>" +
                                  "<li>Tellimuse esitamise kohene kinnitamine ja info edastamine tellimuse staatuse kohta.</li>" +
                                  "<li>Üksikasjalik kataloog toodete ja materjalide kohta.</li>" +
                              "</ul>") +
                          GetQuestionAndAnswer(6, "Millal saadetakse minu tellimus teele?",
                              "V: Ettevõttel on aega 2 tööpäeva peale tellimuse eest arve tasumist, et tellimus kokku panna ja ära saata. Pange palun tähele, et kuu lõpus esitatud tellimuste puhul võib kohaletoimetamisel ette tulla viivitusi.");
        }

        #endregion
    }
}
