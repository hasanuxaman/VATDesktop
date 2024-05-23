using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{

    public class StaticCodeVM
    {
        public int Id { get; set; }
        public string Field_Name { get; set; }
        public string Field_Value { get; set; }
        public string Url { get; set; }
        public string File_Name { get; set; }
        public string Class_Name { get; set; }
        public string Method_Name { get; set; }

    }
}
