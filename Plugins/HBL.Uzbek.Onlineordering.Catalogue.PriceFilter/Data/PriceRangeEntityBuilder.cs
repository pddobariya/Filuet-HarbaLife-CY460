using FluentMigrator.Builders.Create.Table;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain;
using Nop.Data.Mapping.Builders;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Data
{
    public class PriceRangeEntityBuilder : NopEntityBuilder<PriceRange>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PriceRange.MaxPrice))
                .AsDecimal().NotNullable()
                .WithColumn(nameof(PriceRange.MinPrice))
                .AsDecimal().NotNullable()
                .WithColumn(nameof(PriceRange.Name))
                .AsString()
                .WithColumn(nameof(PriceRange.OrderNumber))
                .AsInt32().NotNullable();

        }

        #endregion
    }
}
