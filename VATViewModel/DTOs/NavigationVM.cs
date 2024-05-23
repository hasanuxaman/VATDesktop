using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class NavigationVM
    {

        public string ButtonName { get; set; }
        public string ItemNo { get; set; }
        public string Code { get; set; }

        public string TransactionType { get; set; }

        public int FiscalYear { get; set; }
        public int BranchId { get; set; }

        public int Id { get; set; }
        public string InvoiceNo { get; set; }


        public string ProductType { get; set; }
    }
}
