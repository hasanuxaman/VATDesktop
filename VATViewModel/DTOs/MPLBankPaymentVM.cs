using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{

    public class MPLBankPaymentReceiveVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int BDBankId { get; set; }
        public string Code { get; set; }
        public string CustomerId { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public string Post { get; set; }
        public decimal Amount { get; set; }
        public bool IsUsed { get; set; }
        public bool IsUsedDS { get; set; }
        public string TransactionDateTime { get; set; }
        public string Operation { get; set; }
        public string BDBankName { get; set; }
        public string BDBankCode { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }
        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }
        public string SalesInvoiceRefId { get; set; }
        public decimal TotalPaymentAmnt { get; set; }

        public List<string> IDs { get; set; }

        public List<MPLBankPaymentReceiveDetailsVM> BankPaymentReceiveDetailsVMs { get; set; }
        
        public List<BranchProfileVM> BranchList { get; set; }
    }
    public class MPLBankPaymentReceiveDetailsVM
    {
        public int Id { get; set; }
        public int BankPaymentReceiveId { get; set; }
        public int BranchId { get; set; }
        public int BDBankId { get; set; }
        public string CustomerId { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsUsed { get; set; }
        public bool IsUsedDS { get; set; }
        public string TransactionDateTime { get; set; }
        public string BDBankName { get; set; }
        public string BDBankCode { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string Post { get; set; }

        public string SalesInvoiceRefId { get; set; }



        public List<BranchProfileVM> BranchList { get; set; }
    }
    public class MPLSalesInvoiceBankPaymentVM
    {
        public int Id { get; set; }
        public int SalesInvoiceMPLHeaderId { get; set; }
        public int BranchId { get; set; }
        public int BankId { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsUsedDS { get; set; }

        public string IDs { get; set; }
        public string SelfBankName { get; set; }


    }

}

