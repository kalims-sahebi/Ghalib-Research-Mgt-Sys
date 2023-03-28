using GhalibResearch.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.ViewModels
{
    public class AddOrEditSentArticleViewModel
    {
        public SentArticleModel SentArticle { get; set; }
        public IEnumerable<SelectListItem> Article { get; set; }
        public IEnumerable<OrganizationModel> Organization { get; set; }

        public IEnumerable<SentArticleListViewModel> SentArticleList { get; set; }



    }
}
