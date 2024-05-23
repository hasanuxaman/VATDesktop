using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VATViewModel.DTOs
{
    public class AuditVM
    {
        public int Id { get; set; }
        public string FiscalYear { get; set; }
        public string ServerPath { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public HttpPostedFileBase Attachment { get; set; }

    }
}
