using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class DisposeFinishMasterVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string DisposeNo { get; set; }
        public string TransactionDateTime { get; set; }
        public string ShiftId { get; set; }
        public string ReferenceNo { get; set; }
        public string SerialNo { get; set; }
        public string ImportIDExcel { get; set; }
        public string Comments { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string IsSynced { get; set; }
        public string FiscalYear { get; set; }
        public string AppVersion { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public List<DisposeFinishDetailVM> Details { get; set; }


        public string IsSaleable { get; set; }


        public decimal UnitPrice { get; set; }
        public decimal OfferUnitPrice { get; set; }

        public string FinishItemNo { get; set; }

        public decimal Quantity { get; set; }

        public string UOM { get; set; }

        public int BOMId { get; set; }

        public int PeriodId { get; set; }
        public string Operation { get; set; }
        public string DisposeDateTimeFrom { get; set; }
        public string DisposeDateTimeTo { get; set; }
        public string ProductType { get; set; }
        public int ProductCategoryId { get; set; }
        public bool IsSaleableChecked { get; set; }
        public string FinishItemName { get; set; }

        public int DecimalPlace { get; set; }


    }



    public class DisposeFinishDetailVM
    {
        public string Id { get; set; }
        public string DisposeNo { get; set; }
        public string DisposeLineNo { get; set; }
        public string BranchId { get; set; }
        public string TransactionDateTime { get; set; }
        public string TransactionType { get; set; }

        public string ItemNo { get; set; }
        public string ProductName { get; set; }
        public string UOM { get; set; }
        public decimal UsedQuantity { get; set; }
        public string PurchaseNo { get; set; }
        public decimal PurchaseQuantity { get; set; }

        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }

        public decimal RebateRate { get; set; }
        public decimal RebateVATAmount { get; set; }

        public string Post { get; set; }
        public string Comments { get; set; }
        public string IsSynced { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public int PeriodId { get; set; }
        public string ItemName { get; set; }
        public string ProductCode { get; set; }
    }
}
