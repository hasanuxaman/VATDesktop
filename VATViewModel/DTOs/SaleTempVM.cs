using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class SaleTempVM
    {
        //ID	Customer_Name	Customer_Code	Delivery_Address	Invoice_Date_Time	Delivery_Date_Time
        //Item_Code	Item_Name	Quantity	NBR_Price	UOM	Total_Price	VAT_Rate	Vehicle_No	Reference_No
        //Comments	Sale_Type	Previous_Invoice_No	Is_Print	Tender_Id	Post	LC_Number	Currency_Code
        //SD_Rate	Non_Stock	Trading_MarkUp	Type	Discount_Amount	Promotional_Quantity	VAT_Name
        //ExpDescription	ExpQuantity	ExpGrossWeight	ExpNetWeight	ExpNumberFrom	ExpNumberTo	Branch_Code



        public string ID { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_Code { get; set; }
        public string Delivery_Address { get; set; }
        public string Invoice_Date_Time { get; set; }
        public string Delivery_Date_Time { get; set; }
        public string Item_Code { get; set; }
        public string Item_Name { get; set; }
        public string Quantity { get; set; }
        public string NBR_Price { get; set; }
        public string Total_Price { get; set; }
        public string VAT_Rate { get; set; }
        public string Vehicle_No { get; set; }
        public string Reference_No { get; set; }
        public string Comments { get; set; }
        public string UOM { get; set; }
        public string Sale_Type { get; set; }
        public string Previous_Invoice_No { get; set; }
        public string Is_Print { get; set; }
        public string Tender_Id { get; set; }
        public string Post { get; set; }
        public string LC_Number { get; set; }
        public string Currency_Code { get; set; }
        public string SD_Rate { get; set; }
        public string Non_Stock { get; set; }
        public string Trading_MarkUp { get; set; }
        public string Type { get; set; }
        public string Discount_Amount { get; set; }
        public string Promotional_Quantity { get; set; }
        public string VAT_Name { get; set; }
        public string ExpDescription { get; set; }
        public string ExpQuantity { get; set; }
        public string ExpGrossWeight { get; set; }
        public string ExpNetWeight { get; set; }
        public string ExpNumberFrom { get; set; }
        public string ExpNumberTo { get; set; }
        public string Branch_Code { get; set; }

    }
}
