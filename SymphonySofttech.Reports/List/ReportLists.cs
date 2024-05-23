using System;
using System.Collections.Generic;
using System.Text;

namespace SymphonySofttech.Reports.List
{

    public class MIS19VM
    {
        public int Id { get; set; }  // Serial No   
        public string One { get; set; }
        public string Two { get; set; }
        public decimal Three { get; set; }  // Serial No   
        public decimal FourA { get; set; }  // Serial No   
        public decimal FourB { get; set; }  // Serial No   
        public decimal FourC { get; set; }  // Serial No   
        public decimal FourD { get; set; }  // Serial No   
        public decimal Five { get; set; }  // Serial No   
        public decimal Six { get; set; }  // Serial No   
        public decimal SevenA { get; set; }  // Serial No   
        public decimal SevenB { get; set; }  // Serial No   
        public decimal SevenC { get; set; }  // Serial No   
        public decimal SevenD { get; set; }  // Serial No   
        public decimal EightA { get; set; }  // Serial No   
        public decimal EightB { get; set; }  // Serial No   
        public decimal EightC { get; set; }  // Serial No   
        public decimal EightD { get; set; }  // Serial No   
        public decimal Nine { get; set; }  // Serial No   
        public decimal Ten { get; set; }  // Serial No   
        public decimal Eleven { get; set; }  // Serial No   
        public decimal Twelve { get; set; }  // Serial No   
        public decimal Thirteen { get; set; }  // Serial No   
        public decimal Fourteen { get; set; }  // Serial No   
        public decimal Fifteen { get; set; }  // Serial No   
        public decimal Sixteen { get; set; }  // Serial No   
        public decimal Seventeen { get; set; }  // Serial No   
        public string Eighteen { get; set; }  // Serial No   
    }
    public class VAT_17
    {
        public decimal Column1 { get; set; }  // Serial No   
        public DateTime Column2 { get; set; }  // Date
        public decimal Column3 { get; set; }  // Opening Quantity
        public decimal Column4 { get; set; }  // Opening Price
        public decimal Column5 { get; set; }  // Production Quantity
        public decimal Column6 { get; set; }  // Production Price
        public string Column7 { get; set; }  // Customer Name
        public string Column8 { get; set; }  // Customer VAT Reg No
        public string Column9 { get; set; }  // Customer Address
        public string Column10 { get; set; } // Sale Invoice No
        public DateTime Column11 { get; set; } // Sale Invoice Date and Time
        public string Column11string { get; set; } // Sale Invoice Date and Time
        public string Column12 { get; set; } // Sale Product Name
        public decimal Column13 { get; set; } // Sale Product Quantity
        public decimal Column14 { get; set; } // Sale Product Sale Price(NBR Price with out VAT and SD amount)
        public decimal Column15 { get; set; } // SD Amount
        public decimal Column16 { get; set; } // VAT Amount
        public decimal Column17 { get; set; } // Closing Quantity
        public decimal Column18 { get; set; } // Closing Amount
        public string Column19 { get; set; } // Remarks
        public string temp1 { get; set; } // Remarks
        public string temp2 { get; set; } // Remarks
        public string temp3 { get; set; } // Remarks

        public string IsMonthly { get; set; } // MonthFlag
        public string Day { get; set; } // MonthFlag
        public string Month { get; set; } // MonthFlag

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string FormUom { get; set; }
        public string ToUom { get; set; }
        public decimal AdjustmentValue { get; set; }
        public decimal ClosingRate { get; set; }
        public decimal DeclaredPrice { get; set; }
        public decimal RunningTotal { get; set; }
        public decimal RunningTotalValue { get; set; }
        public decimal RunningTotalValueFinal { get; set; }
        public decimal RunningOpeningValueFinal { get; set; }
        public decimal RunningOpeningQuantityFinal { get; set; } 
        public string ProductDesc { get; set; }
        public string PeriodId { get; set; }
        public string BranchId { get; set; }
        public string UserId { get; set; }




    }

    public class VAT_16
    {

        public Decimal Column1 { get; set; }  // Serial No   
        public DateTime Column2 { get; set; }  // Date
        public Decimal Column3 { get; set; }  // Opening Quantity
        public Decimal Column4 { get; set; }  // Opening Price
        public string Column5 { get; set; }  // Invoice/BE No
        public DateTime Column6 { get; set; }  // Invoice/BE Date
        public string Column6String { get; set; }  // Invoice/BE Date
        public string Column7 { get; set; }  // Vendor Name
        public string Column8 { get; set; }  // Vendor Address
        public string Column9 { get; set; }  // Vendor VAT Reg No
        public string Column10 { get; set; } // Product Information
        public Decimal Column11 { get; set; } // Purchase Quantity
        public Decimal Column12 { get; set; } // Price(Sub total) without VAT and SD
        public Decimal Column13 { get; set; } // SD Amount
        public Decimal Column14 { get; set; } // VAT Amount
        public Decimal Column15 { get; set; } // Issue Quantity for Production
        public Decimal Column16 { get; set; } // Issue Price for Production
        public Decimal Column17 { get; set; } // Closing Quantity
        public Decimal Column18 { get; set; } // Closing Amount
        public string Column19 { get; set; } // Remarks
        public string temp1 { get; set; } // Remarks
        public string temp2 { get; set; } // Remarks
        public string temp3 { get; set; } // Remarks
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string FormUom { get; set; }
        public string ToUom { get; set; }
        public Decimal AvgRate { get; set; }
        public Decimal RunningTotal { get; set; }
        public Decimal RunningOpeningValueFinal { get; set; }
        public Decimal RunningOpeningQuantityFinal { get; set; } 
        public string Day { get; set; }
        public string UserId { get; set; }


    }


    public class InputValueChange
    {
        public string ItemNo { get; set; } // ItemNo 
        public string ProductCode { get; set; } // Item/Product Code
        public string ProductName { get; set; } // Product Name
        public string UOM { get; set; } // UOM
        public DateTime EffectDate { get; set; } // Effect Date
        public string VATName { get; set; } // VAT Name
        public decimal ApprovedInputValue { get; set; } // Approved Input Value
        public decimal LatestInputValue { get; set; } // Latest Input Value
        public decimal Increase { get; set; } // Increase/(Decrease) in %
        public string Comments { get; set; } // Comments
        public DateTime CurrentDate { get; set; } // Current Date
    }

    public class Sale_Monthly
    {
        public decimal SerialNo { get; set; }  // Serial No   
        public string MName { get; set; } // Month
        public string YName { get; set; } // Year
        public string ItemNo { get; set; } // ItemNo 
        public string ProductCode { get; set; } // Item/Product Code
        public string ProductName { get; set; } // Product Name
        public string CustomerName { get; set; } // Customer Name
        public decimal JanVATAmount { get; set; } // VATAmount
        public decimal JanSubtotal { get; set; } // SubTotal
        public decimal JanQuantity { get; set; } // Quantity
        public decimal FebVATAmount { get; set; } // VATAmount
        public decimal FebSubtotal { get; set; } // SubTotal
        public decimal FebQuantity { get; set; } // Quantity
        public decimal MarVATAmount { get; set; } // VATAmount
        public decimal MarSubtotal { get; set; } // SubTotal
        public decimal MarQuantity { get; set; } // Quantity
        public decimal AprVATAmount { get; set; } // VATAmount
        public decimal AprSubtotal { get; set; } // SubTotal
        public decimal AprQuantity { get; set; } // Quantity
        public decimal MayVATAmount { get; set; } // VATAmount
        public decimal MaySubtotal { get; set; } // SubTotal
        public decimal MayQuantity { get; set; } // Quantity
        public decimal JunVATAmount { get; set; } // VATAmount
        public decimal JunSubtotal { get; set; } // SubTotal
        public decimal JunQuantity { get; set; } // Quantity
        public decimal JulVATAmount { get; set; } // VATAmount
        public decimal JulSubtotal { get; set; } // SubTotal
        public decimal JulQuantity { get; set; } // Quantity
        public decimal AugVATAmount { get; set; } // VATAmount
        public decimal AugSubtotal { get; set; } // SubTotal
        public decimal AugQuantity { get; set; } // Quantity
        public decimal SepVATAmount { get; set; } // VATAmount
        public decimal SepSubtotal { get; set; } // SubTotal
        public decimal SepQuantity { get; set; } // Quantity
        public decimal OctVATAmount { get; set; } // VATAmount
        public decimal OctSubtotal { get; set; } // SubTotal
        public decimal OctQuantity { get; set; } // Quantity
        public decimal NovVATAmount { get; set; } // VATAmount
        public decimal NovSubtotal { get; set; } // SubTotal
        public decimal NovQuantity { get; set; } // Quantity
        public decimal DecVATAmount { get; set; } // VATAmount
        public decimal DecSubtotal { get; set; } // SubTotal
        public decimal DecQuantity { get; set; } // Quantity
        public decimal TotalVAT { get; set; } // VATAmount
        public decimal TotalAmount { get; set; } // SubTotal
        public decimal TotalQuantity { get; set; } // Quantity

    }

    public class BanderolForm_4
    {
        public decimal ColSerialNo { get; set; }
        public string ColFiscalYear { get; set; }
        public DateTime ColTranDate { get; set; }
        public string ColDemandNo { get; set; }
        public string ColProductName { get; set; }
        public string ColPack { get; set; }
        public string ColBanderolInfo { get; set; }
        public decimal ColDemQuantity { get; set; }
        public string ColRecNoDate { get; set; }
        public decimal ColRecQuantity { get; set; }
        public decimal ColCFYRecQty { get; set; }  // Current Financial Year Received Qty
        public decimal ColUsedQty { get; set; }
        public decimal ColWastageQty { get; set; }
        public decimal ColClosingQty { get; set; } // Closing Quantity
        public decimal ColSaleQty { get; set; }
        public string ColRemarks { get; set; }
        public string ColTransType { get; set; }

        public string ColBanderolID { get; set; }
        public string ColPackagingId { get; set; }


    }

    public class BanderolForm_5
    {
        public decimal SerialNo { get; set; }
        public string FiscalYear { get; set; }
        public string MonthName { get; set; }
        public string DemandNo_Date { get; set; }
        public string ProductName { get; set; }
        public string PackNature { get; set; }
        public string BanderolInfo { get; set; }
        public decimal DemandQty { get; set; }
        public string RefNo_Date { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal TotalRecQty { get; set; }  // Current Financial Year Received Qty
        public decimal UsedQty { get; set; }
        public decimal WastageQty { get; set; }
        public decimal ClosingQty { get; set; } // Closing Quantity
        public decimal SaleQty { get; set; }
        public string Remarks { get; set; }
        public string TransType { get; set; }

        public string ColBanderolID { get; set; }
        public string ColPackagingId { get; set; }

    }

    public class BanderolSaleInfo
    {
        public string ItemNo { get; set; }
        public string SaleProduct { get; set; }
        public decimal ColTotalSale { get; set; }


    }

    public class MISVAT_18
    {
        public string Column0 { get; set; }  //Month
        public string Column1 { get; set; }  //FYear
        public decimal Column2 { get; set; }  // Opening   

        public decimal Column3 { get; set; }  // Purchase-Import
        public decimal Column4 { get; set; }  // Purchase-Local
        public decimal Column5 { get; set; }  // TotalPurchase
        public decimal Column6 { get; set; }// Treasury
        public decimal Column7 { get; set; }  // Rebatable
        public decimal Column8 { get; set; }  // Total Purchase,treasury,rebatable
        public decimal Column9 { get; set; }  // Sale
        public decimal Column10 { get; set; }  // Credit
        public decimal Column11 { get; set; }// Total Sale
        public decimal Column12 { get; set; }  // Closing
    }
}
