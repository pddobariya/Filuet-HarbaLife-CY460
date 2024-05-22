using HBL.Baltic.OnlineOrdering.Payments.Payeezy;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Payeezy.SchedulerTask
{
    public class PayeezyPaymentScheduleTask : IScheduleTask
    {
        #region Fields

        private ISettingService _settingsService;
        private ILogger _logger;

        #endregion

        #region Ctor

        public PayeezyPaymentScheduleTask(ISettingService settingsService,
            ILogger logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        #endregion

        #region Properties

        public const string Name = "Calling close business day Payeezy API method at least once per day";
        public static string TaskType => typeof(PayeezyPaymentScheduleTask).AssemblyQualifiedName;

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            try
            {
                _logger.Information("Starting PayeezyPaymentScheduleTask schedule task...");
                PayeezyPaymentSettings payeezyPaymentSettings =await _settingsService.LoadSettingAsync<PayeezyPaymentSettings>();

                string bankUrl = payeezyPaymentSettings.IsSandbox ? payeezyPaymentSettings.APISandboxEndpoint : payeezyPaymentSettings.APIProductionEndpoint;
                string certKey = payeezyPaymentSettings.IsSandbox ? payeezyPaymentSettings.SandboxClientCertificateThumbprint : payeezyPaymentSettings.ProductionClientCertificateThumbprint;
                PayeezyManager payeezyManager = new PayeezyManager(bankUrl, null, certKey);
                PayeezyBusinessDayStatsAPIResponse response = payeezyManager.CloseBusinessDay();

                if (response != null && !response.IsFailed)
                {
                    _logger.Information("Finishing PayeezyPaymentScheduleTask schedule task successfully...");
                }
                else
                {
                    if (response == null)
                    {
                        throw new Exception("Response is null");
                    }
                    if (response.HasError)
                    {
                        throw new Exception("API error: " + response.ServiceMessage);
                    }
                    if (response.HasWarning)
                    {
                        throw new Exception("API warning: " + response.ServiceMessage);
                    }
                }

            }
            catch (Exception exc)
            {
                _logger.Error("Error occurred during executing PayeezyPaymentScheduleTask schedule task", exc);
                throw;
            }
        }

        #endregion
    }
}
