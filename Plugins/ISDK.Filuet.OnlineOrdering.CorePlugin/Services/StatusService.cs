using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public class StatusService : IStatusService
    {
        #region Fields

        private readonly IRepository<FiluetStatus> _filuetStatusRepository;

        #endregion

        #region Ctor

        public StatusService(
            IRepository<FiluetStatus> filuetStatusRepository)
        {
            _filuetStatusRepository = filuetStatusRepository;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<FiluetStatus>> GetStatusesAsync(Func<IQueryable<FiluetStatus>, IQueryable<FiluetStatus>> func = null)
        {
            return await _filuetStatusRepository.GetAllAsync(func, cache => default);
        }

        public async Task<FiluetStatus> GetStatusByNameAsync(string statusName)
        {
            return await _filuetStatusRepository.Table
                .FirstOrDefaultAsync(p => p.ExternalStatusName.ToLower() == statusName.ToLower());
        }

        #endregion
    }
}
