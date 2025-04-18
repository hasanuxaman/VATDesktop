﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class VDSMasterVM
    {
        public string Id { get; set; }
        public string VendorId { get; set; }
        public string VDSId { get; set; }
        public string VendorName { get; set; }

        public Decimal BillAmount { get; set; }
        public string BillDate { get; set; }

        public Decimal BillDeductedAmount { get; set; }
        public string DepositNumber { get; set; }
        public string DepositDate { get; set; }
        public string IssueDate { get; set; }
        public string PaymentDate { get; set; }
        public string Remarks { get; set; }
        public string PurchaseNumber { get; set; }

        public decimal VDSPercent { get; set; }
        public decimal VDSAmount { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string IsPercent { get; set; }
        public string IsPurchase { get; set; }
        public string ReverseVDSId { get; set; }
        public int BranchId { get; set; }

        public string BillNo { get; set; }
        public string Email { get; set; }

        public decimal VATAmount { get; set; }

        public string IsMailSend { get; set; }
    }


   //public class VDSMasterVM
   // {
   //     public string Id { get; set; }
   //     public string VendorId { get; set; }
   //     public Decimal BillAmount { get; set; }
   //     public string BillDate { get; set; }

   //     /// <summary>
   //     /// VDSAmount as previous
   //     /// </summary>
   //     public Decimal BillDeductedAmount { get; set; }
   //     public string DepositNumber { get; set; }
   //     public string DepositDate { get; set; }
   //     public string IssueDate { get; set; }
   //     public string Remarks { get; set; }
   //     public string PurchaseNumber { get; set; }
   //     public decimal VDSPercent { get; set; }
   //     public string CreatedBy { get; set; }
   //     public string CreatedOn { get; set; }
   //     public string LastModifiedBy { get; set; }
   //     public string LastModifiedOn { get; set; }
   //     public string IsPercent { get; set; }
   //     public string IsPurchase { get; set; }
   // }
}
