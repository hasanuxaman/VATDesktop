using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{

    public class DistrictVM
    {
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public string Name { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public string IDs { get; set; }
        public string Operation { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string SelectTop { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string DivisionName { get; set; }


    }
    

}

