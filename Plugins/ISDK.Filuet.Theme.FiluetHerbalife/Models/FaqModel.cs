using Nop.Web.Models.Topics;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models
{
    public record FaqListModel
    {
        #region Ctor

        public FaqListModel()
        {
            FaqModels = new List<FaqModel>();
        }

        #endregion

        #region Properties

        public List<FaqModel> FaqModels { get; set; }
        public bool AllQuestionsActive { get; set; }

        #endregion
      
    }

    public record FaqModel
    {
        #region Ctor

        public FaqModel()
        {
            Faq = new TopicModel();
        }

        #endregion

        #region Properties

        public TopicModel Faq { get; set; }
        public bool IsActive { get; set; }

        #endregion
    }
}
