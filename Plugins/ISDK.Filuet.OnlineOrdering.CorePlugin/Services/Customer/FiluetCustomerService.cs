using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    /// <summary>
    /// Extended Customer Service class
    /// </summary>
    public class FiluetCustomerService : CustomerService, IFiluetCustomerService
    {

        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FiluetCustomerService(
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            INopDataProvider dataProvider,
            IRepository<Address> customerAddressRepository, 
            IRepository<BlogComment> blogCommentRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerAddressMapping> customerAddressMappingRepository, 
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<GenericAttribute> gaRepository, 
            IRepository<NewsComment> newsCommentRepository,
            IRepository<Order> orderRepository, 
            IRepository<ProductReview> productReviewRepository, 
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
            IRepository<ShoppingCartItem> shoppingCartRepository,
            IStaticCacheManager staticCacheManager, 
            IStoreContext storeContext,
            ShoppingCartSettings shoppingCartSettings,
            IWorkContext workContext) 
            : base(customerSettings,
                  genericAttributeService,
                  dataProvider,
                  customerAddressRepository,
                  blogCommentRepository,
                  customerRepository,
                  customerAddressMappingRepository,
                  customerCustomerRoleMappingRepository,
                  customerPasswordRepository, 
                  customerRoleRepository,
                  forumPostRepository, 
                  forumTopicRepository,
                  gaRepository,
                  newsCommentRepository,
                  orderRepository, 
                  productReviewRepository, 
                  productReviewHelpfulnessRepository,
                  pollVotingRecordRepository,
                  shoppingCartRepository,
                  staticCacheManager,
                  storeContext, 
                  shoppingCartSettings)
        {
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async void AddStreetAddress(string streetAddress)
        {
            var currentCustomer =await _workContext.GetCurrentCustomerAsync();
            var attribute =await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CoreGenericAttributes.StreetAddressAttribute);
            List<string> additionalStreetAddresses;
            if (attribute != null)
            {
                additionalStreetAddresses = JsonConvert.DeserializeObject<List<string>>(attribute);
            }
            else
            {
                additionalStreetAddresses = new List<string>();
            }

            if (!additionalStreetAddresses.Contains(streetAddress) && await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,currentCustomer.StreetAddress) != streetAddress)
            {
                additionalStreetAddresses.Add(streetAddress);
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, CoreGenericAttributes.StreetAddressAttribute, JsonConvert.SerializeObject(additionalStreetAddresses));
            }
        }

        public async void AddPhone(string phone)
        {
            var currentCustomer =await _workContext.GetCurrentCustomerAsync();
            var attribute =await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CoreGenericAttributes.PhoneAttribute);
            List<string> additionalPhone;
            if (attribute != null)
            {
                additionalPhone = JsonConvert.DeserializeObject<List<string>>(attribute).Where(phone => !string.IsNullOrWhiteSpace(phone)).ToList();
            }
            else
            {
                additionalPhone = new List<string>();
            }

            if (!additionalPhone.Contains(phone) &&await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,currentCustomer.Phone) != phone)
            {
                additionalPhone.Add(phone);
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, CoreGenericAttributes.PhoneAttribute, JsonConvert.SerializeObject(additionalPhone));
            }
        }

        #endregion
    }
}
