using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.ViewModels
{
    public class SentArticleListViewModel
    {
        public int SentArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string OrganizationName { get; set; }
        public int CopiesCount { get; set; }
        public DateTime SentDate { get; set; }
        public string SentDateString { get; set; }

    }
}
