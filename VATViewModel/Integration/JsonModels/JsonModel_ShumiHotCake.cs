using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.JsonModels
{
    public class JsonModel_ShumiHotCake
    {

        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        public int OutletCode { get; set; }
        public string OutletName { get; set; }
        public string TicketNumber { get; set; }
        public DateTime OrderCreatedDateTime { get; set; }
        public int MenuItemCode { get; set; }
        public string MenuItemName { get; set; }
        public string UOM { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Sales { get; set; }
        public decimal Discount { get; set; }
        public decimal SDPerc { get; set; }
        public decimal SD { get; set; }
        public decimal VatFivePerc { get; set; }
        public decimal VatFive { get; set; }
        public decimal VatFifteenPerc { get; set; }
        public decimal VatFifteen { get; set; }
        public decimal AutoRoundMinus { get; set; }
        public decimal AutoRoundPlus { get; set; }
        public decimal Cash { get; set; }
        public decimal VisaCard { get; set; }
        public decimal bKash { get; set; }
        public decimal Total { get; set; }
        public decimal? Void { get; set; }
        public decimal? Gift { get; set; }
        public decimal NetSales { get; set; }


    }
}
