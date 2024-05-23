using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VATViewModel.DTOs
{
    public class CustomerDiscountVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal Rate { get; set; }
        public string Section { get; set; }
        public string CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public bool IsArchive { get; set; }

        public string Operation { get; set; }
      
    }
}
