using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    //public class AdjustmentHistoryVM
    //{
    //    public string AdjHistoryID { get; set; }
    //    public string AdjHistoryNo { get; set; }
    //    public string AdjId { get; set; } 
    //    public string AdjDate { get; set; }
    //    public decimal AdjInputAmount { get; set; }
    //    public decimal AdjInputPercent { get; set; }
    //    public decimal AdjAmount { get; set; }
    //    public decimal AdjVATRate { get; set; }
    //    public decimal AdjVATAmount { get; set; }
    //    public decimal AdjSD { get; set; }
    //    public decimal AdjSDAmount { get; set; }
    //    public decimal AdjOtherAmount { get; set; }
    //    public string AdjType { get; set; }
    //    public string AdjDescription { get; set; }
    //    public string AdjReferance { get; set; }
    //}
    public class AdjustmentHistoryVM
    {
        public string AdjHistoryID { get; set; }
        public string AdjHistoryNo { get; set; }
        public string AdjId { get; set; }
        public string AdjDate { get; set; }
        public decimal AdjInputAmount { get; set; }
        public decimal AdjInputPercent { get; set; }
        public decimal AdjAmount { get; set; }
        public decimal AdjVATRate { get; set; }
        public decimal AdjVATAmount { get; set; }
        public decimal AdjSD { get; set; }
        public decimal AdjSDAmount { get; set; }
        public decimal AdjOtherAmount { get; set; }
        public string AdjType { get; set; }
        public string AdjDescription { get; set; }
        public string AdjReferance { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string HeadName { get; set; }
        public string Post { get; set; }
        public string Operation { get; set; }
        public string IsAdjSD { get; set; }

        public int BranchId { get; set; }

    }
    public class CashPayableVM
    {
        public AdjustmentHistoryVM Adjustment { get; set; }

        public DepositMasterVM Deposit { get; set; }

        public string Operation { get; set; }
    }

    public class SaleDHLReport
    {
        public string SalesInvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string TelephoneNo { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string ProductNameOld { get; set; }
        public string ProductDescription { get; set; }
        public string ProductGroup { get; set; }
        public string UOM { get; set; }
        public string ProductCommercialName { get; set; }
        public string VATRegistrationNo { get; set; }
        public string SerialNo { get; set; }
        public int AlReadyPrint { get; set; }
        public string ImportIDExcel { get; set; }
        public string Comments { get; set; }
        public string VATType { get; set; }
        public string LCNumber { get; set; }
        public string LCBank { get; set; }
        public string PINo { get; set; }
        public string EXPFormNo { get; set; }
        public int BranchId { get; set; }
        public string SignatoryName { get; set; }
        public string SignatoryDesig { get; set; }
        public string SerialNo1 { get; set; }
        public string SaleType { get; set; }
        public string PreviousSalesInvoiceNo { get; set; }
        public string TransactionType { get; set; }
        public string CurrencyID { get; set; }
        public string TPurchaseInvoiceNo { get; set; }
        public string TBENumber { get; set; }
        public string TCustomHouse { get; set; }
        public string FileName { get; set; }
        public double VATRate { get; set; }
        public double SD { get; set; }
        public string CustomerCode { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
        public double SDAmount { get; set; }
        public double VATAmount { get; set; }
        public double Fixed_Subtotal { get; set; }
        public double LineTotal { get; set; }
        public int Sort { get; set; }


    }


    public  class DhlCreditNoteMOdel
    {
        public string SalesInvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string TelephoneNo { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string ProductNameOld { get; set; }
        public string ProductDescription { get; set; }
        public string ProductGroup { get; set; }
        public string UOM { get; set; }
        public string ProductCommercialName { get; set; }
        public string VATRegistrationNo { get; set; }
        public string SerialNo { get; set; }
        public int AlReadyPrint { get; set; }
        public string ImportIDExcel { get; set; }
        public string Comments { get; set; }
        public string VATType { get; set; }
        public string LCNumber { get; set; }
        public string LCBank { get; set; }
        public string PINo { get; set; }
        public string EXPFormNo { get; set; }
        public int BranchId { get; set; }
        public string SignatoryName { get; set; }
        public string FileName { get; set; }
        public string SignatoryDesig { get; set; }
        public string SerialNo1 { get; set; }
        public string SaleType { get; set; }
        public string PreviousSalesInvoiceNo { get; set; }
        public string TransactionType { get; set; }
        public string CurrencyID { get; set; }
        public string TPurchaseInvoiceNo { get; set; }
        public string TBENumber { get; set; }
        public string TCustomHouse { get; set; }
        public double VATRate { get; set; }
        public string CustomerCode { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
        public double SDAmount { get; set; }
        public double VATAmount { get; set; }
        public int Sort { get; set; }

        public decimal PreviousNBRPrice { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal PreviousSubTotal { get; set; }
        public decimal PreviousVATRate { get; set; }
        public decimal PreviousVATAmount { get; set; }
        public decimal PreviousSD { get; set; }
        public decimal PreviousSDAmount { get; set; }
        public decimal LineTotal { get; set; }
        public decimal PreLineTotal { get; set; }
        public decimal Fixed_Subtotal { get; set; }

        public string ReasonOfReturn { get; set; }
        public string PreviousSalesInvoiceNoD { get; set; }
        public string PreviousInvoiceDateTime { get; set; }
        public string PreviousUOM { get; set; }


    }
}
