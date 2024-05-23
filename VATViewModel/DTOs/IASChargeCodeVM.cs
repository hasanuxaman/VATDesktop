using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class IASChargeCodeVM
    {
       public int  Id { get; set; }     
        public string ChargeCode{get;set;}
        public string Description { get; set; }


        public string CodeType { get; set; }
    }
}
