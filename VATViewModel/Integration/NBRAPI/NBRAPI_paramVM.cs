using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.NBRAPI
{
    public class NBRAPI_paramVM
    {

        public string CompanyBIN { get; set; }
        public string PeriodKey { get; set; }
        public string Serial { get; set; }

        public string BoENumber { get; set; }

        public string GoodsServiceCode { get; set; }
        public string ItemID { get; set; }
        public string SDRate { get; set; }
        public string VATRate { get; set; }
        public string Note { get; set; }
        public string CategoryName { get; set; }
        public string CategoryID { get; set; }
        public string BoEItemNo { get; set; }



    }
}
