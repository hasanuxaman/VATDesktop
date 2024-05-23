using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
   public class CustomerAddressVM
    {
        public int Id { get; set; }
        public string CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerGroupName { get; set; }        
        public string CustomerVATRegNo { get; set; }

        public string Operation { get; set; }


    }
}
