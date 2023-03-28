using GhalibResearch.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.ViewModels
{
    public class MemberViewModel
    {
        public MemberModel Member { get; set; }
        public IEnumerable<SelectListItem> Degree { get; set; }
        public IEnumerable<SelectListItem> MemberFaculty { get; set; }
        public IEnumerable<SelectListItem> EducationField { get; set; }
        public IEnumerable<SelectListItem> Role { get; set; }
        public IEnumerable<SelectListItem> MemberType { get; set; }
    }
}
