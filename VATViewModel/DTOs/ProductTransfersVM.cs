using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class ProductTransfersVM
    {
      public int Id { get; set; }
      public string TransferCode { get; set; }
      public string TransferDate { get; set; }
      public int BranchId { get; set; }
      public string IsWastage { get; set; }
      public string Post { get; set; }
      public string Comments { get; set; }
      public string ActiveStatus { get; set; }
      public string CreatedBy { get; set; }
      public string CreatedOn { get; set; }
      public string LastModifiedBy { get; set; }
      public string LastModifiedOn { get; set; }
      public List<ProductTransfersDetailVM> Details { get; set; }

      public string TransferDateFrom { get; set; }
      public string TransferDateTo { get; set; }
      public string Operation { get; set; }
      public string TransactionType { get; set; }
       
      public string GetTransactionType()
      {
          if (IsWastage == "Y")
              return "WastageCTC";
          if (IsWastage == "R")
              return "RawCTC";
          if (IsWastage == "F")
              return "FinishCTC";
          throw new Exception("Transfer Type Not Found");
      }
    }

   public class ProductTransfersDetailVM
    {
       public int Id { get; set; }
       public int ProductTransferId { get; set; }
       public int BranchId { get; set; }
       public string FromItemNo { get; set; }
       public string FromUOM { get; set; }
       public Decimal FromQuantity { get; set; }
       public Decimal FromUOMConversion { get; set; }
       public string ToItemNo { get; set; }
       public string ToUOM { get; set; }
       public Decimal ToQuantity { get; set; }
       public string IsWastage { get; set; }
       public string FromItemName { get; set; }
       public string ToItemName { get; set; }
       public string TransferDate { get; set; }
       public string Post { get; set; }

       public string TransactionType { get; set; }

       public decimal IssuePrice { get; set; }
       public decimal ReceivePrice { get; set; }
       public decimal FromUnitPrice { get; set; }
       public decimal ToUnitPrice { get; set; }
       public decimal PackSize { get; set; }

       private decimal GetAvgPrice(decimal quantity, decimal amount)
       {
           if (quantity > 0)
           {
               return amount / quantity;
           }

           return 0;
       }


       public void SetIssuePrice(decimal quantity, decimal amount)
       {
           IssuePrice = GetAvgPrice(quantity, amount);
       }


       public void SetReceivePrice(decimal quantity, decimal amount)
       {
           ReceivePrice = GetAvgPrice(quantity, amount);
       }

       public string GetAvgPriceDate()
       {
           return Convert.ToDateTime(TransferDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
       }
    }
}
