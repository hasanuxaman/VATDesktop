using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class EnumDAL
    {
        #region VAT/VMS

        public static string[] SaleTransactionType = new string[] { 
 "Local"
,"Trading"
,"Export"
,"Tender"
,"Tender(Trading)"
,"Service(Stock)"
,"Service(NonStock)"
,"Package"
,"Wastage"
,"RawSale"
        };

        public IEnumerable<object> GetSaleTransactionTypeList()
        {
            IEnumerable<object> enumList = from e in DepositType select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        public IList<string> SaleTransactionTypeList
        {
            get
            {
                return SaleTransactionType.ToList<string>();

            }
        }



        static string[] DepositType = new string[] { "Cash", "Cheque", "NotDeposited", "BEFTN", "NPSB" };
        public IEnumerable<object> GetDepositTypeList()
        {
            IEnumerable<object> enumList = from e in DepositType select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] TreasuryDepositType = new string[] { "Opening", "OpeningAdjustment", "ClosingBalanceVAT(18.6)", "RequestedAmountForRefundVAT", "Cash", "Cheque", "NotDeposited","BEFTN","NPSB" };
        public IEnumerable<object> GetTreasuryDepositTypeList()
        {
            IEnumerable<object> enumList = from e in TreasuryDepositType select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] SDDepositType = new string[] { "Opening", "OpeningAdjustment", "ClosingBalanceSD(18.6)", "RequestedAmountForRefundSD", "Cash", "Cheque", "NotDeposited", "BEFTN", "NPSB" };
        public IEnumerable<object> GetSDDepositTypeList()
        {
            IEnumerable<object> enumList = from e in SDDepositType select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        //static string[] PurchaseVATType = new string[] { "VAT", "NonVAT", "Exempted", "FixedVAT", "OtherRate", "Truncated", "Turnover", "UnRegister", "NonRebate" };
        static string[] PurchaseVATType = new string[] { "NonVAT","Exempted","VAT","OtherRate","FixedVAT","Ternover","UnRegister","NonRebate","NonVAT-Local","NonVAT-Import",
            "Exempted-Local","Exempted-Import","VAT-Local","VAT-Import","OtherRate-Local","OtherRate-Import","NonRebate-Local","NonRebate-Import" };
        public IEnumerable<object> GetPurchaseVATTypeList()
        {
            IEnumerable<object> enumList = from e in PurchaseVATType select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] PurchaseVATTypeWeb = new string[] { "VAT", "NonVAT", "Exempted", "FixedVAT", "OtherRate", "Truncated", "Turnover", "UnRegister", "NonRebate" };
        public IEnumerable<object> GetPurchaseVATTypeListWeb()
        {
            IEnumerable<object> enumList = from e in PurchaseVATTypeWeb select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] EnumVAT = new string[] { "Select","VAT", "NonVAT", "MRPRate", "FixedVAT", "OtherRate", "Retail", "MRPRate(SC)" };
        public IEnumerable<object> GetEnumVATList()
        {
            IEnumerable<object> enumList = from e in EnumVAT select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        public IEnumerable<object> GetEnumVATExportList()
        {
            string[] EnumVATExport = new string[] { "Export", "DeemExport" };

            IEnumerable<object> enumList = from e in EnumVATExport select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] PriceDeclarations = new string[] { 
              "VAT 4.3"
              ,"VAT 4.3 (Toll Issue)"
              ,"VAT 4.3 (Toll Receive)"
              ,"VAT 4.3 (Tender)"
              ,"VAT 4.3 (Package)"
              ,"VAT 4.3 (Wastage)"
              ,"VAT 4.3 (Internal Issue)"
              ,"VAT Porisisto Ka"
        };

        public IEnumerable<object> GetPriceDeclarationList()
        {
            IEnumerable<object> enumList = from e in PriceDeclarations
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        public IEnumerable<object> RecordSelectList()
        {
            IEnumerable<object> enumList = from e in RecordSelect.vrecordSelect
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] Types = new string[] { "Local", "Import" };
        public IEnumerable<object> GetTypeList()
        {
            IEnumerable<object> enumList = from e in Types
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        #endregion

        #region GL

        static string[] GLStatus = { "Created", "Posted", "Decline", "Rejected", "Approval Completed" };

        public IEnumerable<object> GLStatusList()
        {
            IEnumerable<object> enumList = from e in GLStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] GLDocumentTypes = { "MTR", "MAR", "FIR", "MISC", "MAH", "ENG" };

        public IEnumerable<object> GLDocumentTypeList()
        {
            IEnumerable<object> enumList = from e in GLDocumentTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] GLAccountNature = new string[] { "Dr", "Cr" };

        public IEnumerable<object> GLGetAccountNatureList()
        {
            IEnumerable<object> enumList = from e in GLAccountNature
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #endregion 

        static string[] DeductionState = new string[] { "Absent", "Late In", "Early Out" };
        public IEnumerable<object> GetDeductionStateList()
        {
            IEnumerable<object> enumList = from e in DeductionState
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] Decisions = new string[] { "Y", "N" };
        public IEnumerable<object> DecisionList()
        {
            IEnumerable<object> enumList = from e in Decisions
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] CPCTypes = new string[] { "Sale", "Purchase" };
        public IEnumerable<object> CPCTypeList()
        {
            IEnumerable<object> enumList = from e in CPCTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        #region Acc
        //Acc  
        static string[] PostStatus = new string[] { "Posted", "Not Posted" };

        public IEnumerable<object> GetPostStatusList()
        {
            IEnumerable<object> enumList = from e in PostStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] AccountNature = new string[] { "Dr", "Cr" };

        public IEnumerable<object> GetAccountNatureList()
        {
            IEnumerable<object> enumList = from e in AccountNature
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] ReportType = new string[] { "BalanceSheet", "IncomeStatement" };

        public IEnumerable<object> GetReportTypeList()
        {
            IEnumerable<object> enumList = from e in ReportType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] AccountType = new string[] { "Bank", "Cash", "Other" };

        public IEnumerable<object> GetAccountTypeList()
        {
            IEnumerable<object> enumList = from e in AccountType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] AreaNameTypes = new string[] { "Accounts", "Factory", "Production", "SalePoint" };

        public IEnumerable<object> GetAreaNameList()
        {
            IEnumerable<object> enumList = from e in AreaNameTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        //EnumTransactionType

        static string[] TransactionTypeList = new string[] { "Journal", "Payment", "Collection", "BankDeposit", "Withdraw", "FundTransfer" };

        public IEnumerable<object> GetTransactionTypeList()
        {
            IEnumerable<object> enumList = from e in TransactionTypeList
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] SaleCollectionGroupByTypes = new string[] { "Code", "Date", "Customer", "Pyament Type" };

        public IEnumerable<object> GetSaleCollectionGroupByList()
        {
            IEnumerable<object> enumList = from e in SaleCollectionGroupByTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] NonStockTypes = new string[] { "NonStock", "Stock" };

        public IEnumerable<object> GetNonStockTypeList()
        {
            IEnumerable<object> enumList = from e in NonStockTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] GroupTypes = new string[] { "Local", "Export" };
        public IEnumerable<object> GetGroupTypeList()
        {
            IEnumerable<object> enumList = from e in GroupTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] ProductTypes = new string[] { "Finish", "Non Stock", "Overhead", "Pack", "Raw", "Service", "Trading", "WIP", "Service(NonStock)", "NonInventory" };
        public IEnumerable<object> GetProductTypeList()
        {
            IEnumerable<object> enumList = from e in ProductTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] ProductTransferTypes = new string[] { "Wastage", "Raw", "Finish" };
        public IEnumerable<object> GetProductTransferTypes()
        {
            IEnumerable<object> enumList = from e in ProductTransferTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        #endregion Acc


        #region Other
        
        static string[] DaysOfWeek = new string[] { "Friday", "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
        public IEnumerable<object> DaysOfWeekList()
        {

            IEnumerable<object> enumList = from e in DaysOfWeek
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }

        static string[] VatTypes = new string[] { "VAT 1", "VAT 1 Ka (Tarrif)", "VAT 1 Kha (Trading)", "VAT 1 Ga (Export)", "VAT 1 Gha (Fixed)", "VAT 1(Internal Issue)", "VAT 4.3 (Toll Issue)", "VAT 4.3 (Toll Receive)", "VAT 4.3 (Tender)", "VAT 4.3 (Package)", "VAT 4.3 (Wastage)" };
        public IEnumerable<object> VatTypesList()
        {

            IEnumerable<object> enumList = from e in VatTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }

        static string[] MasterTables = new string[] { "Bank", "Customer", "Products", "Vehicle", "Vendor", "Costing", "UOM", "ProductOpening", "Product" };
        public IEnumerable<object> MasterTableList()
        {

            IEnumerable<object> enumList = from e in MasterTables
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }

        static string[] MPLMasterTables = new string[] { "SA-02", "Transfer"};
        public IEnumerable<object> MPLMasterTableList()
        {

            IEnumerable<object> enumList = from e in MPLMasterTables
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }

        static string[] SaleTypes = new string[] { "Local", "Trading-local", "Trading-Export", "Export", "Tender-Local", "Tender-Export", "Tender(Trading)-local", "Tender(Trading)-Export", "Service(Stock)-Local", "Service(Stock)-Export", "Service(N Stock)-Local", "Service(N Stock)-Export", "Service(N Stock)-Export Credit", "Package-Local", "Package-Export", "Credit", "Debit", "VAT11GaGa" };
        public IEnumerable<object> SaleTypesList()
        {
            IEnumerable<object> enumList = from e in SaleTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] AdjTypes = new string[] { "Cash Payable", "Credit Payable", "Rebatable", "Shortage Rebatable" };
        public IEnumerable<object> AdjTypesList()
        {
            IEnumerable<object> enumList = from e in AdjTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] printers = new string[] { "Send To OneNote 2010", "Microsoft XPS Document Writer", "Fax", "NPI2505B3 (HP LaserJet M402dn) (Copy 1)" };
        public IEnumerable<object> PrinterList()
        {
            IEnumerable<object> enumList = from e in printers
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        //public IEnumerable<object> InsPrinterList()
        //{
        //    List<string> names = new List<string>();
        //    foreach (string printerNames in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
        //    {
        //        names.Add(printerNames);
        //    }
        //    return names;
        //}


        static string[] productTypes = new string[] { "Overhead", "Raw", "Pack", "Finish", "Service", "Trading", "WIP", "Export", "Service(NonStock)", "NonInventory", "Toll" };
        public IEnumerable<object> productTypeList()
        {

            IEnumerable<object> enumList = from e in productTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }

        static string[] EmployeeStatus = new string[] { "Active", "InActive", "All" };

        public IEnumerable<object> GetEmployeeStatus()
        {
            IEnumerable<object> enumList = from e in EmployeeStatus
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] StructureTypes = new string[] { "Employee Group", "Leave Structure", "Salary Structure", "PF Structure"
            , "Tax Structure", "Bonus Structure","Leave Posting"};

        public IEnumerable<object> GetStructureTypeList()
        {
            IEnumerable<object> enumList = from e in StructureTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] absentDeductFrom = new string[] { "Gross", "Basic" };
        public IEnumerable<object> AbsentDeductFromList()
        {

            IEnumerable<object> enumList = from e in absentDeductFrom
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }
        static string[] daysCount = new string[] { "DOM", "30" };

        public IEnumerable<object> DaysCountList()
        {

            IEnumerable<object> enumList = from e in daysCount
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;

        }



        //static string[] salaryHeadType = new string[] { "Gross", "Basic" };
        static string[] salaryHeadType = new string[] { "Basic" };
        public static IList<string> SalaryHeadType
        {
            get
            {
                return salaryHeadType.ToList<string>();
            }
        }

        static string[] salarySheetName = new string[] { "Salary Sheet(1)", "Salary Sheet(2)", "Salary Sheet(3)", "Salary Sheet(4)", "Pay Slip", "Pay Slip (Email)" };
        public static IList<string> SalarySheetName
        {
            get
            {
                return salarySheetName.ToList<string>();
            }
        }
        public IEnumerable<object> GetSalarySheetNameList()
        {
            IEnumerable<object> enumList = from e in SalarySheetName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] LetterName = new string[] { "Appointment Letter", "Transfer Letter", "Promotion Letter", "Increment Letter" };
        public IEnumerable<object> GetLetterNameList()
        {
            IEnumerable<object> enumList = from e in LetterName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }



        static string[] holiDayTypeName = new string[] { "Govt", "Festival", "Special" };
        public IEnumerable<object> GetHoliDayTypeNameList()
        {
            IEnumerable<object> enumList = from e in holiDayTypeName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] attnStatusName = new string[] { "All", "Present", "Absent", "Late", "All Missing", "In Miss", "Out Miss" };
        public IEnumerable<object> GetAttnStatusNameList()
        {
            IEnumerable<object> enumList = from e in attnStatusName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] loanInterestPolicy = new string[] { "Fixed", "Rate", "Reduce" };
        public static IList<string> LoanInterestPolicy
        {
            get
            {
                return loanInterestPolicy.ToList<string>();
            }
        }
        public IEnumerable<object> GetLoanInterestPolicyList()
        {
            IEnumerable<object> enumList = from e in LoanInterestPolicy
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] salaryProcessName = new string[] { "ALL", "SALARY EARNING", "PF", "TAX", "LOAN", "OTHER EARNING", "OTHER DEDUCTION", "ATTENDANCE", "EMPLOYEE STATUS" };
        public static IList<string> SalaryProcessName
        {
            get
            {
                return salaryProcessName.ToList<string>();
            }
        }
        public IEnumerable<object> GetSalaryProcessNameList()
        {
            IEnumerable<object> enumList = from e in SalaryProcessName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        public IEnumerable<object> GetSalaryHeadTypeList()
        {
            IEnumerable<object> enumList = from e in SalaryHeadType
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] earningRptParamName = new string[] { "Fiscal Period", "Employee Name", "Earning Type" };

        public IEnumerable<object> GetEarningRptParamNameList()
        {
            IEnumerable<object> enumList = from e in earningRptParamName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] deductionRptParamName = new string[] { "Fiscal Period", "Employee Name", "Deduction Type" };

        public IEnumerable<object> GetDeductionRptParamNameList()
        {
            IEnumerable<object> enumList = from e in deductionRptParamName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }


        static string[] pfTaxRptParamName = new string[] { "Fiscal Period", "Employee Name" };

        public IEnumerable<object> GetPFTaxRptParamNameList()
        {
            IEnumerable<object> enumList = from e in pfTaxRptParamName
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }
        static string[] TransferTypes = new string[] { "6.1Out", "6.2Out" };

        public IEnumerable<object> GetTransferIssueTypesList()
        {
            IEnumerable<object> enumList = from e in TransferTypes
                                           select new { Id = e.ToString(), Name = e.ToString() };
            return enumList;
        }

        static string[] VATRate = new string[] { "15", "10", "7.5","5","4.5","3","2.4","2","0" };
        public IEnumerable<EnumVM> GetVATRateList()
        {
            IEnumerable<EnumVM> enumList = VATRate.Select(x => new EnumVM() { Id = x, Name = x });
            return enumList;
        }


        #endregion

    }
}
