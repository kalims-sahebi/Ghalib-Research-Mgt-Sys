using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Models
{
    public class ProgramModel
    {
        public byte ProgramId { get; set; }
        public byte? FacultyId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramNameEnglish { get; set; }
        public bool? IsDegree { get; set; }
        public bool? IsDeploma { get; set; }
        public string ProgramDescription { get; set; }
    }
}
