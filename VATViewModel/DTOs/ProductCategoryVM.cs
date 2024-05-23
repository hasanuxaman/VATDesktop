using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class ProductCategoryVM
    {

        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ReportType { get; set; }
        public string Comments { get; set; }
        public string IsRaw { get; set; }
        public string HSCodeNo { get; set; }
        public decimal VATRate { get; set; }
        public string PropergatingRate { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal SD { get; set; }
        public string Trading { get; set; }
        public string NonStock { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string Operation { get; set; }

    }
}
