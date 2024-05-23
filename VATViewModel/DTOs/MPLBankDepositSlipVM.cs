using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{

    public class MPLBankDepositSlipHeaderVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }
        public int SelfBankId { get; set; }
        public string TransactionDateTime { get; set; }
        public string TransactionType { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public string IDs { get; set; }
        public string Operation { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string SelfBankName { get; set; }
        public string BranchName { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }

        public int? BDBankId { get; set; }
        public string SalesInvoiceNo { get; set; }
        public int? SalesInvoiceMPLHeaderId { get; set; }
        public int? MPLBankPaymentReceivesId { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public string BankCode { get; set; }
        public string CustomerName { get; set; }
        public string TType { get; set; }
        public int RefId { get; set; }
        public string Post { get; set; }

        public List<MPLBankDepositSlipDetailVM> MPLBankDepositSlipDetailVMs { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }
        public string BankSlipType { get; set; }



        
    }

    public class MPLBankDepositSlipDetailVM
    {
        public int Id { get; set; }
        public int BankDepositSlipHeaderId { get; set; }
        public int BranchId { get; set; }
        public int BDBankId { get; set; }
        public int SalesInvoiceMPLHeaderId { get; set; }
        public int MPLBankPaymentReceivesId { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public decimal Amount { get; set; }


        public bool IsUsedDS { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string BDBankName { get; set; }
        public string BankCode { get; set; }
        public string CustomerName { get; set; }

        public string TType { get; set; }
        public int RefId { get; set; }

    }

}

