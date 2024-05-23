using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
    public class ProductDTO
    {
        public string ItemNo { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string UOM { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public string IsRaw { get; set; }
        public string SerialNo { get; set; }
        public string HSCodeNo { get; set; }
        public decimal VATRate { get; set; }
        public string ActiveStatus { get; set; }
        public decimal OpeningBalance { get; set; }
        public string Comments { get; set; }
        public string HSDescription { get; set; }
        public decimal Stock { get; set; }
        public decimal SD { get; set; }
        public decimal Packetprice { get; set; }
        public bool Trading { get; set; }
        public decimal TradingMarkUp { get; set; }
        public bool NonStock { get; set; }
        public decimal QuantityInHand { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal ReceivePrice { get; set; }
        public decimal IssuePrice { get; set; }
        public string ProductCode { get; set; }
    }

    public class ProductMiniDTO
    {
        public string ItemNo { get; set; }  //all
        public string ProductName { get; set; } //all
        public string ProductCode { get; set; } //all
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string UOM { get; set; } //all
        public string IsRaw { get; set; }
        public string HSCodeNo { get; set; } //all
        public decimal VATRate { get; set; }
        public decimal Stock { get; set; } //all
        public decimal SD { get; set; }
        public decimal Packetprice { get; set; }
        public bool Trading { get; set; }
        public decimal TradingMarkUp { get; set; }
        public bool NonStock { get; set; }
        public decimal QuantityInHand { get; set; }
        public decimal CostPrice { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public decimal ReceivePrice { get; set; }//Receive
        public decimal IssuePrice { get; set; }//Issue
        public decimal TenderPrice { get; set; }
        public decimal ExportPrice { get; set; }
        public decimal InternalIssuePrice { get; set; }
        public decimal TollIssuePrice { get; set; }
        public decimal TollCharge { get; set; }
        public decimal TenderQty { get; set; }
        public decimal TenderStock { get; set; }
        public decimal PurchaseQty { get; set; }

        public decimal LIFOPrice { get; set; }
        public decimal BOMPrice { get; set; }
        public decimal RebatePercent { get; set; }
        public string ProductNameCode { get; set; }
        public string BOMId { get; set; }


    }
    public class ProductCmbDTO
    {
        public string ItemNo { get; set; }  //all
        public string ProductName { get; set; } //all
        public string ProductCode { get; set; } //all
        public string Quantity { get; set; } //all
        public string Value { get; set; } //all

    }

    public class ProductbySaleInvoiceDTO
    {
        public string ItemNo { get; set; }  //all
        public string ProductName { get; set; } //all
        public string ProductCode { get; set; } //all
        public decimal Quantity { get; set; } //all
        public decimal SubTotal { get; set; } //al
        public string UOM { get; set; } //all
    }

    public class ProductbyPurchaseInvoiceDTO
    {
        public string ItemNo { get; set; }  //all
        public string ProductName { get; set; } //all
        public string ProductCode { get; set; } //all
        public decimal Quantity { get; set; } //all
        public decimal SubTotal { get; set; } //al
        public decimal CostPrice { get; set; }
        public string UOM { get; set; } //all
        public decimal CD { get; set; } //all
        public decimal RD { get; set; } //al
        public decimal SD { get; set; } //al
        public decimal VATAmount { get; set; } //al
        public decimal CnFAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal TVBAmount { get; set; }
        public decimal TVAAmount { get; set; }
        public decimal ATVAmount { get; set; }
        public decimal OthersAmount { get; set; }
    }

    public class BanderolProductMiniDTO
    {
        public string BandProductId { get; set; }
        public string ItemNo { get; set; }  //all
        public string ProductName { get; set; } //all
        public string ProductCode { get; set; } //all
        public string BanderolId { get; set; } //all
        public string BanderolName { get; set; }
        public string BanderolSize { get; set; } //all
        public string BanderolUom { get; set; }
        public string PackagingId { get; set; } //all
        public string PackagingName { get; set; }
        public string PackagingSize { get; set; }
        public string PackagingUom { get; set; }
        public decimal BUsedQty { get; set; }
        
    }

    public class TrackingCmbDTO
    {
        public string ItemNo { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string VatName { get; set; }
        public string EffectiveDate { get; set; }
        public string BOMId { get; set; }
        public string Qty { get; set; }
        public string ReceiveNo { get; set; }
        public string SaleInvoiceNo { get; set; }
        public string IssueNo { get; set; }
        public string TransferIssueno { get; set; }
    }

    public class FinishProductInfoDTO
    {
        public string FinishItemNo { get; set; }
        public string RawItemNo { get; set; }
        public decimal FinishQty { get; set; }
        public decimal UsedRawQty { get; set; }
        public decimal TotalUsedRawQty { get; set; } 

    }

    
}
