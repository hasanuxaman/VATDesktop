using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
    public class Toll6_3InvoiceVM
    {
        public int Id { get; set; }
        public string TollNo { get; set; }
        public string CustomerID { get; set; }
        public int VendorID { get; set; }
        public string Address { get; set; }
        public string TollDateTime { get; set; }
        public string Post { get; set; }
        
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public int BranchId { get; set; }

        public string SignatoryName { get; set; }
        public string SignatoryDesig { get; set; }

        public string CustomerName { get; set; }
        public string VendorName { get; set; }

        public string TransactionType { get; set; }

        public string Operation { get; set; }
        public string ActiveStatus { get; set; }

        /// <summary>
        /// JBR
        /// </summary>
        /// 

        public string IsPrint { get; set; }
        public string EXPFormNo { get; set; }
        public string TollDateFrom { get; set; }
        public string TollDateTo { get; set; }
        public string RefNo { get; set; }
        public List<Toll6_3InvoiceDetailVM> Details { get; set; }

    }
    public class Toll6_3InvoiceDetailVM
    {
        public int Id { get; set; }
        public string TollNo { get; set; }
        public string TollLineNo { get; set; }
        public int BranchId { get; set; }

        public string SalesInvoiceNo { get; set; }
        public string InvoiceDateTime { get; set; }

        public string Post { get; set; }
        public string Comments { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedOn { get; set; }

        public string TransactionType { get; set; }


    }
}
