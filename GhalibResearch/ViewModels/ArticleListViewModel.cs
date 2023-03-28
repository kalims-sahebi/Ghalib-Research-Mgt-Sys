using GhalibResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.ViewModels
{
    public class ArticleListViewModel
    {
        public int ArticleId { get; set; }
        public string ArticleTypeDari { get; set; }
        public int ChapterStartingNumber { get; set; }
        public string ArticleTitle { get; set; }
        public DateTime StartingYear { get; set; }
        public DateTime EndingYear { get; set; }
        public DateTime AcademicCouncilProtocolDate { get; set; }
        public DateTime ResearchComitteeProtocolDate { get; set; }
        public string PublishedYear { get; set; }
        public string AcademicCouncilProtocolNumber { get; set; }
        public string ResearchComitteeProtocolNumber { get; set; }
        public string Researchers { get; set; }
        public string CoResearchers { get; set; }
        public string Budget { get; set; }
        public string EthicCode { get; set; }
        public string PublishedInJournal { get; set; }


    }
}
