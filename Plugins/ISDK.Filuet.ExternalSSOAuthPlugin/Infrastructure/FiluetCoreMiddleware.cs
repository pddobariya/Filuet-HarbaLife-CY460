using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure
{
    public class FiluetCoreMiddleware
    {  
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="next">Next</param>
        public FiluetCoreMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Task</returns>
        public Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/customer/register")
            {
                return Task.CompletedTask;
            }
            //call the next middleware in the request pipeline
            return _next(context);
        }
        
        #endregion
    }
}
