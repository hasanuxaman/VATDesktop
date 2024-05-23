using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.JsonModels
{

    #region Invoice

    public class JsonModels_Ernst
    {
        [JsonProperty("n0:Invoice")]
        public N0Invoice N0Invoice { get; set; }

    }


    public class N0Invoice
    {
        //////[JsonProperty("@xmlns:n0")]
        //////public string XmlnsN0 { get; set; }

        //////[JsonProperty("@xmlns:n1")]
        //////public string XmlnsN1 { get; set; }

        //////[JsonProperty("@xmlns:n2")]
        //////public string XmlnsN2 { get; set; }

        //////[JsonProperty("@xmlns:n3")]
        //////public string XmlnsN3 { get; set; }

        //////[JsonProperty("@xmlns:n4")]
        //////public string XmlnsN4 { get; set; }

        //////[JsonProperty("@xmlns:n5")]
        //////public string XmlnsN5 { get; set; }

        //////[JsonProperty("@xmlns:prx")]
        //////public string XmlnsPrx { get; set; }

        //////[JsonProperty("n3:UBLExtensions")]
        //////public N3UBLExtensions N3UBLExtensions { get; set; }

        //////[JsonProperty("n2:UBLVersionID")]
        //////public string N2UBLVersionID { get; set; }

        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:IssueDate")]
        public string N2IssueDate { get; set; }

        [JsonProperty("n2:DueDate")]
        public string N2DueDate { get; set; }

        [JsonProperty("n2:InvoiceTypeCode")]
        public string N2InvoiceTypeCode { get; set; }

        [JsonProperty("n2:Note")]
        public string N2Note { get; set; }

        [JsonProperty("n2:DocumentCurrencyCode")]
        public string N2DocumentCurrencyCode { get; set; }

        [JsonProperty("n2:BuyerReference")]
        public string N2BuyerReference { get; set; }

        [JsonProperty("n1:OrderReference")]
        public N1OrderReference N1OrderReference { get; set; }

        [JsonProperty("n1:InvoicePeriod")]
        public N1InvoicePeriod N1InvoicePeriod { get; set; }

        [JsonProperty("n1:AccountingSupplierParty")]
        public N1AccountingSupplierParty N1AccountingSupplierParty { get; set; }

        [JsonProperty("n1:AccountingCustomerParty")]
        public N1AccountingCustomerParty N1AccountingCustomerParty { get; set; }

        //[JsonProperty("n1:PaymentMeans")]
        //public N1PaymentMeans N1PaymentMeans { get; set; }

        [JsonProperty("n1:ExchangeRate")]
        public N1ExchangeRate N1ExchangeRate { get; set; }

        [JsonProperty("n1:TaxTotal")]
        public N1TaxTotal N1TaxTotal { get; set; }

        [JsonProperty("n1:LegalMonetaryTotal")]
        public N1LegalMonetaryTotal N1LegalMonetaryTotal { get; set; }

        [JsonProperty("n1:InvoiceLine")]
        public List<N1InvoiceLine> N1InvoiceLine { get; set; }

        [JsonProperty("n1:OriginatorDocumentReference")]
        public N1OriginatorDocumentReference N1OriginatorDocumentReference { get; set; }

        [JsonProperty("n1:AdditionalDocumentReference")]
        public N1AdditionalDocumentReference N1AdditionalDocumentReference { get; set; }
    }

    public class N2InvoicedQuantity
    {
        [JsonProperty("@unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    #region Common

    public class N1AccountingCustomerParty
    {
        [JsonProperty("n2:SupplierAssignedAccountID")]
        public string N2SupplierAssignedAccountID { get; set; }

        [JsonProperty("n1:Party")]
        public N1Party N1Party { get; set; }

    }

    public class N1AccountingSupplierParty
    {
        [JsonProperty("n1:Party")]
        public N1Party N1Party { get; set; }

        [JsonProperty("n1:SellerContact")]
        public N1SellerContact N1SellerContact { get; set; }
    }

    public class N1AdditionalDocumentReference
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:IssueDate")]
        public string N2IssueDate { get; set; }

        [JsonProperty("n2:IssueTime")]
        public string N2IssueTime { get; set; }

        [JsonProperty("n1:IssuerParty")]
        public N1IssuerParty N1IssuerParty { get; set; }

    }

    public class N1Address
    {
        [JsonProperty("n2:StreetName")]
        public string N2StreetName { get; set; }

        [JsonProperty("n2:CityName")]
        public string N2CityName { get; set; }

        [JsonProperty("n2:PostalZone")]
        public string N2PostalZone { get; set; }

    }

    public class N1Contact
    {
        [JsonProperty("n2:Telephone")]
        public string N2Telephone { get; set; }

        [JsonProperty("n2:Name")]
        public string N2Name { get; set; }

        [JsonProperty("n2:ElectronicMail")]
        public string N2ElectronicMail { get; set; }

    }

    public class N1Country
    {
        [JsonProperty("n2:IdentificationCode")]
        public string N2IdentificationCode { get; set; }

    }

    public class N1ExchangeRate
    {
        [JsonProperty("n2:SourceCurrencyCode")]
        public string N2SourceCurrencyCode { get; set; }

        [JsonProperty("n2:TargetCurrencyCode")]
        public string N2TargetCurrencyCode { get; set; }

        [JsonProperty("n2:CalculationRate")]
        public string N2CalculationRate { get; set; }

    }

    public class N1FinancialInstitution
    {
        [JsonProperty("n2:Name")]
        public string N2Name { get; set; }

        [JsonProperty("n1:Address")]
        public N1Address N1Address { get; set; }

    }

    public class N1FinancialInstitutionBranch
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:Name")]
        public string N2Name { get; set; }

        [JsonProperty("n1:FinancialInstitution")]
        public N1FinancialInstitution N1FinancialInstitution { get; set; }

    }

    public class N1InvoiceLine
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:InvoicedQuantity")]
        public N2InvoicedQuantity N2InvoicedQuantity { get; set; }

        [JsonProperty("n2:LineExtensionAmount")]
        public N2LineExtensionAmount N2LineExtensionAmount { get; set; }

        [JsonProperty("n1:OrderLineReference")]
        public N1OrderLineReference N1OrderLineReference { get; set; }

        [JsonProperty("n1:TaxTotal")]
        public N1TaxTotal N1TaxTotal { get; set; }

        [JsonProperty("n1:Item")]
        public N1Item N1Item { get; set; }

        [JsonProperty("n1:Price")]
        public N1Price N1Price { get; set; }
    }

    public class N1InvoicePeriod
    {
        [JsonProperty("n2:StartDate")]
        public string N2StartDate { get; set; }

        [JsonProperty("n2:EndDate")]
        public string N2EndDate { get; set; }
    }

    public class N1IssuerParty
    {
        [JsonProperty("n1:PartyName")]
        public N1PartyName N1PartyName { get; set; }
    }

    public class N1Item
    {
        [JsonProperty("n2:Description")]
        public string N2Description { get; set; }

        [JsonProperty("n2:PackQuantity")]
        public N2PackQuantity N2PackQuantity { get; set; }

        [JsonProperty("n2:Name")]
        public string N2Name { get; set; }

        [JsonProperty("n2:AdditionalInformation")]
        public string N2AdditionalInformation { get; set; }
    }

    public class N1Language
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }
    }

    public class N1LegalMonetaryTotal
    {
        [JsonProperty("n2:TaxExclusiveAmount")]
        public N2TaxExclusiveAmount N2TaxExclusiveAmount { get; set; }

        [JsonProperty("n2:TaxInclusiveAmount")]
        public N2TaxInclusiveAmount N2TaxInclusiveAmount { get; set; }

        [JsonProperty("n2:PayableAmount")]
        public N2PayableAmount N2PayableAmount { get; set; }

        [JsonProperty("n2:LineExtensionAmount")]
        public N2LineExtensionAmount N2LineExtensionAmount { get; set; }

    }

    public class N1OrderLineReference
    {
        [JsonProperty("n2:LineID")]
        public string N2LineID { get; set; }
    }

    public class N1OrderReference
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:SalesOrderID")]
        public string N2SalesOrderID { get; set; }

        [JsonProperty("n2:CustomerReference")]
        public string N2CustomerReference { get; set; }

    }

    public class N1OriginatorDocumentReference
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:DocumentTypeCode")]
        public string N2DocumentTypeCode { get; set; }

        [JsonProperty("n2:DocumentType")]
        public string N2DocumentType { get; set; }
    }

    public class N1Party
    {
        [JsonProperty("n1:PartyIdentification")]
        public N1PartyIdentification N1PartyIdentification { get; set; }

        [JsonProperty("n1:PartyName")]
        public N1PartyName N1PartyName { get; set; }

        [JsonProperty("n1:PostalAddress")]
        public N1PostalAddress N1PostalAddress { get; set; }

        //[JsonProperty("n1:PartyTaxScheme")]
        //public N1PartyTaxScheme N1PartyTaxScheme { get; set; }

        [JsonProperty("n1:PartyLegalEntity")]
        public N1PartyLegalEntity N1PartyLegalEntity { get; set; }

        [JsonProperty("n1:Contact")]
        public N1Contact N1Contact { get; set; }

        [JsonProperty("n1:Language")]
        public N1Language N1Language { get; set; }

    }

    public class N1PartyIdentification
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }
    }

    public class N1PartyLegalEntity
    {
        [JsonProperty("n2:RegistrationName")]
        public string N2RegistrationName { get; set; }

        [JsonProperty("n2:CompanyID")]
        public string N2CompanyID { get; set; }

        [JsonProperty("n1:TaxScheme")]
        public N1TaxScheme N1TaxScheme { get; set; }
    }

    public class N1PartyName
    {
        [JsonProperty("n2:Name")]
        public string N2Name { get; set; }
    }

    public class N1PartyTaxScheme
    {
        [JsonProperty("n2:RegistrationName")]
        public string N2RegistrationName { get; set; }

        [JsonProperty("n2:CompanyID")]
        public string N2CompanyID { get; set; }

        [JsonProperty("n1:TaxScheme")]
        public object N1TaxScheme { get; set; }
    }

    public class N1PayeeFinancialAccount
    {
        [JsonProperty("n2:CurrencyCode")]
        public string N2CurrencyCode { get; set; }

        [JsonProperty("n2:AccountTypeCode")]
        public string N2AccountTypeCode { get; set; }

        [JsonProperty("n2:AccountFormatCode")]
        public string N2AccountFormatCode { get; set; }

        [JsonProperty("n1:FinancialInstitutionBranch")]
        public N1FinancialInstitutionBranch N1FinancialInstitutionBranch { get; set; }
    }

    public class N1PaymentMeans
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:PaymentMeansCode")]
        public string N2PaymentMeansCode { get; set; }

        [JsonProperty("n2:PaymentDueDate")]
        public string N2PaymentDueDate { get; set; }

        [JsonProperty("n2:PaymentChannelCode")]
        public string N2PaymentChannelCode { get; set; }

        [JsonProperty("n2:PaymentID")]
        public string N2PaymentID { get; set; }

        [JsonProperty("n1:PayeeFinancialAccount")]
        public N1PayeeFinancialAccount N1PayeeFinancialAccount { get; set; }
    }

    public class N1PostalAddress
    {
        [JsonProperty("n2:StreetName")]
        public string N2StreetName { get; set; }

        [JsonProperty("n2:CityName")]
        public string N2CityName { get; set; }

        [JsonProperty("n2:PostalZone")]
        public string N2PostalZone { get; set; }

        [JsonProperty("n1:Country")]
        public N1Country N1Country { get; set; }
    }

    public class N1Price
    {
        [JsonProperty("n2:PriceAmount")]
        public N2PriceAmount N2PriceAmount { get; set; }

        [JsonProperty("n2:BaseQuantity")]
        public N2BaseQuantity N2BaseQuantity { get; set; }
    }

    public class N1SellerContact
    {
        [JsonProperty("n2:ElectronicMail")]
        public string N2ElectronicMail { get; set; }
    }

    public class N1TaxCategory
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:Percent")]
        public string N2Percent { get; set; }

        [JsonProperty("n1:TaxScheme")]
        public N1TaxScheme N1TaxScheme { get; set; }
    }

    public class N1TaxScheme
    {
        [JsonProperty("n2:ID")]
        public string N2ID { get; set; }

        [JsonProperty("n2:TaxTypeCode")]
        public string N2TaxTypeCode { get; set; }
    }

    public class N1TaxSubtotal
    {
        [JsonProperty("n2:TaxableAmount")]
        public N2TaxableAmount N2TaxableAmount { get; set; }

        [JsonProperty("n2:TaxAmount")]
        public N2TaxAmount N2TaxAmount { get; set; }

        [JsonProperty("n1:TaxCategory")]
        public N1TaxCategory N1TaxCategory { get; set; }
    }

    public class N1TaxTotal
    {
        [JsonProperty("n2:TaxAmount")]
        public N2TaxAmount N2TaxAmount { get; set; }

        [JsonProperty("n1:TaxSubtotal")]
        public N1TaxSubtotal N1TaxSubtotal { get; set; }
    }

    public class N2BaseQuantity
    {
        [JsonProperty("@unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }


    public class N2LineExtensionAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2PackQuantity
    {
        [JsonProperty("@unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2PayableAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2PriceAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2TaxableAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2TaxAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2TaxExclusiveAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N2TaxInclusiveAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class N3ExtensionContent
    {
        [JsonProperty("n4:AdditionalInformation")]
        public N4AdditionalInformation N4AdditionalInformation { get; set; }

        [JsonProperty("n4:PreviewLines")]
        public List<N4PreviewLine> N4PreviewLines { get; set; }
    }

    public class N3UBLExtension
    {
        [JsonProperty("n3:ExtensionContent")]
        public N3ExtensionContent N3ExtensionContent { get; set; }
    }

    public class N3UBLExtensions
    {
        [JsonProperty("n3:UBLExtension")]
        public N3UBLExtension N3UBLExtension { get; set; }
    }

    public class N4AdditionalInformation
    {
        [JsonProperty("n5:ZZY_INVDELMETH")]
        public string N5ZZYINVDELMETH { get; set; }

    }

    public class N4ATTR3
    {
        [JsonProperty("n5:PriceAmount")]
        public N5PriceAmount N5PriceAmount { get; set; }
    }

    public class N4ATTR6
    {
        [JsonProperty("n5:PriceAmount")]
        public N5PriceAmount N5PriceAmount { get; set; }
    }

    public class N4ATTR7
    {
        [JsonProperty("n5:PriceAmount")]
        public N5PriceAmount N5PriceAmount { get; set; }
    }

    public class N4PreviewLine
    {
        [JsonProperty("n5:LineType")]
        public string N5LineType { get; set; }

        [JsonProperty("n5:SequenceNumber")]
        public string N5SequenceNumber { get; set; }

        [JsonProperty("n5:ATTR1")]
        public string N5ATTR1 { get; set; }

        [JsonProperty("n4:ATTR3")]
        public N4ATTR3 N4ATTR3 { get; set; }

        [JsonProperty("n4:ATTR6")]
        public N4ATTR6 N4ATTR6 { get; set; }

        [JsonProperty("n4:ATTR7")]
        public N4ATTR7 N4ATTR7 { get; set; }

    }

    public class N5PriceAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    #endregion

    #endregion

    #region Credit Note

    public class JsonModels_Ernst_CreditNote
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }
        public CreditNote CreditNote { get; set; }
    }

    public class SellerContact
    {
        public string ElectronicMail { get; set; }
    }

    public class TaxableAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class TaxAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class TaxCategory
    {
        public string ID { get; set; }
        public string Percent { get; set; }
        public CNTaxScheme TaxScheme { get; set; }
    }

    public class TaxExclusiveAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class TaxInclusiveAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class CNTaxScheme
    {
        public string ID { get; set; }
        public string TaxTypeCode { get; set; }
    }

    public class TaxSubtotal
    {
        public TaxableAmount TaxableAmount { get; set; }
        public TaxAmount TaxAmount { get; set; }
        public TaxCategory TaxCategory { get; set; }
    }

    public class TaxTotal
    {
        public TaxAmount TaxAmount { get; set; }
        public TaxSubtotal TaxSubtotal { get; set; }
    }

    public class UBLExtension
    {
        public ExtensionContent ExtensionContent { get; set; }
    }

    public class UBLExtensions
    {
        public UBLExtension UBLExtension { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }

    public class AccountingCustomerParty
    {
        public string SupplierAssignedAccountID { get; set; }
        public Party Party { get; set; }
    }

    public class AccountingSupplierParty
    {
        public SupplierParty Party { get; set; }
        public SellerContact SellerContact { get; set; }
    }

    public class AdditionalDocumentReference
    {
        public object ID { get; set; }
        public string IssueDate { get; set; }
        public string IssueTime { get; set; }
        public IssuerParty IssuerParty { get; set; }
    }

    public class AdditionalInformation
    {
        public string ZZY_INVDELMETH { get; set; }
    }

    public class Address
    {
        public string StreetName { get; set; }
        public string CityName { get; set; }
        public string PostalZone { get; set; }
    }

    public class ATTR3
    {
        public PriceAmount PriceAmount { get; set; }
    }

    public class ATTR6
    {
        public PriceAmount PriceAmount { get; set; }
    }

    public class ATTR7
    {
        public PriceAmount PriceAmount { get; set; }
    }

    public class BaseQuantity
    {
        [JsonProperty("@unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class Contact
    {
        public string Telephone { get; set; }
        public string Name { get; set; }
        public string ElectronicMail { get; set; }
    }

    public class Country
    {
        public string IdentificationCode { get; set; }
    }

    public class CreditedQuantity
    {
        [JsonProperty("@unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class CreditNote
    {
        public UBLExtensions UBLExtensions { get; set; }
        public string UBLVersionID { get; set; }
        public string ID { get; set; }
        public string IssueDate { get; set; }
        public string CreditNoteTypeCode { get; set; }
        public string TaxPointDate { get; set; }
        public string DocumentCurrencyCode { get; set; }
        public string BuyerReference { get; set; }
        public InvoicePeriod InvoicePeriod { get; set; }
        public OrderReference OrderReference { get; set; }
        public AccountingSupplierParty AccountingSupplierParty { get; set; }
        public AccountingCustomerParty AccountingCustomerParty { get; set; }
        public PaymentMeans PaymentMeans { get; set; }
        public ExchangeRate ExchangeRate { get; set; }
        public TaxTotal TaxTotal { get; set; }
        public LegalMonetaryTotal LegalMonetaryTotal { get; set; }
        public List<CreditNoteLine> CreditNoteLine { get; set; }
        public OriginatorDocumentReference OriginatorDocumentReference { get; set; }
        public AdditionalDocumentReference AdditionalDocumentReference { get; set; }
    }

    public class CreditNoteLine
    {
        public string ID { get; set; }
        public CreditedQuantity CreditedQuantity { get; set; }
        public LineExtensionAmount LineExtensionAmount { get; set; }
        public OrderLineReference OrderLineReference { get; set; }
        public TaxTotal TaxTotal { get; set; }
        public CNItem Item { get; set; }
        public Price Price { get; set; }
    }

    public class ExchangeRate
    {
        public string SourceCurrencyCode { get; set; }
        public string TargetCurrencyCode { get; set; }
        public string CalculationRate { get; set; }
    }

    public class ExtensionContent
    {
        public AdditionalInformation AdditionalInformation { get; set; }
        public List<PreviewLine> PreviewLines { get; set; }
    }

    public class FinancialInstitution
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }

    public class FinancialInstitutionBranch
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public FinancialInstitution FinancialInstitution { get; set; }
    }

    public class InvoicePeriod
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class IssuerParty
    {
        public PartyName PartyName { get; set; }
    }

    public class CNItem
    {
        public string Description { get; set; }
        public PackQuantity PackQuantity { get; set; }
        public string Name { get; set; }
        public object AdditionalInformation { get; set; }
    }

    public class Language
    {
        public string ID { get; set; }
    }

    public class LegalMonetaryTotal
    {
        public TaxExclusiveAmount TaxExclusiveAmount { get; set; }
        public TaxInclusiveAmount TaxInclusiveAmount { get; set; }
        public PayableAmount PayableAmount { get; set; }
        public LineExtensionAmount LineExtensionAmount { get; set; }
    }

    public class LineExtensionAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class OrderLineReference
    {
        public string LineID { get; set; }
    }

    public class OrderReference
    {
        public string ID { get; set; }
        public string SalesOrderID { get; set; }
        public string CustomerReference { get; set; }
    }

    public class OriginatorDocumentReference
    {
        public object ID { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentType { get; set; }
    }

    public class PackQuantity
    {
        [JsonProperty("@unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class Party : PartyBase
    {
        public List<PartyTaxScheme> PartyTaxScheme { get; set; }
    }
    public class SupplierParty : PartyBase
    {
        public PartyTaxScheme PartyTaxScheme { get; set; }
    }

    public class PartyBase
    {
        public PartyIdentification PartyIdentification { get; set; }
        public PartyName PartyName { get; set; }
        public PostalAddress PostalAddress { get; set; }
        public PartyLegalEntity PartyLegalEntity { get; set; }
        public Contact Contact { get; set; }
        public Language Language { get; set; }
    }


    public class PartyIdentification
    {
        public string ID { get; set; }
    }

    public class PartyLegalEntity
    {
        public string RegistrationName { get; set; }
        public string CompanyID { get; set; }
        public CNTaxScheme TaxScheme { get; set; }
    }

    public class PartyName
    {
        public string Name { get; set; }
    }

    public class PartyTaxScheme
    {
        public string RegistrationName { get; set; }
        public string CompanyID { get; set; }
        public string TaxScheme { get; set; }
    }

    public class PayableAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class PayeeFinancialAccount
    {
        public string CurrencyCode { get; set; }
        public string AccountTypeCode { get; set; }
        public string AccountFormatCode { get; set; }
        public FinancialInstitutionBranch FinancialInstitutionBranch { get; set; }
    }

    public class PaymentMeans
    {
        public string ID { get; set; }
        public string PaymentMeansCode { get; set; }
        public string PaymentDueDate { get; set; }
        public string PaymentChannelCode { get; set; }
        public object PaymentID { get; set; }
        public PayeeFinancialAccount PayeeFinancialAccount { get; set; }
    }

    public class PostalAddress
    {
        public string StreetName { get; set; }
        public string CityName { get; set; }
        public string PostalZone { get; set; }
        public Country Country { get; set; }
    }

    public class PreviewLine
    {
        public string LineType { get; set; }
        public string SequenceNumber { get; set; }
        public string ATTR1 { get; set; }
        public ATTR3 ATTR3 { get; set; }
        public ATTR6 ATTR6 { get; set; }
        public ATTR7 ATTR7 { get; set; }
    }

    public class Price
    {
        public PriceAmount PriceAmount { get; set; }
        public BaseQuantity BaseQuantity { get; set; }
    }

    public class PriceAmount
    {
        [JsonProperty("@currencyID")]
        public string CurrencyID { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    #endregion

    

    

}
