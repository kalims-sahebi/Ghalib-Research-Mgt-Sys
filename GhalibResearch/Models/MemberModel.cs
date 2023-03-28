using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Models
{
    public class MemberModel
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public int DegreeId { get; set; }
        public int FacultyId { get; set; }
        public int EducationFieldId { get; set; }
        public int MemberTypeId { get; set; }
        public int RoleId { get; set; }
    }
}
