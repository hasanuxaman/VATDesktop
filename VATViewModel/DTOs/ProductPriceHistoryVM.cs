using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class ProductPriceHistoryVM
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ItemNo { get; set; }
        public string EffectDate { get; set; }
        public decimal VatablePrice { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

    }
}
