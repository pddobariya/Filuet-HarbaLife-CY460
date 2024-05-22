using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Logging;
using System;
using System.Threading.Tasks;
using LogLevel = Nop.Core.Domain.Logging.LogLevel;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class FiluetLogger: DefaultLogger
    {
        #region Fields

        private readonly ILogger<FiluetLogger> _logger;

        #endregion

        #region Ctor

        public FiluetLogger(CommonSettings commonSettings, IRepository<Log> logRepository, IWebHelper webHelper, ILogger<FiluetLogger> logger) : base(commonSettings, logRepository, webHelper)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        public override async Task ErrorAsync(string message, Exception exception = null, Customer customer = null)
        {
            _logger.LogError(exception, message);
            
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Error))
                await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
        }

        public override Task<Log> InsertLogAsync(Nop.Core.Domain.Logging.LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            var convertedLogLevel = (Microsoft.Extensions.Logging.LogLevel)((int)logLevel/10);
            _logger.Log(convertedLogLevel, "{shortMessage}:{fullMessage}", shortMessage, fullMessage);
            return base.InsertLogAsync(logLevel, shortMessage, fullMessage, customer);
        }

        #endregion
    }
}