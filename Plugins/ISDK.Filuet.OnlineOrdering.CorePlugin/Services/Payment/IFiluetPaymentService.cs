using Nop.Services.Payments;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Payment
{
    public interface IFiluetPaymentService
    {
        #region Methods

        Task<ProcessPaymentResult> ProcessPayment(ProcessPaymentRequest processPaymentRequest);

        #endregion
    }
}