using System.Collections.Generic;

namespace VATViewModel.DTOs
{
    public class PrinterParam
    {
        public PrinterParam()
        {

            PrintCopy = 1;
        }

        public string SalesInvoiceNo { get; set; }

        public string SalesId { get; set; }

        public string PrinterName { get; set; }

        public string LoginInfo { get; set; }

        public List<string> IDs { get; set; }
        public int PrintCopy { get; set; }


    }
}