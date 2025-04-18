﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{

    public class TenderMasterVM
    {
        public string TenderId { get; set; }
        public string RefNo { get; set; }
        public string CustomerId { get; set; }
        public string TenderDate { get; set; }
        public string DeleveryDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Comments { get; set; }
        public string CustomerGrpID { get; set; }
        public string ImportID { get; set; }
        public string Post { get; set; }
        public string Operation { get; set; }
        public string GroupName { get; set; }
        public string CustomerName { get; set; }
        public List<TenderDetailVM> Details { get; set; }
        public int BranchId { get; set; }
        //master properties 


    }
    public class TenderDetailVM
    {
        //details properties
        public string TenderIdD { get; set; }
        public string ItemNo { get; set; }
        public string PCode { get; set; }
        public string ItemName { get; set; }
        //public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string UOM { get; set; }
        public decimal SubTotal { get; set; }

        public decimal TenderQty { get; set; }
        public decimal TenderPrice { get; set; }
        public string CreatedByD { get; set; }
        public string CreatedOnD { get; set; }
        public string LastModifiedByD { get; set; }
        public string LastModifiedOnD { get; set; }
        public string BOMId { get; set; }
        public int BranchId { get; set; }

        //details properties

    }
    //public class TenderMasterVM
    //{
    //    ////master properties
    //    //string TenderId = TenderHeaderFields[0];
    //    //string RefNo = TenderHeaderFields[1];
    //    //string CustomerId = TenderHeaderFields[2];
    //    //DateTime TenderDate = Convert.ToDateTime(TenderHeaderFields[3]);
    //    //DateTime DeleveryDate = Convert.ToDateTime(TenderHeaderFields[4]);
    //    //string CreatedBy = TenderHeaderFields[5];
    //    //DateTime CreatedOn = Convert.ToDateTime(TenderHeaderFields[6]);
    //    //string LastModifiedBy = TenderHeaderFields[7];
    //    //DateTime LastModifiedOn = Convert.ToDateTime(TenderHeaderFields[8]);
    //    //string Comments = TenderHeaderFields[9];
    //    ////master properties

    //    //master properties
    //    public string TenderId { get; set; }
    //    public string RefNo { get; set; }
    //    public string CustomerId { get; set; }
    //    public string TenderDate { get; set; }
    //    public string DeleveryDate { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public string Comments { get; set; }
    //    public string CustomerGrpID { get; set; }
    //    public string ImportID { get; set; }
    //    //master properties 


    //}
    //public class TenderDetailVM
    //{
    //    //details properties
    //    public string TenderIdD { get; set; }
    //    public string ItemNo { get; set; }
    //    public string PCode { get; set; }
    //    public string ItemName { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal UnitPrice { get; set; }
    //    public string UOM { get; set; }


    //    public decimal TenderQty { get; set; }
    //    public decimal TenderPrice { get; set; }
    //    public string CreatedByD { get; set; }
    //    public string CreatedOnD { get; set; }
    //    public string LastModifiedByD { get; set; }
    //    public string LastModifiedOnD { get; set; }
    //    public string BOMId { get; set; }

    //    //details properties

    //}
}
