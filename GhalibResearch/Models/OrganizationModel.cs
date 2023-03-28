using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Models
{
    public class OrganizationModel
    {
        public short OrganizationId { get; set; }

        [Required(ErrorMessage = "Please Input Name")]
        public string OrganizationName { get; set; }
    }
}
