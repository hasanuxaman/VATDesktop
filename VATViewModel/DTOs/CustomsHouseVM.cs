using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class CustomsHouseVM
    {
        public string ID { get; set; }
        //[Display(Name = "Code")]
        public string Code { get; set; }

        //[Display(Name = "Customs House Name")]
        public string CustomsHouseName { get; set; }

        //[Display(Name = "Customs House Address")]
        public string CustomsHouseAddress { get; set; }

        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
        public string IsActive { get; set; }
    }
}
