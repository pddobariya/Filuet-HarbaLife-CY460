using FluentMigrator;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using System;
using Nop.Data.Extensions;
using System.Collections.Generic;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Data
{
    [NopMigration("2020/01/31 11:24:16:2551771", "Nop.Data base schema", MigrationProcessType.Installation)]
    public class PriceFilterSchemaMigration : Migration
    {
        #region Methods

        public override void Down()
        {
            var PriceRangeTablename = NameCompatibilityManager.GetTableName(typeof(PriceRange));
            if (Schema.Table(PriceRangeTablename).Exists())
                Delete.Table(PriceRangeTablename);
        }
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(PriceRange))).Exists())
                Create.TableFor<PriceRange>();
        }
        public class BaseNameCompatibility : INameCompatibility
        {
            public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(PriceRange), "PriceRange" },
        };
            public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
            {
            };
        }

        #endregion
    }
}
