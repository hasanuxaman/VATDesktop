using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
  public  class ProductTypeDTO
    {
        public string TypeID { get; set; }
        public string ProductType { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Description { get; set; }     

    }
    public class VDSDTO
    {
        public string VDSId { get; set; }
        public string VendorId { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime BillDate { get; set; }
        public decimal BillDeductAmount { get; set; }
        public string DepositNumber { get; set; }
        public DateTime DepositDate { get; set; }
        public string Remarks { get; set; }
        public DateTime IssueDate { get; set; }
        public string PurchaseNumber { get; set; } 


    }

    public class DisposeItemsDTO
    {

    public string RFId{ get; set; }
    public string BENumber{ get; set; }
    public string PurchaseNumber{ get; set; }
    public int    LineNumber{ get; set; }
    public string ItemNo{ get; set; }
    public Decimal Quantity{ get; set; }
    public Decimal RealPrice { get; set; }
    public Decimal VATAmount { get; set; }
    public string DisposeType{ get; set; }
    public Decimal PresentPrice { get; set; }
    public string Remarks{ get; set; }
    public DateTime PostingDate { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string ProductDescription { get; set; }
    public string UOM { get; set; }

       
    }

    public class TenderHeadersDTO
    {
        //public string TenderId { get; set; }
        //public string RefNo { get; set; }
        //public string CustomerId { get; set; }
        //public string CustomerName { get; set; }
        //public string Address1 { get; set; }
        //public string Address2 { get; set; }
        //public string Address3 { get; set; }
        //public DateTime TenderDate { get; set; }
        //public DateTime DeleveryDate { get; set; }
        //public string Comments { get; set; }
        //public string CustomerGroupName { get; set; }
        //public string CustomerGroupID { get; set; }

        public string TenderId { get; set; }
        public string RefNo { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        //public string Address1 { get; set; }
        //public string Address2 { get; set; }
        //public string Address3 { get; set; }
        public string CustomerGroupName { get; set; }
        public DateTime TenderDate { get; set; }
        public DateTime DeleveryDate { get; set; }
        public string Comments { get; set; }
        
        public string CustomerGroupID { get; set; }


        
        
    }
}
