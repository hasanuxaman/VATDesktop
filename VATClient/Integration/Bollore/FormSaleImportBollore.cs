using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Library.Integration;
using VATViewModel.DTOs;

namespace VATClient.Integration.Bollore
{
    public partial class FormSaleImportBollore : Form
    {
        #region Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public string preSelectTable;
        public string transactionType;
        private string _saleRow = "";
        DataTable IntegrationData = new DataTable();
        string[] result = new string[6];

        #endregion

        public FormSaleImportBollore()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormSaleImportBollore form = new FormSaleImportBollore();
            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormSaleImportBollore_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;

            backgroundSaveSale.RunWorkerAsync();
        }

        private void backgroundSaveSale_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region XML

                string xml = @"            
<UniversalInterchangeBatch>
	<Batch>31</Batch>
	<Company>BD1</Company>
	<BatchGenTimeInfo>
		<CompanyCode>BD1</CompanyCode>
		<UTC>2022-03-21T05:15:38.143</UTC>
		<LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
		<CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
		<TimeZone>Bangladesh Standard Time</TimeZone>
		<UTCOffset>+06:00</UTCOffset>
		<isDST>0</isDST>
	</BatchGenTimeInfo>
	<BatchCreateTimeInfo>
		<CompanyCode>BD1</CompanyCode>
		<UTC>2022-03-21T05:15:05.827</UTC>
		<LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
		<CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
		<TimeZone>Bangladesh Standard Time</TimeZone>
		<UTCOffset>+06:00</UTCOffset>
		<isDST>0</isDST>
	</BatchCreateTimeInfo>
	<UniversalInterchange>
		<Header>
			<RecipientIds>
				<RecipientIdCount>1</RecipientIdCount>
				<RecipientId>
					<SystemMap>
						<AcctSystem>SUN</AcctSystem>
						<AcctCompanyCode>DAT</AcctCompanyCode>
						<CFTMonitorName>DACPEG</CFTMonitorName>
					</SystemMap>
					<ID>ACCOUNTING</ID>
				</RecipientId>
			</RecipientIds>
		</Header>
		<BatchGenTimeInfo>
			<CompanyCode>BD1</CompanyCode>
			<UTC>2022-03-21T05:15:38.143</UTC>
			<LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
			<CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
			<TimeZone>Bangladesh Standard Time</TimeZone>
			<UTCOffset>+06:00</UTCOffset>
			<isDST>0</isDST>
		</BatchGenTimeInfo>
		<BatchCreateTimeInfo>
			<CompanyCode>BD1</CompanyCode>
			<UTC>2022-03-21T05:15:05.827</UTC>
			<LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
			<CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
			<TimeZone>Bangladesh Standard Time</TimeZone>
			<UTCOffset>+06:00</UTCOffset>
			<isDST>0</isDST>
		</BatchCreateTimeInfo>
		<Body>
			<UniversalTransaction>
				<TransactionInfo>
					<AgreedPaymentMethod>V</AgreedPaymentMethod>
					<ARAccountGroup>
						<Code>TPY</Code>
						<Description>THIRD PARTY COMPANY</Description>
					</ARAccountGroup>
					<Branch>
						<Code>B05</Code>
						<Name>BOLLORE LOGISTICS BANGLADESH - DHAKA</Name>
					</Branch>
					<Category>FIN</Category>
					<ComplianceSubType></ComplianceSubType>
					<CreateTime>2022-03-21T05:12:00</CreateTime>
					<Department>
						<Code>FES</Code>
						<Name>Forwarding Export Sea</Name>
					</Department>
					<Description>S00091725</Description>
					<DueDate>2022-03-21T11:12:00</DueDate>
					<ExchangeRate>1.000000</ExchangeRate>
					<ExternalDebtorCode>CT02821</ExternalDebtorCode>
					<InvoiceTerms>COD</InvoiceTerms>
					<InvoiceTermDays>0</InvoiceTermDays>
					<IsCancelled>FALSE</IsCancelled>
					<IsCreatedByMatchingProcess>FALSE</IsCreatedByMatchingProcess>
					<IsPrinted>FALSE</IsPrinted>
					<JOBID>
						<Type>S00091725</Type>
						<Key>JS</Key>
					</JOBID>
					<JobInvoiceNumber>S00091725/B</JobInvoiceNumber>
					<Ledger>AR</Ledger>
					<LocalCurrency>
						<Code>BDT</Code>
						<Description>Bangladeshi Taka</Description>
						<Precision>100</Precision>
					</LocalCurrency>
					<LocalTotal>1000.00</LocalTotal>
					<LocalVATAmount>0.00</LocalVATAmount>
					<LocalWHTAmount>0.00</LocalWHTAmount>
					<NumberOfSupportingDocuments>1</NumberOfSupportingDocuments>
					<Organization>4STITCBDGAZ</Organization>
					<OSCurrency>
						<Code>BDT</Code>
						<Precision>100</Precision>
					</OSCurrency>
					<OSGSTVATAmount>0.00</OSGSTVATAmount>
					<OSTotal>1000.00</OSTotal>
					<OutstandingAmount>1000.00</OutstandingAmount>
					<PostDate>2022-03-21T11:12:00</PostDate>
					<TransactionNumber>F220000042</TransactionNumber>
					<TransactionType>INV</TransactionType>
					<OSTotalWithNoTax>1000.00</OSTotalWithNoTax>
					<LocalTaxableAmount>0.00</LocalTaxableAmount>
					<OSTaxableAmount>0.00</OSTaxableAmount>
					<LocalNonTaxableAmount>1000.00</LocalNonTaxableAmount>
					<OSNonTaxableAmount>1000.00</OSNonTaxableAmount>
					<CurrentBatchNumber>31</CurrentBatchNumber>
					<OriginalBatchNumber>31</OriginalBatchNumber>
					<HighestBatchNumber>31</HighestBatchNumber>
					<InvoiceDate>2022-03-21T11:12:00</InvoiceDate>
					<LegacyAcctJournal>SALEW</LegacyAcctJournal>
					<InvoiceRemittanceReference>00001041</InvoiceRemittanceReference>
					<CreatorEMail>manish.mukherjee@bollore.com</CreatorEMail>
					<EditorEMail>manish.mukherjee@bollore.com</EditorEMail>
					<CreatorFullName>Manish Mukherjee</CreatorFullName>
					<BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
					<OHPK>73102663-A13A-4899-8FA8-A735150D3375</OHPK>
					<RelatedOrgs>
						<RelatedOrgCount>1</RelatedOrgCount>
						<RelatedOrg>
							<Address1>WEST SHAILDUBI WARD-05</Address1>
							<Address2>P.S.- KASHIMPUR P.O.- KASHIMOUR</Address2>
							<City>GAZIPUR</City>
							<CompanyCode>BD1</CompanyCode>
							<Country>BD</Country>
							<CountryName>Bangladesh</CountryName>
							<CurrentBatchNumber>31</CurrentBatchNumber>
							<FullName>4 STITCH KNIT COMPOSITE LTD</FullName>
							<JOBID>S00091725</JOBID>
							<JobType>JS</JobType>
							<Location>I</Location>
							<OrgCode>4STITCBDGAZ</OrgCode>
							<PostCode>1700</PostCode>
							<Role>BPT</Role>
							<RoleName>BillingParty</RoleName>
							<Source>O</Source>
							<UNLOCO>BDGAZ</UNLOCO>
							<VATCode>TAX</VATCode>
							<IsValidAddress>FALSE</IsValidAddress>
							<Language>EN-US</Language>
							<AddressType>OFC</AddressType>
							<IsMainAddress>TRUE</IsMainAddress>
							<AddIndex>0</AddIndex>
						</RelatedOrg>
					</RelatedOrgs>
					<PostingJournalCollection>
						<PostingJournal>
							<AccountHeaderPK>5531C195-E285-483F-B3BE-4A1DFEE5C576</AccountHeaderPK>
							<AccountLinePK>BB6F7906-E583-4498-8BF2-A05B1B76F608</AccountLinePK>
							<BatchSequence>3</BatchSequence>
							<Branch>BOLLORE LOGISTICS BANGLADESH - DHAKA</Branch>
							<BranchCode>B05</BranchCode>
							<ChargeCode>10569</ChargeCode>
							<ChargeCodeDescription>BASIC FREIGHT CHARGE</ChargeCodeDescription>
							<ChargeCodeLocalDescription>BASIC FREIGHT CHARGE</ChargeCodeLocalDescription>
							<ChargeCurrency>BDT</ChargeCurrency>
							<ChargeExchangeRate>1.000000</ChargeExchangeRate>
							<ChargeTotalAmount>1000.00</ChargeTotalAmount>
							<ChargeTotalExtraVATAmount>0.00</ChargeTotalExtraVATAmount>
							<ChargeTotalVATAmount>0.00</ChargeTotalVATAmount>
							<CompanyCode>BD1</CompanyCode>
							<CreditGLAccount>Revenue suspense control account(487.1000.0)</CreditGLAccount>
							<CurrentBatchNumber>31</CurrentBatchNumber>
							<DebitGLAccount>Trade debtors control(411.1000.0)</DebitGLAccount>
							<Department>FES</Department>
							<Description>EXEMPT TAX MESSAGE MATRIX</Description>
							<ExternalDebtorCode>CT02821</ExternalDebtorCode>
							<ExtraVATAmountInTxnCurrency>0.00</ExtraVATAmountInTxnCurrency>
							<GLAccountCode>701.1010.0</GLAccountCode>
							<GLAccountDescription>FRT REVENUE ACTUAL</GLAccountDescription>
							<GLPostDate>2022-03-21T11:12:00</GLPostDate>
							<GovernmentReportingChargeCode>S01510</GovernmentReportingChargeCode>
							<GSTVATBasis>FALSE</GSTVATBasis>
							<InvoiceReference>S00091725/B</InvoiceReference>
							<IsFinalCharge>FALSE</IsFinalCharge>
							<JOBID>S00091725</JOBID>
							<LocalCurrency>BDT</LocalCurrency>
							<LocalExtraVATAmount>0.00</LocalExtraVATAmount>
							<LocalTotalAmount>1000.00</LocalTotalAmount>
							<LocalVATAmount>0.00</LocalVATAmount>
							<LocalWHTAmount>0.00</LocalWHTAmount>
							<MappedCreditGLAccount>487900</MappedCreditGLAccount>
							<MappedCreditGLAccountSequence>SEQ2565</MappedCreditGLAccountSequence>
							<MappedDebitGLAccount>471999</MappedDebitGLAccount>
							<MappedDebitGLAccountSequence>SEQ2559</MappedDebitGLAccountSequence>
							<MappedExtraTaxCreditGLAccount>UNKNOWN</MappedExtraTaxCreditGLAccount>
							<MappedExtraTaxCreditGLAccountSequence>UNKNOWN</MappedExtraTaxCreditGLAccountSequence>
							<MappedExtraTaxDebitGLAccount>UNKNOWN</MappedExtraTaxDebitGLAccount>
							<MappedExtraTaxDebitGLAccountSequence>UNKNOWN</MappedExtraTaxDebitGLAccountSequence>
							<MappedTaxCreditGLAccount>UNKNOWN</MappedTaxCreditGLAccount>
							<MappedTaxCreditGLAccountSequence>UNKNOWN</MappedTaxCreditGLAccountSequence>
							<MappedTaxDebitGLAccount>UNKNOWN</MappedTaxDebitGLAccount>
							<MappedTaxDebitGLAccountSequence>UNKNOWN</MappedTaxDebitGLAccountSequence>
							<MappedWithholdingTaxCreditGLAccount>UNKNOWN</MappedWithholdingTaxCreditGLAccount>
							<MappedWithholdingTaxCreditGLAccountSequence>UNKNOWN</MappedWithholdingTaxCreditGLAccountSequence>
							<MappedWithholdingTaxDebitGLAccount>UNKNOWN</MappedWithholdingTaxDebitGLAccount>
							<MappedWithholdingTaxDebitGLAccountSequence>UNKNOWN</MappedWithholdingTaxDebitGLAccountSequence>
							<MappedGLAccount>706005</MappedGLAccount>
							<MappedGLAccountSequence>SEQ2597</MappedGLAccountSequence>
							<Organization>4STITCBDGAZ</Organization>
							<OriginalBatchNumber>0</OriginalBatchNumber>
							<OriginalBatchSequence>0</OriginalBatchSequence>
							<PostingPeriod>202203</PostingPeriod>
							<RevenueRecognitionType>DEP</RevenueRecognitionType>
							<TaxMessageID>MATEXEMPT</TaxMessageID>
							<TransactionCurrency>BDT</TransactionCurrency>
							<TransactionNumber>F220000042</TransactionNumber>
							<TransactionType>REV</TransactionType>
							<TxnAmountInTxnCurrency>-1000.00</TxnAmountInTxnCurrency>
							<TxnAmountInTxnCurrencyWithVAT>-1000.00</TxnAmountInTxnCurrencyWithVAT>
							<TxnType>APS</TxnType>
							<VATAmountInTxnCurrency>0.00</VATAmountInTxnCurrency>
							<VATTaxIDExtraTaxRate>0.000</VATTaxIDExtraTaxRate>
							<VATTaxIDTaxCode>EXEMPT</VATTaxIDTaxCode>
							<VATTaxIDTaxDescription>Exempt Rated</VATTaxIDTaxDescription>
							<VATTaxIDTaxRate>0.000</VATTaxIDTaxRate>
							<VATTaxIDTaxTypeCode>EXT</VATTaxIDTaxTypeCode>
							<WithholdingAmountInTxnCurrency>0.00</WithholdingAmountInTxnCurrency>
							<WithholdingTaxIDTaxRate>0</WithholdingTaxIDTaxRate>
							<ChargeExpenseGroup>FRT</ChargeExpenseGroup>
							<ChargeGroup>FRT</ChargeGroup>
							<ChargeIsActive>TRUE</ChargeIsActive>
							<ChargeOtherGroups>PRC</ChargeOtherGroups>
							<ChargeSalesGroup>FRT</ChargeSalesGroup>
							<ChargeServiceType>SRV</ChargeServiceType>
							<ChargeType>MJA</ChargeType>
							<IsConsolServiceCharge>TRUE</IsConsolServiceCharge>
							<Polarity>P</Polarity>
							<Ledger>AR</Ledger>
							<LegacyAcctJournal>UNKNOWN</LegacyAcctJournal>
							<AcctTaxCode>EXEMPT</AcctTaxCode>
							<NatureVATAcct>C</NatureVATAcct>
							<ARAccountGroup>
								<Code>TPY</Code>
								<Description>THIRD PARTY COMPANY</Description>
							</ARAccountGroup>
							<IsSSC>N</IsSSC>
							<IsGateway>N</IsGateway>
							<Rate>1000.0000</Rate>
							<RateUnit>Unit</RateUnit>
							<Quantity>1.0000</Quantity>
							<QuantityUnit>Unit</QuantityUnit>
							<ChargeCurrencyPrecision>100</ChargeCurrencyPrecision>
							<LocalCurrencyPrecision>100</LocalCurrencyPrecision>
							<TransactionCurrencyPrecision>100</TransactionCurrencyPrecision>
							<BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
							<OHPK>73102663-A13A-4899-8FA8-A735150D3375</OHPK>
							<CostGovtChargeCode>S01510</CostGovtChargeCode>
							<SellGovtChargeCode>S01510</SellGovtChargeCode>
						</PostingJournal>
					</PostingJournalCollection>
					<OpsData>
						<ShipmentCollection>
							<ShipmentCount>1</ShipmentCount>
							<Shipment>
								<BillingBranch>B05</BillingBranch>
								<BillingDepartment>FES</BillingDepartment>
								<CompanyCode>BD1</CompanyCode>
								<JOBID>S00091725</JOBID>
								<RepOps>MMK</RepOps>
								<RepSales>HKO</RepSales>
								<JobStatus>JRC</JobStatus>
								<RelatedOrgs>
									<RelatedOrgCount>1</RelatedOrgCount>
									<RelatedOrg>
										<Address1>SAFURA TOWER (8TH FLOOR),</Address1>
										<Address2>20 KEMAL ATATURK AVENUE, BANANI</Address2>
										<City>DHAKA</City>
										<CompanyCode>BD1</CompanyCode>
										<Country>BD</Country>
										<CountryName>Bangladesh</CountryName>
										<CurrentBatchNumber>31</CurrentBatchNumber>
										<FullName>BOLLORE LOGISTICS BANGLADESH - DHAKA</FullName>
										<JOBID>S00091725</JOBID>
										<JobType>JS</JobType>
										<Location>H</Location>
										<OrgCode>BLXIFFBDDAC</OrgCode>
										<PostCode>1213</PostCode>
										<Role>BBR</Role>
										<RoleName>BillingBranch</RoleName>
										<Source>O</Source>
										<State>13</State>
										<UNLOCO>BDDAC</UNLOCO>
										<ArianeCode>048000</ArianeCode>
										<CRMID>1-6ZKT-1631</CRMID>
										<VATCode>TAX</VATCode>
										<IsValidAddress>FALSE</IsValidAddress>
										<Language>EN-US</Language>
										<AddressType>OFC</AddressType>
										<IsMainAddress>TRUE</IsMainAddress>
										<AddIndex>0</AddIndex>
									</RelatedOrg>
								</RelatedOrgs>
								<ControllerCode>VETIRFRSPP</ControllerCode>
								<ControllerFullName>VETIR SAS</ControllerFullName>
								<GoodsDescription>READYMADE GARMENTS</GoodsDescription>
								<HouseBill>BD100091725</HouseBill>
								<INCO>FOB</INCO>
								<JSDestination>DEHAM</JSDestination>
								<JSOrigin>BDDAC</JSOrigin>
								<ServiceLevelCode>STD</ServiceLevelCode>
								<VerticalType>82</VerticalType>
								<JSActualChargeable>0.000</JSActualChargeable>
								<JSActualVolume>0.000</JSActualVolume>
								<JSActualWeight>0.000</JSActualWeight>
								<JSTransportmode>SEA</JSTransportmode>
								<OuterPacks>0</OuterPacks>
								<PackType>PLT</PackType>
								<ShipmentUnitOfVolume>M3</ShipmentUnitOfVolume>
								<ShipmentUnitOfWeight>T</ShipmentUnitOfWeight>
								<ShipperCode>FBFOOTBDGAZ</ShipperCode>
								<ShipperFullName>F. B. FOOTWEAR LTD</ShipperFullName>
								<ConsigneeCode>VETIRFRSPP</ConsigneeCode>
								<ConsigneeFullName>VETIR SAS</ConsigneeFullName>
								<TradeLine>83</TradeLine>
								<CommodityCode>GEN</CommodityCode>
								<ConsolCollection>
									<ConsolCount>1</ConsolCount>
									<Consol>
										<BillingBranch>B07</BillingBranch>
										<BillingDepartment>DIS</BillingDepartment>
										<CompanyCode>BD1</CompanyCode>
										<JOBID>S00091725</JOBID>
										<JKID>C00060970</JKID>
										<Vessel>KOTA ARIF</Vessel>
										<Voyage>123</Voyage>
										<JKDestination>DEHAM</JKDestination>
										<JKOrigin>BDCGP</JKOrigin>
										<ServiceLevel>STD</ServiceLevel>
										<JKActualChargeable>0</JKActualChargeable>
										<VolumeTotal>0</VolumeTotal>
										<WeightTotal>0</WeightTotal>
										<JKTransportmode>SEA</JKTransportmode>
										<VolumeUnit>M3</VolumeUnit>
										<WeightUnit>T</WeightUnit>
									</Consol>
								</ConsolCollection>
							</Shipment>
						</ShipmentCollection>
					</OpsData>
				</TransactionInfo>
			</UniversalTransaction>
		</Body>
	</UniversalInterchange>
	<UniversalInterchange>
		<Header>
			<RecipientIds>
				<RecipientIdCount>1</RecipientIdCount>
				<RecipientId>
					<SystemMap>
						<AcctSystem>SUN</AcctSystem>
						<AcctCompanyCode>DAT</AcctCompanyCode>
						<CFTMonitorName>DACPEG</CFTMonitorName>
					</SystemMap>
					<ID>ACCOUNTING</ID>
				</RecipientId>
			</RecipientIds>
		</Header>
		<BatchGenTimeInfo>
			<CompanyCode>BD1</CompanyCode>
			<UTC>2022-03-21T05:15:38.143</UTC>
			<LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
			<CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
			<TimeZone>Bangladesh Standard Time</TimeZone>
			<UTCOffset>+06:00</UTCOffset>
			<isDST>0</isDST>
		</BatchGenTimeInfo>
		<BatchCreateTimeInfo>
			<CompanyCode>BD1</CompanyCode>
			<UTC>2022-03-21T05:15:05.827</UTC>
			<LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
			<CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
			<TimeZone>Bangladesh Standard Time</TimeZone>
			<UTCOffset>+06:00</UTCOffset>
			<isDST>0</isDST>
		</BatchCreateTimeInfo>
		<Body>
			<UniversalTransaction>
				<TransactionInfo>
					<AgreedPaymentMethod>V</AgreedPaymentMethod>
					<ARAccountGroup>
						<Code>TPY</Code>
						<Description>THIRD PARTY COMPANY</Description>
					</ARAccountGroup>
					<Branch>
						<Code>B05</Code>
						<Name>BOLLORE LOGISTICS BANGLADESH - DHAKA</Name>
					</Branch>
					<Category>FIN</Category>
					<ComplianceSubType></ComplianceSubType>
					<CreateTime>2022-03-21T05:12:00</CreateTime>
					<Department>
						<Code>FES</Code>
						<Name>Forwarding Export Sea</Name>
					</Department>
					<Description>S00091725</Description>
					<DueDate>2022-03-21T11:12:00</DueDate>
					<ExchangeRate>1.000000</ExchangeRate>
					<ExternalDebtorCode>CT00192</ExternalDebtorCode>
					<InvoiceTerms>COD</InvoiceTerms>
					<InvoiceTermDays>0</InvoiceTermDays>
					<IsCancelled>FALSE</IsCancelled>
					<IsCreatedByMatchingProcess>FALSE</IsCreatedByMatchingProcess>
					<IsPrinted>FALSE</IsPrinted>
					<JOBID>
						<Type>S00091725</Type>
						<Key>JS</Key>
					</JOBID>
					<JobInvoiceNumber>S00091725</JobInvoiceNumber>
					<Ledger>AR</Ledger>
					<LocalCurrency>
						<Code>BDT</Code>
						<Description>Bangladeshi Taka</Description>
						<Precision>100</Precision>
					</LocalCurrency>
					<LocalTotal>2000.00</LocalTotal>
					<LocalVATAmount>300.00</LocalVATAmount>
					<LocalWHTAmount>0.00</LocalWHTAmount>
					<NumberOfSupportingDocuments>1</NumberOfSupportingDocuments>
					<Organization>FBFOOTBDGAZ</Organization>
					<OSCurrency>
						<Code>BDT</Code>
						<Precision>100</Precision>
					</OSCurrency>
					<OSGSTVATAmount>300.00</OSGSTVATAmount>
					<OSTotal>2300.00</OSTotal>
					<OutstandingAmount>2300.00</OutstandingAmount>
					<PostDate>2022-03-21T11:12:00</PostDate>
					<TransactionNumber>F220000040</TransactionNumber>
					<TransactionType>INV</TransactionType>
					<OSTotalWithNoTax>2000.00</OSTotalWithNoTax>
					<LocalTaxableAmount>2000.00</LocalTaxableAmount>
					<OSTaxableAmount>-2000.00</OSTaxableAmount>
					<LocalNonTaxableAmount>0.00</LocalNonTaxableAmount>
					<OSNonTaxableAmount>0.00</OSNonTaxableAmount>
					<CurrentBatchNumber>31</CurrentBatchNumber>
					<OriginalBatchNumber>31</OriginalBatchNumber>
					<HighestBatchNumber>31</HighestBatchNumber>
					<InvoiceDate>2022-03-21T11:12:00</InvoiceDate>
					<LegacyAcctJournal>SALEW</LegacyAcctJournal>
					<InvoiceRemittanceReference>00001039</InvoiceRemittanceReference>
					<CreatorEMail>manish.mukherjee@bollore.com</CreatorEMail>
					<EditorEMail>manish.mukherjee@bollore.com</EditorEMail>
					<CreatorFullName>Manish Mukherjee</CreatorFullName>
					<BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
					<OHPK>C7E7CC49-AA94-445D-82A5-0336D1FA808A</OHPK>
					<RelatedOrgs>
						<RelatedOrgCount>1</RelatedOrgCount>
						<RelatedOrg>
							<Address1>ULOSHARA. KALIAKOIR.</Address1>
							<City>GAZIPUR</City>
							<CompanyCode>BD1</CompanyCode>
							<Country>BD</Country>
							<CountryName>Bangladesh</CountryName>
							<CurrentBatchNumber>31</CurrentBatchNumber>
							<FullName>F. B. FOOTWEAR LTD</FullName>
							<JOBID>S00091725</JOBID>
							<JobType>JS</JobType>
							<Location>I</Location>
							<OrgCode>FBFOOTBDGAZ</OrgCode>
							<PostCode>1750</PostCode>
							<Role>BPT</Role>
							<RoleName>BillingParty</RoleName>
							<Source>O</Source>
							<UNLOCO>BDGAZ</UNLOCO>
							<CRMID>1-TX6AHN</CRMID>
							<VATCode>TAX</VATCode>
							<IsValidAddress>FALSE</IsValidAddress>
							<Language>EN-US</Language>
							<AddressType>OFC</AddressType>
							<IsMainAddress>TRUE</IsMainAddress>
							<AddIndex>0</AddIndex>
						</RelatedOrg>
					</RelatedOrgs>
					<PostingJournalCollection>
						<PostingJournal>
							<AccountHeaderPK>59DE55E7-A274-4612-A6E4-2ACB4E48F0AC</AccountHeaderPK>
							<AccountLinePK>C85E9BB2-DA2B-43EC-9496-0EC104E9CF3D</AccountLinePK>
							<BatchSequence>1</BatchSequence>
							<Branch>BOLLORE LOGISTICS BANGLADESH - DHAKA</Branch>
							<BranchCode>B05</BranchCode>
							<ChargeCode>10841</ChargeCode>
							<ChargeCodeDescription>ORIGIN HANDLING</ChargeCodeDescription>
							<ChargeCodeLocalDescription>ORIGIN HANDLING</ChargeCodeLocalDescription>
							<ChargeCurrency>BDT</ChargeCurrency>
							<ChargeExchangeRate>1.000000</ChargeExchangeRate>
							<ChargeTotalAmount>2000.00</ChargeTotalAmount>
							<ChargeTotalExtraVATAmount>0.00</ChargeTotalExtraVATAmount>
							<ChargeTotalVATAmount>300.00</ChargeTotalVATAmount>
							<CompanyCode>BD1</CompanyCode>
							<CreditGLAccount>Revenue suspense control account(487.1000.0)</CreditGLAccount>
							<CurrentBatchNumber>31</CurrentBatchNumber>
							<DebitGLAccount>Trade debtors control(411.1000.0)</DebitGLAccount>
							<Department>FES</Department>
							<Description>VAT TAX MESSAGE FROM MATRIX</Description>
							<ExternalDebtorCode>CT00192</ExternalDebtorCode>
							<ExtraVATAmountInTxnCurrency>0.00</ExtraVATAmountInTxnCurrency>
							<GLAccountCode>701.1010.0</GLAccountCode>
							<GLAccountDescription>FRT REVENUE ACTUAL</GLAccountDescription>
							<GLPostDate>2022-03-21T11:12:00</GLPostDate>
							<GovernmentReportingChargeCode>S09920</GovernmentReportingChargeCode>
							<GSTVATBasis>FALSE</GSTVATBasis>
							<InvoiceReference>S00091725</InvoiceReference>
							<IsFinalCharge>FALSE</IsFinalCharge>
							<JOBID>S00091725</JOBID>
							<LocalCurrency>BDT</LocalCurrency>
							<LocalExtraVATAmount>0.00</LocalExtraVATAmount>
							<LocalTotalAmount>2000.00</LocalTotalAmount>
							<LocalVATAmount>300.00</LocalVATAmount>
							<LocalWHTAmount>0.00</LocalWHTAmount>
							<MappedCreditGLAccount>487900</MappedCreditGLAccount>
							<MappedCreditGLAccountSequence>SEQ2565</MappedCreditGLAccountSequence>
							<MappedDebitGLAccount>471999</MappedDebitGLAccount>
							<MappedDebitGLAccountSequence>SEQ2559</MappedDebitGLAccountSequence>
							<MappedExtraTaxCreditGLAccount>UNKNOWN</MappedExtraTaxCreditGLAccount>
							<MappedExtraTaxCreditGLAccountSequence>UNKNOWN</MappedExtraTaxCreditGLAccountSequence>
							<MappedExtraTaxDebitGLAccount>UNKNOWN</MappedExtraTaxDebitGLAccount>
							<MappedExtraTaxDebitGLAccountSequence>UNKNOWN</MappedExtraTaxDebitGLAccountSequence>
							<MappedTaxCreditGLAccount>UNKNOWN</MappedTaxCreditGLAccount>
							<MappedTaxCreditGLAccountSequence>SEQ2562</MappedTaxCreditGLAccountSequence>
							<MappedTaxDebitGLAccount>UNKNOWN</MappedTaxDebitGLAccount>
							<MappedTaxDebitGLAccountSequence>SEQ2559</MappedTaxDebitGLAccountSequence>
							<MappedWithholdingTaxCreditGLAccount>UNKNOWN</MappedWithholdingTaxCreditGLAccount>
							<MappedWithholdingTaxCreditGLAccountSequence>UNKNOWN</MappedWithholdingTaxCreditGLAccountSequence>
							<MappedWithholdingTaxDebitGLAccount>UNKNOWN</MappedWithholdingTaxDebitGLAccount>
							<MappedWithholdingTaxDebitGLAccountSequence>UNKNOWN</MappedWithholdingTaxDebitGLAccountSequence>
							<MappedGLAccount>706027</MappedGLAccount>
							<MappedGLAccountSequence>SEQ2600</MappedGLAccountSequence>
							<Organization>FBFOOTBDGAZ</Organization>
							<OriginalBatchNumber>0</OriginalBatchNumber>
							<OriginalBatchSequence>0</OriginalBatchSequence>
							<PostingPeriod>202203</PostingPeriod>
							<RevenueRecognitionType>DEP</RevenueRecognitionType>
							<TaxCreditGLAccount>OUTPUT TAX PAYABLE(445.7000.0)</TaxCreditGLAccount>
							<TaxDebitGLAccount>TRADE DEBTORS CONTROL(411.1000.0)</TaxDebitGLAccount>
							<TaxMessageID>MATVAT</TaxMessageID>
							<TransactionCurrency>BDT</TransactionCurrency>
							<TransactionNumber>F220000040</TransactionNumber>
							<TransactionType>REV</TransactionType>
							<TxnAmountInTxnCurrency>-2000.00</TxnAmountInTxnCurrency>
							<TxnAmountInTxnCurrencyWithVAT>-2300.00</TxnAmountInTxnCurrencyWithVAT>
							<TxnType>APS</TxnType>
							<VATAmountInTxnCurrency>-300.00</VATAmountInTxnCurrency>
							<VATTaxIDExtraTaxRate>0.000</VATTaxIDExtraTaxRate>
							<VATTaxIDTaxCode>VAT</VATTaxIDTaxCode>
							<VATTaxIDTaxDescription>Standard Rated</VATTaxIDTaxDescription>
							<VATTaxIDTaxRate>15.000</VATTaxIDTaxRate>
							<VATTaxIDTaxTypeCode>RAT</VATTaxIDTaxTypeCode>
							<WithholdingAmountInTxnCurrency>0.00</WithholdingAmountInTxnCurrency>
							<WithholdingTaxIDTaxRate>0</WithholdingTaxIDTaxRate>
							<ChargeExpenseGroup>HAO</ChargeExpenseGroup>
							<ChargeGroup>ORG</ChargeGroup>
							<ChargeIsActive>TRUE</ChargeIsActive>
							<ChargeOtherGroups>PRC</ChargeOtherGroups>
							<ChargeSalesGroup>HAO</ChargeSalesGroup>
							<ChargeServiceType>SRV</ChargeServiceType>
							<ChargeType>MJA</ChargeType>
							<IsConsolServiceCharge>TRUE</IsConsolServiceCharge>
							<Polarity>P</Polarity>
							<Ledger>AR</Ledger>
							<LegacyAcctJournal>UNKNOWN</LegacyAcctJournal>
							<AcctTaxCode>VAT</AcctTaxCode>
							<NatureVATAcct>C</NatureVATAcct>
							<ARAccountGroup>
								<Code>TPY</Code>
								<Description>THIRD PARTY COMPANY</Description>
							</ARAccountGroup>
							<IsSSC>N</IsSSC>
							<IsGateway>N</IsGateway>
							<Rate>2000.0000</Rate>
							<RateUnit>Unit</RateUnit>
							<Quantity>1.0000</Quantity>
							<QuantityUnit>Unit</QuantityUnit>
							<ChargeCurrencyPrecision>100</ChargeCurrencyPrecision>
							<LocalCurrencyPrecision>100</LocalCurrencyPrecision>
							<TransactionCurrencyPrecision>100</TransactionCurrencyPrecision>
							<BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
							<OHPK>C7E7CC49-AA94-445D-82A5-0336D1FA808A</OHPK>
							<CostGovtChargeCode>S09920</CostGovtChargeCode>
							<SellGovtChargeCode>S09920</SellGovtChargeCode>
						</PostingJournal>
					</PostingJournalCollection>
					<OpsData>
						<ShipmentCollection>
							<ShipmentCount>1</ShipmentCount>
							<Shipment>
								<BillingBranch>B05</BillingBranch>
								<BillingDepartment>FES</BillingDepartment>
								<CompanyCode>BD1</CompanyCode>
								<JOBID>S00091725</JOBID>
								<RepOps>MMK</RepOps>
								<RepSales>HKO</RepSales>
								<JobStatus>JRC</JobStatus>
								<RelatedOrgs>
									<RelatedOrgCount>1</RelatedOrgCount>
									<RelatedOrg>
										<Address1>SAFURA TOWER (8TH FLOOR),</Address1>
										<Address2>20 KEMAL ATATURK AVENUE, BANANI</Address2>
										<City>DHAKA</City>
										<CompanyCode>BD1</CompanyCode>
										<Country>BD</Country>
										<CountryName>Bangladesh</CountryName>
										<CurrentBatchNumber>31</CurrentBatchNumber>
										<FullName>BOLLORE LOGISTICS BANGLADESH - DHAKA</FullName>
										<JOBID>S00091725</JOBID>
										<JobType>JS</JobType>
										<Location>H</Location>
										<OrgCode>BLXIFFBDDAC</OrgCode>
										<PostCode>1213</PostCode>
										<Role>BBR</Role>
										<RoleName>BillingBranch</RoleName>
										<Source>O</Source>
										<State>13</State>
										<UNLOCO>BDDAC</UNLOCO>
										<ArianeCode>048000</ArianeCode>
										<CRMID>1-6ZKT-1631</CRMID>
										<VATCode>TAX</VATCode>
										<IsValidAddress>FALSE</IsValidAddress>
										<Language>EN-US</Language>
										<AddressType>OFC</AddressType>
										<IsMainAddress>TRUE</IsMainAddress>
										<AddIndex>0</AddIndex>
									</RelatedOrg>
								</RelatedOrgs>
								<ControllerCode>VETIRFRSPP</ControllerCode>
								<ControllerFullName>VETIR SAS</ControllerFullName>
								<GoodsDescription>READYMADE GARMENTS</GoodsDescription>
								<HouseBill>BD100091725</HouseBill>
								<INCO>FOB</INCO>
								<JSDestination>DEHAM</JSDestination>
								<JSOrigin>BDDAC</JSOrigin>
								<ServiceLevelCode>STD</ServiceLevelCode>
								<VerticalType>82</VerticalType>
								<JSActualChargeable>0.000</JSActualChargeable>
								<JSActualVolume>0.000</JSActualVolume>
								<JSActualWeight>0.000</JSActualWeight>
								<JSTransportmode>SEA</JSTransportmode>
								<OuterPacks>0</OuterPacks>
								<PackType>PLT</PackType>
								<ShipmentUnitOfVolume>M3</ShipmentUnitOfVolume>
								<ShipmentUnitOfWeight>T</ShipmentUnitOfWeight>
								<ShipperCode>FBFOOTBDGAZ</ShipperCode>
								<ShipperFullName>F. B. FOOTWEAR LTD</ShipperFullName>
								<ConsigneeCode>VETIRFRSPP</ConsigneeCode>
								<ConsigneeFullName>VETIR SAS</ConsigneeFullName>
								<TradeLine>83</TradeLine>
								<CommodityCode>GEN</CommodityCode>
								<ConsolCollection>
									<ConsolCount>1</ConsolCount>
									<Consol>
										<BillingBranch>B07</BillingBranch>
										<BillingDepartment>DIS</BillingDepartment>
										<CompanyCode>BD1</CompanyCode>
										<JOBID>S00091725</JOBID>
										<JKID>C00060970</JKID>
										<Vessel>KOTA ARIF</Vessel>
										<Voyage>123</Voyage>
										<JKDestination>DEHAM</JKDestination>
										<JKOrigin>BDCGP</JKOrigin>
										<ServiceLevel>STD</ServiceLevel>
										<JKActualChargeable>0</JKActualChargeable>
										<VolumeTotal>0</VolumeTotal>
										<WeightTotal>0</WeightTotal>
										<JKTransportmode>SEA</JKTransportmode>
										<VolumeUnit>M3</VolumeUnit>
										<WeightUnit>T</WeightUnit>
									</Consol>
								</ConsolCollection>
							</Shipment>
						</ShipmentCollection>
					</OpsData>
				</TransactionInfo>
			</UniversalTransaction>
		</Body>
	</UniversalInterchange>
	<UniversalInterchange>
		<Header>
			<RecipientIds>
				<RecipientIdCount>2</RecipientIdCount>
				<RecipientId>
					<SystemMap>
						<AcctSystem>SUN</AcctSystem>
						<AcctCompanyCode>DAT</AcctCompanyCode>
						<CFTMonitorName>DACPEG</CFTMonitorName>
					</SystemMap>
					<ID>ACCOUNTING</ID>
				</RecipientId>
				<RecipientId>
					<ID>LEGACY</ID>
				</RecipientId>
			</RecipientIds>
		</Header>
		<BatchGenTimeInfo>
			<CompanyCode>BD1</CompanyCode>
			<UTC>2022-03-21T05:15:38.143</UTC>
			<LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
			<CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
			<TimeZone>Bangladesh Standard Time</TimeZone>
			<UTCOffset>+06:00</UTCOffset>
			<isDST>0</isDST>
		</BatchGenTimeInfo>
		<BatchCreateTimeInfo>
			<CompanyCode>BD1</CompanyCode>
			<UTC>2022-03-21T05:15:05.827</UTC>
			<LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
			<CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
			<TimeZone>Bangladesh Standard Time</TimeZone>
			<UTCOffset>+06:00</UTCOffset>
			<isDST>0</isDST>
		</BatchCreateTimeInfo>
		<Body>
			<UniversalTransaction>
				<TransactionInfo>
					<AgreedPaymentMethod>V</AgreedPaymentMethod>
					<APAccountGroup>
						<Code>TPY</Code>
						<Description>THIRD PARTY COMPANY</Description>
					</APAccountGroup>
					<ARAccountGroup>
						<Code>TPY</Code>
						<Description>THIRD PARTY COMPANY</Description>
					</ARAccountGroup>
					<Branch>
						<Code>B05</Code>
						<Name>BOLLORE LOGISTICS BANGLADESH - DHAKA</Name>
					</Branch>
					<Category>CUR</Category>
					<ComplianceSubType></ComplianceSubType>
					<CreateTime>2022-03-21T05:12:00</CreateTime>
					<Department>
						<Code>FES</Code>
						<Name>Forwarding Export Sea</Name>
					</Department>
					<Description>S00091725</Description>
					<DueDate>2022-05-30T00:00:00</DueDate>
					<ExchangeRate>85.975000</ExchangeRate>
					<ExternalDebtorCode>CS01258</ExternalDebtorCode>
					<ExternalCreditorCode>SS01258</ExternalCreditorCode>
					<InvoiceTerms>MTH</InvoiceTerms>
					<InvoiceTermDays>60</InvoiceTermDays>
					<IsCancelled>FALSE</IsCancelled>
					<IsCreatedByMatchingProcess>FALSE</IsCreatedByMatchingProcess>
					<IsPrinted>TRUE</IsPrinted>
					<JOBID>
						<Type>S00091725</Type>
						<Key>JS</Key>
					</JOBID>
					<JobInvoiceNumber>S00091725/A</JobInvoiceNumber>
					<Ledger>AR</Ledger>
					<LocalCurrency>
						<Code>BDT</Code>
						<Description>Bangladeshi Taka</Description>
						<Precision>100</Precision>
					</LocalCurrency>
					<LocalTotal>257925.00</LocalTotal>
					<LocalVATAmount>38688.75</LocalVATAmount>
					<LocalWHTAmount>0.00</LocalWHTAmount>
					<NumberOfSupportingDocuments>1</NumberOfSupportingDocuments>
					<Organization>BLXIFFCNBJS</Organization>
					<OSCurrency>
						<Code>USD</Code>
						<Precision>100</Precision>
					</OSCurrency>
					<OSGSTVATAmount>450.00</OSGSTVATAmount>
					<OSTotal>3450.00</OSTotal>
					<OutstandingAmount>296613.75</OutstandingAmount>
					<PostDate>2022-03-21T11:12:00</PostDate>
					<TransactionNumber>F220000041</TransactionNumber>
					<TransactionType>INV</TransactionType>
					<OSTotalWithNoTax>3000.00</OSTotalWithNoTax>
					<LocalTaxableAmount>257925.00</LocalTaxableAmount>
					<OSTaxableAmount>-3000.00</OSTaxableAmount>
					<LocalNonTaxableAmount>0.00</LocalNonTaxableAmount>
					<OSNonTaxableAmount>0.00</OSNonTaxableAmount>
					<CurrentBatchNumber>31</CurrentBatchNumber>
					<OriginalBatchNumber>31</OriginalBatchNumber>
					<HighestBatchNumber>31</HighestBatchNumber>
					<InvoiceDate>2022-03-21T11:12:00</InvoiceDate>
					<LegacyAcctJournal>SALEW</LegacyAcctJournal>
					<InvoiceRemittanceReference>00001040</InvoiceRemittanceReference>
					<CreatorEMail>manish.mukherjee@bollore.com</CreatorEMail>
					<EditorEMail>manish.mukherjee@bollore.com</EditorEMail>
					<CreatorFullName>Manish Mukherjee</CreatorFullName>
					<BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
					<OHPK>1491E5F9-27E8-440B-9734-8A3F9A0ADF27</OHPK>
					<RelatedOrgs>
						<RelatedOrgCount>1</RelatedOrgCount>
						<RelatedOrg>
							<Address1>BEIJING BRANCH, 11TH FL. TOWER A</Address1>
							<Address2>8 NORTH DONGSANHUAN ROAD CHAOYANG D</Address2>
							<City>BEIJING</City>
							<CompanyCode>BD1</CompanyCode>
							<Country>CN</Country>
							<CountryName>China</CountryName>
							<CurrentBatchNumber>31</CurrentBatchNumber>
							<FullName>BEIJING BRANCH</FullName>
							<JOBID>S00091725</JOBID>
							<JobType>JS</JobType>
							<Location>I</Location>
							<OrgCode>BLXIFFCNBJS</OrgCode>
							<PostCode>100004</PostCode>
							<Role>BPT</Role>
							<RoleName>BillingParty</RoleName>
							<Source>O</Source>
							<UNLOCO>CNBJS</UNLOCO>
							<NettingCode>12580000000</NettingCode>
							<ArianeCode>085801</ArianeCode>
							<CRMID>1-5AGY-1475</CRMID>
							<VATCode>TAX</VATCode>
							<IsValidAddress>FALSE</IsValidAddress>
							<Language>EN-US</Language>
							<AddressType>OFC</AddressType>
							<IsMainAddress>TRUE</IsMainAddress>
							<AddIndex>0</AddIndex>
						</RelatedOrg>
					</RelatedOrgs>
					<PostingJournalCollection>
						<PostingJournal>
							<AccountHeaderPK>C846145E-9BCB-4B34-95F9-238B6C319E9E</AccountHeaderPK>
							<AccountLinePK>6B7AE83A-1212-497D-94CE-EFC7E70B72B0</AccountLinePK>
							<BatchSequence>2</BatchSequence>
							<Branch>BOLLORE LOGISTICS BANGLADESH - DHAKA</Branch>
							<BranchCode>B05</BranchCode>
							<ChargeCode>10841</ChargeCode>
							<ChargeCodeDescription>ORIGIN HANDLING</ChargeCodeDescription>
							<ChargeCodeLocalDescription>ORIGIN HANDLING</ChargeCodeLocalDescription>
							<ChargeCurrency>USD</ChargeCurrency>
							<ChargeExchangeRate>85.975000</ChargeExchangeRate>
							<ChargeTotalAmount>3000.00</ChargeTotalAmount>
							<ChargeTotalExtraVATAmount>0.00</ChargeTotalExtraVATAmount>
							<ChargeTotalVATAmount>450.00</ChargeTotalVATAmount>
							<CompanyCode>BD1</CompanyCode>
							<CreditGLAccount>Revenue suspense control account(487.1000.0)</CreditGLAccount>
							<CurrentBatchNumber>31</CurrentBatchNumber>
							<DebitGLAccount>Trade debtors control(411.1000.0)</DebitGLAccount>
							<Department>FES</Department>
							<Description>VAT TAX MESSAGE FROM MATRIX</Description>
							<ExternalDebtorCode>CS01258</ExternalDebtorCode>
							<ExtraVATAmountInTxnCurrency>0.00</ExtraVATAmountInTxnCurrency>
							<GLAccountCode>701.1010.0</GLAccountCode>
							<GLAccountDescription>FRT REVENUE ACTUAL</GLAccountDescription>
							<GLPostDate>2022-03-21T11:12:00</GLPostDate>
							<GovernmentReportingChargeCode>S09920</GovernmentReportingChargeCode>
							<GSTVATBasis>FALSE</GSTVATBasis>
							<InvoiceReference>S00091725/A</InvoiceReference>
							<IsFinalCharge>FALSE</IsFinalCharge>
							<JOBID>S00091725</JOBID>
							<LocalCurrency>BDT</LocalCurrency>
							<LocalExtraVATAmount>0.00</LocalExtraVATAmount>
							<LocalTotalAmount>257925.00</LocalTotalAmount>
							<LocalVATAmount>38688.75</LocalVATAmount>
							<LocalWHTAmount>0.00</LocalWHTAmount>
							<MappedCreditGLAccount>487900</MappedCreditGLAccount>
							<MappedCreditGLAccountSequence>SEQ2565</MappedCreditGLAccountSequence>
							<MappedDebitGLAccount>471999</MappedDebitGLAccount>
							<MappedDebitGLAccountSequence>SEQ2559</MappedDebitGLAccountSequence>
							<MappedExtraTaxCreditGLAccount>UNKNOWN</MappedExtraTaxCreditGLAccount>
							<MappedExtraTaxCreditGLAccountSequence>UNKNOWN</MappedExtraTaxCreditGLAccountSequence>
							<MappedExtraTaxDebitGLAccount>UNKNOWN</MappedExtraTaxDebitGLAccount>
							<MappedExtraTaxDebitGLAccountSequence>UNKNOWN</MappedExtraTaxDebitGLAccountSequence>
							<MappedTaxCreditGLAccount>UNKNOWN</MappedTaxCreditGLAccount>
							<MappedTaxCreditGLAccountSequence>SEQ2562</MappedTaxCreditGLAccountSequence>
							<MappedTaxDebitGLAccount>UNKNOWN</MappedTaxDebitGLAccount>
							<MappedTaxDebitGLAccountSequence>SEQ2559</MappedTaxDebitGLAccountSequence>
							<MappedWithholdingTaxCreditGLAccount>UNKNOWN</MappedWithholdingTaxCreditGLAccount>
							<MappedWithholdingTaxCreditGLAccountSequence>UNKNOWN</MappedWithholdingTaxCreditGLAccountSequence>
							<MappedWithholdingTaxDebitGLAccount>UNKNOWN</MappedWithholdingTaxDebitGLAccount>
							<MappedWithholdingTaxDebitGLAccountSequence>UNKNOWN</MappedWithholdingTaxDebitGLAccountSequence>
							<MappedGLAccount>706027</MappedGLAccount>
							<MappedGLAccountSequence>SEQ2600</MappedGLAccountSequence>
							<Organization>BLXIFFCNBJS</Organization>
							<OriginalBatchNumber>0</OriginalBatchNumber>
							<OriginalBatchSequence>0</OriginalBatchSequence>
							<PostingPeriod>202203</PostingPeriod>
							<RevenueRecognitionType>DEP</RevenueRecognitionType>
							<TaxCreditGLAccount>OUTPUT TAX PAYABLE(445.7000.0)</TaxCreditGLAccount>
							<TaxDebitGLAccount>TRADE DEBTORS CONTROL(411.1000.0)</TaxDebitGLAccount>
							<TaxMessageID>MATVAT</TaxMessageID>
							<TransactionCurrency>USD</TransactionCurrency>
							<TransactionNumber>F220000041</TransactionNumber>
							<TransactionType>REV</TransactionType>
							<TxnAmountInTxnCurrency>-3000.00</TxnAmountInTxnCurrency>
							<TxnAmountInTxnCurrencyWithVAT>-3450.00</TxnAmountInTxnCurrencyWithVAT>
							<TxnType>APS</TxnType>
							<VATAmountInTxnCurrency>-450.00</VATAmountInTxnCurrency>
							<VATTaxIDExtraTaxRate>0.000</VATTaxIDExtraTaxRate>
							<VATTaxIDTaxCode>VAT</VATTaxIDTaxCode>
							<VATTaxIDTaxDescription>Standard Rated</VATTaxIDTaxDescription>
							<VATTaxIDTaxRate>15.000</VATTaxIDTaxRate>
							<VATTaxIDTaxTypeCode>RAT</VATTaxIDTaxTypeCode>
							<WithholdingAmountInTxnCurrency>0.00</WithholdingAmountInTxnCurrency>
							<WithholdingTaxIDTaxRate>0</WithholdingTaxIDTaxRate>
							<ChargeExpenseGroup>HAO</ChargeExpenseGroup>
							<ChargeGroup>ORG</ChargeGroup>
							<ChargeIsActive>TRUE</ChargeIsActive>
							<ChargeOtherGroups>PRC</ChargeOtherGroups>
							<ChargeSalesGroup>HAO</ChargeSalesGroup>
							<ChargeServiceType>SRV</ChargeServiceType>
							<ChargeType>MJA</ChargeType>
							<IsConsolServiceCharge>TRUE</IsConsolServiceCharge>
							<Polarity>P</Polarity>
							<Ledger>AR</Ledger>
							<LegacyAcctJournal>UNKNOWN</LegacyAcctJournal>
							<AcctTaxCode>VAT</AcctTaxCode>
							<NatureVATAcct>C</NatureVATAcct>
							<APAccountGroup>
								<Code>TPY</Code>
								<Description>THIRD PARTY COMPANY</Description>
							</APAccountGroup>
							<ARAccountGroup>
								<Code>TPY</Code>
								<Description>THIRD PARTY COMPANY</Description>
							</ARAccountGroup>
							<IsSSC>N</IsSSC>
							<IsGateway>N</IsGateway>
							<Rate>3000.0000</Rate>
							<RateUnit>Unit</RateUnit>
							<Quantity>1.0000</Quantity>
							<QuantityUnit>Unit</QuantityUnit>
							<ChargeCurrencyPrecision>100</ChargeCurrencyPrecision>
							<LocalCurrencyPrecision>100</LocalCurrencyPrecision>
							<TransactionCurrencyPrecision>100</TransactionCurrencyPrecision>
							<BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
							<OHPK>1491E5F9-27E8-440B-9734-8A3F9A0ADF27</OHPK>
							<CostGovtChargeCode>S09920</CostGovtChargeCode>
							<SellGovtChargeCode>S09920</SellGovtChargeCode>
						</PostingJournal>
					</PostingJournalCollection>
					<OpsData>
						<ShipmentCollection>
							<ShipmentCount>1</ShipmentCount>
							<Shipment>
								<BillingBranch>B05</BillingBranch>
								<BillingDepartment>FES</BillingDepartment>
								<CompanyCode>BD1</CompanyCode>
								<JOBID>S00091725</JOBID>
								<RepOps>MMK</RepOps>
								<RepSales>HKO</RepSales>
								<JobStatus>JRC</JobStatus>
								<RelatedOrgs>
									<RelatedOrgCount>1</RelatedOrgCount>
									<RelatedOrg>
										<Address1>SAFURA TOWER (8TH FLOOR),</Address1>
										<Address2>20 KEMAL ATATURK AVENUE, BANANI</Address2>
										<City>DHAKA</City>
										<CompanyCode>BD1</CompanyCode>
										<Country>BD</Country>
										<CountryName>Bangladesh</CountryName>
										<CurrentBatchNumber>31</CurrentBatchNumber>
										<FullName>BOLLORE LOGISTICS BANGLADESH - DHAKA</FullName>
										<JOBID>S00091725</JOBID>
										<JobType>JS</JobType>
										<Location>H</Location>
										<OrgCode>BLXIFFBDDAC</OrgCode>
										<PostCode>1213</PostCode>
										<Role>BBR</Role>
										<RoleName>BillingBranch</RoleName>
										<Source>O</Source>
										<State>13</State>
										<UNLOCO>BDDAC</UNLOCO>
										<ArianeCode>048000</ArianeCode>
										<CRMID>1-6ZKT-1631</CRMID>
										<VATCode>TAX</VATCode>
										<IsValidAddress>FALSE</IsValidAddress>
										<Language>EN-US</Language>
										<AddressType>OFC</AddressType>
										<IsMainAddress>TRUE</IsMainAddress>
										<AddIndex>0</AddIndex>
									</RelatedOrg>
								</RelatedOrgs>
								<ControllerCode>VETIRFRSPP</ControllerCode>
								<ControllerFullName>VETIR SAS</ControllerFullName>
								<GoodsDescription>READYMADE GARMENTS</GoodsDescription>
								<HouseBill>BD100091725</HouseBill>
								<INCO>FOB</INCO>
								<JSDestination>DEHAM</JSDestination>
								<JSOrigin>BDDAC</JSOrigin>
								<ServiceLevelCode>STD</ServiceLevelCode>
								<VerticalType>82</VerticalType>
								<JSActualChargeable>0.000</JSActualChargeable>
								<JSActualVolume>0.000</JSActualVolume>
								<JSActualWeight>0.000</JSActualWeight>
								<JSTransportmode>SEA</JSTransportmode>
								<OuterPacks>0</OuterPacks>
								<PackType>PLT</PackType>
								<ShipmentUnitOfVolume>M3</ShipmentUnitOfVolume>
								<ShipmentUnitOfWeight>T</ShipmentUnitOfWeight>
								<ShipperCode>FBFOOTBDGAZ</ShipperCode>
								<ShipperFullName>F. B. FOOTWEAR LTD</ShipperFullName>
								<ConsigneeCode>VETIRFRSPP</ConsigneeCode>
								<ConsigneeFullName>VETIR SAS</ConsigneeFullName>
								<TradeLine>83</TradeLine>
								<CommodityCode>GEN</CommodityCode>
								<ConsolCollection>
									<ConsolCount>1</ConsolCount>
									<Consol>
										<BillingBranch>B07</BillingBranch>
										<BillingDepartment>DIS</BillingDepartment>
										<CompanyCode>BD1</CompanyCode>
										<JOBID>S00091725</JOBID>
										<JKID>C00060970</JKID>
										<Vessel>KOTA ARIF</Vessel>
										<Voyage>123</Voyage>
										<JKDestination>DEHAM</JKDestination>
										<JKOrigin>BDCGP</JKOrigin>
										<ServiceLevel>STD</ServiceLevel>
										<JKActualChargeable>0</JKActualChargeable>
										<VolumeTotal>0</VolumeTotal>
										<WeightTotal>0</WeightTotal>
										<JKTransportmode>SEA</JKTransportmode>
										<VolumeUnit>M3</VolumeUnit>
										<WeightUnit>T</WeightUnit>
									</Consol>
								</ConsolCollection>
							</Shipment>
						</ShipmentCollection>
					</OpsData>
				</TransactionInfo>
			</UniversalTransaction>
		</Body>
	</UniversalInterchange>
</UniversalInterchangeBatch>
            ";

                #endregion

                #region XML 2

                string XML2 = @"                
<UniversalInterchangeBatch>
   <Batch>31</Batch>
   <Company>BD1</Company>
   <BatchGenTimeInfo>
      <CompanyCode>BD1</CompanyCode>
      <UTC>2022-03-21T05:15:38.143</UTC>
      <LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
      <CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
      <TimeZone>Bangladesh Standard Time</TimeZone>
      <UTCOffset>+06:00</UTCOffset>
      <isDST>0</isDST>
   </BatchGenTimeInfo>
   <BatchCreateTimeInfo>
      <CompanyCode>BD1</CompanyCode>
      <UTC>2022-03-21T05:15:05.827</UTC>
      <LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
      <CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
      <TimeZone>Bangladesh Standard Time</TimeZone>
      <UTCOffset>+06:00</UTCOffset>
      <isDST>0</isDST>
   </BatchCreateTimeInfo>
   <UniversalInterchange>
      <Header>
         <RecipientIds>
            <RecipientIdCount>1</RecipientIdCount>
            <RecipientId>
               <SystemMap>
                  <AcctSystem>SUN</AcctSystem>
                  <AcctCompanyCode>DAT</AcctCompanyCode>
                  <CFTMonitorName>DACPEG</CFTMonitorName>
               </SystemMap>
               <ID>ACCOUNTING</ID>
            </RecipientId>
         </RecipientIds>
      </Header>
      <BatchGenTimeInfo>
         <CompanyCode>BD1</CompanyCode>
         <UTC>2022-03-21T05:15:38.143</UTC>
         <LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
         <CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
         <TimeZone>Bangladesh Standard Time</TimeZone>
         <UTCOffset>+06:00</UTCOffset>
         <isDST>0</isDST>
      </BatchGenTimeInfo>
      <BatchCreateTimeInfo>
         <CompanyCode>BD1</CompanyCode>
         <UTC>2022-03-21T05:15:05.827</UTC>
         <LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
         <CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
         <TimeZone>Bangladesh Standard Time</TimeZone>
         <UTCOffset>+06:00</UTCOffset>
         <isDST>0</isDST>
      </BatchCreateTimeInfo>
      <Body>
         <UniversalTransaction>
            <TransactionInfo>
               <AgreedPaymentMethod>V</AgreedPaymentMethod>
               <ARAccountGroup>
                  <Code>TPY</Code>
                  <Description>THIRD PARTY COMPANY</Description>
               </ARAccountGroup>
               <Branch>
                  <Code>B05</Code>
                  <Name>BOLLORE LOGISTICS BANGLADESH - DHAKA</Name>
               </Branch>
               <Category>FIN</Category>
               <ComplianceSubType />
               <CreateTime>2022-03-21T05:12:00</CreateTime>
               <Department>
                  <Code>FES</Code>
                  <Name>Forwarding Export Sea</Name>
               </Department>
               <Description>S00091725</Description>
               <DueDate>2022-03-21T11:12:00</DueDate>
               <ExchangeRate>1.000000</ExchangeRate>
               <ExternalDebtorCode>CT02821</ExternalDebtorCode>
               <InvoiceTerms>COD</InvoiceTerms>
               <InvoiceTermDays>0</InvoiceTermDays>
               <IsCancelled>FALSE</IsCancelled>
               <IsCreatedByMatchingProcess>FALSE</IsCreatedByMatchingProcess>
               <IsPrinted>FALSE</IsPrinted>
               <JOBID>
                  <Type>S00091725</Type>
                  <Key>JS</Key>
               </JOBID>
               <JobInvoiceNumber>S00091725/B</JobInvoiceNumber>
               <Ledger>AR</Ledger>
               <LocalCurrency>
                  <Code>BDT</Code>
                  <Description>Bangladeshi Taka</Description>
                  <Precision>100</Precision>
               </LocalCurrency>
               <LocalTotal>1000.00</LocalTotal>
               <LocalVATAmount>0.00</LocalVATAmount>
               <LocalWHTAmount>0.00</LocalWHTAmount>
               <NumberOfSupportingDocuments>1</NumberOfSupportingDocuments>
               <Organization>4STITCBDGAZ</Organization>
               <OSCurrency>
                  <Code>BDT</Code>
                  <Precision>100</Precision>
               </OSCurrency>
               <OSGSTVATAmount>0.00</OSGSTVATAmount>
               <OSTotal>1000.00</OSTotal>
               <OutstandingAmount>1000.00</OutstandingAmount>
               <PostDate>2022-03-21T11:12:00</PostDate>
               <TransactionNumber>F220000042</TransactionNumber>
               <TransactionType>INV</TransactionType>
               <OSTotalWithNoTax>1000.00</OSTotalWithNoTax>
               <LocalTaxableAmount>0.00</LocalTaxableAmount>
               <OSTaxableAmount>0.00</OSTaxableAmount>
               <LocalNonTaxableAmount>1000.00</LocalNonTaxableAmount>
               <OSNonTaxableAmount>1000.00</OSNonTaxableAmount>
               <CurrentBatchNumber>31</CurrentBatchNumber>
               <OriginalBatchNumber>31</OriginalBatchNumber>
               <HighestBatchNumber>31</HighestBatchNumber>
               <InvoiceDate>2022-03-21T11:12:00</InvoiceDate>
               <LegacyAcctJournal>SALEW</LegacyAcctJournal>
               <InvoiceRemittanceReference>00001041</InvoiceRemittanceReference>
               <CreatorEMail>manish.mukherjee@bollore.com</CreatorEMail>
               <EditorEMail>manish.mukherjee@bollore.com</EditorEMail>
               <CreatorFullName>Manish Mukherjee</CreatorFullName>
               <BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
               <OHPK>73102663-A13A-4899-8FA8-A735150D3375</OHPK>
               <RelatedOrgs>
                  <RelatedOrgCount>1</RelatedOrgCount>
                  <RelatedOrg>
                     <Address1>WEST SHAILDUBI WARD-05</Address1>
                     <Address2>P.S.- KASHIMPUR P.O.- KASHIMOUR</Address2>
                     <City>GAZIPUR</City>
                     <CompanyCode>BD1</CompanyCode>
                     <Country>BD</Country>
                     <CountryName>Bangladesh</CountryName>
                     <CurrentBatchNumber>31</CurrentBatchNumber>
                     <FullName>4 STITCH KNIT COMPOSITE LTD</FullName>
                     <JOBID>S00091725</JOBID>
                     <JobType>JS</JobType>
                     <Location>I</Location>
                     <OrgCode>4STITCBDGAZ</OrgCode>
                     <PostCode>1700</PostCode>
                     <Role>BPT</Role>
                     <RoleName>BillingParty</RoleName>
                     <Source>O</Source>
                     <UNLOCO>BDGAZ</UNLOCO>
                     <VATCode>TAX</VATCode>
                     <IsValidAddress>FALSE</IsValidAddress>
                     <Language>EN-US</Language>
                     <AddressType>OFC</AddressType>
                     <IsMainAddress>TRUE</IsMainAddress>
                     <AddIndex>0</AddIndex>
                  </RelatedOrg>
               </RelatedOrgs>
               <PostingJournalCollection>
                  <PostingJournal>
                     <AccountHeaderPK>5531C195-E285-483F-B3BE-4A1DFEE5C576</AccountHeaderPK>
                     <AccountLinePK>BB6F7906-E583-4498-8BF2-A05B1B76F608</AccountLinePK>
                     <BatchSequence>3</BatchSequence>
                     <Branch>BOLLORE LOGISTICS BANGLADESH - DHAKA</Branch>
                     <BranchCode>B05</BranchCode>
                     <ChargeCode>10569</ChargeCode>
                     <ChargeCodeDescription>BASIC FREIGHT CHARGE</ChargeCodeDescription>
                     <ChargeCodeLocalDescription>BASIC FREIGHT CHARGE</ChargeCodeLocalDescription>
                     <ChargeCurrency>BDT</ChargeCurrency>
                     <ChargeExchangeRate>1.000000</ChargeExchangeRate>
                     <ChargeTotalAmount>1000.00</ChargeTotalAmount>
                     <ChargeTotalExtraVATAmount>0.00</ChargeTotalExtraVATAmount>
                     <ChargeTotalVATAmount>0.00</ChargeTotalVATAmount>
                     <CompanyCode>BD1</CompanyCode>
                     <CreditGLAccount>Revenue suspense control account(487.1000.0)</CreditGLAccount>
                     <CurrentBatchNumber>31</CurrentBatchNumber>
                     <DebitGLAccount>Trade debtors control(411.1000.0)</DebitGLAccount>
                     <Department>FES</Department>
                     <Description>EXEMPT TAX MESSAGE MATRIX</Description>
                     <ExternalDebtorCode>CT02821</ExternalDebtorCode>
                     <ExtraVATAmountInTxnCurrency>0.00</ExtraVATAmountInTxnCurrency>
                     <GLAccountCode>701.1010.0</GLAccountCode>
                     <GLAccountDescription>FRT REVENUE ACTUAL</GLAccountDescription>
                     <GLPostDate>2022-03-21T11:12:00</GLPostDate>
                     <GovernmentReportingChargeCode>S01510</GovernmentReportingChargeCode>
                     <GSTVATBasis>FALSE</GSTVATBasis>
                     <InvoiceReference>S00091725/B</InvoiceReference>
                     <IsFinalCharge>FALSE</IsFinalCharge>
                     <JOBID>S00091725</JOBID>
                     <LocalCurrency>BDT</LocalCurrency>
                     <LocalExtraVATAmount>0.00</LocalExtraVATAmount>
                     <LocalTotalAmount>1000.00</LocalTotalAmount>
                     <LocalVATAmount>0.00</LocalVATAmount>
                     <LocalWHTAmount>0.00</LocalWHTAmount>
                     <MappedCreditGLAccount>487900</MappedCreditGLAccount>
                     <MappedCreditGLAccountSequence>SEQ2565</MappedCreditGLAccountSequence>
                     <MappedDebitGLAccount>471999</MappedDebitGLAccount>
                     <MappedDebitGLAccountSequence>SEQ2559</MappedDebitGLAccountSequence>
                     <MappedExtraTaxCreditGLAccount>UNKNOWN</MappedExtraTaxCreditGLAccount>
                     <MappedExtraTaxCreditGLAccountSequence>UNKNOWN</MappedExtraTaxCreditGLAccountSequence>
                     <MappedExtraTaxDebitGLAccount>UNKNOWN</MappedExtraTaxDebitGLAccount>
                     <MappedExtraTaxDebitGLAccountSequence>UNKNOWN</MappedExtraTaxDebitGLAccountSequence>
                     <MappedTaxCreditGLAccount>UNKNOWN</MappedTaxCreditGLAccount>
                     <MappedTaxCreditGLAccountSequence>UNKNOWN</MappedTaxCreditGLAccountSequence>
                     <MappedTaxDebitGLAccount>UNKNOWN</MappedTaxDebitGLAccount>
                     <MappedTaxDebitGLAccountSequence>UNKNOWN</MappedTaxDebitGLAccountSequence>
                     <MappedWithholdingTaxCreditGLAccount>UNKNOWN</MappedWithholdingTaxCreditGLAccount>
                     <MappedWithholdingTaxCreditGLAccountSequence>UNKNOWN</MappedWithholdingTaxCreditGLAccountSequence>
                     <MappedWithholdingTaxDebitGLAccount>UNKNOWN</MappedWithholdingTaxDebitGLAccount>
                     <MappedWithholdingTaxDebitGLAccountSequence>UNKNOWN</MappedWithholdingTaxDebitGLAccountSequence>
                     <MappedGLAccount>706005</MappedGLAccount>
                     <MappedGLAccountSequence>SEQ2597</MappedGLAccountSequence>
                     <Organization>4STITCBDGAZ</Organization>
                     <OriginalBatchNumber>0</OriginalBatchNumber>
                     <OriginalBatchSequence>0</OriginalBatchSequence>
                     <PostingPeriod>202203</PostingPeriod>
                     <RevenueRecognitionType>DEP</RevenueRecognitionType>
                     <TaxMessageID>MATEXEMPT</TaxMessageID>
                     <TransactionCurrency>BDT</TransactionCurrency>
                     <TransactionNumber>F220000042</TransactionNumber>
                     <TransactionType>REV</TransactionType>
                     <TxnAmountInTxnCurrency>-1000.00</TxnAmountInTxnCurrency>
                     <TxnAmountInTxnCurrencyWithVAT>-1000.00</TxnAmountInTxnCurrencyWithVAT>
                     <TxnType>APS</TxnType>
                     <VATAmountInTxnCurrency>0.00</VATAmountInTxnCurrency>
                     <VATTaxIDExtraTaxRate>0.000</VATTaxIDExtraTaxRate>
                     <VATTaxIDTaxCode>EXEMPT</VATTaxIDTaxCode>
                     <VATTaxIDTaxDescription>Exempt Rated</VATTaxIDTaxDescription>
                     <VATTaxIDTaxRate>0.000</VATTaxIDTaxRate>
                     <VATTaxIDTaxTypeCode>EXT</VATTaxIDTaxTypeCode>
                     <WithholdingAmountInTxnCurrency>0.00</WithholdingAmountInTxnCurrency>
                     <WithholdingTaxIDTaxRate>0</WithholdingTaxIDTaxRate>
                     <ChargeExpenseGroup>FRT</ChargeExpenseGroup>
                     <ChargeGroup>FRT</ChargeGroup>
                     <ChargeIsActive>TRUE</ChargeIsActive>
                     <ChargeOtherGroups>PRC</ChargeOtherGroups>
                     <ChargeSalesGroup>FRT</ChargeSalesGroup>
                     <ChargeServiceType>SRV</ChargeServiceType>
                     <ChargeType>MJA</ChargeType>
                     <IsConsolServiceCharge>TRUE</IsConsolServiceCharge>
                     <Polarity>P</Polarity>
                     <Ledger>AR</Ledger>
                     <LegacyAcctJournal>UNKNOWN</LegacyAcctJournal>
                     <AcctTaxCode>EXEMPT</AcctTaxCode>
                     <NatureVATAcct>C</NatureVATAcct>
                     <ARAccountGroup>
                        <Code>TPY</Code>
                        <Description>THIRD PARTY COMPANY</Description>
                     </ARAccountGroup>
                     <IsSSC>N</IsSSC>
                     <IsGateway>N</IsGateway>
                     <Rate>1000.0000</Rate>
                     <RateUnit>Unit</RateUnit>
                     <Quantity>1.0000</Quantity>
                     <QuantityUnit>Unit</QuantityUnit>
                     <ChargeCurrencyPrecision>100</ChargeCurrencyPrecision>
                     <LocalCurrencyPrecision>100</LocalCurrencyPrecision>
                     <TransactionCurrencyPrecision>100</TransactionCurrencyPrecision>
                     <BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
                     <OHPK>73102663-A13A-4899-8FA8-A735150D3375</OHPK>
                     <CostGovtChargeCode>S01510</CostGovtChargeCode>
                     <SellGovtChargeCode>S01510</SellGovtChargeCode>
                  </PostingJournal>
               </PostingJournalCollection>
               <OpsData>
                  <ShipmentCollection>
                     <ShipmentCount>1</ShipmentCount>
                     <Shipment>
                        <BillingBranch>B05</BillingBranch>
                        <BillingDepartment>FES</BillingDepartment>
                        <CompanyCode>BD1</CompanyCode>
                        <JOBID>S00091725</JOBID>
                        <RepOps>MMK</RepOps>
                        <RepSales>HKO</RepSales>
                        <JobStatus>JRC</JobStatus>
                        <RelatedOrgs>
                           <RelatedOrgCount>1</RelatedOrgCount>
                           <RelatedOrg>
                              <Address1>SAFURA TOWER (8TH FLOOR),</Address1>
                              <Address2>20 KEMAL ATATURK AVENUE, BANANI</Address2>
                              <City>DHAKA</City>
                              <CompanyCode>BD1</CompanyCode>
                              <Country>BD</Country>
                              <CountryName>Bangladesh</CountryName>
                              <CurrentBatchNumber>31</CurrentBatchNumber>
                              <FullName>BOLLORE LOGISTICS BANGLADESH - DHAKA</FullName>
                              <JOBID>S00091725</JOBID>
                              <JobType>JS</JobType>
                              <Location>H</Location>
                              <OrgCode>BLXIFFBDDAC</OrgCode>
                              <PostCode>1213</PostCode>
                              <Role>BBR</Role>
                              <RoleName>BillingBranch</RoleName>
                              <Source>O</Source>
                              <State>13</State>
                              <UNLOCO>BDDAC</UNLOCO>
                              <ArianeCode>048000</ArianeCode>
                              <CRMID>1-6ZKT-1631</CRMID>
                              <VATCode>TAX</VATCode>
                              <IsValidAddress>FALSE</IsValidAddress>
                              <Language>EN-US</Language>
                              <AddressType>OFC</AddressType>
                              <IsMainAddress>TRUE</IsMainAddress>
                              <AddIndex>0</AddIndex>
                           </RelatedOrg>
                        </RelatedOrgs>
                        <ControllerCode>VETIRFRSPP</ControllerCode>
                        <ControllerFullName>VETIR SAS</ControllerFullName>
                        <GoodsDescription>READYMADE GARMENTS</GoodsDescription>
                        <HouseBill>BD100091725</HouseBill>
                        <INCO>FOB</INCO>
                        <JSDestination>DEHAM</JSDestination>
                        <JSOrigin>BDDAC</JSOrigin>
                        <ServiceLevelCode>STD</ServiceLevelCode>
                        <VerticalType>82</VerticalType>
                        <JSActualChargeable>0.000</JSActualChargeable>
                        <JSActualVolume>0.000</JSActualVolume>
                        <JSActualWeight>0.000</JSActualWeight>
                        <JSTransportmode>SEA</JSTransportmode>
                        <OuterPacks>0</OuterPacks>
                        <PackType>PLT</PackType>
                        <ShipmentUnitOfVolume>M3</ShipmentUnitOfVolume>
                        <ShipmentUnitOfWeight>T</ShipmentUnitOfWeight>
                        <ShipperCode>FBFOOTBDGAZ</ShipperCode>
                        <ShipperFullName>F. B. FOOTWEAR LTD</ShipperFullName>
                        <ConsigneeCode>VETIRFRSPP</ConsigneeCode>
                        <ConsigneeFullName>VETIR SAS</ConsigneeFullName>
                        <TradeLine>83</TradeLine>
                        <CommodityCode>GEN</CommodityCode>
                        <ConsolCollection>
                           <ConsolCount>1</ConsolCount>
                           <Consol>
                              <BillingBranch>B07</BillingBranch>
                              <BillingDepartment>DIS</BillingDepartment>
                              <CompanyCode>BD1</CompanyCode>
                              <JOBID>S00091725</JOBID>
                              <JKID>C00060970</JKID>
                              <Vessel>KOTA ARIF</Vessel>
                              <Voyage>123</Voyage>
                              <JKDestination>DEHAM</JKDestination>
                              <JKOrigin>BDCGP</JKOrigin>
                              <ServiceLevel>STD</ServiceLevel>
                              <JKActualChargeable>0</JKActualChargeable>
                              <VolumeTotal>0</VolumeTotal>
                              <WeightTotal>0</WeightTotal>
                              <JKTransportmode>SEA</JKTransportmode>
                              <VolumeUnit>M3</VolumeUnit>
                              <WeightUnit>T</WeightUnit>
                           </Consol>
                        </ConsolCollection>
                     </Shipment>
                  </ShipmentCollection>
               </OpsData>
            </TransactionInfo>
         </UniversalTransaction>
      </Body>
   </UniversalInterchange>
   <UniversalInterchange>
      <Header>
         <RecipientIds>
            <RecipientIdCount>1</RecipientIdCount>
            <RecipientId>
               <SystemMap>
                  <AcctSystem>SUN</AcctSystem>
                  <AcctCompanyCode>DAT</AcctCompanyCode>
                  <CFTMonitorName>DACPEG</CFTMonitorName>
               </SystemMap>
               <ID>ACCOUNTING</ID>
            </RecipientId>
         </RecipientIds>
      </Header>
      <BatchGenTimeInfo>
         <CompanyCode>BD1</CompanyCode>
         <UTC>2022-03-21T05:15:38.143</UTC>
         <LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
         <CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
         <TimeZone>Bangladesh Standard Time</TimeZone>
         <UTCOffset>+06:00</UTCOffset>
         <isDST>0</isDST>
      </BatchGenTimeInfo>
      <BatchCreateTimeInfo>
         <CompanyCode>BD1</CompanyCode>
         <UTC>2022-03-21T05:15:05.827</UTC>
         <LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
         <CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
         <TimeZone>Bangladesh Standard Time</TimeZone>
         <UTCOffset>+06:00</UTCOffset>
         <isDST>0</isDST>
      </BatchCreateTimeInfo>
      <Body>
         <UniversalTransaction>
            <TransactionInfo>
               <AgreedPaymentMethod>V</AgreedPaymentMethod>
               <ARAccountGroup>
                  <Code>TPY</Code>
                  <Description>THIRD PARTY COMPANY</Description>
               </ARAccountGroup>
               <Branch>
                  <Code>B05</Code>
                  <Name>BOLLORE LOGISTICS BANGLADESH - DHAKA</Name>
               </Branch>
               <Category>FIN</Category>
               <ComplianceSubType />
               <CreateTime>2022-03-21T05:12:00</CreateTime>
               <Department>
                  <Code>FES</Code>
                  <Name>Forwarding Export Sea</Name>
               </Department>
               <Description>S00091725</Description>
               <DueDate>2022-03-21T11:12:00</DueDate>
               <ExchangeRate>1.000000</ExchangeRate>
               <ExternalDebtorCode>CT00192</ExternalDebtorCode>
               <InvoiceTerms>COD</InvoiceTerms>
               <InvoiceTermDays>0</InvoiceTermDays>
               <IsCancelled>FALSE</IsCancelled>
               <IsCreatedByMatchingProcess>FALSE</IsCreatedByMatchingProcess>
               <IsPrinted>FALSE</IsPrinted>
               <JOBID>
                  <Type>S00091725</Type>
                  <Key>JS</Key>
               </JOBID>
               <JobInvoiceNumber>S00091725</JobInvoiceNumber>
               <Ledger>AR</Ledger>
               <LocalCurrency>
                  <Code>BDT</Code>
                  <Description>Bangladeshi Taka</Description>
                  <Precision>100</Precision>
               </LocalCurrency>
               <LocalTotal>2000.00</LocalTotal>
               <LocalVATAmount>300.00</LocalVATAmount>
               <LocalWHTAmount>0.00</LocalWHTAmount>
               <NumberOfSupportingDocuments>1</NumberOfSupportingDocuments>
               <Organization>FBFOOTBDGAZ</Organization>
               <OSCurrency>
                  <Code>BDT</Code>
                  <Precision>100</Precision>
               </OSCurrency>
               <OSGSTVATAmount>300.00</OSGSTVATAmount>
               <OSTotal>2300.00</OSTotal>
               <OutstandingAmount>2300.00</OutstandingAmount>
               <PostDate>2022-03-21T11:12:00</PostDate>
               <TransactionNumber>F220000040</TransactionNumber>
               <TransactionType>INV</TransactionType>
               <OSTotalWithNoTax>2000.00</OSTotalWithNoTax>
               <LocalTaxableAmount>2000.00</LocalTaxableAmount>
               <OSTaxableAmount>-2000.00</OSTaxableAmount>
               <LocalNonTaxableAmount>0.00</LocalNonTaxableAmount>
               <OSNonTaxableAmount>0.00</OSNonTaxableAmount>
               <CurrentBatchNumber>31</CurrentBatchNumber>
               <OriginalBatchNumber>31</OriginalBatchNumber>
               <HighestBatchNumber>31</HighestBatchNumber>
               <InvoiceDate>2022-03-21T11:12:00</InvoiceDate>
               <LegacyAcctJournal>SALEW</LegacyAcctJournal>
               <InvoiceRemittanceReference>00001039</InvoiceRemittanceReference>
               <CreatorEMail>manish.mukherjee@bollore.com</CreatorEMail>
               <EditorEMail>manish.mukherjee@bollore.com</EditorEMail>
               <CreatorFullName>Manish Mukherjee</CreatorFullName>
               <BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
               <OHPK>C7E7CC49-AA94-445D-82A5-0336D1FA808A</OHPK>
               <RelatedOrgs>
                  <RelatedOrgCount>1</RelatedOrgCount>
                  <RelatedOrg>
                     <Address1>ULOSHARA. KALIAKOIR.</Address1>
                     <City>GAZIPUR</City>
                     <CompanyCode>BD1</CompanyCode>
                     <Country>BD</Country>
                     <CountryName>Bangladesh</CountryName>
                     <CurrentBatchNumber>31</CurrentBatchNumber>
                     <FullName>F. B. FOOTWEAR LTD</FullName>
                     <JOBID>S00091725</JOBID>
                     <JobType>JS</JobType>
                     <Location>I</Location>
                     <OrgCode>FBFOOTBDGAZ</OrgCode>
                     <PostCode>1750</PostCode>
                     <Role>BPT</Role>
                     <RoleName>BillingParty</RoleName>
                     <Source>O</Source>
                     <UNLOCO>BDGAZ</UNLOCO>
                     <CRMID>1-TX6AHN</CRMID>
                     <VATCode>TAX</VATCode>
                     <IsValidAddress>FALSE</IsValidAddress>
                     <Language>EN-US</Language>
                     <AddressType>OFC</AddressType>
                     <IsMainAddress>TRUE</IsMainAddress>
                     <AddIndex>0</AddIndex>
                  </RelatedOrg>
               </RelatedOrgs>
               <PostingJournalCollection>
                  <PostingJournal>
                     <AccountHeaderPK>59DE55E7-A274-4612-A6E4-2ACB4E48F0AC</AccountHeaderPK>
                     <AccountLinePK>C85E9BB2-DA2B-43EC-9496-0EC104E9CF3D</AccountLinePK>
                     <BatchSequence>1</BatchSequence>
                     <Branch>BOLLORE LOGISTICS BANGLADESH - DHAKA</Branch>
                     <BranchCode>B05</BranchCode>
                     <ChargeCode>10841</ChargeCode>
                     <ChargeCodeDescription>ORIGIN HANDLING</ChargeCodeDescription>
                     <ChargeCodeLocalDescription>ORIGIN HANDLING</ChargeCodeLocalDescription>
                     <ChargeCurrency>BDT</ChargeCurrency>
                     <ChargeExchangeRate>1.000000</ChargeExchangeRate>
                     <ChargeTotalAmount>2000.00</ChargeTotalAmount>
                     <ChargeTotalExtraVATAmount>0.00</ChargeTotalExtraVATAmount>
                     <ChargeTotalVATAmount>300.00</ChargeTotalVATAmount>
                     <CompanyCode>BD1</CompanyCode>
                     <CreditGLAccount>Revenue suspense control account(487.1000.0)</CreditGLAccount>
                     <CurrentBatchNumber>31</CurrentBatchNumber>
                     <DebitGLAccount>Trade debtors control(411.1000.0)</DebitGLAccount>
                     <Department>FES</Department>
                     <Description>VAT TAX MESSAGE FROM MATRIX</Description>
                     <ExternalDebtorCode>CT00192</ExternalDebtorCode>
                     <ExtraVATAmountInTxnCurrency>0.00</ExtraVATAmountInTxnCurrency>
                     <GLAccountCode>701.1010.0</GLAccountCode>
                     <GLAccountDescription>FRT REVENUE ACTUAL</GLAccountDescription>
                     <GLPostDate>2022-03-21T11:12:00</GLPostDate>
                     <GovernmentReportingChargeCode>S09920</GovernmentReportingChargeCode>
                     <GSTVATBasis>FALSE</GSTVATBasis>
                     <InvoiceReference>S00091725</InvoiceReference>
                     <IsFinalCharge>FALSE</IsFinalCharge>
                     <JOBID>S00091725</JOBID>
                     <LocalCurrency>BDT</LocalCurrency>
                     <LocalExtraVATAmount>0.00</LocalExtraVATAmount>
                     <LocalTotalAmount>2000.00</LocalTotalAmount>
                     <LocalVATAmount>300.00</LocalVATAmount>
                     <LocalWHTAmount>0.00</LocalWHTAmount>
                     <MappedCreditGLAccount>487900</MappedCreditGLAccount>
                     <MappedCreditGLAccountSequence>SEQ2565</MappedCreditGLAccountSequence>
                     <MappedDebitGLAccount>471999</MappedDebitGLAccount>
                     <MappedDebitGLAccountSequence>SEQ2559</MappedDebitGLAccountSequence>
                     <MappedExtraTaxCreditGLAccount>UNKNOWN</MappedExtraTaxCreditGLAccount>
                     <MappedExtraTaxCreditGLAccountSequence>UNKNOWN</MappedExtraTaxCreditGLAccountSequence>
                     <MappedExtraTaxDebitGLAccount>UNKNOWN</MappedExtraTaxDebitGLAccount>
                     <MappedExtraTaxDebitGLAccountSequence>UNKNOWN</MappedExtraTaxDebitGLAccountSequence>
                     <MappedTaxCreditGLAccount>UNKNOWN</MappedTaxCreditGLAccount>
                     <MappedTaxCreditGLAccountSequence>SEQ2562</MappedTaxCreditGLAccountSequence>
                     <MappedTaxDebitGLAccount>UNKNOWN</MappedTaxDebitGLAccount>
                     <MappedTaxDebitGLAccountSequence>SEQ2559</MappedTaxDebitGLAccountSequence>
                     <MappedWithholdingTaxCreditGLAccount>UNKNOWN</MappedWithholdingTaxCreditGLAccount>
                     <MappedWithholdingTaxCreditGLAccountSequence>UNKNOWN</MappedWithholdingTaxCreditGLAccountSequence>
                     <MappedWithholdingTaxDebitGLAccount>UNKNOWN</MappedWithholdingTaxDebitGLAccount>
                     <MappedWithholdingTaxDebitGLAccountSequence>UNKNOWN</MappedWithholdingTaxDebitGLAccountSequence>
                     <MappedGLAccount>706027</MappedGLAccount>
                     <MappedGLAccountSequence>SEQ2600</MappedGLAccountSequence>
                     <Organization>FBFOOTBDGAZ</Organization>
                     <OriginalBatchNumber>0</OriginalBatchNumber>
                     <OriginalBatchSequence>0</OriginalBatchSequence>
                     <PostingPeriod>202203</PostingPeriod>
                     <RevenueRecognitionType>DEP</RevenueRecognitionType>
                     <TaxCreditGLAccount>OUTPUT TAX PAYABLE(445.7000.0)</TaxCreditGLAccount>
                     <TaxDebitGLAccount>TRADE DEBTORS CONTROL(411.1000.0)</TaxDebitGLAccount>
                     <TaxMessageID>MATVAT</TaxMessageID>
                     <TransactionCurrency>BDT</TransactionCurrency>
                     <TransactionNumber>F220000040</TransactionNumber>
                     <TransactionType>REV</TransactionType>
                     <TxnAmountInTxnCurrency>-2000.00</TxnAmountInTxnCurrency>
                     <TxnAmountInTxnCurrencyWithVAT>-2300.00</TxnAmountInTxnCurrencyWithVAT>
                     <TxnType>APS</TxnType>
                     <VATAmountInTxnCurrency>-300.00</VATAmountInTxnCurrency>
                     <VATTaxIDExtraTaxRate>0.000</VATTaxIDExtraTaxRate>
                     <VATTaxIDTaxCode>VAT</VATTaxIDTaxCode>
                     <VATTaxIDTaxDescription>Standard Rated</VATTaxIDTaxDescription>
                     <VATTaxIDTaxRate>15.000</VATTaxIDTaxRate>
                     <VATTaxIDTaxTypeCode>RAT</VATTaxIDTaxTypeCode>
                     <WithholdingAmountInTxnCurrency>0.00</WithholdingAmountInTxnCurrency>
                     <WithholdingTaxIDTaxRate>0</WithholdingTaxIDTaxRate>
                     <ChargeExpenseGroup>HAO</ChargeExpenseGroup>
                     <ChargeGroup>ORG</ChargeGroup>
                     <ChargeIsActive>TRUE</ChargeIsActive>
                     <ChargeOtherGroups>PRC</ChargeOtherGroups>
                     <ChargeSalesGroup>HAO</ChargeSalesGroup>
                     <ChargeServiceType>SRV</ChargeServiceType>
                     <ChargeType>MJA</ChargeType>
                     <IsConsolServiceCharge>TRUE</IsConsolServiceCharge>
                     <Polarity>P</Polarity>
                     <Ledger>AR</Ledger>
                     <LegacyAcctJournal>UNKNOWN</LegacyAcctJournal>
                     <AcctTaxCode>VAT</AcctTaxCode>
                     <NatureVATAcct>C</NatureVATAcct>
                     <ARAccountGroup>
                        <Code>TPY</Code>
                        <Description>THIRD PARTY COMPANY</Description>
                     </ARAccountGroup>
                     <IsSSC>N</IsSSC>
                     <IsGateway>N</IsGateway>
                     <Rate>2000.0000</Rate>
                     <RateUnit>Unit</RateUnit>
                     <Quantity>1.0000</Quantity>
                     <QuantityUnit>Unit</QuantityUnit>
                     <ChargeCurrencyPrecision>100</ChargeCurrencyPrecision>
                     <LocalCurrencyPrecision>100</LocalCurrencyPrecision>
                     <TransactionCurrencyPrecision>100</TransactionCurrencyPrecision>
                     <BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
                     <OHPK>C7E7CC49-AA94-445D-82A5-0336D1FA808A</OHPK>
                     <CostGovtChargeCode>S09920</CostGovtChargeCode>
                     <SellGovtChargeCode>S09920</SellGovtChargeCode>
                  </PostingJournal>
               </PostingJournalCollection>
               <OpsData>
                  <ShipmentCollection>
                     <ShipmentCount>1</ShipmentCount>
                     <Shipment>
                        <BillingBranch>B05</BillingBranch>
                        <BillingDepartment>FES</BillingDepartment>
                        <CompanyCode>BD1</CompanyCode>
                        <JOBID>S00091725</JOBID>
                        <RepOps>MMK</RepOps>
                        <RepSales>HKO</RepSales>
                        <JobStatus>JRC</JobStatus>
                        <RelatedOrgs>
                           <RelatedOrgCount>1</RelatedOrgCount>
                           <RelatedOrg>
                              <Address1>SAFURA TOWER (8TH FLOOR),</Address1>
                              <Address2>20 KEMAL ATATURK AVENUE, BANANI</Address2>
                              <City>DHAKA</City>
                              <CompanyCode>BD1</CompanyCode>
                              <Country>BD</Country>
                              <CountryName>Bangladesh</CountryName>
                              <CurrentBatchNumber>31</CurrentBatchNumber>
                              <FullName>BOLLORE LOGISTICS BANGLADESH - DHAKA</FullName>
                              <JOBID>S00091725</JOBID>
                              <JobType>JS</JobType>
                              <Location>H</Location>
                              <OrgCode>BLXIFFBDDAC</OrgCode>
                              <PostCode>1213</PostCode>
                              <Role>BBR</Role>
                              <RoleName>BillingBranch</RoleName>
                              <Source>O</Source>
                              <State>13</State>
                              <UNLOCO>BDDAC</UNLOCO>
                              <ArianeCode>048000</ArianeCode>
                              <CRMID>1-6ZKT-1631</CRMID>
                              <VATCode>TAX</VATCode>
                              <IsValidAddress>FALSE</IsValidAddress>
                              <Language>EN-US</Language>
                              <AddressType>OFC</AddressType>
                              <IsMainAddress>TRUE</IsMainAddress>
                              <AddIndex>0</AddIndex>
                           </RelatedOrg>
                        </RelatedOrgs>
                        <ControllerCode>VETIRFRSPP</ControllerCode>
                        <ControllerFullName>VETIR SAS</ControllerFullName>
                        <GoodsDescription>READYMADE GARMENTS</GoodsDescription>
                        <HouseBill>BD100091725</HouseBill>
                        <INCO>FOB</INCO>
                        <JSDestination>DEHAM</JSDestination>
                        <JSOrigin>BDDAC</JSOrigin>
                        <ServiceLevelCode>STD</ServiceLevelCode>
                        <VerticalType>82</VerticalType>
                        <JSActualChargeable>0.000</JSActualChargeable>
                        <JSActualVolume>0.000</JSActualVolume>
                        <JSActualWeight>0.000</JSActualWeight>
                        <JSTransportmode>SEA</JSTransportmode>
                        <OuterPacks>0</OuterPacks>
                        <PackType>PLT</PackType>
                        <ShipmentUnitOfVolume>M3</ShipmentUnitOfVolume>
                        <ShipmentUnitOfWeight>T</ShipmentUnitOfWeight>
                        <ShipperCode>FBFOOTBDGAZ</ShipperCode>
                        <ShipperFullName>F. B. FOOTWEAR LTD</ShipperFullName>
                        <ConsigneeCode>VETIRFRSPP</ConsigneeCode>
                        <ConsigneeFullName>VETIR SAS</ConsigneeFullName>
                        <TradeLine>83</TradeLine>
                        <CommodityCode>GEN</CommodityCode>
                        <ConsolCollection>
                           <ConsolCount>1</ConsolCount>
                           <Consol>
                              <BillingBranch>B07</BillingBranch>
                              <BillingDepartment>DIS</BillingDepartment>
                              <CompanyCode>BD1</CompanyCode>
                              <JOBID>S00091725</JOBID>
                              <JKID>C00060970</JKID>
                              <Vessel>KOTA ARIF</Vessel>
                              <Voyage>123</Voyage>
                              <JKDestination>DEHAM</JKDestination>
                              <JKOrigin>BDCGP</JKOrigin>
                              <ServiceLevel>STD</ServiceLevel>
                              <JKActualChargeable>0</JKActualChargeable>
                              <VolumeTotal>0</VolumeTotal>
                              <WeightTotal>0</WeightTotal>
                              <JKTransportmode>SEA</JKTransportmode>
                              <VolumeUnit>M3</VolumeUnit>
                              <WeightUnit>T</WeightUnit>
                           </Consol>
                        </ConsolCollection>
                     </Shipment>
                  </ShipmentCollection>
               </OpsData>
            </TransactionInfo>
         </UniversalTransaction>
      </Body>
   </UniversalInterchange>
   <UniversalInterchange>
      <Header>
         <RecipientIds>
            <RecipientIdCount>2</RecipientIdCount>
            <RecipientId>
               <SystemMap>
                  <AcctSystem>SUN</AcctSystem>
                  <AcctCompanyCode>DAT</AcctCompanyCode>
                  <CFTMonitorName>DACPEG</CFTMonitorName>
               </SystemMap>
               <ID>ACCOUNTING</ID>
            </RecipientId>
            <RecipientId>
               <ID>LEGACY</ID>
            </RecipientId>
         </RecipientIds>
      </Header>
      <BatchGenTimeInfo>
         <CompanyCode>BD1</CompanyCode>
         <UTC>2022-03-21T05:15:38.143</UTC>
         <LocalSystemTime>2022-03-21T06:15:38.143</LocalSystemTime>
         <CompanyTime>2022-03-21T11:15:38.147</CompanyTime>
         <TimeZone>Bangladesh Standard Time</TimeZone>
         <UTCOffset>+06:00</UTCOffset>
         <isDST>0</isDST>
      </BatchGenTimeInfo>
      <BatchCreateTimeInfo>
         <CompanyCode>BD1</CompanyCode>
         <UTC>2022-03-21T05:15:05.827</UTC>
         <LocalSystemTime>2022-03-21T06:15:05.827</LocalSystemTime>
         <CompanyTime>2022-03-21T11:15:05.83</CompanyTime>
         <TimeZone>Bangladesh Standard Time</TimeZone>
         <UTCOffset>+06:00</UTCOffset>
         <isDST>0</isDST>
      </BatchCreateTimeInfo>
      <Body>
         <UniversalTransaction>
            <TransactionInfo>
               <AgreedPaymentMethod>V</AgreedPaymentMethod>
               <APAccountGroup>
                  <Code>TPY</Code>
                  <Description>THIRD PARTY COMPANY</Description>
               </APAccountGroup>
               <ARAccountGroup>
                  <Code>TPY</Code>
                  <Description>THIRD PARTY COMPANY</Description>
               </ARAccountGroup>
               <Branch>
                  <Code>B05</Code>
                  <Name>BOLLORE LOGISTICS BANGLADESH - DHAKA</Name>
               </Branch>
               <Category>CUR</Category>
               <ComplianceSubType />
               <CreateTime>2022-03-21T05:12:00</CreateTime>
               <Department>
                  <Code>FES</Code>
                  <Name>Forwarding Export Sea</Name>
               </Department>
               <Description>S00091725</Description>
               <DueDate>2022-05-30T00:00:00</DueDate>
               <ExchangeRate>85.975000</ExchangeRate>
               <ExternalDebtorCode>CS01258</ExternalDebtorCode>
               <ExternalCreditorCode>SS01258</ExternalCreditorCode>
               <InvoiceTerms>MTH</InvoiceTerms>
               <InvoiceTermDays>60</InvoiceTermDays>
               <IsCancelled>FALSE</IsCancelled>
               <IsCreatedByMatchingProcess>FALSE</IsCreatedByMatchingProcess>
               <IsPrinted>TRUE</IsPrinted>
               <JOBID>
                  <Type>S00091725</Type>
                  <Key>JS</Key>
               </JOBID>
               <JobInvoiceNumber>S00091725/A</JobInvoiceNumber>
               <Ledger>AR</Ledger>
               <LocalCurrency>
                  <Code>BDT</Code>
                  <Description>Bangladeshi Taka</Description>
                  <Precision>100</Precision>
               </LocalCurrency>
               <LocalTotal>257925.00</LocalTotal>
               <LocalVATAmount>38688.75</LocalVATAmount>
               <LocalWHTAmount>0.00</LocalWHTAmount>
               <NumberOfSupportingDocuments>1</NumberOfSupportingDocuments>
               <Organization>BLXIFFCNBJS</Organization>
               <OSCurrency>
                  <Code>USD</Code>
                  <Precision>100</Precision>
               </OSCurrency>
               <OSGSTVATAmount>450.00</OSGSTVATAmount>
               <OSTotal>3450.00</OSTotal>
               <OutstandingAmount>296613.75</OutstandingAmount>
               <PostDate>2022-03-21T11:12:00</PostDate>
               <TransactionNumber>F220000041</TransactionNumber>
               <TransactionType>INV</TransactionType>
               <OSTotalWithNoTax>3000.00</OSTotalWithNoTax>
               <LocalTaxableAmount>257925.00</LocalTaxableAmount>
               <OSTaxableAmount>-3000.00</OSTaxableAmount>
               <LocalNonTaxableAmount>0.00</LocalNonTaxableAmount>
               <OSNonTaxableAmount>0.00</OSNonTaxableAmount>
               <CurrentBatchNumber>31</CurrentBatchNumber>
               <OriginalBatchNumber>31</OriginalBatchNumber>
               <HighestBatchNumber>31</HighestBatchNumber>
               <InvoiceDate>2022-03-21T11:12:00</InvoiceDate>
               <LegacyAcctJournal>SALEW</LegacyAcctJournal>
               <InvoiceRemittanceReference>00001040</InvoiceRemittanceReference>
               <CreatorEMail>manish.mukherjee@bollore.com</CreatorEMail>
               <EditorEMail>manish.mukherjee@bollore.com</EditorEMail>
               <CreatorFullName>Manish Mukherjee</CreatorFullName>
               <BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
               <OHPK>1491E5F9-27E8-440B-9734-8A3F9A0ADF27</OHPK>
               <RelatedOrgs>
                  <RelatedOrgCount>1</RelatedOrgCount>
                  <RelatedOrg>
                     <Address1>BEIJING BRANCH, 11TH FL. TOWER A</Address1>
                     <Address2>8 NORTH DONGSANHUAN ROAD CHAOYANG D</Address2>
                     <City>BEIJING</City>
                     <CompanyCode>BD1</CompanyCode>
                     <Country>CN</Country>
                     <CountryName>China</CountryName>
                     <CurrentBatchNumber>31</CurrentBatchNumber>
                     <FullName>BEIJING BRANCH</FullName>
                     <JOBID>S00091725</JOBID>
                     <JobType>JS</JobType>
                     <Location>I</Location>
                     <OrgCode>BLXIFFCNBJS</OrgCode>
                     <PostCode>100004</PostCode>
                     <Role>BPT</Role>
                     <RoleName>BillingParty</RoleName>
                     <Source>O</Source>
                     <UNLOCO>CNBJS</UNLOCO>
                     <NettingCode>12580000000</NettingCode>
                     <ArianeCode>085801</ArianeCode>
                     <CRMID>1-5AGY-1475</CRMID>
                     <VATCode>TAX</VATCode>
                     <IsValidAddress>FALSE</IsValidAddress>
                     <Language>EN-US</Language>
                     <AddressType>OFC</AddressType>
                     <IsMainAddress>TRUE</IsMainAddress>
                     <AddIndex>0</AddIndex>
                  </RelatedOrg>
               </RelatedOrgs>
               <PostingJournalCollection>
                  <PostingJournal>
                     <AccountHeaderPK>C846145E-9BCB-4B34-95F9-238B6C319E9E</AccountHeaderPK>
                     <AccountLinePK>6B7AE83A-1212-497D-94CE-EFC7E70B72B0</AccountLinePK>
                     <BatchSequence>2</BatchSequence>
                     <Branch>BOLLORE LOGISTICS BANGLADESH - DHAKA</Branch>
                     <BranchCode>B05</BranchCode>
                     <ChargeCode>10841</ChargeCode>
                     <ChargeCodeDescription>ORIGIN HANDLING</ChargeCodeDescription>
                     <ChargeCodeLocalDescription>ORIGIN HANDLING</ChargeCodeLocalDescription>
                     <ChargeCurrency>USD</ChargeCurrency>
                     <ChargeExchangeRate>85.975000</ChargeExchangeRate>
                     <ChargeTotalAmount>3000.00</ChargeTotalAmount>
                     <ChargeTotalExtraVATAmount>0.00</ChargeTotalExtraVATAmount>
                     <ChargeTotalVATAmount>450.00</ChargeTotalVATAmount>
                     <CompanyCode>BD1</CompanyCode>
                     <CreditGLAccount>Revenue suspense control account(487.1000.0)</CreditGLAccount>
                     <CurrentBatchNumber>31</CurrentBatchNumber>
                     <DebitGLAccount>Trade debtors control(411.1000.0)</DebitGLAccount>
                     <Department>FES</Department>
                     <Description>VAT TAX MESSAGE FROM MATRIX</Description>
                     <ExternalDebtorCode>CS01258</ExternalDebtorCode>
                     <ExtraVATAmountInTxnCurrency>0.00</ExtraVATAmountInTxnCurrency>
                     <GLAccountCode>701.1010.0</GLAccountCode>
                     <GLAccountDescription>FRT REVENUE ACTUAL</GLAccountDescription>
                     <GLPostDate>2022-03-21T11:12:00</GLPostDate>
                     <GovernmentReportingChargeCode>S09920</GovernmentReportingChargeCode>
                     <GSTVATBasis>FALSE</GSTVATBasis>
                     <InvoiceReference>S00091725/A</InvoiceReference>
                     <IsFinalCharge>FALSE</IsFinalCharge>
                     <JOBID>S00091725</JOBID>
                     <LocalCurrency>BDT</LocalCurrency>
                     <LocalExtraVATAmount>0.00</LocalExtraVATAmount>
                     <LocalTotalAmount>257925.00</LocalTotalAmount>
                     <LocalVATAmount>38688.75</LocalVATAmount>
                     <LocalWHTAmount>0.00</LocalWHTAmount>
                     <MappedCreditGLAccount>487900</MappedCreditGLAccount>
                     <MappedCreditGLAccountSequence>SEQ2565</MappedCreditGLAccountSequence>
                     <MappedDebitGLAccount>471999</MappedDebitGLAccount>
                     <MappedDebitGLAccountSequence>SEQ2559</MappedDebitGLAccountSequence>
                     <MappedExtraTaxCreditGLAccount>UNKNOWN</MappedExtraTaxCreditGLAccount>
                     <MappedExtraTaxCreditGLAccountSequence>UNKNOWN</MappedExtraTaxCreditGLAccountSequence>
                     <MappedExtraTaxDebitGLAccount>UNKNOWN</MappedExtraTaxDebitGLAccount>
                     <MappedExtraTaxDebitGLAccountSequence>UNKNOWN</MappedExtraTaxDebitGLAccountSequence>
                     <MappedTaxCreditGLAccount>UNKNOWN</MappedTaxCreditGLAccount>
                     <MappedTaxCreditGLAccountSequence>SEQ2562</MappedTaxCreditGLAccountSequence>
                     <MappedTaxDebitGLAccount>UNKNOWN</MappedTaxDebitGLAccount>
                     <MappedTaxDebitGLAccountSequence>SEQ2559</MappedTaxDebitGLAccountSequence>
                     <MappedWithholdingTaxCreditGLAccount>UNKNOWN</MappedWithholdingTaxCreditGLAccount>
                     <MappedWithholdingTaxCreditGLAccountSequence>UNKNOWN</MappedWithholdingTaxCreditGLAccountSequence>
                     <MappedWithholdingTaxDebitGLAccount>UNKNOWN</MappedWithholdingTaxDebitGLAccount>
                     <MappedWithholdingTaxDebitGLAccountSequence>UNKNOWN</MappedWithholdingTaxDebitGLAccountSequence>
                     <MappedGLAccount>706027</MappedGLAccount>
                     <MappedGLAccountSequence>SEQ2600</MappedGLAccountSequence>
                     <Organization>BLXIFFCNBJS</Organization>
                     <OriginalBatchNumber>0</OriginalBatchNumber>
                     <OriginalBatchSequence>0</OriginalBatchSequence>
                     <PostingPeriod>202203</PostingPeriod>
                     <RevenueRecognitionType>DEP</RevenueRecognitionType>
                     <TaxCreditGLAccount>OUTPUT TAX PAYABLE(445.7000.0)</TaxCreditGLAccount>
                     <TaxDebitGLAccount>TRADE DEBTORS CONTROL(411.1000.0)</TaxDebitGLAccount>
                     <TaxMessageID>MATVAT</TaxMessageID>
                     <TransactionCurrency>USD</TransactionCurrency>
                     <TransactionNumber>F220000041</TransactionNumber>
                     <TransactionType>REV</TransactionType>
                     <TxnAmountInTxnCurrency>-3000.00</TxnAmountInTxnCurrency>
                     <TxnAmountInTxnCurrencyWithVAT>-3450.00</TxnAmountInTxnCurrencyWithVAT>
                     <TxnType>APS</TxnType>
                     <VATAmountInTxnCurrency>-450.00</VATAmountInTxnCurrency>
                     <VATTaxIDExtraTaxRate>0.000</VATTaxIDExtraTaxRate>
                     <VATTaxIDTaxCode>VAT</VATTaxIDTaxCode>
                     <VATTaxIDTaxDescription>Standard Rated</VATTaxIDTaxDescription>
                     <VATTaxIDTaxRate>15.000</VATTaxIDTaxRate>
                     <VATTaxIDTaxTypeCode>RAT</VATTaxIDTaxTypeCode>
                     <WithholdingAmountInTxnCurrency>0.00</WithholdingAmountInTxnCurrency>
                     <WithholdingTaxIDTaxRate>0</WithholdingTaxIDTaxRate>
                     <ChargeExpenseGroup>HAO</ChargeExpenseGroup>
                     <ChargeGroup>ORG</ChargeGroup>
                     <ChargeIsActive>TRUE</ChargeIsActive>
                     <ChargeOtherGroups>PRC</ChargeOtherGroups>
                     <ChargeSalesGroup>HAO</ChargeSalesGroup>
                     <ChargeServiceType>SRV</ChargeServiceType>
                     <ChargeType>MJA</ChargeType>
                     <IsConsolServiceCharge>TRUE</IsConsolServiceCharge>
                     <Polarity>P</Polarity>
                     <Ledger>AR</Ledger>
                     <LegacyAcctJournal>UNKNOWN</LegacyAcctJournal>
                     <AcctTaxCode>VAT</AcctTaxCode>
                     <NatureVATAcct>C</NatureVATAcct>
                     <APAccountGroup>
                        <Code>TPY</Code>
                        <Description>THIRD PARTY COMPANY</Description>
                     </APAccountGroup>
                     <ARAccountGroup>
                        <Code>TPY</Code>
                        <Description>THIRD PARTY COMPANY</Description>
                     </ARAccountGroup>
                     <IsSSC>N</IsSSC>
                     <IsGateway>N</IsGateway>
                     <Rate>3000.0000</Rate>
                     <RateUnit>Unit</RateUnit>
                     <Quantity>1.0000</Quantity>
                     <QuantityUnit>Unit</QuantityUnit>
                     <ChargeCurrencyPrecision>100</ChargeCurrencyPrecision>
                     <LocalCurrencyPrecision>100</LocalCurrencyPrecision>
                     <TransactionCurrencyPrecision>100</TransactionCurrencyPrecision>
                     <BranchProxyCode>BLXIFFBDDAC</BranchProxyCode>
                     <OHPK>1491E5F9-27E8-440B-9734-8A3F9A0ADF27</OHPK>
                     <CostGovtChargeCode>S09920</CostGovtChargeCode>
                     <SellGovtChargeCode>S09920</SellGovtChargeCode>
                  </PostingJournal>
               </PostingJournalCollection>
               <OpsData>
                  <ShipmentCollection>
                     <ShipmentCount>1</ShipmentCount>
                     <Shipment>
                        <BillingBranch>B05</BillingBranch>
                        <BillingDepartment>FES</BillingDepartment>
                        <CompanyCode>BD1</CompanyCode>
                        <JOBID>S00091725</JOBID>
                        <RepOps>MMK</RepOps>
                        <RepSales>HKO</RepSales>
                        <JobStatus>JRC</JobStatus>
                        <RelatedOrgs>
                           <RelatedOrgCount>1</RelatedOrgCount>
                           <RelatedOrg>
                              <Address1>SAFURA TOWER (8TH FLOOR),</Address1>
                              <Address2>20 KEMAL ATATURK AVENUE, BANANI</Address2>
                              <City>DHAKA</City>
                              <CompanyCode>BD1</CompanyCode>
                              <Country>BD</Country>
                              <CountryName>Bangladesh</CountryName>
                              <CurrentBatchNumber>31</CurrentBatchNumber>
                              <FullName>BOLLORE LOGISTICS BANGLADESH - DHAKA</FullName>
                              <JOBID>S00091725</JOBID>
                              <JobType>JS</JobType>
                              <Location>H</Location>
                              <OrgCode>BLXIFFBDDAC</OrgCode>
                              <PostCode>1213</PostCode>
                              <Role>BBR</Role>
                              <RoleName>BillingBranch</RoleName>
                              <Source>O</Source>
                              <State>13</State>
                              <UNLOCO>BDDAC</UNLOCO>
                              <ArianeCode>048000</ArianeCode>
                              <CRMID>1-6ZKT-1631</CRMID>
                              <VATCode>TAX</VATCode>
                              <IsValidAddress>FALSE</IsValidAddress>
                              <Language>EN-US</Language>
                              <AddressType>OFC</AddressType>
                              <IsMainAddress>TRUE</IsMainAddress>
                              <AddIndex>0</AddIndex>
                           </RelatedOrg>
                        </RelatedOrgs>
                        <ControllerCode>VETIRFRSPP</ControllerCode>
                        <ControllerFullName>VETIR SAS</ControllerFullName>
                        <GoodsDescription>READYMADE GARMENTS</GoodsDescription>
                        <HouseBill>BD100091725</HouseBill>
                        <INCO>FOB</INCO>
                        <JSDestination>DEHAM</JSDestination>
                        <JSOrigin>BDDAC</JSOrigin>
                        <ServiceLevelCode>STD</ServiceLevelCode>
                        <VerticalType>82</VerticalType>
                        <JSActualChargeable>0.000</JSActualChargeable>
                        <JSActualVolume>0.000</JSActualVolume>
                        <JSActualWeight>0.000</JSActualWeight>
                        <JSTransportmode>SEA</JSTransportmode>
                        <OuterPacks>0</OuterPacks>
                        <PackType>PLT</PackType>
                        <ShipmentUnitOfVolume>M3</ShipmentUnitOfVolume>
                        <ShipmentUnitOfWeight>T</ShipmentUnitOfWeight>
                        <ShipperCode>FBFOOTBDGAZ</ShipperCode>
                        <ShipperFullName>F. B. FOOTWEAR LTD</ShipperFullName>
                        <ConsigneeCode>VETIRFRSPP</ConsigneeCode>
                        <ConsigneeFullName>VETIR SAS</ConsigneeFullName>
                        <TradeLine>83</TradeLine>
                        <CommodityCode>GEN</CommodityCode>
                        <ConsolCollection>
                           <ConsolCount>1</ConsolCount>
                           <Consol>
                              <BillingBranch>B07</BillingBranch>
                              <BillingDepartment>DIS</BillingDepartment>
                              <CompanyCode>BD1</CompanyCode>
                              <JOBID>S00091725</JOBID>
                              <JKID>C00060970</JKID>
                              <Vessel>KOTA ARIF</Vessel>
                              <Voyage>123</Voyage>
                              <JKDestination>DEHAM</JKDestination>
                              <JKOrigin>BDCGP</JKOrigin>
                              <ServiceLevel>STD</ServiceLevel>
                              <JKActualChargeable>0</JKActualChargeable>
                              <VolumeTotal>0</VolumeTotal>
                              <WeightTotal>0</WeightTotal>
                              <JKTransportmode>SEA</JKTransportmode>
                              <VolumeUnit>M3</VolumeUnit>
                              <WeightUnit>T</WeightUnit>
                           </Consol>
                        </ConsolCollection>
                     </Shipment>
                  </ShipmentCollection>
               </OpsData>
            </TransactionInfo>
         </UniversalTransaction>
      </Body>
   </UniversalInterchange>
</UniversalInterchangeBatch>
                ";

                #endregion

                xml = XML2;

                BolloreIntegrationDAL _dal = new BolloreIntegrationDAL();

                IntegrationData = new DataTable();

                IntegrationData = _dal.GetXMLData(xml);
                TableValidation(IntegrationData, transactionType);

                SaleDAL saleDal = new SaleDAL();

                result = saleDal.SaveAndProcess(IntegrationData, () => { }, Program.BranchId, "", connVM, null, null, null, Program.CurrentUserID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundSaveSale_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                MessageBox.Show(result[0] + "~" + result[1]);

                dgvLoadedTable.DataSource = null;
                dgvLoadedTable.DataSource = IntegrationData;
                lblRecord.Text = "Record Count: " + IntegrationData.Rows.Count;

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar1.Visible = false;

            }
        }

        private void TableValidation(DataTable salesData, string TType)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = Program.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = TType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }
            if (!salesData.Columns.Contains("TransactionType"))
            {
                salesData.Columns.Add(TransactionType);
            }

            salesData.Columns.Add(BOMId);
        }



    }
}
