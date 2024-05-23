using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class MeghnaMISViewModel
    {

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public DataTable dt { get; set; }
        public DataTable dtComapnyProfile { get; set; }
        public string FileName { get; set; }
        public int FontSize { get; set; }
        public int Id { get; set; }
        public bool Post { get; set; }
        public bool PreviewOnly { get; set; }
        public string ReportHeaderName { get; set; }
        public string ReportType { get; set; }
        public string SheetName { get; set; }
        public string TransactionType { get; set; }
        public ExcelPackage varExcelPackage { get; set; }
        public string varFileDirectory { get; set; }
        public FileStream varFileStream { get; set; }
        public string ZoneId { get; set; }
        public string BranchId { get; set; }
        public string SelfBankId { get; set; }
        public string BDBankId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ModeOfPayment { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public bool IsExcel { get; set; }


    }
}
