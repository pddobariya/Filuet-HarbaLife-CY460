using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public partial class FiluetClearLogTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FiluetClearLogTask(
            ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            
            var logsToDelete =
                await _logger.GetAllLogsAsync(DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow.AddDays(-9));
            await _logger.DeleteLogsAsync(logsToDelete);
        }

        #endregion
    }
}