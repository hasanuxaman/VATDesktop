﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
    public class ServiceVM
    {
        public string FinishItemNo { get; set; }
        public string RawItemNo { get; set; }
        public decimal UseQuantity { get; set; }
        public decimal WastageQuantity { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal SD { get; set; }
        public DateTime EffectDate { get; set; }
        public string VATName { get; set; }
        public decimal Cost { get; set; }
        public string BOMLineNo { get; set; }
        public decimal PacketPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TradingMarkUp { get; set; }
        public decimal RawOHCost { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }
        public decimal UOMUQty { get; set; }
        public decimal UOMWQty { get; set; }
        public int BranchId { get; set; }
    }

    public class BOMItemVM
    {
        public string BOMRawId { get; set; }
        public string BOMId { get; set; }
        public string LineItemNo { get; set; }
        public string FinishItemNo { get; set; }
        public string RawItemNo { get; set; }
        public string RawItemCode { get; set; }
        public string RawItemName { get; set; }
        public string RawItemType { get; set; }
        public string EffectDate { get; set; }
        public string VatName { get; set; }
        public decimal UseQuantity { get; set; }
        public decimal WastageQuantity { get; set; }
        public decimal Cost { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VatAmount { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public decimal TradingMarkUp { get; set; }
        public decimal RebateRate { get; set; }
        public decimal MarkUpValue { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal UnitCost { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal UOMUQty { get; set; }
        public decimal UOMWQty { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Post { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string BOMLineNo { get; set; }
        public decimal PacketPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public decimal RawOHCost { get; set; }
        public string PBOMId { get; set; }
        public string Mark { get; set; }
        public string PInvoiceNo { get; set; }
        public string TransactionType { get; set; }
        public string CustomerID { get; set; }
        public string IssueOnProduction { get; set; }
        public int BranchId { get; set; }
        public string CategoryId { get; set; }

    }

    public class BOMOHVM
    {

        public string BOMId { get; set; }
        public string HeadName { get; set; }
        public string OHCode { get; set; }
        public string HeadID { get; set; }
        public decimal HeadAmount { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string FinishItemNo { get; set; }
        public string EffectDate { get; set; }
        public string OHLineNo { get; set; }
        public decimal RebatePercent { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal AdditionalCost { get; set; }
        public string Post { get; set; }
        public int BranchId { get; set; }
        public string CustomerID { get; set; }
    }

    public class BOMNBRVM
    {
        public string ReferenceNo { get; set; }


        public string BOMId { get; set; }
        public string ItemNo { get; set; }
        public string FinishItemName { get; set; }
        public string FinishItemCode { get; set; }
        public string FinishCategory { get; set; }
        public string EffectDate { get; set; }
        public string VATName { get; set; }
        public decimal VATRate { get; set; }
        public string UOM { get; set; }
        public decimal SDRate { get; set; }
        public decimal TradingMarkup { get; set; }
        public decimal RebateRate { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal RawTotal { get; set; }
        public decimal PackingTotal { get; set; }
        public decimal RebateTotal { get; set; }
        public decimal AdditionalTotal { get; set; }
        public decimal RebateAdditionTotal { get; set; }
        public decimal PNBRPrice { get; set; }
        public decimal PPacketPrice { get; set; }
        public decimal RawOHCost { get; set; }
        public decimal LastNBRPrice { get; set; }
        public decimal LastNBRWithSDAmount { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal WholeSalePrice { get; set; }
        public decimal NBRWithSDAmount { get; set; }
        public decimal MarkupValue { get; set; }
        public decimal LastMarkupValue { get; set; }
        public decimal LastSDAmount { get; set; }
        public decimal LastAmount { get; set; }
        public decimal ReceiveCost { get; set; }
        public decimal Margin { get; set; }

        public decimal FUOMPrice { get; set; }
        public decimal FUOMc { get; set; }

        public string FUOMn { get; set; }

        public string Post { get; set; }
        public string IsImported { get; set; }
        public string CustomerID { get; set; }

        public List<BOMItemVM> Items { get; set; }
        public List<BOMOHVM> Overheads { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string CustomerName { get; set; }
        public string  Operation { get; set; }

        public decimal Other { get; set; }
        public decimal OtherAmount { get; set; }
        public int BranchId { get; set; }
        public string FirstSupplyDate { get; set; }

        public string SelectTop { get; set; }
        public List<string> IDs { get; set; }
        public string FinishItemNo { get; set; }
        public string AutoIssue { get; set; }
        public bool ExportOverhead { get; set; }
        public string MasterComments { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string ServerPath { get; set; }
        public string SubmittedFileName { get; set; }
        public string SubmittedFilePath { get; set; }
        public string ApprovedFileName { get; set; }
        public string ApprovedFilePath { get; set; }

    }

    public class NBRMaster
    {
        public List<BOMNBRVM> NbrVMs { get; set; }
        public string Operation { get; set; }
    }

    public class BOMSearchVM
    {
        public string BOMId { get; set; }
        public string FinishItemNo { get; set; }
        public string productname { get; set; }

        public DateTime EffectDate { get; set; }
        public string VATName { get; set; }
        public decimal NBRPrice { get; set; }
        public string ItemNo { get; set; }
        public string ProductCode { get; set; }
        public string UOM { get; set; }
        public string HSCodeNo { get; set; }
        public int BranchId { get; set; }
        

    }

    #region Comments on Feb-04-2020
    
    //public class ServiceVM
    //{
        

    //    public string FinishItemNo { get; set; }
    //    public string RawItemNo { get; set; }
    //    public decimal UseQuantity { get; set; }
    //    public decimal WastageQuantity { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public DateTime LastModifiedOn { get; set; }
    //    public string UOM { get; set; }
    //    public decimal VATRate { get; set; }
    //    public decimal SD { get; set; }
    //    public DateTime EffectDate { get; set; }
    //    public string VATName { get; set; }
    //    public decimal Cost { get; set; }
    //    public string BOMLineNo { get; set; }
    //    public decimal PacketPrice { get; set; }
    //    public decimal NBRPrice { get; set; }
    //    public decimal UnitCost { get; set; }
    //    public decimal TradingMarkUp { get; set; }
    //    public decimal RawOHCost { get; set; }
    //    public decimal UOMPrice { get; set; }
    //    public decimal UOMc { get; set; }
    //    public string UOMn { get; set; }
    //    public decimal UOMUQty { get; set; }
    //    public decimal UOMWQty { get; set; }
    //}
    //public class BOMItemVM
    //{
    //    public string BOMRawId { get; set; }
    //    public string BOMId { get; set; }
    //    public string LineItemNo { get; set; }

    //    public string FinishItemNo { get; set; }
    //    public string RawItemNo { get; set; }
    //    public string RawItemName { get; set; }
    //    public string RawItemType { get; set; }

    //    public string EffectDate { get; set; }
    //    public string VatName { get; set; }
    //    public decimal UseQuantity { get; set; }
    //    public decimal WastageQuantity { get; set; }
    //    public decimal Cost { get; set; }
    //    public string UOM { get; set; }
    //    public decimal VATRate { get; set; }
    //    public decimal VatAmount { get; set; }
    //    public decimal SD { get; set; }
    //    public decimal SDAmount { get; set; }
    //    public decimal TradingMarkUp { get; set; }
    //    public decimal RebateRate { get; set; }
    //    public decimal MarkUpValue { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public decimal UnitCost { get; set; }
    //    public string UOMn { get; set; }
    //    public decimal UOMc { get; set; }
    //    public decimal UOMPrice { get; set; }
    //    public decimal UOMUQty { get; set; }
    //    public decimal UOMWQty { get; set; }
    //    public decimal TotalQuantity { get; set; }
    //    public string Post { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string BOMLineNo { get; set; }
    //    public decimal PacketPrice { get; set; }
    //    public decimal NBRPrice { get; set; }
    //    public decimal RawOHCost { get; set; }
    //    public string PBOMId { get; set; }
    //    public string Mark { get; set; }
    //    public string PInvoiceNo { get; set; }
    //    public string TransactionType { get; set; }
    //    public string CustomerID { get; set; }
    //    public string IssueOnProduction { get; set; }
    //}
    //public class BOMOHVM
    //{

    //    public string BOMId { get; set; }
    //    public string HeadName { get; set; }
    //    public string OHCode { get; set; }
    //    public string HeadID { get; set; }
    //    public decimal HeadAmount { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public string FinishItemNo { get; set; }
    //    public string EffectDate { get; set; }
    //    public string OHLineNo { get; set; }
    //    public decimal RebatePercent { get; set; }
    //    public decimal RebateAmount { get; set; }
    //    public decimal AdditionalCost { get; set; }
    //    public string Post { get; set; }
    //    public string CustomerID { get; set; }
    //}

    //public class BOMNBRVM
    //{
    //    public string BOMId { get; set; }
    //    public string ItemNo { get; set; }
    //    public string FinishItemName { get; set; }
    //    public string EffectDate { get; set; }
    //    public string VATName { get; set; }
    //    public decimal VATRate { get; set; }
    //    public string UOM { get; set; }
    //    public decimal SDRate { get; set; }
    //    public decimal TradingMarkup { get; set; }
    //    public decimal RebateRate { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public decimal RawTotal { get; set; }
    //    public decimal PackingTotal { get; set; }
    //    public decimal RebateTotal { get; set; }
    //    public decimal AdditionalTotal { get; set; }
    //    public decimal RebateAdditionTotal { get; set; }
    //    public decimal PNBRPrice { get; set; }
    //    public decimal PPacketPrice { get; set; }
    //    public decimal RawOHCost { get; set; }
    //    public decimal LastNBRPrice { get; set; }
    //    public decimal LastNBRWithSDAmount { get; set; }
    //    public decimal TotalQuantity { get; set; }
    //    public decimal SDAmount { get; set; }
    //    public decimal VatAmount { get; set; }
    //    public decimal WholeSalePrice { get; set; }
    //    public decimal NBRWithSDAmount { get; set; }
    //    public decimal MarkupValue { get; set; }
    //    public decimal LastMarkupValue { get; set; }
    //    public decimal LastSDAmount { get; set; }
    //    public decimal LastAmount { get; set; }
    //    public decimal ReceiveCost { get; set; }
    //    public decimal Margin { get; set; }
    //    public string Post { get; set; }
    //    public string IsImported { get; set; }
    //    public string CustomerID { get; set; }








    //}

    //public class BOMSearchVM
    //{
    //    public string BOMId { get; set; }
    //    public string FinishItemNo { get; set; }
    //    public string productname { get; set; }

    //    public DateTime EffectDate { get; set; }
    //    public string VATName { get; set; }
    //    public decimal NBRPrice { get; set; }
    //    public string ItemNo { get; set; }
    //    public string ProductCode { get; set; }
    //    public string UOM { get; set; }
    //    public string HSCodeNo { get; set; }


    //}

    #endregion

}
