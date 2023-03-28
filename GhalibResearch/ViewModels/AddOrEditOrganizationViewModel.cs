using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace GhalibResearch.Models
{
    public class AddOrEditOrganizationViewModel
    {
        public OrganizationModel AddModel { get; set; }
        public OrganizationModel EditModel { get; set; }
        public IEnumerable<OrganizationModel> OrganizationList { get; set; }
    }
    
}
