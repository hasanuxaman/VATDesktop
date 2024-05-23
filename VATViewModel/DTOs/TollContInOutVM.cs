using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class TollContInOutVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Code { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string DateTime { get; set; }
        public string PeriodID { get; set; }
        public string RefNo { get; set; }
        public string Comments { get; set; }
        public string ImportID { get; set; }
        public string Post { get; set; }
        public string TransactionType { get; set; }
        public bool IsCancle { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }


        public string DateTimeFrom { get; set; }
        public string DateTimeTo { get; set; }
        public string SelectTop { get; set; }
        public string ProductType { get; set; }
        public string Operation { get; set; }
        public int ProductCategoryId { get; set; }
        public decimal TotalAmount { get; set; }
        public string AppVersion { get; set; }


        public List<TollContInOutDetailVM> Details { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }

    }
}
