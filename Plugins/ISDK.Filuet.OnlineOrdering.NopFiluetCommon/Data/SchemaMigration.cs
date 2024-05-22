using FluentMigrator;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Ḍata
{
    [NopMigration("2023/07/02 07:40:11:1666700", "Misc.NopFiluetCommon base schema")]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>

        #region Up Methods

        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(RefreshToken))).Exists())
                Create.TableFor<RefreshToken>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(CustomerGenericAttribute))).Exists())
                Create.TableFor<CustomerGenericAttribute>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetFusionShippingComputationOption))).Exists())
                Create.TableFor<FiluetFusionShippingComputationOption>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetFusionShippingComputationOptionCustomerData))).Exists())
                Create.TableFor<FiluetFusionShippingComputationOptionCustomerData>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(CustomerLimits))).Exists())
                Create.TableFor<CustomerLimits>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(OrdersStatuses))).Exists())
                Create.TableFor<OrdersStatuses>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetStatus))).Exists())
                Create.TableFor<FiluetStatus>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetOrderStatus))).Exists())
                Create.TableFor<FiluetOrderStatus>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetStatusLocaleString))).Exists())
                Create.TableFor<FiluetStatusLocaleString>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(OrderStatuses))).Exists())
                Create.TableFor<OrderStatuses>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(OrderStatusesLanguage))).Exists())
                Create.TableFor<OrderStatusesLanguage>();
        }

        #endregion

        #region Down Methods

        public override void Down()
        {
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(RefreshToken))).Exists())
                Delete.Table("RefreshToken");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(CustomerGenericAttribute))).Exists())
                Delete.Table("CustomerGenericAttribute");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetFusionShippingComputationOption))).Exists())
                Delete.Table("FiluetFusionShippingComputationOption");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetFusionShippingComputationOptionCustomerData))).Exists())
                Delete.Table("FiluetFusionShippingComputationOptionCustomerData");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(CustomerLimits))).Exists())
                Delete.Table("CustomerLimits");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(OrdersStatuses))).Exists())
                Delete.Table("OrdersStatuses");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(OrderStatuses))).Exists())
                Delete.Table("OrderStatuses");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(OrderStatusesLanguage))).Exists())
                Delete.Table("OrderStatusesLanguage");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetOrderStatus))).Exists())
                Delete.Table("FiluetOrderStatus");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetStatus))).Exists())
                Delete.Table("FiluetStatus");
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(FiluetStatusLocaleString))).Exists())
                Delete.Table("FiluetStatusLocaleString");
        }

        #endregion
    }
}