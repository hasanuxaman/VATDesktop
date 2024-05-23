using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VATViewModel.Integration.JsonModels
{
    
    public class Customer
    {
        public string customer_code { get; set; }
        public string address_line1 { get; set; }
        public string company_name { get; set; }
        public decimal total_due { get; set; }
        public decimal total_od { get; set; }
    }

    public class Location
    {
        public int id { get; set; }
        public string location_name { get; set; }
        public string delivery_address { get; set; }
        public string name { get; set; }

    }

    public class FromLocation
    {
        public int id { get; set; }
        public string location_name { get; set; }
        public string delivery_address { get; set; }
    }

    public class ToLocation
    {
        public int id { get; set; }
        public string location_name { get; set; }
        public string delivery_address { get; set; }
    }

    public class Unit
    {
        public string name { get; set; }
    }

    public class Product
    {
        public string product_code { get; set; }
        public string name { get; set; }
        public decimal packet_size { get; set; }
        public Unit unit { get; set; }

        public string code { get; set; }
        //public string unit { get; set; }
        public int packSize { get; set; }
        public string type { get; set; }

        public string qty { get; set; }

    }

    public class StockTransferLine
    {
        public Product product { get; set; }
        public decimal quantity { get; set; }
        public decimal? receive_quantity { get; set; }
        public string description { get; set; }
    }

    public class OrderLine
    {
        public int id { get; set; }
        public decimal quantity { get; set; }
        public decimal rate { get; set; }
        public decimal discount { get; set; }
        public int foc { get; set; }
        public decimal total_price { get; set; }
        public Product product { get; set; }
        public decimal vat { get; set; }
        public decimal gAmt { get; set; }
        public decimal sd_amount { get; set; }
    }

    public class Order
    {
        public int id { get; set; }
        public int invoice_no { get; set; }
        public DateTime invoice_date { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public DateTime approved_at { get; set; }
        public Customer customer { get; set; }
        public Location location { get; set; }
        public List<OrderLine> order_lines { get; set; }
    }

    public class SaleReturnLine
    {
        public int id { get; set; }
        public decimal quantity { get; set; }
        public decimal rate { get; set; }
        public Product product { get; set; }
        public decimal discount { get; set; }
    }

    public class Purchase
    {
        public int id { get; set; }
        public string purchase_no { get; set; }
        public DateTime purchase_date { get; set; }
        public Customer customer { get; set; }
    }
    public class Requisition
    {
        public Location location { get; set; }
    }
    public class RequisitionLine
    {
        public Requisition requisition { get; set; }
        public Product product { get; set; }
    }
    public class PurchaseLine
    {
        public RequisitionLine requisition_line { get; set; }
        public decimal rate { get; set; }
        public decimal? discount { get; set; }
        public decimal? vat_percent { get; set; }
    }
    public class PurchaseReceiveLine
    {
        public PurchaseLine purchase_line { get; set; }
        public decimal receive_quantity { get; set; }
        public decimal chalan_quantity { get; set; }
        public decimal? invoice_discount { get; set; }
        public string chalan_no { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string invoice_no { get; set; }
        public DateTime invoice_date { get; set; }
        public string description { get; set; }
        public string due_date { get; set; }
        public int purchaser_id { get; set; }
        public string purchaser_mobile_no { get; set; }
        public string territory { get; set; }
        public string terms { get; set; }
        public string officer_name { get; set; }
        public string officer_mobile { get; set; }
        public string depot_name { get; set; }

        public string status { get; set; }
        public DateTime approved_at { get; set; }
        public Customer customer { get; set; }
        public Location location { get; set; }
        public List<OrderLine> order_lines { get; set; }

        public Company company { get; set; }
        public Order order { get; set; }
        public List<SaleReturnLine> sale_return_lines { get; set; }

        public Product product { get; set; }
        public List<ProductionPlanningLine> production_planning_lines { get; set; }
        public List<object> prod_plan_sub_products { get; set; }
        public decimal? quantity { get; set; }
        public List<ProductionPlanningPackagingLine> production_planning_packaging_lines { get; set; }
        public decimal? receive_quantity { get; set; }
        public ProductionPlanning production_planning { get; set; }

        public Purchase purchase { get; set; }
        public DateTime qc_approved_at { get; set; }
        public DateTime invoice_at { get; set; }
        public string remark { get; set; }
        public List<PurchaseReceiveLine> purchase_receive_lines { get; set; }

        public CommercialInvoice commercial_invoice { get; set; }
        public List<LcProductLine> lc_product_lines { get; set; }

        public List<LcClearingAndForwardingLine> lc_clearing_and_forwarding_lines { get; set; }

        ////public DateTime updated_at { get; set; }
        //public List<StockTransferLine> stock_transfer_lines { get; set; }

        public int planningId { get; set; }
        public int rcvId { get; set; }
        public ProductInfo product_info { get; set; }
        public decimal? rcv_qty { get; set; }
        public ReceivedAt received_at { get; set; }
        public decimal rate { get; set; }


        public int stockTransferId { get; set; }
        public FromLocation from_location { get; set; }
        public ToLocation to_location { get; set; }
        public string qty { get; set; }
        public TransferredAt transferred_at { get; set; }

        public Date date { get; set; }
        public List<ProductWrapper> products { get; set; }


    }

    public class ProductWrapper
    {
        public Product product { get; set; }
        public string qty { get; set; }

    }

    public class Date
    {
        public string date { get; set; }
        public int timezone_type { get; set; }
        public string timezone { get; set; }
    }

    public class TransferredAt
    {
        public string date { get; set; }
        public int timezone_type { get; set; }
        public string timezone { get; set; }
    }

    public class ProductionPlanningLine
    {
        public int id { get; set; }
        public Product product { get; set; }
        public decimal quantity { get; set; }
        public decimal final_quantity { get; set; }
        public BomLine bom_line { get; set; }
    }

    public class ProductionPlanningPackagingLine
    {
        public int id { get; set; }
        public decimal quantity { get; set; }
        public decimal per_quantity { get; set; }
        public Product product { get; set; }
        public FinishedProduct finished_product { get; set; }
    }

    public class ProductionPlanning
    {
        public int id { get; set; }
        public Company company { get; set; }
        public Location location { get; set; }
        public Product product { get; set; }
        public List<ProductionPlanningLine> production_planning_lines { get; set; }
        public List<object> prod_plan_sub_products { get; set; }
        public decimal? quantity { get; set; }
        public List<object> production_planning_packaging_lines { get; set; }
        public decimal? receive_quantity { get; set; }
        public int status { get; set; }
        public DateTime approved_at { get; set; }
    }

    public class FinishedProduct
    {
        public string product_code { get; set; }
        public string name { get; set; }
        public int packet_size { get; set; }
        public Unit unit { get; set; }
    }

    public class BomLine
    {
        public int id { get; set; }
        public decimal quantity { get; set; }
        public int type { get; set; }
        public int potency_type { get; set; }
        public Product product { get; set; }
        public decimal per_quantity { get; set; }
    }
    public class LcInformation
    {
        public string lc_no { get; set; }
    }
   
    public class CommercialInvoice
    {
        public int id { get; set; }

        public LcInformation lc_information { get; set; }
    }
    public class LcProductLine
    {
        public int id { get; set; }
        public decimal quantity { get; set; }
        public Product product { get; set; }
    }

    public class Pagination
    {
        public string self { get; set; }
        public int first { get; set; }
        public int last { get; set; }
        public int next { get; set; }
    }



    public class DataRoot
    {
        public List<Item> items { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public Pagination pagination { get; set; }

        public List<List> list { get; set; }
    }

    public class CostType
    {
    }

    public class CostHead
    {
        public int id { get; set; }
        public string cost_name { get; set; }
        public CostType cost_type { get; set; }
    }
    public class LcClearingAndForwardingLine
    {
        public int id { get; set; }
        public double cost_amount { get; set; }
        public CostHead cost_head { get; set; }
    }
   


    public class AuthCredentials
    {
        [JsonProperty("_username")]
        public string UserName { get; set; }

        [JsonProperty("_password")]
        public string Password { get; set; }


        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Company
    {
        public int id { get; set; }
        public string name { get; set; }

        //public int id { get; set; }
        public string company_name { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public string file_number { get; set; }
        public DateTime created_at { get; set; }
        public string users { get; set; }
        public string group { get; set; }
        public string stock_transfer_from_ledger { get; set; }
        public string stock_transfer_to_ledger { get; set; }
    }

    public class AuthRoot
    {
        public string token { get; set; }
        public List<Company> company { get; set; }
        public string user { get; set; }
    }


    //////public class Root
    //////{
    //////    public List<List> list { get; set; }
    //////    public int count { get; set; }
    //////}

    public class List
    {
        public int planningId { get; set; }
        public int rcvId { get; set; }
        public Location location { get; set; }
        public ProductInfo product_info { get; set; }
        public decimal rcv_qty { get; set; }
        public ReceivedAt received_at { get; set; }
        public decimal rate { get; set; }

        public int id { get; set; }
        public string customer_code { get; set; }
        public string salutation { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string primary_mobile { get; set; }
        public string primary_telephone { get; set; }
        public string primary_fax { get; set; }
        public string email { get; set; }
        public bool status { get; set; }
        public MarketOrganogram market_organogram { get; set; }
        public CustomerType customer_type { get; set; }
        public int? credit_limit { get; set; }
        public string supplier_credit_days { get; set; }
        public Employee employee { get; set; }
        public string company_name { get; set; }
        public string nation_id { get; set; }
        public string trade_license { get; set; }
        public bool? agreement { get; set; }
        public bool? micr_cheque { get; set; }
        public string dealer_type { get; set; }
        public string file_no { get; set; }
        public string pesticide_license { get; set; }
        public string check_amount { get; set; }
        public string branch { get; set; }
        public string bank { get; set; }
        public string cheque_no { get; set; }
        public bool restrict_invoice { get; set; }
        public string restrict_advance { get; set; }
        public InvoiceType invoice_type { get; set; }
        //public string chain_shop { get; set; }
        //public string remark { get; set; }
        //public string customer_grade { get; set; }
        //public string cash_percentage { get; set; }
        //public string created_at { get; set; }
        //public string created_by { get; set; }
        //public DateTime? updated_at { get; set; }
        //public UpdatedBy updated_by { get; set; }
        //public bool cash_customer { get; set; }
        public string vat_reg_no { get; set; }
        public object tin_no { get; set; }
        public object pesticide_license_date_of_validity { get; set; }
        public object trade_license_date_of_validity { get; set; }
        public object inter_company { get; set; }
    }

    public class ReceivedAt
    {
        public string date { get; set; }
        public int timezone_type { get; set; }
        public string timezone { get; set; }
    }

    public class ProductInfo
    {
        public string code { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        //public decimal? packSize { get; set; }
        public string type { get; set; }
    }

    //public class Company
    //{
    //    public int id { get; set; }
    //    public string company_name { get; set; }
    //    public object description { get; set; }
    //    public string address { get; set; }
    //    public object file_number { get; set; }
    //    public DateTime created_at { get; set; }
    //    public object users { get; set; }
    //    public object group { get; set; }
    //    public object stock_transfer_from_ledger { get; set; }
    //    public object stock_transfer_to_ledger { get; set; }
    //}

    public class CustomerType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Employee
    {
        public int id { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string mobile { get; set; }
        public string telephone { get; set; }
        public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
    }

    public class InvoiceType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class MarketOrganogram
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool status { get; set; }
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        public Company company { get; set; }
        public Organogram organogram { get; set; }
        public string diff { get; set; }
        //public string employee { get; set; }
        //public int credit_limit { get; set; }
    }

    public class Organogram
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool status { get; set; }
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        public string company { get; set; }
        public string organogram { get; set; }
        public string diff { get; set; }
        //public string employee { get; set; }
        public int? credit_limit { get; set; }
    }
    public class UpdatedBy
    {
    }



   
}
