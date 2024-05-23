using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class MPLDayEndClosingVM
    {
        public int Id { get; set; }
        public string DayEndDate { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string PostedBy { get; set; }
        public string PostedOn { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }
        public string IDs { get; set; }
        public string Operation { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }



    }

   


}
