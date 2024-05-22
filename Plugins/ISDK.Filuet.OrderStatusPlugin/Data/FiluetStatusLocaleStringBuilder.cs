using DocumentFormat.OpenXml.Drawing.Diagrams;
using FluentMigrator.Builders.Create.Table;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace ISDK.Filuet.OrderStatusPlugin.Data
{
    public class FiluetStatusLocaleStringBuilder : NopEntityBuilder<FiluetStatusLocaleString>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(FiluetStatusLocaleString.StatusId)).AsInt32().ForeignKey<FiluetStatus>();
        }

        #endregion
    }
}
