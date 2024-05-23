using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class ProductionMISViewModel
    {
        public string IssueNo { get; set; }
        public string IssueDateFrom { get; set; }
        public string IssueDateTo { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string ReportType { get; set; }
        public bool Post { get; set; }
        public bool Wastage { get; set; }
        public bool PreviewOnly { get; set; }
        public string shift { get; set; }
        public bool Summary { get; set; }
        public int FontSize { get; set; }
        public int BranchId { get; set; }
        public bool ExcelRpt { get; set; }
        public System.Data.DataTable dtComapnyProfile { get; set; }
        public ExcelPackage varExcelPackage { get; set; }

        public FileStream varFileStream { get; set; }

        public string varFileDirectory { get; set; }

        public string FileName { get; set; }
        public string SheetName { get; set; }

        public string IsPost { get; set; }
        public string IsWastage { get; set; }

    }
}
