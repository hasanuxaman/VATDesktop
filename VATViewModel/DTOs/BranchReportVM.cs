using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class BranchReportVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DBName { get; set; }
        public bool IsSelf { get; set; }
        public bool IsHeadOffice { get; set; }
    }
}
