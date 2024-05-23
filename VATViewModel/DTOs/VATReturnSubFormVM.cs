using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VATViewModel.Integration.JsonModels;

namespace VATViewModel.DTOs
{
    public class VATReturnSubFormVM
    {
        public int NoteNo { get; set; }
        public bool IsVersion2 { get; set; }

        public string PeriodName { get; set; }
        public string ExportInBDT { get; set; }
        public int BranchId { get; set; }

        public string FontSize { get; set; }

        public string SheetName { get; set; }

        public string Type { get; set; }

        public System.Data.DataTable dtVATReturnSubForm { get; set; }

        public System.Data.DataTable dtComapnyProfile { get; set; }

        public ExcelPackage varExcelPackage { get; set; }

        public FileStream varFileStream { get; set; }

        public string varFileDirectory { get; set; }

        public string FileName { get; set; }
        public string Post { get; set; }

        public string post1 { get; set; }
        public string post2 { get; set; }

        public bool IsSummary { get; set; }

        public int[] NoteNos { get; set; }

        public string PeriodId { get; set; }

        public string TableName { get; set; }
        public string SubformTableName { get; set; }

        public string PeriodKey { get; set; }
        public string DecimalPlace { get; set; }

        public bool IsSaveSubForm { get; set; }

    }

    public class PrepareXmlVm
    {
        public XmlDocument XmlDocument { get; set; }

        public DataRow DataRow { get; set; }

        public List<Item> Items { get; set; }

        public CompanyProfileVM ProfileVm { get; set; }
    }
}
