using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.JsonModels
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    
    public class DataRoot_Decathlon
    {
        public string status { get; set; }
        public string message { get; set; }
        public string data { get; set; }

        public string INVOICE_NO { get; set; }
        public DateTime INVOICE_DT { get; set; }
        public string BARCODE { get; set; }
        public string SAL_BARCODE { get; set; }
        public string NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public decimal CPU { get; set; }
        public decimal MRP { get; set; }
        public decimal SQTY { get; set; }
        public decimal SUB_TOTAL { get; set; }
        public decimal DISC_PRCNT { get; set; }
        public decimal DISC_AMT { get; set; }
        public string DISC_TYPE { get; set; }

        public decimal VAT_PRCNT { get; set; }
        public decimal VAT_AMT { get; set; }
        public decimal SPECIAL_DISCOUNT { get; set; }
        public decimal RQTY { get; set; }
        public decimal RTN_SUB_AMT { get; set; }
        public decimal RTN_VAT { get; set; }
        public decimal RTN_DISC { get; set; }
        public decimal RTN_AMT { get; set; }
        public decimal NET_AMT { get; set; }
        public string SALESMAN { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string STORE_CODE { get; set; }
        public string STORE_NAME { get; set; }
        public string TERMINAL_ID { get; set; }
        public string SSummary { get; set; }
        public string SUB_CATEGORY_NAME { get; set; }
        public string TIME { get; set; }
        public string TERMINAL_NO { get; set; }
        public string CUS_GROUP { get; set; }
        public string USER_BARCODE { get; set; }
        public decimal SKU { get; set; }
        public bool IS_PRICE_INCLD_VAT { get; set; }
        public string SERIAL_NO { get; set; }
        public string HOLD_TOKEN { get; set; }
        public DateTime Hold_TIME { get; set; }
        public decimal LINE { get; set; }
        public string RTN_INV_REF { get; set; }
        public string IS_GIFT { get; set; }
        public decimal ACT_CPU { get; set; }
        public decimal ADJ_AMT { get; set; }
        public decimal SL { get; set; }
        public decimal TOTAL_VALUE { get; set; }
        public string VENDOR_NAME { get; set; }
        public string VENDOR_CODE { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public decimal GP_AMT { get; set; }
        public decimal GP_PCT { get; set; }
        public decimal GP_CONT { get; set; }
        public string BRAND_CODE { get; set; }
        public string BRAND_NAME { get; set; }
        public decimal EXG_AMT { get; set; }
        public decimal EXG_QTY { get; set; }
        public decimal COMM_PRCNT { get; set; }
        public string PACKAGE_CODE { get; set; }
        public decimal PKG_MRP { get; set; }
        public bool IS_CARTON { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public string Attr { get; set; }
        public string AttrVal { get; set; }
        public string VAttr1 { get; set; }
        public string VAttr1Val { get; set; }
        public string VAttr2 { get; set; }
        public string VAttr2Val { get; set; }
        public decimal AMRP { get; set; }
        public string SAL_EXECUTIVE { get; set; }
        public bool IS_CLAIM { get; set; }
        public string CLAIM_CODE { get; set; }
        public string RTN_INV_STORE { get; set; }
        public decimal SD_PERCENT { get; set; }
        public decimal SD_AMT { get; set; }
        public decimal RTN_SD_AMT { get; set; }
        public string DISC_NOTE { get; set; }
        public string ReasonForPrice { get; set; }
        public string ReturnType { get; set; }
        public string CreditNoteIssueReason { get; set; }
        public bool IsCreditNote { get; set; }
        public bool IsCashReturnDone { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }


    }

    public class DataRoot_DecathlonPur
    {
        public string status { get; set; }
        public string message { get; set; }
        public string data { get; set; }

        public string MEMO_NO { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string STORE_CODE { get; set; }
        public string STORE_NAME { get; set; }
        public string BARCODE { get; set; }
        public string SAL_BARCODE { get; set; }
        public string USER_BARCODE { get; set; }
        public object USER_BARCODE1 { get; set; }
        public string DISPLAY_NAME { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public DateTime MANUFACTURE_DATE { get; set; }
        public decimal PUR_PRICE { get; set; }
        public decimal PUR_QTY { get; set; }
        public decimal FREE_QTY { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal VAT { get; set; }
        public decimal VAT_PRCNT { get; set; }
        public decimal IS_PRICE_INCLD_VAT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public decimal ACT_PUR_PRICE { get; set; }
        public string PurchaseReceive { get; set; }
        public string PRODUCT_NAME { get; set; }
        public decimal SAL_PRICE { get; set; }
        public decimal IS_MANUFACTURE_DT_REQUIRED { get; set; }
        public decimal IS_EXPIRY_ITEM { get; set; }
        public string BARCODE_INCLUDE { get; set; }
        public bool is_gift { get; set; }
        public decimal IS_GIFT { get; set; }
        public decimal RCV_QTY { get; set; }
        public decimal ORDER_QTY { get; set; }
        public string PUR_UOM_NAME { get; set; }
        public decimal REM_QTY { get; set; }
        public bool selected { get; set; }
        public string VENDOR_CODE { get; set; }
        public string VENDOR_NAME { get; set; }
        public string ORDER_NO { get; set; }
        public decimal RCV_VAT { get; set; }
        public decimal RCV_AMOUNT { get; set; }
        public string NAME { get; set; }
        public string BRAND_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string SUB_CATEGORY_NAME { get; set; }
        public string REF_NO { get; set; }
        public decimal DEL_QTY { get; set; }
        public decimal SAL_BAL_QTY { get; set; }
        public string SAL_UOM_NAME { get; set; }
        public int GIFT { get; set; }
        public decimal SL { get; set; }
        public bool IS_CARTON { get; set; }
        public string DELIVERY_TO { get; set; }
        public string DELIVERY_NAME { get; set; }
        public decimal CPU { get; set; }
        public string REGIONAL_NAME { get; set; }
        public decimal PRODUCT_SERIAL { get; set; }
        public decimal BARCODE_FACTOR { get; set; }
        public decimal RCV_PRICE { get; set; }
        public decimal PUR_VAT_PERCENT { get; set; }


    }

    public class DataRoot_DecathlonTransfer
    {
        public string status { get; set; }
        public string message { get; set; }
        public string data { get; set; }

        public string CHALLAN_NO { get; set; }
        public DateTime DELIVERY_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string STORE_CODE { get; set; }
        public string DELIVERY_TO { get; set; }
        public string BARCODE { get; set; }
        public decimal CPU { get; set; }
        public decimal RPU { get; set; }
        public decimal DEL_QTY { get; set; }
        public decimal TotalSale { get; set; }
        public decimal TotalCost { get; set; }
        public string ENTRY_BY { get; set; }
        public DateTime ENTRY_DATE { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string StoreDelivery { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string SAL_BARCODE { get; set; }
        public decimal SAL_PRICE { get; set; }
        public decimal RCV_QTY { get; set; }
        public DateTime RECEIVE_DATE { get; set; }
        public string USER_BARCODE { get; set; }
        public string IS_GIFT { get; set; }
        public string SAL_UOM_NAME { get; set; }
        public decimal SL { get; set; }
        public bool IS_CARTON { get; set; }
        public decimal DISC_PRCNT { get; set; }
        public string REGIONAL_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string VENDOR_CODE { get; set; }
        public string VENDOR_NAME { get; set; }
        public decimal DML_QTY { get; set; }
        public decimal USEABLE_QTY { get; set; }
        public decimal PUR_RTN_QTY { get; set; }
        public string VendorList { get; set; }
        public string VEHICLE_NO { get; set; }
    }

    public class DecathlonParam
    {
        public string DepotId { get; set; }//= "4000";
        public string CompanyId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string ApiKey { get; set; }
        ////////public string ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";

    }

    public class DataRoot_DecathlonProduct
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Datum> Data { get; set; }
        public int COUNT { get; set; }
        public int CODE { get; set; }
    }

    public class Datum
    {
        public decimal SKU { get; set; }
        public string ITEM_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BARCODE { get; set; }
        public string SBARCODE { get; set; }
        //public string USER_BARCODE { get; set; }
        //public string USER_BARCODE1 { get; set; }
        //public string USER_BARCODE2 { get; set; }
        //public string USER_BARCODE3 { get; set; }
        public string NAME { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string SUB_CATEGORY_CODE { get; set; }
        public string SUB_CATEGORY_NAME { get; set; }
        public string BRAND_CODE { get; set; }
        public string BRAND_NAME { get; set; }
        public string COUNTRY_OF_ORGIN { get; set; }
        public decimal PUR_UOM { get; set; }
        public string PUR_UOM_NAME { get; set; }
        public decimal SALE_UOM { get; set; }
        public string SAL_UOM_NAME { get; set; }
        public decimal CONVERTION_RATE { get; set; }
        public decimal IS_SCALE { get; set; }
        public decimal PUR_VAT_PERCENT { get; set; }
        public decimal SAL_VAT_PERCENT { get; set; }
        public decimal IS_PRICE_INCLD_VAT { get; set; }
        public bool IS_NEGATIVE { get; set; }
        public decimal MRP { get; set; }
        public decimal WSP { get; set; }
        public decimal PRICE1 { get; set; }
        public decimal PRICE2 { get; set; }
        public decimal PRICE3 { get; set; }
        public decimal PRICE4 { get; set; }
        public decimal MRP_VAT { get; set; }
        public decimal WSP_VAT { get; set; }
        public decimal PRICE1_VAT { get; set; }
        public decimal PRICE2_VAT { get; set; }
        public decimal PRICE3_VAT { get; set; }
        public decimal PRICE4_VAT { get; set; }
        public decimal MRP_INCLD_VAT { get; set; }
        public decimal WSP_INCLD_VAT { get; set; }
        public decimal PRICE1_INCLD_VAT { get; set; }
        public decimal PRICE2_INCLD_VAT { get; set; }
        public decimal PRICE3_INCLD_VAT { get; set; }
        public decimal PRICE4_INCLD_VAT { get; set; }
        public decimal MRP_EXCLD_VAT { get; set; }
        public decimal WSP_EXCLD_VAT { get; set; }
        public decimal PRICE1_EXCLD_VAT { get; set; }
        public decimal PRICE2_EXCLD_VAT { get; set; }
        public decimal PRICE3_EXCLD_VAT { get; set; }
        public decimal PRICE4_EXCLD_VAT { get; set; }
        public decimal IS_MANAGE_STOCK { get; set; }
        public decimal MIN_STK_QTY { get; set; }
        public decimal REORDER_QTY { get; set; }
        public decimal OPENING_STK { get; set; }
        public decimal LAST_PUR_PRICE { get; set; }
        public decimal IS_EXPIRY_ITEM { get; set; }
        public decimal IS_NOT_FOR_SALE { get; set; }
        public decimal IS_SHOP_REQ { get; set; }
        public decimal HAS_BOM { get; set; }
        public decimal IS_ACTIVE { get; set; }
        public string DESCRIPTION { get; set; }
        public string IMAGE_URL { get; set; }
        public string PARENT_PRODUCT { get; set; }
        //public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        //public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        //public string SUB_PRODUCT_DESCRIPTION { get; set; }
        //public List<string> ProductAttributeList { get; set; }
        //public string SubProductList { get; set; }
        public string DISPLAY_NAME { get; set; }
        //public List<string> UserBarcodeList { get; set; }
        public decimal BAL_QTY { get; set; }
        public decimal SAL_BAL_QTY { get; set; }
        public decimal SAL_PRICE { get; set; }
        //public string SAL_BARCODE { get; set; }
        //public string PACK_SIZE { get; set; }
        public string VENDOR_CODE { get; set; }
        public decimal IS_MANUFACTURE_DT_REQUIRED { get; set; }
        public string BARCODE_INCLUDE { get; set; }
        public decimal ORDER_QTY { get; set; }
        public decimal QUEUE_QTY { get; set; }
        public decimal PUR_PRICE { get; set; }
        public decimal FREE_QTY { get; set; }
        //public string ProductBarcodeList { get; set; }
        public List<ProductVendorList> ProductVendorList { get; set; }
        public bool is_gift { get; set; }
        public bool selected { get; set; }
        public decimal RCV_QTY { get; set; }
        public decimal REM_QTY { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public DateTime MANUFACTURE_DATE { get; set; }
        public decimal RCV_VAT { get; set; }
        public decimal RCV_AMOUNT { get; set; }
        //public string REF_NO { get; set; }
        //public string ProductPriceList { get; set; }
        //public string PRD_ATT { get; set; }
        public int COUNT { get; set; }
        public string VENDOR_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public bool HAS_CARTON { get; set; }
        public decimal CARTON_SIZE { get; set; }
        public decimal CARTON_CPU { get; set; }
        public decimal CARTON_MRP { get; set; }
        //public string CARTON_BARCODE { get; set; }
        public bool IS_CARTON { get; set; }
        //public string CARTON_USER_BARCODE { get; set; }
        public decimal WH_BAL_QTY { get; set; }
        public decimal ST_BAL_QTY { get; set; }
        public decimal LAST_PO_QTY { get; set; }
        public decimal LAST_PUR_QTY { get; set; }
        public decimal LastMonthSale { get; set; }
        public decimal LastWeekSale { get; set; }
        public string REGIONAL_NAME { get; set; }
        public string BARCODE_NAME { get; set; }
        //public string DELETED_BY { get; set; }
        public DateTime DELETED_DATE { get; set; }
        //public string ChildProductList { get; set; }
        //public string SelectedVarianceList { get; set; }
        public bool IS_TRACK_RCV_SERIAL { get; set; }
        public bool IS_CONVERTABLE { get; set; }
        public string COMPANY_NAME { get; set; }
        //public List<string> ConvertionList { get; set; }
        public decimal SD_PERCENT { get; set; }
        public bool ZERO_PRICE_SAL { get; set; }
        //public List<string> ProductCartonList { get; set; }
        public bool IS_PUR_PRICE_INCLD_VAT { get; set; }
        public decimal RCV_PRICE { get; set; }
        public bool IS_BARCODE_PRICE_SAL { get; set; }
        public string PRD_INV_NAME { get; set; }
        public bool IS_DISPLAY_SALE_BUTTON { get; set; }
        //public string STORE_CODE { get; set; }
        public decimal WEIGHT { get; set; }
        public string LIFE_STAGE_CODE { get; set; }
    }

    public class ProductVendorList
    {
        public string COMPANY_CODE { get; set; }
        public string BARCODE { get; set; }
        public string VENDOR_CODE { get; set; }
        public string VENDOR_NAME { get; set; }
        //public string ENTRY_BY { get; set; }
        public DateTime ENTRY_DATE { get; set; }
        public string Product { get; set; }
        public string PAYMENT_METHOD { get; set; }
        public decimal IS_MANAGE_STOCK { get; set; }
    }
}
