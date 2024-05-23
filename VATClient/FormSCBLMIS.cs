using System;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;

using VATClient.ReportPages;

using VATClient.ModelDTO;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Data.Odbc;
using CrystalDecisions.Shared;

namespace VATClient
{
    public partial class FormSCBLMIS : Form
    {
        public FormSCBLMIS()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
     
        private void btnLocalPurchase_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "LocalPurchase";
            frmSCBL.ShowDialog();
        }

        private void btnImportData_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "ImportData";
            frmSCBL.ShowDialog();
        }

        private void btnReceiedVsSale_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "ReceiedVsSale";
            frmSCBL.ShowDialog();
        }

        private void btnSalesStatementForService_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "SalesStatementForService";
            frmSCBL.ShowDialog();
        }

        private void btnSalesStatementDelivery_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "SalesStatementDelivery";
            frmSCBL.ShowDialog();
        }

        private void btnStockReportFG_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "StockReportFG";
            frmSCBL.ShowDialog();
        }

        private void btnStockReportRM_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "StockReportRM";
            frmSCBL.ShowDialog();
        }

        private void btnTransferToDepot_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "TransferToDepot";
            frmSCBL.ShowDialog();
        }

        private void btnVDSStatement_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "VDSStatement";
            frmSCBL.ShowDialog();
        }

        private void btnChakKa_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();      
            frmSCBL.reportType = "ChakKa";
            frmSCBL.ShowDialog();
        }

        private void btnChakkha_Click(object sender, System.EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            frmSCBL.reportType = "ChakKha";
            frmSCBL.ShowDialog();
        }

        private void FormSCBLMIS_Load(object sender, System.EventArgs e)
        {

        }
    }
}
