using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class ExtensionHtmlHelper
    {
        public static bool IsDebug(this IHtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
      return false;
#endif
        }

        public static string GetVersionFile(this IHtmlHelper htmlHelper, string filePath)
        {
            INopFileProvider fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            if (fileProvider.FileExists(filePath))
            {
                var lastWriteTimeMainJs = fileProvider.GetLastWriteTime(filePath);
                return lastWriteTimeMainJs.ToString("yyyyddMMHHmmss", CultureInfo.InvariantCulture);
            }

            return "1";
        }

        public static bool FileExists(this IHtmlHelper htmlHelper, string filePath)
        {
            INopFileProvider fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            var fileMapPath = fileProvider.MapPath(filePath);
            var isExists = fileProvider.FileExists(fileMapPath);
            return isExists;
        }
    }
}
