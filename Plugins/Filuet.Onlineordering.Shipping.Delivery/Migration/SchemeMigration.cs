using Filuet.Onlineordering.Shipping.Delivery.Domain;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;

namespace Filuet.Onlineordering.Shipping.Delivery.Data
{
    [NopMigration("2023/09/21 01:20:17:6455422", "Filuet.Onlineordering.Shipping.Delivery base schema", MigrationProcessType.Update)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Up Method
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryCity))).Exists())
                Create.TableFor<DeliveryCity>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SalesCenter))).Exists())
                Create.TableFor<SalesCenter>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryCityLanguage))).Exists())
                Create.TableFor<DeliveryCityLanguage>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SalesCenterLanguage))).Exists())
                Create.TableFor<SalesCenterLanguage>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryOperator))).Exists())
                Create.TableFor<DeliveryOperator>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(PickupPointsLanguage))).Exists())
                Create.TableFor<PickupPointsLanguage>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(PickupPointsOperator))).Exists())
                Create.TableFor<PickupPointsOperator>(); 
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryType))).Exists())
                Create.TableFor<DeliveryType>(); 
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Price))).Exists())
                Create.TableFor<Price>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(AutoPostOffice))).Exists())
                Create.TableFor<AutoPostOffice>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryOperator_DeliveryType_DeliveryCity_Dependency))).Exists())
                Create.TableFor<DeliveryOperator_DeliveryType_DeliveryCity_Dependency>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(AutoPostOfficeLanguage))).Exists())
                Create.TableFor<AutoPostOfficeLanguage>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryOperator_DeliveryCity_Dependency))).Exists())
                Create.TableFor<DeliveryOperator_DeliveryCity_Dependency>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryOperator_DeliveryType_Dependency))).Exists())
                Create.TableFor<DeliveryOperator_DeliveryType_Dependency>();
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(DeliveryTypeLanguage))).Exists())
                Create.TableFor<DeliveryTypeLanguage>();
           
        }

        #endregion
    }
}
