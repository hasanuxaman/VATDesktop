using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class StockMISViewModel
    {
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool Post { get; set; }
        public bool PreviewOnly { get; set; }
        public bool WithOutZero { get; set; }
        public bool QuantityOnly { get; set; }
        public bool Summary { get; set; }
        public string ReportType { get; set; }
        public string ShiftId { get; set; }
        public int FontSize { get; set; }
        public int ReportDecimal { get; set; }
        public ExcelPackage varExcelPackage { get; set; }
        public string FileName { get; set; }
        public System.Data.DataTable dtComapnyProfile { get; set; }      
        public FileStream varFileStream { get; set; }
        public string varFileDirectory { get; set; }
        public string SheetName { get; set; }
        public System.Data.DataTable dt { get; set; }
        public int BranchId { get; set; }
        public string ReportHeaderName { get; set; }
        public string OrderBy { get; set; }
        public string ZoneID { get; set; }


    }
}
