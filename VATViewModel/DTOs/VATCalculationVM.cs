using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class VATCalculationVM
    {

        public decimal InpTotalAmount { get; set; }
        public decimal InpVAT_Rate { get; set; }
        public decimal InpSD_Rate { get; set; }

        public decimal OutSubTotal { get; set; }
        public decimal OutVAT_Amount { get; set; }
        public decimal OutSDAmount { get; set; }

    }
}
