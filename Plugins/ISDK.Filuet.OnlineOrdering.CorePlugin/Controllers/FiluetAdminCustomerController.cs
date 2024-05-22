using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    [Area("Admin")]
    public class FiluetAdminCustomerController : CustomerController
    {
        #region Fields

        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor
       
        public FiluetAdminCustomerController(
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            EmailAccountSettings emailAccountSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings, 
            IAddressAttributeParser addressAttributeParser, 
            IAddressService addressService,
            ICustomerActivityService customerActivityService, 
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService, 
            ICustomerModelFactory customerModelFactory, 
            ICustomerRegistrationService customerRegistrationService, 
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IForumService forumService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            IRewardPointService rewardPointService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            TaxSettings taxSettings) 
            : base(customerSettings,
                  dateTimeSettings,
                  emailAccountSettings,
                  forumSettings,
                  gdprSettings,
                  addressAttributeParser,
                  addressService,
                  customerActivityService,
                  customerAttributeParser,
                  customerAttributeService,
                  customerModelFactory,
                  customerRegistrationService,
                  customerService,
                  dateTimeHelper, 
                  emailAccountService, 
                  eventPublisher, 
                  exportManager, 
                  forumService, 
                  gdprService, 
                  genericAttributeService, 
                  localizationService,
                  newsLetterSubscriptionService,
                  notificationService, 
                  permissionService,
                  queuedEmailService,
                  rewardPointService,
                  storeContext, 
                  storeService,
                  taxService, 
                  workContext,
                  workflowMessageService,
                  taxSettings)
        {
            _customerModelFactory = customerModelFactory;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> CustomerList(CustomerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            var filuetCustomerSearchModel = Nop.Core.Infrastructure.Mapper.AutoMapperConfiguration.Mapper.Map<FiluetCustomerSearchModel>(searchModel);
            filuetCustomerSearchModel.SearchExternalIdentifier = Request.Form["SearchExternalIdentifier"];
            //prepare model
            var model = await _customerModelFactory.PrepareCustomerListModelAsync(filuetCustomerSearchModel);
            return Json(model);
        }

        #endregion
    }
}
