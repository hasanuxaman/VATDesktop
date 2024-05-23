using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
    public class ImportVM
    {
        public string TableName { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public int BranchId { get; set; }
    }
}
