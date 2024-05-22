using Nop.Core;
using Nop.Core.Domain.Customers;
using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public partial class RefreshToken : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets refresh token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Token creation date
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Token expiration date
        /// </summary>
        public DateTime? ExpiredOnUtc { get; set; }

        #endregion
    }
}
