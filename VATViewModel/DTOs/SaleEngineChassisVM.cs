using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class SaleEngineChassisVM
    {

        public string ProductCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductName { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string CompanyCode { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string PSalesInvoiceNo { get; set; }
        public string TransactionType { get; set; }
        public string ItemNo { get; set; }
        public string BranchId { get; set; }
        public List<SaleEngineChassisDetailsVM> IDs { get; set; }
    }

   public class SaleEngineChassisDetailsVM
   {

       public string EngineNumber { get; set; }
       public string ChassisNumber { get; set; }
       public string SalesInvoiceNo { get; set; }
       public string ItemNo { get; set; }
       public string TransactionType { get; set; }
       public string ProductCode { get; set; }

   }


    public class SaleEngineDto
    {
        public List<SaleEngineChassisDetailsVM> IDs { get; set; }

    }
}
