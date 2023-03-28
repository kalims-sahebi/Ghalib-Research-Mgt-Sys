using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Models
{
    public class DepartmentModel
    {
        public byte DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public byte? ProgramId { get; set; }
        public byte? FacultyId { get; set; }
        public bool? IsGeneral { get; set; }
        public string DepartmentNameEnglish { get; set; }
        public string DepartmentNamePashto { get; set; }
    }
}
