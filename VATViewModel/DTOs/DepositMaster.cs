﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{
   //public class DepositMasterVM
   // {

   //     public string Id { get; set; }
   //     public string TreasuryNo { get; set; }
   //     public string DepositDate { get; set; }
   //     public string DepositType { get; set; }
   //     public decimal DepositAmount { get; set; }
   //     public string ChequeNo { get; set; }
   //     public string ChequeBank { get; set; }
   //     public string ChequeBankBranch { get; set; }
   //     public string ChequeDate { get; set; }
   //     public string BankId { get; set; }
   //     public string TreasuryCopy { get; set; }
   //     public string DepositPerson { get; set; }
   //     public string DepositPersonDesignation { get; set; }

   //     public string DepositPersonContactNo { get; set; }
   //     public string DepositPersonAddress { get; set; }

   //     public string Comments { get; set; }
   //     public string CreatedBy { get; set; }
   //     public string CreatedOn { get; set; }
   //     public string LastModifiedBy { get; set; }
   //     public string LastModifiedOn { get; set; }
   //     public string Info2 { get; set; }
   //     public string Info3 { get; set; }
   //     public string Info4 { get; set; }
   //     public string Info5 { get; set; }
   //     public string TransactionType { get; set; }
   //     public string Post { get; set; }
   //     public string ReturnID { get; set; }


   // }
    public class DepositMasterVM
    {
        public string Id { get; set; }

        public string IsPurchase { get; set; }

        //[Display(Name="Id")]
        public string DepositId { get; set; }
        //[Display(Name = "Treasury No")]
        public string TreasuryNo { get; set; }
        //[Display(Name = "Deposit Date")]
        public string DepositDate { get; set; }
        public string BankDepositDate { get; set; }
        //[Display(Name = "Deposit Type")]
        public string DepositType { get; set; }
        //[Display(Name = "Deposit Amount")]
        public decimal DepositAmount { get; set; }
        //[Display(Name = "Checque No")]
        public string ChequeNo { get; set; }
        //[Display(Name = "Cheque Bank")]
        public string ChequeBank { get; set; }
        public string ChequeBankBranch { get; set; }
        //[Display(Name = "Cheque Date")]
        public string ChequeDate { get; set; }
        public string BankId { get; set; }
        public string TreasuryCopy { get; set; }
        //[Display(Name = "Deposit Person")]
        public string DepositPerson { get; set; }
        //[Display(Name = "Deposit Person Designation")]

        public string DepositPersonDesignation { get; set; }
        public string DepositPersonContactNo { get; set; }
        public string DepositPersonAddress { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string ReturnID { get; set; }
        public string ReverseDepositId { get; set; }
        public int BranchId { get; set; }
        public bool ExportAll { get; set; }

        public string ImportID { get; set; }


        //[Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public string District { get; set; }
        public string BranchName { get; set; }
        //[Display(Name = "Account No.")]
        public string AccountNumber { get; set; }


        public string Operation { get; set; }
        //[Display(Name = "Deposit Date From")]
        public string IssueDateTimeFrom { get; set; }
        //[Display(Name = "Deposit Date To")]
        public string IssueDateTimeTo { get; set; }
        public List<VDSMasterVM> Details { get; set; }
        //[Display(Name = "Cheque Date From")]
        public string CheckDateFrom { get; set; }
        //[Display(Name = "Cheque Date To")]
        public string CheckDateTo { get; set; }
        public bool SingleTR6 { get; set; }

        public List<string> IDs { get; set; }
        public string CurrentUser { get; set; }


        //JBR////
        public string SelectTop { get; set; }
        public string VendorGroup { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string BranchCode { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }

        public string TreasuryN { get; set; }


    }
}
