using Nop.Services.Events;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Events
{
    public class ExtTopicUrlRecordEntityNameRequested : IConsumer<CustomUrlRecordEntityNameRequestedEvent>
    {
        #region Methods

        public Task HandleEventAsync(CustomUrlRecordEntityNameRequestedEvent eventMessage)
        {
            return Task.Run(() =>
            {
                var urlRecord = eventMessage.UrlRecord;
                if (urlRecord.EntityName.Equals("topic", StringComparison.OrdinalIgnoreCase))
                {
                    // Allow reading of the Privacy Notice page by guests
                    if (urlRecord.Slug.Equals("privacy-notice", StringComparison.OrdinalIgnoreCase))
                    {
                        eventMessage.RouteData.DataTokens["namespaces"] = new string[]
                            {"ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers"};
                        eventMessage.RouteData.Values["controller"] = "ExtendedTopic";
                        eventMessage.RouteData.Values["action"] = "TopicDetails";
                        eventMessage.RouteData.Values["topicId"] = urlRecord.EntityId;
                        eventMessage.RouteData.Values["SeName"] = urlRecord.Slug;
                    }
                    // Default - GenericPathRoute commented lines + namespace
                    else
                    {
                        eventMessage.RouteData.DataTokens["namespaces"] = new string[] {"Nop.Web.Controllers"};
                        eventMessage.RouteData.Values["controller"] = "Topic";
                        eventMessage.RouteData.Values["action"] = "TopicDetails";
                        eventMessage.RouteData.Values["topicId"] = urlRecord.EntityId;
                        eventMessage.RouteData.Values["SeName"] = urlRecord.Slug;
                    }
                }
            });
        }

        #endregion
    }
}
