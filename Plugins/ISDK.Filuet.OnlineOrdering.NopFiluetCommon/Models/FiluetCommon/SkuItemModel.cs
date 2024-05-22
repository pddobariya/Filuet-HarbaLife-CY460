using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Element of <see cref="SkuItemModel"/>
    /// </summary>
    public class SkuItemModel : IEquatable<SkuItemModel>
    {
        #region Properties

        /// <summary>
        /// SKU Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// SKU Warehouse Code
        /// </summary>
        public string Warehouse { get; set; }

        /// <summary>
        /// Determines whether this instance and another object have the same <see cref="SkuItemModel.Name"/> and <see cref="SkuItemModel.Warehouse"/>.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>The equality result</returns>
        public override bool Equals(object obj)
        {
            SkuItemModel item = obj as SkuItemModel;
            if (item == null)
            {
                return false;
            }

            return this.Name.Equals(item.Name) && this.Warehouse.Equals(item.Warehouse);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="SkuItemModel"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397) ^ (this.Warehouse != null ? this.Warehouse.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Determines whether this instance and another <see cref="SkuItemModel"/> instance have the same <see cref="SkuItemModel.Name"/> and <see cref="SkuItemModel.Warehouse"/>.
        /// </summary>
        /// <param name="item">The instance of <see cref="SkuItemModel"/> to compare to this instance.</param>
        /// <returns>The equality result</returns>
        public bool Equals(SkuItemModel item)
        {
            if (item == null)
            {
                return false;
            }

            return this.Name.Equals(item.Name) && this.Warehouse.Equals(item.Warehouse);
        }

        #endregion
    }
}
