using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class AdjustmentVM
    {

        public string AdjId { get; set; }
        public string AdjName { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
        public int Branchid { get; set; }
    }
    /*
    public class AdjustmentNameDTO
    {
        public string AdjId { get; set; }
        public string AdjName { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        
    }
    */
   

}
