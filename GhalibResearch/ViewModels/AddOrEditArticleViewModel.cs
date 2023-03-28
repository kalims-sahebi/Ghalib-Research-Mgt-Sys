using GhalibResearch.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.ViewModels
{
    public class AddOrEditArticleViewModel
    {
        public ArticleModel AddModel { get; set; }
        public ArticleModel ArticleEditModel { get; set; }
        public IEnumerable<ArticleMember> ArticleMemberList { get; set; }
      //  public IEnumerable<SelectListItem> SubjectList { get; set; }
        public IEnumerable<SelectListItem> DepartmentList { get; set; }
        public IEnumerable<SelectListItem> FacultyList { get; set; }
        public IEnumerable<SelectListItem> ArticleTypeList { get; set; }
        public IEnumerable<ArticleListViewModel> ArticleList { get; set; }
        public string ArticleTypeDari { get; set; }

        /// <summary>
        /// ///// Member Attributes
        /// </summary>
        public IEnumerable<SelectListItem> Member { get; set; }
        public IEnumerable<SelectListItem> Degree { get; set; }
        public IEnumerable<SelectListItem> EducationField { get; set; }
        public IEnumerable<SelectListItem> Role { get; set; }
        public IEnumerable<SelectListItem> MemberType { get; set; }

        public List<Members> MembersList { get; set; }

    }
    public class Members
    {
        public IEnumerable<SelectListItem> Member { get; set; }
        public IEnumerable<SelectListItem> Role { get; set; }
        public IEnumerable<SelectListItem> MemberType { get; set; }

    }
}
