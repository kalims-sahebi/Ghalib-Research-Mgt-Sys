using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Models
{
    public class SentArticleModel
    {
        public int SentArticleId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int ArticleId { get; set; }
        public int CopiesCount { get; set; }
        public DateTime? SentDate { get; set; }
        public string SentDateString { get; set; }
        public string UserName { get; set; }
    }
}
