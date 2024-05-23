using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class CustomerBillProcessVM
    {

        public int Id { get; set; }
        public string CustomerID { get; set; }

        public string Jan { get; set; }
        public bool JanChecked { get; set; }

        public string Feb { get; set; }
        public bool FebChecked { get; set; }

        public string Mar { get; set; }
        public bool MarChecked { get; set; }

        public string Apr { get; set; }
        public bool AprChecked { get; set; }

        public string May { get; set; }
        public bool MayChecked { get; set; }

        public string Jun { get; set; }
        public bool JunChecked { get; set; }

        public string Jul { get; set; }
        public bool JulChecked { get; set; }

        public string Aug { get; set; }
        public bool AugChecked { get; set; }

        public string Sep { get; set; }
        public bool SepChecked { get; set; }

        public string Oct { get; set; }
        public bool OctChecked { get; set; }

        public string Nov { get; set; }
        public bool NovChecked { get; set; }

        public string Dec { get; set; }
        public bool DecChecked { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

      
    }
}
