using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class DepositMISViewModel
    {
        public string DepositNo { get; set; }
        public string DepositDateFrom { get; set; }
        public string DepositDateTo { get; set; }
        public bool Post { get; set; }
        public string ReportType { get; set; }
        public string BankName { get; set; }
        public string BankId { get; set; }
        public string VendorId { get; set; }

        public string TransactionType { get; set; }

        public bool PreviewOnly { get; set; }

        public int FontSize { get; set; }

        public ExcelPackage varExcelPackage { get; set; }
        public string SheetName { get; set; }
        public string FileName { get; set; }

        public System.Data.DataTable dt { get; set; }
        public System.Data.DataTable dtComapnyProfile { get; set; }

        public string varFileDirectory { get; set; }
        public FileStream varFileStream { get; set; }
        public string ReportHeaderName { get; set; }
        public string MailSend { get; set; }
    }
}
