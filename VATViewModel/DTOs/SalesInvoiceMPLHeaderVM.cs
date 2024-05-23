using System;
using System.Collections.Generic;
using System.Web;

namespace VATViewModel.DTOs
{
    public class SalesInvoiceMPLHeaderVM
    {
        public int Id { get; set; }
        public string SalesInvoiceNo { get; set; }
        public int BranchId { get; set; }
        public string CustomerID { get; set; }
        public string DeliveryAddress { get; set; }
        public string InvoiceDateTime { get; set; }
        public string DeliveryDate { get; set; }
        public string TransactionType { get; set; }
        public string SaleType { get; set; }
        public string ReportType { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalSDAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string IsPrint { get; set; }
        public string Post { get; set; }
        public string CurrencyID { get; set; }
        public decimal CurrencyRateFromBDT { get; set; }
        public string AlReadyPrint { get; set; }
        public string CustomerOrder { get; set; }
        public string CustomerOrderDate { get; set; }
        public string Tarcat { get; set; }
        public decimal SupplyVAT { get; set; }
        public decimal TC { get; set; }
        public decimal LF { get; set; }
        public decimal RF { get; set; }
        public decimal ShortExcessAmnt { get; set; }
        public decimal Toll { get; set; }
        public decimal DC { get; set; }
        public decimal SC { get; set; }
        public decimal ATV { get; set; }
        public decimal LessFrightToPay { get; set; }
        public decimal GrandTotal { get; set; }

        public decimal OtherTotalAmnt { get; set; }
        public decimal TotalPaymentAmnt { get; set; }
        public decimal TotalNewCRAmnt { get; set; }
        public string  NewCRCode { get; set; }
        public decimal TotalCRAmnt { get; set; }

        public string RailReceiptNo { get; set; }
        public string RailReceiptDate { get; set; }
        public string RlyInvNo { get; set; }
        public decimal WetCharge { get; set; }
        public decimal ToPay { get; set; }
        public decimal OtherAmnt { get; set; }
        public string Prepaid { get; set; }
        public string Operation { get; set; }

        public string InstrumentDate { get; set; }
        public string ConversionDate { get; set; }

        public string CRDate { get; set; }
        public string VATName { get; set; }
        public string ItemNo { get; set; }
        public string CustomerGroupName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CurrencyCode { get; set; }
        public string DuplicateInvoiceSave { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductType { get; set; }
        public string ProductGroup { get; set; }
        public string FormNumeric { get; set; }
        public string ImportId { get; set; }
        public string IsPackCal { get; set; }

        public List<string> IDs { get; set; }

        public List<SalesInvoiceMPLDetailVM> SalesInvoiceMPLDetailVMs { get; set; }
        public List<SalesInvoiceMPLBankPaymentVM> SalesInvoiceMPLBankPaymentVMs { get; set; }
        public List<SalesInvoiceMPLCRInfoVM> SalesInvoiceMPLCRInfoVMs { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }

    }

    
    public class SalesInvoiceMPLDetailVM
    {
        public int Id { get; set; }
        public int SalesInvoiceMPLHeaderId { get; set; }
        public int BranchId { get; set; }
        public string InvoiceDateTime { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal InputQuantity { get; set; }
        public decimal NBRPrice { get; set; }
        public string UOM { get; set; }
        public string UOMn { get; set; }
        public decimal SDRate { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal UnitPriceWithVAT { get; set; }
        public string LineComments { get; set; }
        public string SaleType { get; set; }
        public string VATType { get; set; }
        public string Post { get; set; }
        public decimal UOMQty { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal UOMc { get; set; }
        public decimal DollerValue { get; set; }
        public decimal CurrencyValue { get; set; }
        public string CConversionDate { get; set; }
        public string TransactionType { get; set; }
        public string VATName { get; set; }
        public decimal TotalSaleVolume { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LineTotal { get; set; }

        public decimal CurrencyRateFromBDT { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        public int TankId { get; set; }
        public string TankCode { get; set; }
        public string IsFixedVAT { get; set; }
        public string IsPackCal { get; set; }

    }

    public class SalesInvoiceMPLBankPaymentVM
    {
        public int Id { get; set; }
        public int SalesInvoiceMPLHeaderId { get; set; }
        public int BankPaymentReceiveId { get; set; }
        public int BranchId { get; set; }
        public int BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal PreviousAmount { get; set; }
        public bool IsUsedDS { get; set; }



    }

    public class SalesInvoiceMPLCRInfoVM
    {
        public int Id { get; set; }
        public string CRCode { get; set; }
        public int BranchId { get; set; }
        public int CustomerId { get; set; }
        public int SalesInvoiceMPLHeaderId { get; set; }
        public string CRDate { get; set; }
        public decimal Amount { get; set; }
        public int SalesInvoiceRefId { get; set; }
        public bool IsUsed { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string CRDateFrom { get; set; }
        public string CRDateTo { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Used { get; set; }


    }
}
