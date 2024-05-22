using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Security
{
    public class FiluetStandardPermissionProvider : StandardPermissionProvider
    {
        #region Methods

        public override HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            var valueTuples = new HashSet<(string systemRoleName, PermissionRecord[] permissions)>();
            valueTuples.Add((NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        AllowCustomerImpersonation,
                        ManageProducts,
                        ManageCategories,
                        ManageManufacturers,
                        ManageProductReviews,
                        ManageProductTags,
                        ManageAttributes,
                        ManageCustomers,
                        ManageVendors,
                        ManageCurrentCarts,
                        ManageOrders,
                        ManageRecurringPayments,
                        ManageGiftCards,
                        ManageReturnRequests,
                        OrderCountryReport,
                        ManageAffiliates,
                        ManageCampaigns,
                        ManageDiscounts,
                        ManageNewsletterSubscribers,
                        ManagePolls,
                        ManageNews,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageForums,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManagePaymentMethods,
                        ManageExternalAuthenticationMethods,
                        ManageTaxSettings,
                        ManageShippingSettings,
                        ManageCurrencies,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageStores,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        DisplayPrices,
                        EnableShoppingCart,
                        PublicStoreAllowNavigation,
                        AccessClosedStore
                    }
                ));
            valueTuples.Add((NopCustomerDefaults.ForumModeratorsRoleName, new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        PublicStoreAllowNavigation
                    }
                ));
            valueTuples.Add((NopCustomerDefaults.GuestsRoleName, new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart
                    }
                ));
            valueTuples.Add((NopCustomerDefaults.RegisteredRoleName, new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        PublicStoreAllowNavigation
                    }
                ));
            valueTuples.Add((NopCustomerDefaults.VendorsRoleName, new[]
                    {
                        AccessAdminPanel,
                        ManageProducts,
                        ManageProductReviews,
                        ManageOrders
                    }
                ));
            return valueTuples;
        }

        #endregion
    }
}
