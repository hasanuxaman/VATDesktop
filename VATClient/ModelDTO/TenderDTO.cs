using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
    public class TenderDTO
    {
        public string TenderId { get; set; }
        public string RefNo { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string TenderDate { get; set; }
        public string DeleveryDate { get; set; }
        public string Comments { get; set; }

    }
    public class TenderDetailDTO
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

    }

    public class TenderDetailMiniDTO
    {
        public string ItemNo { get; set; }  //all
        public string ProductName { get; set; } //all
        public string ProductCode { get; set; } //all
        public string UOM { get; set; } //all

        //public string HSCodeNo { get; set; } //all
        public decimal TenderPrice { get; set; } //all
        public decimal Stock { get; set; } //all


    }

}
