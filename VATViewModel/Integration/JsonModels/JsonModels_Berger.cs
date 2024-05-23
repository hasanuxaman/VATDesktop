using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.JsonModels
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }

    public class Result
    {
        ////public Metadata __metadata { get; set; }
        public string Company { get; set; }
        public DateTime BillingDate { get; set; }
        public string OrderDoc { get; set; }
        public string DeliveryDoc { get; set; }
        public string BillingDoc { get; set; }
        public string AccountGroup { get; set; }
        public string BusArea { get; set; }
        public string BillingTime { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string fkimg_meins { get; set; }
        public string BillQty { get; set; }
        public string UoM { get; set; }
        public string waerk { get; set; }
        public string Revenue { get; set; }
        public string UnitPrice { get; set; }
        public string VATAmount { get; set; }
        public string VatPct { get; set; }
        public string SdAmount { get; set; }
        public string SdPct { get; set; }
        public string CollTax { get; set; }
        public string DisAmount { get; set; }
        public string StorageLoc { get; set; }
        public string ItemCat { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string MovementType { get; set; }
        public string DeliveryAddress { get; set; }
        public string belnr { get; set; }
        public string gjahr { get; set; }
        public string vkorg { get; set; }
        public string matkl { get; set; }
    }

    public class D
    {
        public List<Result> results { get; set; }
    }

    public class Root
    {
        public D d { get; set; }


        #region Product Receive
        
        public string ID { get; set; }
        public string BRANCHCODE { get; set; }
        public string RECEIVE_DATETIME { get; set; }
        public string REFERENCE_NO { get; set; }
        public string COMMENTS { get; set; }
        public string POST { get; set; }
        public string RETURN_ID { get; set; }
        public string WITH_TOLL { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string QUANTITY { get; set; }
        public string NBR_PRICE { get; set; }
        public string UOM { get; set; }
        public string VAT_NAME { get; set; }
        public string CUSTOMERCODE { get; set; }
        public string PRODUCT_GROUP { get; set; }

        #endregion

        public string TRANS_TO_BRANCH_CODE { get; set; }
        public string VEHICLE_NO { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string MATERIAL_TYPE_DESC { get; set; }

    }



    //public class PReceiveJsonModels_Berger
    //{
    //    public List<Root> Data { get; set; }

    //}

    public class DataRoot_Berger
    {
        public string status { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }

    public class BergerParam
    {
        public string DepotId { get; set; }//= "4000";
        public string CompanyId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string ApiKey { get; set; }
        ////////public string ApiKey = "a9b51bd9a20a478cbb21d5d2c51e00bb";

    }
}
