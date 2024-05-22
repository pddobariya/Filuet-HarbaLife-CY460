using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public interface IStatusService
    {
        #region Methods

        Task<IEnumerable<FiluetStatus>> GetStatusesAsync(Func<IQueryable<FiluetStatus>, IQueryable<FiluetStatus>> func = null);

        Task<FiluetStatus> GetStatusByNameAsync(string statusName);

        #endregion
    }
}
