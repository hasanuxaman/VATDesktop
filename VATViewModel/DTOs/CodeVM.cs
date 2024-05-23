using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    //public class CodeVM
    //{
    //    public string CodeId { get; set; }
    //    public string CodeGroup { get; set; }
    //    public string CodeName { get; set; }
    //    public string prefix { get; set; }
    //    public string Lenth { get; set; }
    //    public string ActiveStatus { get; set; }
    //}
    public class CodeVM
    {
        public string CodeId { get; set; }
        public string CodeGroup { get; set; }
        public string CodeName { get; set; }
        public string prefix { get; set; }
        public string Lenth { get; set; }
        public string ActiveStatus { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public string prefixOld { get; set; }
    }
}
