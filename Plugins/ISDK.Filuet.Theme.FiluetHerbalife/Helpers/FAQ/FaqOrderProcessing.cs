using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ
{
    public class FaqOrderProcessing : BaseFaq
    {
        #region Ctor

        public FaqOrderProcessing(ILocalizedModelFactory localizedModelFactory,
            ILanguageService languageService)
            : base(localizedModelFactory, languageService)
        {
        }

        #endregion

        #region Get

        public async Task<TopicModel> Get()
        {
            var body = "Put your FAQ information here. You can edit this in the admin panel.";
            return await FaqModelFactory("Order processing", FiluetThemePluginDefaults.FaqOrderProcessing, 159, body);
        }

        #endregion

        #region GetEnLocales

        protected override void GetEnLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Order processing";
            locale.Body = GetQuestionAndAnswer(1, "What is an order processing window?",
                              "A: The order processing window is used to confirm that your order is filled or selected correctly.") +
                          GetQuestionAndAnswer(2,
                              "Can I complete the order without confirming it in the order processing window?",
                              "A: No, it is not possible to complete the order without confirming it in the order processing window.") +
                          GetQuestionAndAnswer(3,
                              "I would like to change the order delivery address. Where can I do it?",
                              "A: You can change the delivery or collection method in the Sales Centre (only in Latvia) or at the Delivery page during the order processing.") +
                          GetQuestionAndAnswer(4,
                              "Will the Basket contents remain if I leave the website or the order page?",
                              "A: Yes. The Basket contents will remain. Please note that the availability of the products in your Basket will be re-checked when you come back to the order page. If any product is not available at the warehouse when you return to the order, then you will have to remove it from the Basket by making a tick and renewing the Basket.") +
                          GetQuestionAndAnswer(5, "What are the order delivery options and what is the price?",
                              "A: The following delivery options are available:",
                              "<p><strong>OMNIVA: delivery to packomats.</strong> Cost of delivery within the Baltic states is EUR 4.45 plus VAT and usually the term of delivery is two business days. Please ask for details from Herbalife Sales Centre in Riga.</p>" +
                              "<p><strong>ITELLA: delivery to packomats.</strong> Cost of delivery within the Baltic states is EUR 4.45 plus VAT and usually the term of delivery is two business days. Please ask for details from Herbalife Sales Centre in Riga.</p>" +
                              "<p><strong>DPD: delivery to packomats.</strong> Cost of delivery within the Baltic states is EUR 4.45 plus VAT and usually the term of delivery is two business days. Please ask for details from Herbalife Sales Centre in Riga.</p>" +
                              "<p><strong>DPD: courier delivery.</strong> Cost of delivery within the Baltic states is EUR 8.50 plus VAT and usually the term of delivery is two business days. Please ask for details from Herbalife Sales Centre in Riga.</p>" +
                              "<p><strong>DPD: courier delivery at the specified time (B2C).</strong> Cost of delivery within the Baltic states is EUR 11.00 plus VAT and usually the term of delivery is two business days. Please ask for details from Herbalife Sales Centre in Riga.</p>") +
                          GetQuestionAndAnswer(6,
                              "What are the obligatory procedures for placing orders collectible from the Sales Centre (Latvia only)?",
                              "A: Any order collectible from the Sales Centre shall be collected within 10 days after the money for the order has come to the Company’s account.") +
                          GetQuestionAndAnswer(7,
                              "If I specify another e-mail address in the Notification by e-mail field when placing my order, will it change the e-mail address in my profile?",
                              "A: No. The e-mail address in the Notification by e-mail field is used only to notify you about the successful placement if this individual order.") +
                          GetQuestionAndAnswer(8,
                              "I would like to receive the confirmation of the successful order placement to an e-mail address, different from the one in my profile. How can I do it?",
                              "A: You can receive the order confirmation at any e-mail address. Just specify the required e-mail address in the E-mail field when making the order.") +
                          GetQuestionAndAnswer(9, "How can I review my previous orders?",
                              "A: As soon as you have submitted your order, you can review it in My data and reports section of MyHerbalife.com website, by clicking on My orders. Type in your order number in Search the Order Numbers field and click Forward. The data of the specified order will be shown.") +
                          GetQuestionAndAnswer(10,
                              "I would like to remove products from the order or change the amount. Where can I do it?",
                              "A: You can remove products or change the amount in the Basket.") +
                          GetQuestionAndAnswer(11, "How can I change the order before payment?",
                              "A: When asked to confirm the order, press Back. You will be readdressed to the order processing page, where you can change the contents of the order, delivery or collection address or continue buying.") +
                          GetQuestionAndAnswer(12,
                              "What is the difference between the order documents delivery methods: Send the documents with the order and Send the documents to the Independent Partner’s mail address?",
                              "A: Send the documents with the order – the invoice and the package list will be mailed with the ordered products. Send the documents to the Independent Partner’s mail address – the invoice and the package list will be mailed separately to the Independent Partner’s mail address.") +
                          GetQuestionAndAnswer(13, "What methods of payment can I use to pay for my online order?0",
                              "A: You can pay with a bank card only.") +
                          GetQuestionAndAnswer(14, "What types of the bank cards do you accept?",
                              "A: You can pay with personal MasterCard, Visa (Classic / Electron), Maestro.") +
                          GetQuestionAndAnswer(15, "Can I pay my order with more than one credit cards?",
                              "A: You can pay your online order with one bank card only.") +
                          GetQuestionAndAnswer(16, "Why cannot I use my bank card to pay for the order?",
                              "A: Some banks my restrict the use of their cards for Internet payments. In order to be able to pay for the orders, switch on the Internet payment functionality of your bank card!") +
                          GetQuestionAndAnswer(17, "Will I receive a confirmation of the successful order placement?",
                              "A: Yes. The confirmation of the successful placement of order will be sent to the e-mail address that you specified.");
        }

        #endregion

        #region GetRuLocales

        protected override void GetRuLocales(TopicLocalizedModel locale)
        {
            locale.Title = "Оформление заказа";
            locale.Body = GetQuestionAndAnswer(1, "Возможно ли завершить размещение заказа, не подтверждая «окно оформления заказа»?",
                              "О: Нет, завершить размещение заказа, не подтвердив окно оформления заказа, невозможно.") +
                          GetQuestionAndAnswer(2, "Я бы хотел изменить адрес доставки для данного заказа. Где я могу это сделать?",
                              "О: На странице Способ доставки в процессе размещения заказа до момента его оплаты.") +
                          GetQuestionAndAnswer(3, "Сохранится ли содержимое корзины, если я уйду с сайта или со страницы заказа?",
                              "О: Да. Содержание корзины сохранится. Пожалуйста, имейте в виду, что, когда Вы вернетесь к заказу, наличие продуктов в Вашей корзине будет перепроверено. Если какого-либо продукта не окажется на складе в момент возращения к заказу, то он будет автоматически удален из корзины, либо его количество в заказе будет уменьшено. При этом Вы получите уведомление обо всех произведённых изменениях и увидите обновленную стоимость заказа.") +
                          GetQuestionAndAnswer(4, "Какие существуют способы доставки заказа и их стоимость?",
                              "О: Существуют следующие способы доставки заказов:",
                              "<p><strong>OMNIVA: доставка в пакоматы.</strong> Стоимость доставки по странам Балтии составляет 4,45 евро + НДС, сроки доставки заказа в большинстве случаев составляют 2 рабочих дня. За более подробной информацией, пожалуйста, обращайтесь в Центр Продаж Herbalife в Риге.</p>" +
                              "<p><strong>ITELLA: доставка в пакоматы.</strong> Стоимость доставки по странам Балтии составляет 4,45 евро + НДС, сроки доставки заказа в большинстве случаев составляют 2 рабочих дня. За более подробной информацией, пожалуйста, обращайтесь в Центр Продаж Herbalife в Риге.</p>" +
                              "<p><strong>DPD: доставка в пакоматы.</strong> Стоимость доставки по странам Балтии составляет 4,45 евро + НДС, сроки доставки заказа в большинстве случаев составляют 2 рабочих дня. За более подробной информацией, пожалуйста, обращайтесь в Центр Продаж Herbalife в Риге.</p>" +
                              "<p><strong>DPD: курьерская доставка.</strong> Стоимость доставки по странам Балтии составляет 8,50 евро + НДС, сроки доставки заказа в большинстве случаев составляют 2 рабочих дня. За более подробной информацией, пожалуйста, обращайтесь в Центр Продаж Herbalife в Риге.</p>" +
                              "<p><strong>DPD: курьерская доставка с выбором интервала времени (B2C).</strong> Стоимость доставки по странам Балтии составляет 11,00 евро + НДС, сроки доставки заказа в большинстве случаев составляют 2 рабочих дня. За более подробной информацией, пожалуйста, обращайтесь в Центр Продаж Herbalife в Риге.</p>") +
                          GetQuestionAndAnswer(5, "В течение какого времени необходимо забрать заказ, размещенный с получением в Центре Продаж в Риге?",
                              "О: Все заказы, размещенные с получением в Центре Продаж, должны быть получены в течение 10 рабочих дней с момента поступления денег в оплату заказа.") +
                          GetQuestionAndAnswer(6, "Я бы хотел получить подтверждение об успешном размещении заказа на электронный адрес, отличающийся от указанного в базе. Как это сделать?",
                              "О: Вы можете получить подтверждение о размещении заказа на любой электронный адрес. Введите желаемый адрес электронной почты в поле «Э-почта» при оформлении заказа.") +
                          GetQuestionAndAnswer(7, "Как я могу просмотреть свои предыдущие заказы?",
                              "О: Как только Ваш заказ был отправлен, Вы можете просмотреть информацию о нем на сайте myherbalife.com в разделе «Мои данные и отчеты», нажав на раздел «Мои заказы». Введите номер заказа в поле «Поиск по Номеру заказа» и нажмите «Вперед». Детали заказа будут выведены на экран.") +
                          GetQuestionAndAnswer(8, "Я бы хотел удалить продукты из заказа или изменить их количество. Где я могу это сделать?",
                              "О: Вы можете удалить продукты или изменить их количество в Корзине заказа.") +
                          GetQuestionAndAnswer(9, "Как внести изменения в заказ перед совершением оплаты?",
                              "О: При подтверждении заказа нажмите на кнопку «Назад». Вы перейдете на страницу оформления заказа, где сможете изменить позиции в заказе, данные по доставке или месту выдачи или продолжить размещение заказа.") +
                          GetQuestionAndAnswer(10, "Какие способы оплаты я могу использовать для оплаты моего заказа онлайн?",
                              "О: Оплата может быть произведена только банковской картой.") +
                          GetQuestionAndAnswer(11, "Какие типы банковских карт принимаются онлайн?",
                              "О: К оплате на сайте принимаются именные банковские карты MasterCard, Visa (Classic / Electron), Maestro.") +
                          GetQuestionAndAnswer(12, "Можно ли оплатить заказ несколькими кредитными картами?",
                              "О: При размещении заказа онлайн оплату можно произвести только одной банковской картой.") +
                          GetQuestionAndAnswer(13, "Заказ не может быть оплачен моей банковской картой. Возможная причина?",
                              "О: Некоторые банки могут устанавливать ограничения на выпущенные ими карты для расчета в сети интернет. Чтобы оплачивать покупки картой, подключите для карты расчёты в интернете!") +
                          GetQuestionAndAnswer(14, "Получу ли я подтверждение об успешном размещении заказа?",
                              "О: Да. Подтверждение об успешном размещении заказа будет отправлено на адрес электронной почты, указанный при размещении заказа.");
        }

        #endregion

        #region GetLTLocales

        protected override void GetLTLocales(TopicLocalizedModel locale)
        {
            locale.Title = "UŽSAKYMO ĮFORMINIMAS";
            locale.Body = GetQuestionAndAnswer(1,
                              "Kas yra užsakymo įforminimo langas?",
                              "A: Užsakymo įforminimo langas naudojamas jūsų įvestam ar pasirinktam užsakymui patvirtinti.") +
                          GetQuestionAndAnswer(2,
                              "Ar galima baigti užsakymo pateikimą nepatvirtinus užsakymo įforminimo lango?",
                              "A. Ne, nepatvirtinus užsakymo įforminimo lango užsakymo pateikimo baigti negalima.") +
                          GetQuestionAndAnswer(3,
                              "Norėčiau pakeisti šio užsakymo pristatymo adresą. Kur galiu tai padaryti?",
                              "A. Pristatymo arba išdavimo pardavimo centre (galima tik Latvijoje) duomenis galite keisti užsakymo teikimo puslapyje Pristatymo būdas.") +
                          GetQuestionAndAnswer(4,
                              "Ar krepšelio turinys nedings, jei išeisiu iš svetainės ar užsakymo puslapio?",
                              "A. Taip. Krepšelio turinys išliks. Turėkite omenyje, kad grįžus prie užsakymo produktų kiekis krepšelyje nebus patikrintas. Jei grįžus prie užsakymo kokio nors produkto nebus sandėlyje, jį reikės pašalinti iš krepšelio, pažymint varnele ir atnaujinant krepšelį.") +
                          GetQuestionAndAnswer(5,
                              "Kokie yra užsakymo siuntimo variantai ir kokiomis kainomis?",
                              "A. Užsakymų pristatymo būdai yra šie:",
                              "<p><strong>OMNIVA – pristatymas į paštomatus.</strong> Pristatymo kaina Baltijos šalyse yra 4,45 EUR + PVM, užsakymo pristatymo terminas dažniausiai yra 2 darbo dienos. Dėl išsamesnės informacijos kreipkitės į Herbalife Nutrition pardavimo centrą Rygoje.</p>" +
                              "<p>ITELLA – pristatymas į paštomatus.<strong></strong> Pristatymo kaina Baltijos šalyse yra 4,45 EUR + PVM, užsakymo pristatymo terminas dažniausiai yra 2 darbo dienos. Dėl išsamesnės informacijos kreipkitės į Herbalife Nutrition pardavimo centrą Rygoje.</p>" +
                              "<p>DPD – pristatymas į paštomatus.<strong></strong> Pristatymo kaina Baltijos šalyse yra 4,45 EUR + PVM, užsakymo pristatymo terminas dažniausiai yra 2 darbo dienos. Dėl išsamesnės informacijos kreipkitės į Herbalife Nutrition pardavimo centrą Rygoje.</p>" +
                              "<p>DPD – pristatymas per kurjerius.<strong></strong> Pristatymo kaina Baltijos šalyse yra 8,50 EUR + PVM, užsakymo pristatymo terminas dažniausiai yra 2 darbo dienos. Dėl išsamesnės informacijos kreipkitės į Herbalife Nutrition pardavimo centrą Rygoje.</p>" +
                              "<p><strong>DPD – pristatymas per kurjerius pasirenkant laiko intervalą (B2C).</strong> Pristatymo kaina Baltijos šalyse yra 11,00 EUR + PVM, užsakymo pristatymo terminas dažniausiai yra 2 darbo dienos. Dėl išsamesnės informacijos kreipkitės į Herbalife Nutrition pardavimo centrą Rygoje.</p>") +
                          GetQuestionAndAnswer(6,
                              "Kokios yra būtinos procedūros, teikiant užsakymus su atsiėmimu pardavimo centre (tik Latvijai)?",
                              "A. Visi užsakymai, išduodami pardavimo centre, turi būti atsiimti per 10 darbo dienų nuo tada, kai bus gauti pinigai už užsakymo apmokėjimą.") +
                          GetQuestionAndAnswer(7,
                              "Jei teikdamas užsakymą nurodysiu naują el. pašto adresą patvirtinimo el. paštu lauke, ar dėl to nepasikeis el. pašto adresas mano profilyje?",
                              "A. Ne. El. pašto adresas lauke „Patvirtinimas el. paštu“ reikalingas tik pranešimui apie sėkmingą šio užsakymo pateikimą nusiųsti") +
                          GetQuestionAndAnswer(8,
                              "Patvirtinimą apie sėkmingą užsakymo pateikimą norėčiau gauti kitu el. pašto adresu nei nurodytas duomenų bazėje. Kaip tai padaryti?",
                              "A. Patvirtinimą apie užsakymo pateikimą galite gauti bet kokiu el. pašto adresu. Tiesiog formindami užsakymą įveskite norimą el. pašto adresą į lauką „El. paštas“.") +
                          GetQuestionAndAnswer(9,
                              "Kaip galiu peržiūrėti ankstesnius savo užsakymus?",
                              "A. Kai tik jūsų užsakymas bus išsiųstas, galite peržiūrėti informaciją apie baigtą užsakymą svetainės MyHerbalife.com dalyje „Mano duomenys ir ataskaitos“, spustelėję skyrių „Mano užsakymai“. Įveskite užsakymo numerį į lauką „Paieška pagal užsakymo numerį“ ir spustelėkite „Pirmyn“. Šio užsakymo duomenys bus pateikti ekrane.") +
                          GetQuestionAndAnswer(10,
                              "Norėčiau pašalinti produktus iš užsakymo arba pakeisti jų kiekį. Kur galiu tai padaryti?",
                              "A. Produktus pašalinti arba pakeisti jų kiekį galite užsakymo prekių krepšelyje.") +
                          GetQuestionAndAnswer(11,
                              "Kaip įvesti užsakymo keitimus prie atliekant apmokėjimą?",
                              "A. Prieš patvirtindami užsakymą spustelėkite mygtuką „Atgal“. Pereisite prie užsakymo įforminimo, kur galėsite pakeisti užsakymo pozicijas, duomenis apie pristatymą arba išdavimo vietą ir toliau vykdyti pirkimą.") +
                          GetQuestionAndAnswer(12,
                              "Kuo skiriasi užsakymo dokumentų pristatymo būdai: „Nusiųsti dokumentus kartu su užsakymu“ ir „Nusiųsti dokumentus nepriklausomo partnerio pašto adresu“?",
                              "A. „Nusiųsti dokumentus kartu su užsakymu“ – sąskaita faktūra ir važtaraštis bus nusiųsti kartu su užsakymu. „Nusiųsti dokumentus nepriklausomo partnerio pašto adresu“ – sąskaita faktūra ir važtaraštis bus nusiųsti atskirai nepriklausomo partnerio adresu.") +
                          GetQuestionAndAnswer(13,
                              "Kokiais apmokėjimo būdais galiu naudotis, kad apmokėčiau savo užsakymą internetu?",
                              "A. Apmokėti galima tik banko kortele..") +
                          GetQuestionAndAnswer(14,
                              "Kokio tipo banko kortelės priimamos internete?",
                              "A. Apmokėjimui svetainėje priimamos vardinės banko kortelės MasterCard, Visa (Classic / Electron), Maestro.") +
                          GetQuestionAndAnswer(15,
                              "Ar galima užsakymą apmokėti keliomis kredito kortelėmis?",
                              "A. Teikiant užsakymą internetu, apmokėti galima tik viena banko kortele.") +
                          GetQuestionAndAnswer(16,
                              "Kodėl negaliu atsiskaityti už pirkinį banko kortele?",
                              "A. Kai kurie bankai gali riboti mokėjimus jų kortelėmis internetu. Jei norite pirkinius apmokėti kortele, nustatykite leidimą kortele atsiskaityti internetu!") +
                          GetQuestionAndAnswer(17,
                              "Ar gausiu patvirtinimą apie sėkmingą užsakymo pateikimą?",
                              "A. Taip. Jūsų nurodytu el. pašto adresu bus nusiųstas patvirtinimas apie sėkmingą užsakymo pateikimą.");
        }

        #endregion

        #region GetLVLocales

        protected override void GetLVLocales(TopicLocalizedModel locale)
        {
            locale.Title = "PASŪTĪJUMA VEIKŠANA";
            locale.Body = GetQuestionAndAnswer(1,
                              "Vai iespējams pabeigt pasūtījumu neapstiprinot pasūtījuma sagatavošanas logu?",
                              "A: Nē, neapstiprinot pasūtījuma sagatavošanas logu nav iespējams pabeigt pasūtījumu.") +
                          GetQuestionAndAnswer(2,
                              "Es vēlos mainīt piegādes adresi savam pasūtījumam. Kur es to varu izdarīt?",
                              "A: Informāciju par piegādi vai izsniegšanu Pārdošanas centrā (pieejams tikai Latvijā) Jūs varat mainīt pasūtījuma sagatavošanas procesā sadaļā - Piegādes metode.") +
                          GetQuestionAndAnswer(3,
                              "Vai iepirkumu groza saturs saglabāsies, ja es iziešu no vietnes vai pasūtījuma lapas?",
                              "A: Jā. Iepirkumu groza saturs saglabāsies. Lūdzu, ņemiet vērā, ka tad, kad Jūs atgriezīsieties pie pasūtījuma, tiks atkārtoti pārbaudīta Jūsu iepirkumu grozā ievietoto produktu pieejamība. Ja brīdī, kad atgriezīsieties pie pasūtījuma, kāds no produktiem vairs nebūs pieejams noliktavā, to vajadzēs dzēst no groza un atjaunināt iepirkumu groza saturu.") +
                          GetQuestionAndAnswer(4,
                              "Kādi pasūtījuma nosūtīšanas varianti ir pieejami, un kāda ir to cena?",
                              "A: Pastāv šādi pasūtījumu piegādes veidi:",
                              "<p><strong>OMNIVA: piegāde uz pakomātiem.</strong> Piegādes maksa Baltijas valstīs ir 4.45 EUR + PVN. Pasūtījuma piegādes termiņi vairumā gadījumu ir 2 darba dienas. Plašākai informācijai, lūdzu, sazinieties ar Herbalife Pārdošanas centru Rīgā.</p>" +
                              "<p><strong>ITELLA: piegāde uz pakomātiem.</strong> Piegādes maksa Baltijas valstīs ir 4.45 EUR + PVN. Pasūtījuma piegādes termiņi vairumā gadījumu ir 2 darba dienas. Plašākai informācijai, lūdzu, sazinieties ar Herbalife Pārdošanas centru Rīgā.</p>" +
                              "<p><strong>DPD: piegāde uz pakomātiem.</strong> Piegādes maksa Baltijas valstīs ir 4.45 EUR + PVN. Pasūtījuma piegādes termiņi vairumā gadījumu ir 2 darba dienas. Plašākai informācijai, lūdzu, sazinieties ar Herbalife Pārdošanas centru Rīgā.</p>" +
                              "<p><strong>DPD: kurjera piegāde.</strong> Piegādes maksa Baltijas valstīs ir 8.50 EUR + PVN. Pasūtījuma piegādes termiņi vairumā gadījumu ir 2 darba dienas. Plašākai informācijai, lūdzu, sazinieties ar Herbalife Pārdošanas centru Rīgā.</p>" +
                              "<p><strong>DPD: kurjera piegāde ar iespēju izvēlēties piegādes laika intervālu (B2C).</strong> Piegādes maksa Baltijas valstīs ir 11.00 EUR + PVN. Pasūtījuma piegādes termiņi vairumā gadījumu ir 2 darba dienas. Plašākai informācijai, lūdzu, sazinieties ar Herbalife Pārdošanas centru Rīgā.</p>") +
                          GetQuestionAndAnswer(5,
                              "Kādas ir obligātās procedūras pasūtījumu sagatavošanai ar saņemšanu Pārdošanas centrā (pieejams tikai Latvijā)?",
                              "A: Visi pasūtījumi ar saņemšanu Pārdošanas centrā, jāizņem 10 darba dienu laikā no naudas saņemšanas par pasūtījumu.") +
                          GetQuestionAndAnswer(6,
                              "Ja pasūtījuma sagatavošanas laikā es norādīšu jaunu elektroniskā pasta adresi laukā “Paziņojums uz elektronisko pastu”, vai sistēma samainīs šo elektroniskā pasta adresi manā profilā?",
                              "A: Nē. Elektroniskā pasta adrese laukā “Paziņojums uz elektronisko pastu”, kalpo tikai, lai nosūtītu paziņojumu par šī pasūtījuma veiksmīgu sagatavošanu.") +
                          GetQuestionAndAnswer(7,
                              "Es vēlos saņemt apstiprinājumu par veiksmīgu pasūtījuma sagatavošanu uz elektroniskā pasta adresi, kas atšķiras no datu bāzēs norādītās. Kā es to varu izdarīt?",
                              "A: Jūs varat saņemt apstiprinājumu par veiksmīgu pasūtījuma sagatavošanu uz jebkuru elektroniskā pasta adresi. Veicot pasūtījumu, ievadiet vēlamo elektroniskā pasta adresi laukā “E-pasts”.") +
                          GetQuestionAndAnswer(8,
                              "Kā es varu apskatīt savus iepriekšējos pasūtījumus?",
                              "A: Tiklīdz Jūsu pasūtījums tiks iesniegts, Jūs varēsiet apskatīt informāciju par veikto pasūtījumu tīmekļa vietnē MyHerbalife.com sadaļā “Mana informācija un pasūtījumi”, spiežot uz sadaļas “Mani pasūtījumi”. Laukā “Meklēšana pēc pasūtījuma numura”, ievadiet pasūtījuma numuru un spiediet “Tālāk”. Uz ekrāna tiks attēlota šī pasūtījuma informācija.") +
                          GetQuestionAndAnswer(9,
                              "Es vēlos dzēst produktus no pasūtījuma vai mainīt to daudzumu. Kur es varu to izdarīt?",
                              "A: Jūs varat dzēst produktus vai mainīt to daudzumu pasūtījuma Iepirkumu grozā.") +
                          GetQuestionAndAnswer(10,
                              "Kā veikt izmaiņas pasūtījumā pirms apmaksas veikšanas?",
                              "A: Apstiprinot pasūtījumu, spiediet uz lauka “Atpakaļ”. Jūs atgriezīsieties pie pasūtījuma sagatavošanas, kur Jūs varēsiet mainīt izvēlētos produktus, piegādes informāciju vau saņemšanas vietu vai turpināt pievienot jaunus produktus.") +
                          GetQuestionAndAnswer(11,
                              "Kāda ir atšķirība starp pasūtījuma dokumentu piegādes veidiem: “Nosūtīt dokumentus kopā ar pasūtījumu” un “Nosūtīt dokumentus uz Neatkarīgā Partnera pasta adresi”?",
                              "A: “Nosūtīt dokumentus kopā ar pasūtījumu” – rēķins un pavadzīme tiks nosūtīti kopā ar pasūtījumu. “Nosūtīt dokumentus uz Neatkarīgā Partnera pasta adresi” – rēķins un pavadzīme tiks nosūtīti atsevišķi uz Neatkarīgā Partnera pasta adresi.") +
                          GetQuestionAndAnswer(12,
                              "Kādas apmaksas metodes es varu izmantot, lai apmaksātu savu tiešsaistes pasūtījumu?",
                              "A: Apmaksu var veikt tikai ar bankas karti.") +
                          GetQuestionAndAnswer(13,
                              "Kāda veida bankas kartes tiek pieņemtas tiešsaistē?",
                              "A: Apmaksai tiek pieņemtas uz personas vārda reģistrētas bankas kartes MasterCard, Visa (Classic / Electron), Maestro.") +
                          GetQuestionAndAnswer(14,
                              "Vai iespējams apmaksāt pasūtījumu ar vairākām kredītkartēm?",
                              "A: Veicot pasūtījumu tiešsaistē, apmaksu var veikt tikai ar vienu bankas karti.") +
                          GetQuestionAndAnswer(15,
                              "Kādēļ es nevaru norēķināties par pirkumu ar savu bankas karti?",
                              "Atsevišķas bankas var noteikt ierobežojumus savām izdotajām kartēm norēķiniem interneta tīklā. Lai par pirkumiem varētu samaksāt ar karti, aktivizējiet kartei norēķinus internetā!") +
                          GetQuestionAndAnswer(16,
                              "Vai es saņemšu apstiprinājumu par veiksmīgu pasūtījuma pieņemšanu?",
                              "A: Jā. Uz Jūsu norādīto elektroniskā pasta adresi tiks atsūtīts apstiprinājums par veiksmīgu pasūtījuma pieņemšanu.");
        }

        #endregion

        #region GetEELocales

        protected override void GetEELocales(TopicLocalizedModel locale)
        {
            locale.Title = "TELLIMUSE VORMISTAMINE";
            locale.Body = GetQuestionAndAnswer(1,
                              "Mida tähendab tellimuse vormistamise aken?",
                              "V: Tellimuse vormistamise akent kasutatakse selleks, et kinnitada teie poolt sisestatud või valitud tellimuse korrektsust.") +
                          GetQuestionAndAnswer(2,
                              "Kas tellimust on võimalik esitada ilma tellimuse vormistamise akent kinnitamata?",
                              "V: Ei, tellimust ei ole võimalik esitada ilma tellimuse vormistamise akent kinnitamata.") +
                          GetQuestionAndAnswer(3,
                              "Ma tahaksin antud tellimuse puhul kohaletoimetamise aadressi muuta. Kuidas ma saan seda teha?",
                              "V: Te saate muuta kohaletoimetamise andmeid või väljastamist Müügikeskuses (saadaval ainult Lätis) minnes tellimuse esitamise käigus kohaletoimetamise võimaluste lehele.") +
                          GetQuestionAndAnswer(4,
                              "Kas ostukorvi sisu säilib, kui ma veebilehelt või tellimislehelt lahkun?",
                              "V: Ja. Ostukorvi sisu säilib. Palun pange tähele, et kui te tellimuse juurde tagasi tulete, siis vaadatakse selles olevate toodete olemasolu uuesti üle. Kui juhtub, et mõnda toodet ei ole enam tellimuse juurde tagasipöördumise hetkel laos, siis tuleb see ostukorvist kustutada, lisades linnukese ja uuendades ostukorvi.") +
                          GetQuestionAndAnswer(5,
                              "Millised on tellimuse saatmise variandid ja milline on maksumus?",
                              "V: Pakutakse alljärgnevaid tellimuse saatmise võimalusi:",
                              "<p><strong>OMNIVA: saatmine pakiautomaati.</strong> Saatmiskulud Balti riikides on 4,45 eurot + käibemaks, tellimuse kohaletoimetamise aeg on enamikel juhtudel 2 tööpäeva. Lisainfo saamiseks pöörduge Herbalife’i Müügikeskuse poole Riias.</p>" +
                              "<p><strong>ITELLA: saatmine pakiautomaati</strong> Saatmiskulud Balti riikides on 4,45 eurot + käibemaks, tellimuse kohaletoimetamise aeg on enamikel juhtudel 2 tööpäeva. Lisainfo saamiseks pöörduge Herbalife’i Müügikeskuse poole Riias.</p>" +
                              "<p><strong>DPD: saatmine pakiautomaati.</strong> Saatmiskulud Balti riikides on 4,45 eurot + käibemaks, tellimuse kohaletoimetamise aeg on enamikel juhtudel 2 tööpäeva. Lisainfo saamiseks pöörduge Herbalife’i Müügikeskuse poole Riias.</p>" +
                              "<p><strong>DPD: kullerteenus.</strong> Saatmiskulud Balti riikides on 8,50 еurot + käibemaks, tellimuse kohaletoimetamise aeg on enamikel juhtudel 2 tööpäeva. Lisainfo saamiseks pöörduge Herbalife’i Müügikeskuse poole Riias.</p>" +
                              "<p><strong>DPD: kullerteenus valitud ajavahemikusv(B2C).</strong> Saatmiskulud Balti riikides on 11,00 eurot + käibemaks, tellimuse kohaletoimetamise aeg on enamikel juhtudel 2 tööpäeva. Lisainfo saamiseks pöörduge Herbalife’i Müügikeskuse poole Riias.</p>") +
                          GetQuestionAndAnswer(6,
                              "Millised on kohustuslikud protseduurid tellimuste esitamiseks kättesaamisega Müügikeskuses (kehtib ainult Lätis)?",
                              "V: Kõigile tellimustele, mis on esitatud koos kättesaamisega Müügikeskuses, tuleb järele tulla 10 tööpäeva jooksul alates tellimuse eest tasumisest.") +
                          GetQuestionAndAnswer(7,
                              "Juhul kui näitan tellimuse esitamise hetkel e-posti aadressi väljal uut aadressi, siis kas muutub ka minu profiili all minu e-posti aadress?",
                              "V: Ei. E-posti aadress väljal «Teave e-posti aadressi kohta» on ainult selleks, et saata teade käesoleva tellimuse eduka esitamise kohta.") +
                          GetQuestionAndAnswer(8,
                              "Ma sooviksin saada kinnitust tellimuse eduka esitamise kohta e-posti aadressile, mis erineb andmebaasis olevast aadressist. Kuidas seda teha?",
                              "Te võite saada kinnituse tellimuse esitamise kohta mis tahes e-posti aadressile. Sisestage lihtsalt tellimuse vormistamisel soovitud e-posti aadress väljale «E-post».") +
                          GetQuestionAndAnswer(9,
                              "Kuidas ma saan oma eelnevaid tellimusi vaadata?",
                              "V: Niipea, kui olete oma tellimuse ära saatnud, saate tellimust puudutavat infot vaadata veebilehel MyHerbalife.comalajaotuses «Мinu andmed ja aruanded», vajutades jaotusel «Мinu tellimused».Sisestage tellimuse number väljal «Otsing tellimuse numbri järgi» ja vajutage «Edasi». Vastava tellimuse andmed kuvatakse ekraanil.") +
                          GetQuestionAndAnswer(10,
                              "Ma sooviksin tooteid tellimusest kustutada või muuta nende arvu. Kus seda teha saab?",
                              "V: Te saate tooteid kustutada või muuta nende kogust ostukorvis.") +
                          GetQuestionAndAnswer(11,
                              "Kuidas saab enne makse teostamist tellimusse sisse viiа muudatusi?",
                              "V: Enne tellimuse kinnitamist vajutage nupule «Tagasi». Nii liigute tagasi tellimuse vormistamise lehele, kus te saate muuta tellimuses sisalduvaid tooteid, kohaletoimetamise ja väljastamiskoha andmeid või ostmist jätkata.") +
                          GetQuestionAndAnswer(12,
                              "Milles seisneb erinevus kahe võimaluse vahel tellimuse dokumentide kohaletoimetamises: «Saada dokumendid koos tellimusega» ja «Saada dokumendid Sõltumatu Partneri postiaadressile»?",
                              "V: «Saada dokumendid koos tellimusega» - Arve ja saateleht saadetakse koos tellimusega. «Saada dokumendid Sõltumatu Partneri postiaadressile» - Arve ja saateleht saadetakse eraldi Sõltumatu Partneri postiaadressile.") +
                          GetQuestionAndAnswer(13,
                              "Milliseid maksevõimalusi saan ma kasutada interneti teel esitatud tellimuse eest tasumiseks?",
                              "V: Tasumine toimub ainult pangakaardiga.") +
                          GetQuestionAndAnswer(14,
                              "Millist tüüpi pangakaarte internetis aktsepteeritakse?",
                              "V: Veebilehel maksmisel aktsepteeritakse nimelisi pangakaarte MasterCard, Visa (Classic / Electron), Maestro.") +
                          GetQuestionAndAnswer(15,
                              "Kas tellimuse eest on võimalik tasuda mitme krediitkaardiga?",
                              "V: Interneti teel esitatud tellimuse eest on võimalik tasuda ainult ühe kaardiga.") +
                          GetQuestionAndAnswer(16,
                              "Miks ma ei saa oma pangakaardiga ostu eest tasuda?",
                              "V: Mõned pangad võivad enda väljastatud kaartidele interneti keskkonnas arveldamiseks kehtestada piirangud. Selleks, et kaardiga ostude eest tasuda, aktiveerige kaart ostudeks interneti keskkonnas!") +
                          GetQuestionAndAnswer(17,
                              "Kas ma saan kinnituse tellimuse eduka esitamise eest?",
                              "V: Ja. Teie poolt näidatud e-posti aadressile laekub kinnitus tellimuse eduka esitamise kohta.");
        }

        #endregion
    }
}
