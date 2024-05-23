using System;
using System.Collections.Generic;
using System.Web;

namespace VATViewModel.DTOs
{
    public class TankMPLsVM
    {

        public int Id { get; set; }
        public int BranchId { get; set; }
        public string TankCode { get; set; }
        public string ItemNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public int THeight { get; set; }
        public decimal TIntDiameter { get; set; }
        public decimal MaxLoadHeight { get; set; }
        public decimal SafeLoadHeght { get; set; }
        public decimal MaxLoadCap { get; set; }
        public decimal QtyZeroLevelDip { get; set; }

        public string Operation { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string ItemName { get; set; }
        public string BranchName { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        public int TankId { get; set; }

        public int SalesInvoiceDetailsRefId { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }

    }

}
