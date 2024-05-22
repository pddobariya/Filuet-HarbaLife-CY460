using Nop.Core.Domain.Catalog;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.DTO
{
    public class FormFieldMeta
    {
        #region Properties

        public string NameResourceKey { get; set; }

        public string HelptextResourceKey { get; set; }

        public string PlaceholderResourceKey { get; set; }

        public AttributeControlType ControlType { get; set; }

        public int DisplayOrder { get; set; }

        #endregion
    }
}
