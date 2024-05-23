using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
  public  class SalesInvoiceExpVM
    {
        public int ID { get; set; }
        public string LCDate { get; set; }
        public string LCBank { get; set; }
        public string PINo { get; set; }
        public string PIDate { get; set; }
        public string EXPNo { get; set; }
        public string EXPDate { get; set; }
        public string PortFrom { get; set; }
        public string PortTo { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string IsArchive { get; set; }

        public string LCNumber { get; set; }

        public string Operation { get; set; }

    }
}
