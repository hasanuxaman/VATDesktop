using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class ProductStockVM
    {

       public int BranchId { get; set; }
       public int StockId { get; set; }
       public string ItemNo { get; set; }
       public decimal StockQuantity { get; set; }
       public decimal StockValue { get; set; }
       public decimal CurrentStock { get; set; }
       public string Comments { get; set; }

       public decimal WastageTotalQuantity { get; set; }
       public string BranchName { get; set; }



    }

   public class CustomerRateVM
   {

       public int BranchId { get; set; }
       //public int StockId { get; set; }
       public string CustomerId { get; set; }
       public string ItemNo { get; set; }
       public decimal TollCharge { get; set; }
       public decimal NBRPrice { get; set; }
      // public string Comments { get; set; }

     //  public decimal WastageTotalQuantity { get; set; }


   }
}
