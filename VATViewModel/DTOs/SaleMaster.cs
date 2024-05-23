using System;
using System.Collections.Generic;
using System.Web;

namespace VATViewModel.DTOs
{
    public class SaleMasterVM
    {
        public string SalesInvoiceNo { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string VehicleType { get; set; }
        public string InvoiceDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string SaleType { get; set; }
        public string PreviousSalesInvoiceNo { get; set; }
        public string Trading { get; set; }
        public string IsPrint { get; set; }
        public string TenderId { get; set; }
        public string TransactionType { get; set; }
        public string DeliveryDate { get; set; }
        public string VehicleNo { get; set; }
        public bool vehicleSaveInDB { get; set; }
        public string Post { get; set; }
        public string ReturnId { get; set; }
        public string CurrencyID { get; set; }
        public decimal CurrencyRateFromBDT { get; set; }
        public string ImportIDExcel { get; set; }
        public string LCNumber { get; set; }
        public string CompInvoiceNo { get; set; }
        public string Operation { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string Id { get; set; }
        public string LCBank { get; set; }
        public string LCDate { get; set; }
        public string VehicleID { get; set; }
        public List<SaleDetailVm> Details { get; set; }
        public string CustomerGroup { get; set; }
        public string CurrencyCode { get; set; }
        public Decimal DollerRate { get; set; }
        public decimal VDSAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public string ConversionDate { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public int ProductCategoryId { get; set; }
        public string Type { get; set; }
        public string VatName { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string PINo { get; set; }
        public string PIDate { get; set; }
        public string EXPFormNo { get; set; }
        public string EXPFormDate { get; set; }
        public string ShiftId { get; set; }
        public string ValueOnly { get; set; }
        public string DeliveryChallanNo { get; set; }
        public string IsVDS { get; set; }
        public string GetVDSCertificate { get; set; }
        public string VDSCertificateDate { get; set; }
        public string IsGatePass { get; set; }
        public string AlReadyPrint { get; set; }
        public string IsDeemedExport { get; set; }
        public int BranchId { get; set; }
        public string SignatoryName { get; set; }
        public string SignatoryDesig { get; set; }
        public string BranchCode { get; set; }
        public string IsInstitution { get; set; }
        public decimal TotalSubtotal { get; set; }
        public decimal HPSTotalAmount { get; set; }
        public string SaleInvoiceNumber { get; set; }
        public string FiscalYear { get; set; }
        public string AppVersion { get; set; }
        public string FileName { get; set; }
        public string Token { get; set; }
        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }
        public string FormNumeric { get; set; }
        public string ChassisTracking { get; set; }
        public string DuplicateInvoiceSave { get; set; }
        public string RefRequired { get; set; }
        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }
        public string IsExpireDate { get; set; }
        public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public string TrackingTrace { get; set; }
        public List<TrackingVM> Trackings { get; set; }
        public string BOe { get; set; }
        public string OrderNumber { get; set; }
        public string MultipleItemInsert { get; set; }
        public string IsCurrencyConvCompleted { get; set; }
        public string PONo { get; set; }
        public string IsBillCompleted { get; set; }
        public string PODate { get; set; }
        public string SaleDeliveryChallanTracking { get; set; }
        public List<SaleDeliveryTrakingVM> DeliveryTrackingDetails { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }
        public Decimal TCSRate { get; set; }

        public string ReportType { get; set; }

        public string BOeDate { get; set; }

        public string PeriodDateTime { get; set; }


        public List<string> SelectedSalesInvoiceNo { get; set; }

    }
    public class SaleDetailVm
    {
        public string BOMReferenceNo { get; set; }

        
        public int BOMId { get; set; }
        public string InvoiceLineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal PromotionalQuantity { get; set; }
        public decimal SaleQuantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string CommentsD { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VDSAmountD { get; set; }

        public string SaleTypeD { get; set; }
        public string PreviousSalesInvoiceNoD { get; set; }
        public string TradingD { get; set; }
        public string NonStockD { get; set; }
        public decimal TradingMarkUp { get; set; }
        public string Type { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedNBRPrice { get; set; }
        public decimal DollerValue { get; set; }
        public decimal CurrencyValue { get; set; }
        public decimal CurrencyRateFromBDT { get; set; }
        public string VatName { get; set; }
        public string CConversionDate { get; set; }
        public string ReturnTransactionType { get; set; }
        //CP use
        public string Weight { get; set; }
        public string ValueOnly { get; set; }
        public string Post { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        //public decimal PQuantiy { get; set; }
        public decimal Total { get; set; }
        public decimal BDTValue { get; set; }
        //new fields added on 5/5/2019
        public decimal TotalValue { get; set; }
        public decimal WareHouseRent { get; set; }
        public decimal WareHouseVAT { get; set; }
        public decimal ATVRate { get; set; }
        public decimal ATVablePrice { get; set; }
        public decimal ATVAmount { get; set; }
        public string IsCommercialImporter { get; set; }
        public string IsSample { get; set; }

        public decimal TradeVATableValue { get; set; }
        public decimal TradeVATRate { get; set; }
        public decimal TradeVATAmount { get; set; }


        public string SalesInvoiceNo { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedOn { get; set; }

        public string InvoiceDateTime { get; set; }

        public string TransactionType { get; set; }

        public string ReturnId { get; set; }

        public string Id { get; set; }
        public decimal AvgRate { get; set; }

        public string FinishItemNo { get; set; }

        public string BENumber { get; set; }
        public int BranchId { get; set; }

        public decimal CDNVATAmount { get; set; }
        public decimal CDNSDAmount { get; set; }
        public decimal CDNSubtotal { get; set; }
        public string ProductDescription { get; set; }
        public decimal FixedVATAmount { get; set; }
        public string IsFixedVAT { get; set; }

        public decimal HPSRate { get; set; }
        public decimal HPSAmount { get; set; }

        public decimal AdjustmentValue { get; set; }


        #region New Field ADD For DN/CN
        public string PreviousInvoiceDateTime { get; set; }
        public decimal PreviousNBRPrice  { get; set; }
        public decimal PreviousQuantity  { get; set; }
        public decimal PreviousSubTotal  { get; set; }
        public decimal PreviousVATAmount  { get; set; }
        public decimal PreviousVATRate  { get; set; }
        public decimal PreviousSD  { get; set; }
        public decimal PreviousSDAmount  { get; set; }
        public string ReasonOfReturn { get; set; }
        public string PreviousUOM { get; set; } 
        #endregion


        #region New Field For LeaderPolicy
        public string  IsLeader { get; set; }
        public decimal LeaderAmount { get; set; }
        public decimal LeaderVATAmount { get; set; }
        public decimal NonLeaderAmount { get; set; }
        public decimal NonLeaderVATAmount { get; set; }
        #endregion



        public decimal SourcePaidVATAmount { get; set; }

        public decimal SourcePaidQuantity { get; set; }

        public decimal NBRPriceInclusiveVAT { get; set; }

        public string BillingPeriodFrom { get; set; }
        public string BillingPeriodTo { get; set; }
        public int BillingDays { get; set; } 

        public string ProductType { get; set; }


        public string CPCName{ get; set; }
        public string HSCode { get; set; }
        public string BEItemNo { get; set; }
        public string Rowtype { get; set; }
        public string Option1 { get; set; }



    }
    public class SalePopUp
    {

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string TransactionType { get; set; }
        public string CPCName { get; set; }
        public string ItemNo { get; set; }


    }
    public class SaleExportVM
    {

        public string SaleExportNo { get; set; }
        public string SaleExportDate { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Quantity { get; set; }
        public string GrossWeight { get; set; }
        public string NetWeight { get; set; }
        public string NumberFrom { get; set; }
        public string NumberTo { get; set; }
        public string PortFrom { get; set; }
        public string PortTo { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }


        public string SaleLineNo { get; set; }
        public string RefNo { get; set; }

        public string QuantityE { get; set; }
        public string CommentsE { get; set; }
        public string Id { get; set; }
        public int BranchId { get; set; }



    }

   
    public class SaleExportInvoiceVM
    {
        public string SaleExportNo { get; set; }
        public string SL { get; set; }
        public string SalesInvoiceNo { get; set; }
        public int BranchId { get; set; }



    }
    public class BureauSaleDetailVM
    {
        public string InvoiceLineNo { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceDateTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public string ItemNo { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }

        public string Type { get; set; }
        public string PreviousSalesInvoiceNo { get; set; }
        public string ChallanDateTime { get; set; }
        public decimal DollerValue { get; set; }
        public decimal CurrencyValue { get; set; }
        public string InvoiceCurrency { get; set; }
        public string CConversionDate { get; set; }
        public string ReturnTransactionType { get; set; }
        public string BureauType { get; set; }
        public string BureauId { get; set; }
        public int BranchId { get; set; }
        public string CPCName { get; set; }
        public string HSCode { get; set; }
        public string PreviousInvoiceDateTime { get; set; }
        public decimal PreviousNBRPrice { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal PreviousSubTotal { get; set; }
        public decimal PreviousVATAmount { get; set; }



    }

    public class ADAPIResult
    {
        public bool IsValid { get; set; }
    }

    public class BombaySaleDetailsVM
    {

        public string SL { get; set; }
        public string ID { get; set; }
        public string CustomerGroup { get; set; }
        public string CustomerCode { get; set; }
        public string Branch_Code { get; set; }
        public string CustomerName { get; set; }
        public string Delivery_Address { get; set; }
        public string Reference_No { get; set; }
        public string Comments { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public string TransactionType { get; set; }
        public decimal TotalPrice { get; set; }
        public string IsProcessed { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string PreviousInvoiceDateTime { get; set; }
        public string ProcessTime { get; set; }
        public string Item_Name { get; set; }
        public decimal NBR_Price { get; set; }
        public string Invoice_Date { get; set; }
        public string Invoice_Time { get; set; }
        public string VAT_Name { get; set; }
        public decimal VAT_Amount { get; set; }
        public string Remarks { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }

    }
//    public class SaleMasterVM
//    {
//        public string SalesInvoiceNo { get; set; }
//        public string CustomerID { get; set; }
//        public string DeliveryAddress1 { get; set; }
//        public string DeliveryAddress2 { get; set; }
//        public string DeliveryAddress3 { get; set; }
//        public string VehicleType { get; set; }
//        public string InvoiceDateTime { get; set; }
//        public decimal TotalAmount { get; set; }
//        public decimal TotalVATAmount { get; set; }
//        public string SerialNo { get; set; }
//        public string Comments { get; set; }
//        public string CreatedBy { get; set; }
//        public string CreatedOn { get; set; }
//        public string LastModifiedBy { get; set; }
//        public string LastModifiedOn { get; set; }
//        public string SaleType { get; set; }
//        public string PreviousSalesInvoiceNo { get; set; }
//        public string Trading { get; set; }
//        public string IsPrint { get; set; }
//        public string TenderId { get; set; }
//        public string TransactionType { get; set; }
//        public string DeliveryDate { get; set; }
//        public string VehicleNo { get; set; }
//        public bool vehicleSaveInDB { get; set; }
//        public string Post { get; set; }
//        public string ReturnId { get; set; }
//        public string CurrencyID { get; set; }
//        public decimal CurrencyRateFromBDT { get; set; }
//        public string ImportID { get; set; }
//        public string LCNumber { get; set; }
//        public string CompInvoiceNo { get; set; }
//        public string ShiftId { get; set; }




//        public string LCBank { get; set; }
//        public string LCDate { get; set; }

//        public string PINo { get; set; }
//        public string PIDate { get; set; }
//        public string EXPFormNo { get; set; }


//        public string EXPFormDate { get; set; }
//    }
//    public class SaleDetailVM
//    {
//        public string InvoiceLineNo { get; set; }
//        public string ItemNo { get; set; }
//        public decimal Quantity { get; set; }
//        public decimal PromotionalQuantity { get; set; }
//        public decimal SalesPrice { get; set; }
//        public decimal NBRPrice { get; set; }
//        public string UOM { get; set; }
//        public decimal VATRate { get; set; }
//        public decimal VATAmount { get; set; }
//        public decimal SubTotal { get; set; }
//        public string CommentsD { get; set; }
//        //public string CreatedByD { get; set; }
//        //public DateTime CreatedOnD { get; set; }
//        //public string LastModifiedByD { get; set; }
//        //public DateTime LastModifiedOnD { get; set; }
//        public decimal SD { get; set; }
//        public decimal SDAmount { get; set; }
//        public string SaleTypeD { get; set; }
//        public string PreviousSalesInvoiceNoD { get; set; }
//        public string TradingD { get; set; }
//        public string NonStockD { get; set; }
//        public decimal TradingMarkUp { get; set; }
//        public string Type { get; set; }
//        public decimal UOMQty { get; set; }
//        public string UOMn { get; set; }
//        public decimal UOMc { get; set; }
//        public decimal UOMPrice { get; set; }
//        public decimal DiscountAmount { get; set; }
//        public decimal DiscountedNBRPrice { get; set; }
//        public decimal DollerValue { get; set; }
//        public decimal CurrencyValue { get; set; }
//        public string VatName { get; set; }
//        public string CConversionDate { get; set; }
//        public string ReturnTransactionType { get; set; }
//        //CP use
//        public string Weight { get; set; }
//        public string ValueOnly { get; set; }
//        public decimal WareHouseRent { get; set; }
//        public decimal WareHouseVAT { get; set; }
//        public decimal ATVRate { get; set; }
//        public decimal ATVablePrice { get; set; }
//        public decimal ATVAmount { get; set; }
//        public decimal TotalValue { get; set; }
//        public string IsCommercialImporter { get; set; }
        
////TotalValue
////WareHouseRent
////WareHouseVAT
////ATVRate
////ATVablePrice
////ATVAmount
////IsCommercialImporter




//        public decimal TradeVATableValue { get; set; }

//        public decimal TradeVATRate { get; set; }

//        public decimal TradeVATAmount { get; set; }
//    }

//    public class SaleExport
//    {
//        public string SaleLineNo { get; set; }
//        public string RefNo { get; set; }
        
//        public string Description { get; set; }
//        public string QuantityE { get; set; }
//        public string GrossWeight { get; set; }
//        public string NetWeight { get; set; }
//        public string NumberFrom { get; set; }
//        public string NumberTo { get; set; }
//        public string CommentsE { get; set; }


//    }
   
//    public class BureauSaleDetailVM
//    {
//        public string InvoiceLineNo { get; set; }
//        public string InvoiceName { get; set; }
//        public string InvoiceDateTime { get; set; }
//        public decimal Quantity { get; set; }
//        public decimal SalesPrice { get; set; }
//        public string ItemNo { get; set; }
//        public decimal SD { get; set; }
//        public decimal SDAmount { get; set; }
//        public string UOM { get; set; }
//        public decimal VATRate { get; set; }
//        public decimal VATAmount { get; set; }
//        public decimal SubTotal { get; set; }
       
//        public string Type { get; set; }
//        public string PreviousSalesInvoiceNo { get; set; }
//        public string ChallanDateTime { get; set; }
//        public decimal DollerValue { get; set; }
//        public decimal CurrencyValue { get; set; }
//        public string InvoiceCurrency { get; set; }
//        public string CConversionDate { get; set; }
//        public string ReturnTransactionType { get; set; }
//        public string BureauType { get; set; }
//        public string BureauId { get; set; }
        




//    }

    public class SaleDeliveryTrakingVM
    {
        public string Id { get; set; }
        public string CustomerID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string DeliveryChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public string PONo { get; set; }
        public string PODate { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string PartialDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public List<SaleDeliveryTrakingVM> IDs { get; set; }
        public string Post { get; set; }

        

    }

    public class IndexModel
    {
        public string SearchValue { get; set; }
        public string OrderName { get; set; }
        public string orderDir { get; set; }
        public int startRec { get; set; }
        public int pageSize { get; set; }

        public string IsArchive { get; set; }
        public string IsActive { get; set; }

        public string createdBy { get; set; }

    }
}
