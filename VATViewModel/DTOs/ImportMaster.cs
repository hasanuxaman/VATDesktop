using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{

    #region Comments - Before Feb-04-2020
    
    //public class ProductVM
    //{
    //    public string ItemNo { get; set; }
    //    public string ProductName { get; set; }
    //    public string ProductDescription { get; set; }
    //    public string CategoryName { get; set; }
    //    public string UOM { get; set; }
    //    public decimal CostPrice { get; set; }
    //    public decimal SalesPrice { get; set; }
    //    public decimal NBRPrice { get; set; }
    //    public decimal OpeningBalance { get; set; }
    //    public string SerialNo { get; set; }
    //    public string HSCodeNo { get; set; }
    //    public decimal VATRate { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public decimal SD { get; set; }
    //    public decimal Packetprice { get; set; }
    //    public string Trading { get; set; }
    //    public decimal TradingMarkUp { get; set; }
    //    public string NonStock { get; set; }
    //    public string OpeningDate { get; set; }
    //    public string ProductCode { get; set; }
    //    public decimal TollCharge { get; set; }
    //    public decimal OpeningTotalCost { get; set; }
        

    //}

    //public class VendorVM
    //{
    //    public string VendorID { get; set; }
    //    public string VendorCode { get; set; }
    //    public string VendorName { get; set; }
    //    public string VendorGroup { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string City { get; set; }
    //    public string TelephoneNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public string Email { get; set; }
    //    public string StartDateTime { get; set; }
    //    public string ContactPerson { get; set; }
    //    public string ContactPersonDesignation { get; set; }
    //    public string ContactPersonTelephone { get; set; }
    //    public string ContactPersonEmail { get; set; }
    //    public string VATRegistrationNo { get; set; }
    //    public string TINNo { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }

    //    public string Country { get; set; }




    //}

    //public class CustomerVM
    //{
    //    public string CustomerID { get; set; }
    //    public string CustomerCode { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CustomerGroup { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string City { get; set; }
    //    public string TelephoneNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public string Email { get; set; }
    //    public string StartDateTime { get; set; }
    //    public string ContactPerson { get; set; }
    //    public string ContactPersonDesignation { get; set; }
    //    public string ContactPersonTelephone { get; set; }
    //    public string ContactPersonEmail { get; set; }
    //    public string TINNo { get; set; }
    //    public string VATRegistrationNo { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }

    //    public string Country { get; set; }
    //}

    //public class BankInformationVM
    //{
    //    public string BankID { get; set; }
    //    public string BankCode { get; set; }

    //    public string BankName { get; set; }
    //    public string BranchName { get; set; }
    //    public string AccountNumber { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string City { get; set; }
    //    public string TelephoneNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public string Email { get; set; }
    //    public string ContactPerson { get; set; }
    //    public string ContactPersonDesignation { get; set; }
    //    public string ContactPersonTelephone { get; set; }
    //    public string ContactPersonEmail { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }

    //}

    //public class VehicleVM
    //{

    //    public string Code { get; set; }
    //    public string VehicleID { get; set; }
    //    public string VehicleType { get; set; }
    //    public string VehicleNo { get; set; }
    //    public string Description { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }

    //}

    #endregion

    public class ProductVM
    {

        public decimal WastageTotalValue { get; set; }
        
        public string ButtonName { get; set; }
        public string SearchItemNo { get; set; }
        public string SearchProductCode { get; set; }
        public string IsExpireDate { get; set; }
        public string TransactionType { get; set; }
        public string ReportType { get; set; }
        public decimal TransactionHoldDate { get; set; }

        //public string Id { get; set; }
        public string ItemNo { get; set; }
        //[Display(Name = "Name")]
        public string ProductName { get; set; }
        //[Display(Name = "Desciption")]
        public string ShortName { get; set; }
        public string GenericName { get; set; }
        public string DARNo { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryName { get; set; }
        //[Display(Name = "Group")]
        public string CategoryID { get; set; }
        public string UOM { get; set; }
        //[Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }
        //[Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }
        public decimal ReceivePrice { get; set; }
        //[Display(Name = "NBR Price")]
        public decimal NBRPrice { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TollOpeningQuantity { get; set; }
        //[Display(Name = "Serial No")]
        public string SerialNo { get; set; }
        //[Display(Name = "HS Code No")]
        public string HSCodeNo { get; set; }
        //[Display(Name = "Information")]
        public decimal VATRate { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal SD { get; set; }
        public decimal Packetprice { get; set; }
        public string Trading { get; set; }
        //[Display(Name = "Trading MarkUp")]
        public decimal TradingMarkUp { get; set; }
        public decimal Stock { get; set; }
        //[Display(Name = "Non-stock")]
        public string NonStock { get; set; }
        //[Display(Name = "Opening Date")]
        public string OpeningDate { get; set; }
        //[Display(Name = "Product Code")]
        public string ProductCode { get; set; }
        //[Display(Name = "Toll Charge")]
        public decimal TollCharge { get; set; }
        //[Display(Name = "Opening Total Cost")]
        public decimal OpeningTotalCost { get; set; }
        //[Display(Name = "Rebate Percent")]
        public decimal RebatePercent { get; set; }
        public string Banderol { get; set; }
        public string TollProduct { get; set; }
        //[Display(Name = "Product Type")]
        public string ProductType { get; set; }
        public string Type { get; set; }
        public string Operation { get; set; }
        public string TargetId { get; set; }
        public string isActive { get; set; }
        //[Display(Name = "VAT Rate 2")]
        public decimal VATRate2 { get; set; }
        public decimal TradingSaleVATRate { get; set; }
        public decimal TradingSaleSD { get; set; }
        public decimal VDSRate { get; set; }
        public decimal TVBRate { get; set; }
        public decimal CDRate { get; set; }
        public decimal RDRate { get; set; }
        public decimal AITRate { get; set; }
        public decimal TVARate { get; set; }
        public decimal ATVRate { get; set; }
        public decimal CnFRate { get; set; }
        public string[] retResult { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string IsExempted { get; set; }
        public string IsZeroVAT { get; set; }
        public int BranchId { get; set; }
        public List<ProductNameVM>Details { get; set; }
        public string UOM2 { get; set; }
        public decimal UOMConversion { get; set; }
        public string IsHouseRent { get; set; }
        //  From HsCode 
        public decimal SDRate { get; set; }
        public decimal VATRate3 { get; set; }
        public string IsFixedSD { get; set; }
        public string IsFixedCD { get; set; }
        public string IsFixedRD { get; set; }
        public string IsFixedAIT { get; set; }
        public string IsFixedVAT1 { get; set; }
        public string IsFixedAT { get; set; }
        public string IsFixedOtherSD { get; set; }
        public string IsConfirmed { get; set; }

        public string IsVDS { get; set; }
        public decimal HPSRate { get; set; }

        public bool IsFixedCDChecked { get; set; }
        public bool IsFixedSDChecked { get; set; }
        public bool IsFixedRDChecked { get; set; }
        public bool IsFixedAITChecked { get; set; }
        public bool IsFixedVAT1Checked { get; set; }
        public bool IsFixedATChecked { get; set; }
        //
        public string IsRaw { get; set; }

        public string IsTransport { get; set; }

        public string TDSCode { get; set; }

        public decimal FixedVATAmount { get; set; }
        public decimal FixedVATAmountP { get; set; }
        public decimal Volume { get; set; }

        public string IsFixedVAT { get; set; }

        public List<ProductStockVM> ProductStocks { get; set; }

        public int BOMId { get; set; }

        public bool IsFixedVatM
        {
            get { return IsFixedVAT == "Y"; }
        }

        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }
        public List<string> ProductIDs { get; set; }


        public string TotalCount { get; set; }
        public string IsCodeUpdate { get; set; }
        public string IsFixedVATRebate { get; set; }
        public string IsSample { get; set; }
        public string IsChild { get; set; }
        public string MasterProductItemNo { get; set; }
        public string Option1 { get; set; }
        public string VolumeUnit { get; set; }
        public string PackSize { get; set; }
        public string IsPackCal { get; set; }

        
    }

    public class VendorVM
    {
        public string VendorID { get; set; }
        //[Display(Name = "Code")]
        public string VendorCode { get; set; }
        //[Display(Name = "Name")]
        public string VendorName { get; set; }
        public string ShortName { get; set; }
        public string VendorGroup { get; set; }
        //[Display(Name = "Group")]
        public string VendorGroupID { get; set; }
        //[Display(Name = "Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        //[Display(Name = "Telephone No")]
        public string TelephoneNo { get; set; }
        //[Display(Name = "Fax No")]
        public string FaxNo { get; set; }
        public string Email { get; set; }
        //[Display(Name = "Start Date")]
        public string StartDateTime { get; set; }
        //[Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        //[Display(Name = "CP Designation")]
        public string ContactPersonDesignation { get; set; }
        //[Display(Name = "CP Telephone")]
        public string ContactPersonTelephone { get; set; }
        //[Display(Name = "CP Email")]
        public string ContactPersonEmail { get; set; }
        //[Display(Name = "VAT Registration No")]
        public string VATRegistrationNo { get; set; }
        //[Display(Name = "TIN No")]
        public string TINNo { get; set; }
        public string NIDNo { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Country { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public decimal VDSPercent { get; set; }
        //[Display(Name = "Business Type")]
        public string BusinessType { get; set; }
        //[Display(Name = "Business Code")]
        public string BusinessCode { get; set; }
        public string Operation { get; set; }
        //[Display(Name = "Start Date From")]
        public string StartDateFrom { get; set; }
        //[Display(Name = "Start Date To")]
        public string StartDateTo { get; set; }
        public string IsActive { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string IsVDSWithHolder { get; set; }

        public string IsRegister { get; set; }
        public string IsTurnover { get; set; }
        public int BranchId { get; set; }

        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }
        public List<string> VendorIDs { get; set; }
    }
    
    public class CustomerVM
    {
        public string CustomerID { get; set; }
        //[Display(Name = "Code")]
        public string CustomerCode { get; set; }
        ////[Display(Name = "Name")]
        public string CustomerName { get; set; }
        public string CustomerBanglaName { get; set; }

        public string ShortName { get; set; }
        //[Display(Name = "Group")]
        public string CustomerGroupID { get; set; }
        //[Display(Name = "Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        //[Display(Name = "Telephone No")]
        public string TelephoneNo { get; set; }
        //[Display(Name = "Fax No")]
        public string FaxNo { get; set; }
        public string Email { get; set; }
        //[Display(Name = "Start Date")]
        public string StartDateTime { get; set; }
        //[Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        //[Display(Name = "CP Designation")]
        public string ContactPersonDesignation { get; set; }
        //[Display(Name = "CP Telephone")]
        public string ContactPersonTelephone { get; set; }
        //[Display(Name = "CP Email")]
        public string ContactPersonEmail { get; set; }
        //[Display(Name = "TIN No")]
        public string TINNo { get; set; }
        //[Display(Name = "VAT Registration No")]
        public string VATRegistrationNo { get; set; }
        public string NIDNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string Country { get; set; }
        //[Display(Name = "VDS Percent")]
        public decimal VDSPercent { get; set; }
        //[Display(Name = "Business Type")]
        public string BusinessType { get; set; }
        //[Display(Name = "Business Code")]
        public string BusinessCode { get; set; }
        public string Operation { get; set; }
        //[Display(Name = "Customer Group")]
        public string CustomerGroup { get; set; }
        public bool IsArchive { get; set; }
        public string IsActive { get; set; }
        //[Display(Name = "Start Date From")]
        public string StartDateFrom { get; set; }
        //[Display(Name = "Start Date To")]
        public string StartDateTo { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string IsVDSWithHolder { get; set; }
        public string IsInstitution { get; set; }
        public string IsExamted { get; set; }
        public string IsTax { get; set; }
        public string IsSpecialRate { get; set; }
        public int BranchId { get; set; }

        public List<CustomerAddressVM> Details { get; set; }
        public List<CustomerDiscountVM> Discount { get; set; }
        public CustomerBillProcessVM CustomerBillProcess { get; set; }
        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }
        public List<string> CustomerIDs { get; set; }
        public bool CustomerSync { get; set; }

        public string CompanyCode { get; set; }
        public string IsTCS { get; set; }


        public HttpPostedFileBase File { get; set; }


        public string IsCreditCustomer { get; set; }
        public string SC { get; set; }
        public string RF { get; set; }
        public string SSLF { get; set; }
        public string DC { get; set; }


    }
    
    public class BankInformationVM
    {
        public string BankID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string Operation { get; set; }
        public int BranchId{ get; set; }
    }
    
    public class VehicleVM
    {
        public string VehicleID { get; set; }
        //[Display(Name = "Code")]
        public string Code { get; set; }
        //[Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }
        //[Display(Name = "Vehicle No")]
        public string VehicleNo { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string DriverName { get; set; }
        public string Operation { get; set; }
        public string IsActive { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        public bool ExportAll { get; set; }
        public List<string> VehicleIDs { get; set; }
    }

    public class ProductTypeVM
    {
        public string TypeID { get; set; }
        public string ProductType { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string Description { get; set; }
    }

    public class CostingVM
    {
        public string ItemNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string InputDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal AV { get; set; }
        public decimal CD { get; set; }
        public decimal RD { get; set; }
        public decimal TVB { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TVA { get; set; }
        public decimal ATV { get; set; }
        public decimal Other { get; set; }
        public decimal CostPrice { get; set; }//TotalPrice
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string BENumber { get; set; }
        public string RefNo { get; set; }
    }

    public class UOMVM
    {
        public string UOMID { get; set; }
        public string UOMName { get; set; }
        public string UOMCode { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public string UOMFrom { get; set; }

        public string UOMTo { get; set; }

        public string DatabaseName { get; set; }

    }

    public class UOMNameVM
    {
        public string UOMID { get; set; }
        public string UOMName { get; set; }
        public string UOMCode { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
    }

    public class UOMConversionVM
    {
        public string UOMId { get; set; }
        public string UOMFrom { get; set; }
        public string UOMTo { get; set; }
        public decimal Convertion { get; set; }
        public string CTypes { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string ActiveStatus { get; set; }

        public string Operation { get; set; }

    }

    public class CompanyCategoryVM
    {
        public string Id { get; set; }
        public string CATEGORY_ID { get; set; }
        public string CATEGORY { get; set; }

        public string ITEM_ID { get; set; }

        public string GOODS_SERVICE_CODE { get; set; }

        public string GOODS_SERVICE_NAME { get; set; }
    }

    public class ResponseVM
    {
        public string message { get; set; }
        public List<CompanyCategoryVM> items { get; set; }
    }

    public class APIresultVM
    {
        public ResponseVM response { get; set; }
    }

}

