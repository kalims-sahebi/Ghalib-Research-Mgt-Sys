using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Models
{
    public class ArticleModel
    {
        public int ArticleId { get; set; }
        [Required(ErrorMessage ="پر نمودن عنوان مقاله ضروریست")]
        public string ArticleTitle { get; set; }
        public int ChapterStartingNumber { get; set; }
        public string Budget { get; set; }
        public string EthicCode { get; set; }

        public int FacultyId { get; set; }
        public string PublishedInJournal { get; set; }
        public  int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IFormFile  AbstractFile { get; set; }
        public IFormFile  DocumentFile { get; set; }
        public string  AbstractFileString { get; set; }
        public string  DocumentFileString { get; set; }
        public DateTime? StartingYear { get; set; }
        public string StartingYearString { get; set; }
        public DateTime? EndingYear { get; set; }
        public string EndingYearString { get; set; }
        public string AcademicCouncilProtocolNumber { get; set; }
        public DateTime? AcademicCouncilProtocolDate { get; set; }
        public string ResearchComitteeProtocolNumber { get; set; }
        public DateTime? ResearchComitteeProtocolDate { get; set; }
        public string ResearchComitteeProtocolDateString { get; set; }
        public string AcademicCouncilProtocolDateString { get; set; }
        public int ArticleTypeId { get; set; }
        public string UserName { get; set; }
        public string PublishedYear { get; set; }
        public string PublishedYearString { get; set; }
        public int MemberId { get; set; }
        public int MemberTypeId { get; set; }
        public int RoleId { get; set; }

        public List<int> Member { get; set; }
        public List<int> MemberType { get; set; }
        public List<int> Role { get; set; }

    }
}
