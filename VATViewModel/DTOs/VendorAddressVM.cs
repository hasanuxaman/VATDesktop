using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
   public class VendorAddressVM
    {
        public int Id { get; set; }
        public string VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorGroupName { get; set; }        
        public string Operation { get; set; }


    }
}
