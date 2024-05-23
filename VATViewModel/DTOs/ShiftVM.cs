using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class ShiftVM
    {
        public string Id { get; set; }
        public string ShiftName { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string Remarks { get; set; }
        public string Sl { get; set; }
        public string NextDay { get; set; }

        public string Operation { get; set; }

    }
}
