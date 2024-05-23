using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class MISExcelVM
    {
        public string SalesInvoiceNo { get; set; }
        public string SalesFromDate { get; set; }
        public string SalesToDate { get; set; }
        public string ProductName { get; set; }        
        public string CustomerId { get; set; }
        public string ItemNo { get; set; }
        public string PGroupId { get; set; }
        public string cmbType { get; set; }
        public string cmbPost { get; set; }
        public string DiscountOnly { get; set; }
        public bool bPromotional { get; set; }
        public string CustomerGroupID { get; set; }
        public string CategoryID { get; set; }
        public bool chkCategory { get; set; }
        public string PGroup { get; set; }
        public string ShiftId { get; set; }
        public int branchId { get; set; }
        //public int BranchId { get; set; }
        public string VatType { get; set; }
        public string OrderBy { get; set; }
        public string Channel { get; set; }
        public string Toll { get; set; }
        public string Type { get; set; }
        public string ProductGroupName { get; set; }

        public string vTransactionType { get; set; }

        public string FileName { get; set; }
        public string SheetName { get; set; }

        public ExcelPackage varExcelPackage { get; set; }
        public string varFileDirectory { get; set; }
        public FileStream varFileStream { get; set; }
        public string IsRebate { get; set; }



        /////For Purchase////////////
        public string PurchaseNo { get; set; }
        public string VendorGroup { get; set; }
        public string VendorName { get; set; }
        public string VendorId { get; set; }        
        public string ProductType { get; set; }
        //public string ProductName { get; set; }
        //public string ItemNo { get; set; }
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public bool Post { get; set; }
        public bool Duty { get; set; }        
        public string ReportType { get; set; }        
        public string ProductGroupId { get; set; }


    }
}
