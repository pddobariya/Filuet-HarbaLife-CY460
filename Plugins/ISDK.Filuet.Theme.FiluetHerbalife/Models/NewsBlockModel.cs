using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models
{

    #region NewsBlockModel

    public record NewsBlockModel
    {
        #region Ctor

        public NewsBlockModel()
        {
            ArticleBigModel = new ArticleBigModel();
            ArticleModels = new List<ArticleModel>();
        }

        #endregion

        #region Properties
        public ArticleBigModel ArticleBigModel { get; set; }
        public List<ArticleModel> ArticleModels { get; set; }

        #endregion
    }

    #endregion

    #region ArticleBigModel

    public record ArticleBigModel : ArticleModel
    {
        public string BodyOverview { get; set; }
    }

    #endregion

    #region ArticleModel

    public record ArticleModel
    {
        #region Properties

        public int Id { get; set; }
        public string PictureUrl { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string SeName { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string TagColor { get; set; }
        public string TelegramUrl { get; set; }
        public bool IsPinned { get; set; }

        #endregion
    }

    #endregion

}
