using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.JsonModels
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class DataRootBollore
    {
        public UniversalInterchangeBatch UniversalInterchangeBatch { get; set; }
    }

    public class APAccountGroup
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class ARAccountGroup
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class BatchCreateTimeInfo
    {
        public string CompanyCode { get; set; }
        public DateTime UTC { get; set; }
        public DateTime LocalSystemTime { get; set; }
        public DateTime CompanyTime { get; set; }
        public string TimeZone { get; set; }
        public string UTCOffset { get; set; }
        public string isDST { get; set; }
    }

    public class BatchGenTimeInfo
    {
        public string CompanyCode { get; set; }
        public DateTime UTC { get; set; }
        public DateTime LocalSystemTime { get; set; }
        public DateTime CompanyTime { get; set; }
        public string TimeZone { get; set; }
        public string UTCOffset { get; set; }
        public string isDST { get; set; }
    }

    public class BillingBranches
    {
        public string BillingBranchCount { get; set; }
        public RelatedOrg RelatedOrg { get; set; }
    }

    public class Body
    {
        public UniversalTransaction UniversalTransaction { get; set; }
    }

    public class Branch
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Consol
    {
        public string BillingBranch { get; set; }
        public string BillingDepartment { get; set; }
        public string CompanyCode { get; set; }
        public string JOBID { get; set; }
        public string Master { get; set; }
        public string JKID { get; set; }
        public string Vessel { get; set; }
        public string Voyage { get; set; }
        public string JKDestination { get; set; }
        public DateTime JKETA { get; set; }
        public DateTime JKETD { get; set; }
        public string JKOrigin { get; set; }
        public string ServiceLevel { get; set; }
        public string JKActualChargeable { get; set; }
        public string VolumeTotal { get; set; }
        public string WeightTotal { get; set; }
        public string JKTransportmode { get; set; }
        public string VolumeUnit { get; set; }
        public string WeightUnit { get; set; }
        
    }

    public class ConsolCollection
    {
        public string ConsolCount { get; set; }
        public Consol Consol { get; set; }
    }

    public class Department
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Header
    {
        public RecipientIds RecipientIds { get; set; }
    }

    public class JOBID
    {
        public string Type { get; set; }
        public string Key { get; set; }
    }

    public class LocalCurrency
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Precision { get; set; }
    }

    public class OpsData
    {
        public ShipmentCollection ShipmentCollection { get; set; }
    }

    public class OrgNumber
    {
        public string Country { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class OrgNumberCollection
    {
        public string OrgNumberCount { get; set; }
        public List<OrgNumber> OrgNumber { get; set; }
    }

    public class OSCurrency
    {
        public string Code { get; set; }
        public string Precision { get; set; }
    }

    public class PostingJournal
    {
        public string AccountHeaderPK { get; set; }
        public string AccountLinePK { get; set; }
        public string BatchSequence { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public string ChargeCode { get; set; }
        public string ChargeCodeDescription { get; set; }
        public string ChargeCodeLocalDescription { get; set; }
        public string ChargeCurrency { get; set; }
        public string ChargeExchangeRate { get; set; }
        public string ChargeTotalAmount { get; set; }
        public string ChargeTotalExtraVATAmount { get; set; }
        public string ChargeTotalVATAmount { get; set; }
        public string CompanyCode { get; set; }
        public string CreditGLAccount { get; set; }
        public string CurrentBatchNumber { get; set; }
        public string DebitGLAccount { get; set; }
        public string Department { get; set; }
        public string ExternalCreditorCode { get; set; }
        public string ExtraVATAmountInTxnCurrency { get; set; }
        public string GLAccountCode { get; set; }
        public string GLAccountDescription { get; set; }
        public DateTime GLPostDate { get; set; }
        public string GovernmentReportingChargeCode { get; set; }
        public string GSTVATBasis { get; set; }
        public string InvoiceReference { get; set; }
        public string IsFinalCharge { get; set; }
        public string JOBID { get; set; }
        public string LocalCurrency { get; set; }
        public string LocalExtraVATAmount { get; set; }
        public string LocalTotalAmount { get; set; }
        public string LocalVATAmount { get; set; }
        public string LocalWHTAmount { get; set; }
        public string MappedCreditGLAccount { get; set; }
        public string MappedCreditGLAccountSequence { get; set; }
        public string MappedDebitGLAccount { get; set; }
        public string MappedDebitGLAccountSequence { get; set; }
        public string MappedExtraTaxCreditGLAccount { get; set; }
        public string MappedExtraTaxCreditGLAccountSequence { get; set; }
        public string MappedExtraTaxDebitGLAccount { get; set; }
        public string MappedExtraTaxDebitGLAccountSequence { get; set; }
        public string MappedTaxCreditGLAccount { get; set; }
        public string MappedTaxCreditGLAccountSequence { get; set; }
        public string MappedTaxDebitGLAccount { get; set; }
        public string MappedTaxDebitGLAccountSequence { get; set; }
        public string MappedWithholdingTaxCreditGLAccount { get; set; }
        public string MappedWithholdingTaxCreditGLAccountSequence { get; set; }
        public string MappedWithholdingTaxDebitGLAccount { get; set; }
        public string MappedWithholdingTaxDebitGLAccountSequence { get; set; }
        public string MappedGLAccount { get; set; }
        public string MappedGLAccountSequence { get; set; }
        public string Organization { get; set; }
        public string OriginalBatchNumber { get; set; }
        public string OriginalBatchSequence { get; set; }
        public string PostingPeriod { get; set; }
        public string RevenueRecognitionType { get; set; }
        public string TaxMessageID { get; set; }
        public string TransactionCurrency { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string TxnAmountInTxnCurrency { get; set; }
        public string TxnAmountInTxnCurrencyWithVAT { get; set; }
        public string TxnType { get; set; }
        public string VATAmountInTxnCurrency { get; set; }
        public string VATTaxIDExtraTaxRate { get; set; }
        public string VATTaxIDTaxCode { get; set; }
        public string VATTaxIDTaxDescription { get; set; }
        public string VATTaxIDTaxRate { get; set; }
        public string VATTaxIDTaxTypeCode { get; set; }
        public string WithholdingAmountInTxnCurrency { get; set; }
        public string WithholdingTaxIDTaxRate { get; set; }
        public string ChargeExpenseGroup { get; set; }
        public string ChargeGroup { get; set; }
        public string ChargeIsActive { get; set; }
        public string ChargeOtherGroups { get; set; }
        public string ChargeSalesGroup { get; set; }
        public string ChargeServiceType { get; set; }
        public string ChargeType { get; set; }
        public string IsConsolServiceCharge { get; set; }
        public string Polarity { get; set; }
        public string Ledger { get; set; }
        public string LegacyAcctJournal { get; set; }
        public string AcctTaxCode { get; set; }
        public string NatureVATAcct { get; set; }
        public APAccountGroup APAccountGroup { get; set; }
        public ARAccountGroup ARAccountGroup { get; set; }
        public string IsSSC { get; set; }
        public string IsGateway { get; set; }
        public string Rate { get; set; }
        public string RateUnit { get; set; }
        public string Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public string ChargeCurrencyPrecision { get; set; }
        public string LocalCurrencyPrecision { get; set; }
        public string TransactionCurrencyPrecision { get; set; }
        public string BranchProxyCode { get; set; }
        public string OHPK { get; set; }
        public string CostGovtChargeCode { get; set; }
        public string SellGovtChargeCode { get; set; }
        public string PlaceOfSupply { get; set; }
        public string PlaceOfSupplyType { get; set; }
    }

    public class PostingJournalCollection
    {
        ////public PostingJournal PostingJournal { get; set; }
        public List<PostingJournal> PostingJournal { get; set; }
    }

    public class RecipientId
    {
        public SystemMap SystemMap { get; set; }
        public string ID { get; set; }
    }

    public class RecipientIds
    {
        public string RecipientIdCount { get; set; }
        ////public object RecipientId { get; set; }
        public RecipientId RecipientId { get; set; }
    }

    public class Ref
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class ReferenceCollection
    {
        public string ReferenceCount { get; set; }
        public List<Ref> Ref { get; set; }
    }

    public class RelatedOrg
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string CompanyCode { get; set; }
        public string Country { get; set; }
        public string CountryName { get; set; }
        public string CurrentBatchNumber { get; set; }
        public string FullName { get; set; }
        public string JOBID { get; set; }
        public string JobType { get; set; }
        public string Location { get; set; }
        public string OrgCode { get; set; }
        public string PostCode { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Source { get; set; }
        public string UNLOCO { get; set; }
        public string ArianeCode { get; set; }
        public string CRMID { get; set; }
        public string VATCode { get; set; }
        public string IsValidAddress { get; set; }
        public string Language { get; set; }
        public string AddressType { get; set; }
        public string IsMainAddress { get; set; }
        public string AddIndex { get; set; }
        public string NettingCode { get; set; }
        public string State { get; set; }

        public string ProxyBranchCode { get; set; }
        public string OrganizationCategory { get; set; }
        public string OriginVATNumber { get; set; }
        public OrgNumberCollection OrgNumberCollection { get; set; }
        public string APAccountGroupCode { get; set; }
        public string ARAccountGroupCode { get; set; }
    }

    public class RelatedOrgs
    {
        public string RelatedOrgCount { get; set; }
        public RelatedOrg RelatedOrg { get; set; }
    }

    public class Shipment
    {
        public string BillingBranch { get; set; }
        public string BillingDepartment { get; set; }
        public string CompanyCode { get; set; }
        public string JOBID { get; set; }
        public string RepOps { get; set; }
        public string RepSales { get; set; }
        public string JobStatus { get; set; }
        public RelatedOrgs RelatedOrgs { get; set; }
        public string ControllerCode { get; set; }
        public string ControllerFullName { get; set; }
        public string GoodsDescription { get; set; }
        public string HouseBill { get; set; }
        public string INCO { get; set; }
        public string JSDestination { get; set; }
        public string JSOrigin { get; set; }
        public string ServiceLevelCode { get; set; }
        public string VerticalType { get; set; }
        public string JSActualChargeable { get; set; }
        public string JSActualVolume { get; set; }
        public string JSActualWeight { get; set; }
        public string JSTransportmode { get; set; }
        public string OuterPacks { get; set; }
        public string PackType { get; set; }
        public string ShipmentUnitOfVolume { get; set; }
        public string ShipmentUnitOfWeight { get; set; }
        public string ShipperCode { get; set; }
        public string ShipperFullName { get; set; }
        public string ConsigneeCode { get; set; }
        public string ConsigneeFullName { get; set; }
        public string TradeLine { get; set; }
        public string CommodityCode { get; set; }
        public ConsolCollection ConsolCollection { get; set; }

        public DateTime JSETA { get; set; }
        public DateTime JSETD { get; set; }
        public string Direction { get; set; }
        public string Containers { get; set; }
        public ReferenceCollection ReferenceCollection { get; set; }
    }

    public class ShipmentCollection
    {
        public string ShipmentCount { get; set; }
        public Shipment Shipment { get; set; }
    }

    public class SystemMap
    {
        public string AcctSystem { get; set; }
        public string AcctCompanyCode { get; set; }
        public string CFTMonitorName { get; set; }
    }

    public class TransactionInfo
    {
        public string AgreedPaymentMethod { get; set; }
        public APAccountGroup APAccountGroup { get; set; }
        public ARAccountGroup ARAccountGroup { get; set; }
        public Branch Branch { get; set; }
        public string Category { get; set; }
        public object ComplianceSubType { get; set; }
        public DateTime CreateTime { get; set; }
        public Department Department { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string ExchangeRate { get; set; }
        public string ExternalDebtorCode { get; set; }
        public string InvoiceTerms { get; set; }
        public string InvoiceTermDays { get; set; }
        public string ExternalCreditorCode { get; set; }
        public string IsCancelled { get; set; }
        public string IsCreatedByMatchingProcess { get; set; }
        public string IsPrinted { get; set; }
        public JOBID JOBID { get; set; }
        public string JobInvoiceNumber { get; set; }
        public string Ledger { get; set; }
        public LocalCurrency LocalCurrency { get; set; }
        public string LocalTotal { get; set; }
        public string LocalVATAmount { get; set; }
        public string LocalWHTAmount { get; set; }
        public string NumberOfSupportingDocuments { get; set; }
        public string Organization { get; set; }
        public OSCurrency OSCurrency { get; set; }
        public string OSGSTVATAmount { get; set; }
        public string OSTotal { get; set; }
        public string OutstandingAmount { get; set; }
        public DateTime PostDate { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string OSTotalWithNoTax { get; set; }
        public string LocalTaxableAmount { get; set; }
        public string OSTaxableAmount { get; set; }
        public string LocalNonTaxableAmount { get; set; }
        public string OSNonTaxableAmount { get; set; }
        public string CurrentBatchNumber { get; set; }
        public string OriginalBatchNumber { get; set; }
        public string HighestBatchNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string LegacyAcctJournal { get; set; }
        public string InvoiceRemittanceReference { get; set; }
        public string CreatorEMail { get; set; }
        public string EditorEMail { get; set; }
        public string CreatorFullName { get; set; }
        public string BranchProxyCode { get; set; }
        public string OHPK { get; set; }
        public RelatedOrgs RelatedOrgs { get; set; }
        public PostingJournalCollection PostingJournalCollection { get; set; }
        public OpsData OpsData { get; set; }
        public string OriginalReferenceOriginalBranch { get; set; }
        public object OriginalReferenceOriginalTransactionComplianceSubType { get; set; }
        public DateTime OriginalReferenceOriginalTransactionDate { get; set; }
        public string OriginalReferenceOriginalTransactionJobInvoiceNumber { get; set; }
        public string OriginalReferenceOriginalTransactionNumber { get; set; }
        public string TotalLocalVAT { get; set; }
        public string TotalVATInTxnCurrency { get; set; }
        public string TotalLocalExtraVAT { get; set; }
        public string TotalExtraVATInTxnCurrency { get; set; }
        public string TechnicalCancellation { get; set; }
        public BillingBranches BillingBranches { get; set; }
    }

    public class UniversalInterchange
    {
        //public Header Header { get; set; }
        public BatchGenTimeInfo BatchGenTimeInfo { get; set; }
        public BatchCreateTimeInfo BatchCreateTimeInfo { get; set; }
        public Body Body { get; set; }
    }

    public class UniversalInterchangeBatch
    {
        public string Batch { get; set; }
        public string Company { get; set; }
        public BatchGenTimeInfo BatchGenTimeInfo { get; set; }
        public BatchCreateTimeInfo BatchCreateTimeInfo { get; set; }
        public List<UniversalInterchange> UniversalInterchange { get; set; }
        ////public UniversalInterchange UniversalInterchange { get; set; }
    }

    public class UniversalTransaction
    {
        public TransactionInfo TransactionInfo { get; set; }
    }



}
