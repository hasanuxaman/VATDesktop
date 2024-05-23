using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class CustomerItemVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Attention { get; set; }
        public string Notes { get; set; }
        public List<CustomerItemDetailsVM> Details { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string SelectTop { get; set; }
        public string Operation { get; set; }
        public string ProductType { get; set; }
        public int ProductCategoryId { get; set; }
        public string PeriodName { get; set; }
        public decimal TotalValue { get; set; }
        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }
        public int TotalItem { get; set; }
        public string ProcessDate { get; set; }


    }
    public class CustomerItemDetailsVM
    {
        public string InvoiceNo { get; set; }
        public string ItemNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Value { get; set; }
        public decimal VATRate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public int BranchId { get; set; }
        public int ProductCategoryId { get; set; }
        public string UOM { get; set; }
        public string CustomerID { get; set; }
        public string HSCode { get; set; }

    }


}
