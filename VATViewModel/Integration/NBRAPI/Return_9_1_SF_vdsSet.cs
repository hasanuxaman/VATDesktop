using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.NBRAPI
{
    public class Return_9_1_SF_vdsSet
    {
        public string MSGID { get; set; }
        public string FieldID { get; set; }
        public string BuyerSupplyerBIN { get; set; }
        public string BuyerSupplyerName { get; set; }
        public string BuyerSupplyerAddress { get; set; }
        public string Value { get; set; }
        public string DeductedVAT { get; set; }
        public string InvoiceNoChallanBillNo { get; set; }
        public string InvoiceChallanBillDate { get; set; }
        public string VATDeductionatSource { get; set; }
        public string VATDeductionatSourceCerDate { get; set; }
        public string TaxDepositedSerialBookTransfer { get; set; }
        public string TaxDepositedDate { get; set; }
        public string Notes { get; set; }

    }
}
