using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OrderStatusPlugin.Constants;
using ISDK.Filuet.OrderStatusPlugin.Tasks;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OrderStatusPlugin
{
    public class ISDKFiluetOrderStatusPlugin : BasePlugin
    {
        #region Fields

        private readonly IRepository<FiluetStatus> _filuetStatusRepository;
        private readonly IRepository<FiluetStatusLocaleString> _filuetStatusLocaleStringRepository;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public ISDKFiluetOrderStatusPlugin(
            IRepository<FiluetStatus> filuetStatusRepository,
            IRepository<FiluetStatusLocaleString> filuetStatusLocaleStringRepository,
            IScheduleTaskService scheduleTaskService)
        {
            _filuetStatusRepository = filuetStatusRepository;
            _filuetStatusLocaleStringRepository = filuetStatusLocaleStringRepository;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        #region InstallAsync

        public override async Task InstallAsync()
        {
            InstallStatuses();
            InstallTask();
            await base.InstallAsync();
        }

        #endregion

        #region UninstallAsync

        public override async Task UninstallAsync()
        {
            UnInstallTask();
            await base.UninstallAsync();
        }

        #endregion

        #region StatusExists

        bool StatusExists(string externalStatusName)
        {
            var status = _filuetStatusRepository.GetAll();
            return status.Any(status => status.ExternalStatusName == externalStatusName);
        }

        #endregion

        #region InstallStatuses

        private void InstallStatuses()
        {
            // Оплачен
            var newStatus = new FiluetStatus { ExternalStatusName = FiluetStatusContants.Paid };
            if (!StatusExists(newStatus.ExternalStatusName))
            {
                _filuetStatusRepository.InsertAsync(newStatus);
                _filuetStatusLocaleStringRepository.InsertAsync(new FiluetStatusLocaleString
                {
                    StatusId = newStatus.Id,
                    LanguageId = 2,
                    StatusName = "Оплачен"
                });
            }

            // Передан в обработку
            newStatus = new FiluetStatus { ExternalStatusName = FiluetStatusContants.TransferredToProcessing };
            if (!StatusExists(newStatus.ExternalStatusName))
            {
                _filuetStatusRepository.InsertAsync(newStatus);
                _filuetStatusLocaleStringRepository.InsertAsync(new FiluetStatusLocaleString
                {
                    StatusId = newStatus.Id,
                    LanguageId = 2,
                    StatusName = "Передан в обработку"
                });
            }

            // Передан в доставку
            newStatus = new FiluetStatus { ExternalStatusName = FiluetStatusContants.TransferredToDelivery };
            if (!StatusExists(newStatus.ExternalStatusName))
            {
                _filuetStatusRepository.InsertAsync(newStatus);
                _filuetStatusLocaleStringRepository.InsertAsync(new FiluetStatusLocaleString
                {
                    StatusId = newStatus.Id,
                    LanguageId = 2,
                    StatusName = "Передан в доставку"
                });
            }

            // Доставлен
            newStatus = new FiluetStatus { ExternalStatusName = FiluetStatusContants.Delivered };
            if (!StatusExists(newStatus.ExternalStatusName))
            {
                _filuetStatusRepository.InsertAsync(newStatus);
                _filuetStatusLocaleStringRepository.InsertAsync(new FiluetStatusLocaleString
                {
                    StatusId = newStatus.Id,
                    LanguageId = 2,
                    StatusName = "Доставлен"
                });
            }
        }

        #endregion

        #region InstallTask

        private async void InstallTask()
        {
            ScheduleTask task =await _scheduleTaskService.GetTaskByTypeAsync(LoadOrderStatusesTask.TaskType);
            if (task == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                {
                    Name = LoadOrderStatusesTask.Name,
                    Seconds = 30,
                    Type = LoadOrderStatusesTask.TaskType,
                    Enabled = false,
                    StopOnError = false
                });
            }
        }

        #endregion

        #region UnInstallTask

        private async void UnInstallTask()
        {
            ScheduleTask task =await _scheduleTaskService.GetTaskByTypeAsync(LoadOrderStatusesTask.TaskType);
            if (task != null)
            {
              await  _scheduleTaskService.DeleteTaskAsync(task);
            }
        }
      
        #endregion

        #endregion
    }
}
