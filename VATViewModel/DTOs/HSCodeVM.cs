using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
  public class HSCodeVM
    {

        public decimal Id { get; set; }
        public string Code { get; set; }
        public string HSCode { get; set; }
        public string Description { get; set; }
        public string FiscalYear { get; set; }
        public decimal CD { get; set; }
        public decimal SD { get; set; }
        public decimal VAT { get; set; }
        public decimal AIT { get; set; }
        public decimal RD { get; set; }
        public decimal AT { get; set; }
        public decimal OtherSD { get; set; }
        public decimal OtherVAT { get; set; }
        public string IsFixedVAT { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string IsArchive { get; set; }
        public string Operation { get; set; }

        public string IsFixedSD { get; set; }
        public string IsFixedCD { get; set; }
        public string IsFixedRD { get; set; }
        public string IsFixedAIT { get; set; }
        public string IsFixedAT { get; set; }

        public bool IsFixedVatM
        {
            get { return IsFixedVAT == "Y"; }
        }

        public bool IsFixedCDChecked { get; set; }
        public bool IsFixedSDChecked { get; set; }        
        public bool IsFixedRDChecked { get; set; }
        public bool IsFixedAITChecked { get; set; }
        public bool IsFixedVAT1Checked { get; set; }
        public bool IsFixedATChecked { get; set; }

        public string IsFixedOtherVAT { get; set; }
        public string IsFixedOtherSD { get; set; }

        public string IsVDS { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }




    }
}
