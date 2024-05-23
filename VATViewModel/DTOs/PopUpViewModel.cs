using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class PopUpViewModel
    {
        public string TargetId { get; set; }
        public string ProductType { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string TransactionType { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Flag { get; set; }
        public int BranchId{get; set;}
        public List<BranchProfileVM> BranchList { get; set; }
        public List<string> SelectedSalesInvoiceNo { get; set; }
       
        public string CheckDate { get; set; }
        public string CheckDateTo { get; set; }
        public string ChequeNo { get; set; }
        public string TreasuryNo { get; set; }
        public string IssueDateTimeFrom { get; set; }
        public string IssueDateTimeTo { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string ItemNo { get; set; }
        public int CustomerId { get; set; }

        public string SelectTop { get; set; }

    }
}
