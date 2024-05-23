using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{

    public class PurchaseInvoiceMPLHeadersVM
    {
        public string Id { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public string VendorID { get; set; }
        public string VendorGroup { get; set; }
        public string InvoiceDate { get; set; }
        public string ReceiveDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalVDSAmount { get; set; }
        public string SerialNo { get; set; }
        public string LCNumber { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string BENumber { get; set; }
        public string ProductType { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string WithVDS { get; set; }
        public string ReturnId { get; set; }
        public string ImportID { get; set; }
        public string LCDate { get; set; }
        public decimal LandedCost { get; set; }
        public string CustomHouse { get; set; }

        public List<PurchaseInvoiceMPLDetailVM> Details { get; set; }
        public List<PurchaseDutiesVM> Duties { get; set; }
        public List<TrackingVM> Trackings { get; set; }
        public string Operation { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string VendorName { get; set; }
        public bool IsImport { get; set; }
        public string Type { get; set; }

        public int ProductCategoryId { get; set; }
        public string ProductGroup { get; set; }


        public string IsTDS { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }

        public decimal USDInvoiceValue { get; set; }
        public string CurrencyId { get; set; }
        public decimal CurrencyRateFromBDT { get; set; }
        public int BranchId { get; set; }
        public string BranchCode { get; set; }

        public decimal TDSAmount { get; set; }
        public decimal NetBill { get; set; }
        public decimal TotalSubTotal { get; set; }
        public string InvoiceNumber { get; set; }

        public string FiscalYear { get; set; }

        public string AppVersion { get; set; }
        public List<string> IDs { get; set; }

        public string CurrentUser { get; set; }

        public string IsRebate { get; set; }
        public string RebatePeriodId { get; set; }
        public string RebateDate { get; set; }
        public string BankGuarantee { get; set; }
        public bool IsRebates { get; set; }
        public bool IsRebateHolds { get; set; }
        public string IsRebateHold { get; set; }



        public string IsBankingChannelPay { get; set; }

        public string IsTotalPrice { get; set; }
        public string VATTypeVATAutoChange { get; set; }
        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }

        public string CurrentUserId { get; set; }
        public string ExpireDate { get; set; }
        public string IsExpireDate { get; set; }
        public string CustomHouseCode { get; set; }
        public string VendorAddress { get; set; }
        public string TrackingTrace { get; set; }
        public string ItemNo { get; set; }
        public string MultipleItemInsert { get; set; }
        public string RebateWithGRN { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }

        public string PeriodDateTime { get; set; }
        public decimal TDSRate { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string VesselName { get; set; }



        public string CompanyCode { get; set; }

        public string FileName { get; set; }
        public string RegNo { get; set; }
        public string BondNo { get; set; }
        public decimal ProcessingFee { get; set; }

    }

    public class PurchaseInvoiceMPLDetailVM
    {

        public int BOMId { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public string LineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATRate2 { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string Comments { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public string Type { get; set; }
        public string ProductType { get; set; }
        public string BENumber { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public string TDSSection { get; set; }
        public string TDSCode { get; set; }
        public decimal RebateRate { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal CnFAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal AssessableValue { get; set; }
        public decimal CDAmount { get; set; }
        public decimal RDAmount { get; set; }
        public decimal TVBAmount { get; set; }
        public decimal TVAAmount { get; set; }
        public decimal ATVAmount { get; set; }
        public decimal OthersAmount { get; set; }
        public string ReturnTransactionType { get; set; }

        public decimal Total { get; set; }
        public decimal InvoiceValue { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Currency { get; set; }
        public string Rowtype { get; set; }
        public string IsExpireDate { get; set; }
        
        public string ProductName { get; set; }
        public string InvoiceDateTime { get; set; }
        public string ReceiveDate { get; set; }
        public string ProductCode { get; set; }
        public string Post { get; set; }
        public string[] retResults { get; set; }
        public string Id { get; set; }
        public decimal USDValue { get; set; }
        public decimal USDVAT { get; set; }
        public decimal VDSRate { get; set; }
        public decimal VDSAmount { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string InvoiceDate { get; set; }
        public string TransactionType { get; set; }
        public string ReturnId { get; set; }
        public decimal DollerValue { get; set; }
        public decimal CurrencyValue { get; set; }
        public decimal VATableValue { get; set; }
        public string POLineNo { get; set; }
        public int BranchId { get; set; }
        public string DutyRemarks { get; set; }
        public decimal AITAmount { get; set; }
        public decimal ATAmount { get; set; }
        public decimal FixedVATAmount { get; set; }
        public string IsFixedVAT { get; set; }
        public string IsRebate { get; set; }
        public string RebatePeriodId { get; set; }
        public string ExpireDate { get; set; }
        public string CPCName { get; set; }
        public string HSCode { get; set; }
        public string BEItemNo { get; set; }
        public string FixedVATRebate { get; set; }
        public string PurchaseReturnId { get; set; }
        public string PreviousInvoiceDateTime { get; set; }
        public decimal PreviousNBRPrice { get; set; }
        public decimal PreviousQuantity { get; set; }

        public decimal PreviousSD { get; set; }
        public decimal PreviousSDAmount { get; set; }
        public decimal PreviousSubTotal { get; set; }
        public string PreviousUOM { get; set; }
        public decimal PreviousVATAmount { get; set; }
        public decimal PreviousVATRate { get; set; }
        public string ReasonOfReturn { get; set; }

        public string OtherRef { get; set; }
        public int TankId { get; set; }
        public string TankName { get; set; }
        public string DIANo { get; set; }
        public decimal MT { get; set; }



        public string Section21 { get; set; }
        public string IsRebateHold { get; set; }

    }


}
