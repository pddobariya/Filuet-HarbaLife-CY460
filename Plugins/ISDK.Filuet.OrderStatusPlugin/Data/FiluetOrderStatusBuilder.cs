using FluentMigrator.Builders.Create.Table;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace ISDK.Filuet.OrderStatusPlugin.Data
{

    public class FiluetOrderStatusBuilder : NopEntityBuilder<FiluetOrderStatus>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table    
                .WithColumn(nameof(FiluetOrderStatus.OrderId)).AsInt32().ForeignKey<Order>()
                .WithColumn(nameof(FiluetOrderStatus.StatusId)).AsInt32().ForeignKey<FiluetStatus>();
        }
        
        #endregion
    }

}
