using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class TollProductionConsumptionDetailVM
    {
        public int Id { get; set; }
        public int HeaderId { get; set; }
        public int BranchId { get; set; }
        public string DateTime { get; set; }
        public string PeriodID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Post { get; set; }
        public string TransactionType { get; set; }
        public bool IsCancle { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal SubTotal { get; set; }
        public string TollLineNo { get; set; }

        public string UOM { get; set; }
        public string UOMc { get; set; }
        public string UOMn { get; set; }
        public string BomId { get; set; }
        public string FinishItemNo { get; set; }

        public string TollDateTime { get; set; }
        public string Code { get; set; }

    }
}
