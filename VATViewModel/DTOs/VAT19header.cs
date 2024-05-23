using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class VAT19header
    {
        public string MonthYear { get; set; }
        public bool BreakDown { get; set; }
        public bool NewFormat { get; set; }
        public string Branch { get; set; }

    }
}
