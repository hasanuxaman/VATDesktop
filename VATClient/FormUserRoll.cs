using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using System.Globalization;
using System.Data.Odbc;
using System.Threading;
//
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormUserRoll : Form
    {
        #region Constructors

        public FormUserRoll()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string RollResult = string.Empty;
        private static string[] RollLines;
        private bool ChangeData = false;
        public string VFIN = "43";
        private List<string> MergedRowsInFirstColumn = new List<string>();

        #region Global Variable Fo BackgoundWork

        private string RollData = string.Empty;

        private List<UserRollVM> userRollVMs = new List<UserRollVM>();

        #endregion

        #region sql save, update, delete

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Methods

        private void GridLoad()
        {
            //No SOAP Service
            try
            {
                #region Setup

                dgvRoll.Rows.Clear();

                DataGridViewRow NewRow1101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "ItemInformation";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Group";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1101";


                DataGridViewRow NewRow1102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "ItemInformation";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Product";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1102";

                DataGridViewRow NewRow1103 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1103);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "ItemInformation";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Overhead";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1103";


                DataGridViewRow NewRow1201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Vedor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Group";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1201";


                DataGridViewRow NewRow1202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Vedor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Vendor";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1202";

                DataGridViewRow NewRow1301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Customer";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Group";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1301";


                DataGridViewRow NewRow1302 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1302);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Customer";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Customer";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1302";


                DataGridViewRow NewRow1401 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1401);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Bank";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Bank";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1401";

                DataGridViewRow NewRow1501 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1501);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Vehicle";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Vehicle";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1501";

                DataGridViewRow NewRow1601 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1601);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "PriceDeclaration";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT-1";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1601";

                DataGridViewRow NewRow1602 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1602);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "PriceDeclaration";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Service";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1602";

                DataGridViewRow NewRow1603 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1603);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "PriceDeclaration";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Tender";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1603";


                DataGridViewRow NewRow1701 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1701);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Company";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Commpany";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1701";

                DataGridViewRow NewRow1801 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1801);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "FiscalYear";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "FiscalYear";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1801";


                DataGridViewRow NewRow1901 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1901);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Configuration";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Settings";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1901";

                DataGridViewRow NewRow1902 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow1902);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Configuration";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Prefix";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "1902";

                DataGridViewRow NewRow11001 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11001);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Import";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Import";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11001";



                DataGridViewRow NewRow11101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Conversion";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Conversion";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11101";

                DataGridViewRow NewRow11201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Currency";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Currency";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11201";

                DataGridViewRow NewRow11202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Currency";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Conversion";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11202";


                DataGridViewRow NewRow11301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Banderol";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Banderol";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11301";

                DataGridViewRow NewRow11302 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11302);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Banderol";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Packaging";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11302";

                DataGridViewRow NewRow11303 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow11303);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Banderol";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Product";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11303";


                

                #endregion Setup
                #region Purchase
                DataGridViewRow NewRow2101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Local";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2101";

                DataGridViewRow NewRow2102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Trading";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2102";

                DataGridViewRow NewRow2103 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2103);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Import";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2103";


                DataGridViewRow NewRow2104 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2104);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "InputService";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2104";

                DataGridViewRow NewRow2105 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2105);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "PurchaseReturn";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2105";

                DataGridViewRow NewRow2106 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2106);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Service Stock";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2106";

                DataGridViewRow NewRow2107 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow2107);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Service Non Stock";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "2107";
                #endregion Purchase

#region Production
                DataGridViewRow NewRow3101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow3101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Issue";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Issue";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "3101";

                DataGridViewRow NewRow3102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow3102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Issue";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Return";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "3102";

                DataGridViewRow NewRow3201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow3201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Receive";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "WIP";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "3201";

                DataGridViewRow NewRow3202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow3202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Receive";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "FGReceive";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "3202";

                DataGridViewRow NewRow3203 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow3203);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Receive";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Return";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "3203";

                DataGridViewRow NewRow3301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow3301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Receive";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Package";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "3301";
#endregion Production

#region Sale
                DataGridViewRow NewRow4101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow4101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Local";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "4101";


                DataGridViewRow NewRow4102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow4102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Service Stock";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "4102";

                DataGridViewRow NewRow41021 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow41021);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Service Non Stock";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "41021";


                DataGridViewRow NewRow4103 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow4103);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Trading";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "4103";

                DataGridViewRow NewRow4104 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow4104);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Export";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "4104";


                DataGridViewRow NewRow4105 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow4105);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Tender";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "4105";



                DataGridViewRow NewRow4201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow4201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Transfer";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Transfer";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "4201";
#endregion Sale

#region Deposit
                DataGridViewRow NewRow5101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow5101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Deposit";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Treasury";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Treasury";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "5101";


                DataGridViewRow NewRow5201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow5201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Deposit";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VDS";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VDS";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "5201";


                DataGridViewRow NewRow5301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow5301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Deposit";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "SD";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "SD";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "5301";

                DataGridViewRow NewRow5401 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow5401);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Deposit";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Reverse";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "5401";

                //DataGridViewRow NewRow5501 = new DataGridViewRow();
                //dgvRoll.Rows.Add(NewRow5501);
                //dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Deposit";
                //dgvRoll["Child", dgvRoll.RowCount - 1].Value = "TDS";
                //dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "TDS";
                //dgvRoll["ID", dgvRoll.RowCount - 1].Value = "5501";

#endregion Deposit

#region Toll
                DataGridViewRow NewRow6101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow6101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Client";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "RawIssue";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "6101";

                DataGridViewRow NewRow6102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow6102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Client";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "FGReceive";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "6102";


                DataGridViewRow NewRow6201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow6201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Contractor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "RawReceive";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "6201";


                DataGridViewRow NewRow6202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow6202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Contractor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "FGProduction";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "6202";


                DataGridViewRow NewRow6203 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow6203);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Contractor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "FGIssue";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "6203";
#endregion Toll

#region Adjustment
                DataGridViewRow NewRow7101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "AdjustmentHead";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Head";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7101";

                DataGridViewRow NewRow7102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "AdjustmentHead";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Transaction";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7102";

                DataGridViewRow NewRow7201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "DN";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7201";

                DataGridViewRow NewRow7202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "CN";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7202";


                DataGridViewRow NewRow7301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "CN";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7301";


                DataGridViewRow NewRow7302 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7302);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "DN";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7302";


                DataGridViewRow NewRow7401 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7401);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Dispose";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "26";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7401";

                DataGridViewRow NewRow7402 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7402);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Dispose";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "27";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7402";


                DataGridViewRow NewRow7501 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow7501);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Adjustment";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "DDB";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "DDB";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "7501";
#endregion Adjustment
#region NBR Report
                DataGridViewRow NewRow8101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow8101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "NBRReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT1";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "BOM";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "8101";

                DataGridViewRow NewRow8201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow8201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "NBRReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT16";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT16";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "8201";

                DataGridViewRow NewRow8301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow8301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "NBRReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT17";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT17";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "8301";

                DataGridViewRow NewRow8401 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow8401);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "NBRReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT18";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT18";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "8401";

                DataGridViewRow NewRow8501 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow8501);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "NBRReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT19";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT19";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "8501";

                DataGridViewRow NewRow8601 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow8601);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "NBRReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "SDReport";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "SDReport";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "8601";
#endregion NBR Report
#region MIS Report
                DataGridViewRow NewRow9101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9101";

                DataGridViewRow NewRow9102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Purchase";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Trading";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9102";

              

                DataGridViewRow NewRow9201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Issue";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9201";


                DataGridViewRow NewRow9202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "IssueReturn";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9202";


                DataGridViewRow NewRow9203 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9203);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Receive";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9203";


                DataGridViewRow NewRow9204 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9204);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Production";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "InnerIssue";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9204";

                DataGridViewRow NewRow9301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Issue";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9301";

                DataGridViewRow NewRow9302 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9302);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Toll";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Receive";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9302";

                DataGridViewRow NewRow9401 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9401);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Local";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9401";

                DataGridViewRow NewRow9402 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9402);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Service";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9402";

                DataGridViewRow NewRow9403 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9403);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Trading";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9403";

                DataGridViewRow NewRow9404 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9404);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Sale";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Export";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9404";

                DataGridViewRow NewRow9501 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9501);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Stock";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Stock";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9501";

                DataGridViewRow NewRow9601 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9601);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Deposit";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Deposit";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9601";

                DataGridViewRow NewRow9701 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9701);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT16";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT16";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9701";

                DataGridViewRow NewRow9801 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9801);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT17";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT17";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9801";

                DataGridViewRow NewRow9901 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow9901);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "VAT18";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "VAT18";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "9901";

                DataGridViewRow NewRow91001 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow91001);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "MISReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "SDDeposit";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "SDDeposit";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "91001";
#endregion MIS Report

#region Setup Report
                DataGridViewRow NewRow10101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Product";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Type";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10101";

                DataGridViewRow NewRow10102 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10102);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Product";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Group";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10102";

                DataGridViewRow NewRow10103 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10103);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Product";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Product";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10103";

                DataGridViewRow NewRow10201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Customer";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Group";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10201";

                DataGridViewRow NewRow10202 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10202);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Customer";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Customer";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10202";

                DataGridViewRow NewRow10301 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10301);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Vendor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Group";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10301";

                DataGridViewRow NewRow10302 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10302);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Vendor";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Vendor";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10302";

                DataGridViewRow NewRow10401 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10401);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Bank";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Bank";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10401";

                DataGridViewRow NewRow10501 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow10501);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "SetupReport";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Vehicle";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Vehicle";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "10501";


#endregion Setup Report
#region UserAccount
                DataGridViewRow NewRow111010 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow111010);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "UserAccount";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "NewAccount";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "NewAccount";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "20101";


                DataGridViewRow NewRow112010 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow112010);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "UserAccount";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "PasswordChange";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "PasswordChange";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "20201";

                DataGridViewRow NewRow43 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow43);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "UserAccount";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "UserRole";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "UserRole";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "43";

                DataGridViewRow NewRow44 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow44);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "UserAccount";
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "SettingsRole";
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "SettingsRole";
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "44";


                //DataGridViewRow NewRow11401 = new DataGridViewRow();
                //dgvRoll.Rows.Add(NewRow11401);
                //dgvRoll["Root", dgvRoll.RowCount - 1].Value = "UserAccount";
                //dgvRoll["Child", dgvRoll.RowCount - 1].Value = "LogOut";
                //dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "LogOut";
                //dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11401";

                //DataGridViewRow NewRow11501 = new DataGridViewRow();
                //dgvRoll.Rows.Add(NewRow11501);
                //dgvRoll["Root", dgvRoll.RowCount - 1].Value = "UserAccount";
                //dgvRoll["Child", dgvRoll.RowCount - 1].Value = "CloseAll";
                //dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "CloseAll";
                //dgvRoll["ID", dgvRoll.RowCount - 1].Value = "11501";

#endregion UserAccount

                #region Banderol
                DataGridViewRow NewRow33101 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow33101);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Banderol"; //33
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Demand"; //01
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Demand"; //01
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "33101";


                DataGridViewRow NewRow33201 = new DataGridViewRow();
                dgvRoll.Rows.Add(NewRow33201);
                dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Banderol"; //33
                dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Receive"; //02
                dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Receive"; //01
                dgvRoll["ID", dgvRoll.RowCount - 1].Value = "33201";
                #endregion Banderol


                for (int i = 0; i < dgvRoll.RowCount; i++)
                {
                    dgvRoll[0, i].Value = i + 1;
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "GridLoad", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridLoad", exMessage);
            }
            #endregion Catch

        }

        private void RollDetailsInfo()
        {
            //No SOAP Service
            try
            {
                //LineID, 0
                //UserID, 1
                //FormID, 2
                //FormSave, 3
                //FormUpdate, 4
                //FormDelete, 5
                //FormPrint, 6
                //FormSearch 7
                //dgvRoll.Rows.Clear();
                //GridLoad();
                for (int j = 0; j < Convert.ToInt32(RollLines.Length); j++)
                {
                  
                    string[] RollFields = RollLines[j].Split(FieldDelimeter.ToCharArray(),
                                                             StringSplitOptions.RemoveEmptyEntries);

                    if (dgvRoll["ID", j].Value.ToString() == RollFields[2].ToString())
                    {
                        try
                        {
                            dgvRoll["Access", j].Value = RollFields[3].ToString();
                        }
                        catch
                        {
                            dgvRoll["Access", j].Value = "N";
                        }
                        try
                        {
                            dgvRoll["Post", j].Value = RollFields[4].ToString();
                        }
                        catch (Exception)
                        {
                            dgvRoll["Post", j].Value = "N";
                        }
                        try
                        {
                            dgvRoll["Add", j].Value = RollFields[5].ToString();
                        }
                        catch (Exception)
                        {
                            dgvRoll["Add", j].Value = "N";
                        }
                        try
                        {
                            dgvRoll["Update", j].Value = RollFields[6].ToString();
                        }
                        catch (Exception)
                        {
                            dgvRoll["Update", j].Value = "N";
                        }


                        //dgvRoll["Access", j].Value = RollFields[3].ToString();
                        //dgvRoll["Post", j].Value = RollFields[4].ToString();  


                        //if (RollFields.Length <= 3)
                        //{
                        //    dgvRoll["Access", j].Value = "N";

                        //}
                        //else
                        //{
                        //    dgvRoll["Access", j].Value = RollFields[3].ToString();
                        //    //dgvRoll["Post", j].Value = RollFields[4].ToString();    
                        //}
                        //if (RollFields.Length <= 4)
                        //{
                            
                        //    dgvRoll["Post", j].Value = "N";
                        //}
                        //else
                        //{
                        //    //dgvRoll["Access", j].Value = RollFields[3].ToString();
                        //    dgvRoll["Post", j].Value = RollFields[4].ToString();
                        //}
                       
                        
                    }
                    else
                    {
                    }

                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "RollDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "RollDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "RollDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "RollDetailsInfo", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RollDetailsInfo", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RollDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RollDetailsInfo", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RollDetailsInfo", exMessage);
            }
            #endregion Catch
        }

        private void UserRollSearch()
        {
            try
            {
               
                RollData = txtUserID.Text.Trim();

              
                this.btnUserSearch.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerSearch.RunWorkerAsync();

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UserRollSearch", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UserRollSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UserRollSearch", exMessage);
            }
            #endregion Catch
        }

        private void AllSelect()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    
                    if (string.IsNullOrEmpty(dgvRoll["Access", row].Value.ToString()))
                    {
                        dgvRoll["Access", row].Value = "N";
                    }
                    if (dgvRoll["Access", row].Value.ToString() != "Y")
                    {
                        dgvRoll["Access", row].Value = "N";
                    }
                    if (string.IsNullOrEmpty(dgvRoll["Post", row].Value.ToString()))
                    {
                        dgvRoll["Post", row].Value = "N";
                    }
                    if (dgvRoll["Post", row].Value.ToString() != "Y")
                    {
                        dgvRoll["Post", row].Value = "N";
                    }
                    if (string.IsNullOrEmpty(dgvRoll["Add", row].Value.ToString()))
                    {
                       dgvRoll["Add", row].Value = "N"; 
                    }
                    if (dgvRoll["Add", row].Value.ToString() != "Y")
                    {
                        dgvRoll["Add", row].Value = "N";
                    }
                    if (string.IsNullOrEmpty(dgvRoll["Update", row].Value.ToString()))
                    {
                        dgvRoll["Update", row].Value = "N";
                    }
                    if (dgvRoll["Update", row].Value.ToString() != "Y")
                    {
                        dgvRoll["Update", row].Value = "N";
                    }
                    //else
                    //{
                    //    //dgvRoll["Access", row].Value = "Y";
                    //}                       
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "AllSelect", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "AllSelect", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "AllSelect", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "AllSelect", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllSelect", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllSelect", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllSelect", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllSelect", exMessage);
            }
            #endregion Catch
        }

        private void CheckedAllPost()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Post", row].Value = "Y";
                }

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CheckedAllPost", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllPost", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllPost", exMessage);
            }
            #endregion Catch

        }
        
        private void UnCheckedAllPost()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Post", row].Value = "N";
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UnCheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UnCheckedAllPost", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllPost", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllPost", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllPost", exMessage);
            }
            #endregion Catch

        }

        private void CheckedAllAccess()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Access", row].Value = "Y";
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CheckedAllAccess", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAccess", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAccess", exMessage);
            }
            #endregion Catch

        }

        private void UnCheckedAllAccess()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Access", row].Value = "N";
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UnCheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UnCheckedAllAccess", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAccess", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAccess", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAccess", exMessage);
            }
            #endregion Catch

        }

        private void CheckedAllAdd()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Add", row].Value = "Y";
                }

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CheckedAllAdd", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAdd", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllAdd", exMessage);
            }
            #endregion Catch

        }

        private void UnCheckedAllAdd()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Add", row].Value = "N";
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UnCheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UnCheckedAllAdd", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAdd", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAdd", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllAdd", exMessage);
            }
            #endregion Catch

        }

        private void CheckedAllUpdate()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Update", row].Value = "Y";
                }

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CheckedAllUpdate", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllUpdate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CheckedAllUpdate", exMessage);
            }
            #endregion Catch

        }

        private void UnCheckedAllUpdate()
        {
            try
            {
                for (int row = 0; row < dgvRoll.Rows.Count; row++)
                {
                    dgvRoll["Update", row].Value = "N";
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UnCheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UnCheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UnCheckedAllUpdate", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllUpdate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllUpdate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UnCheckedAllUpdate", exMessage);
            }
            #endregion Catch

        }

        private void Merge()
        {
            try
            {
                int[] RowsToMerge = new int[2];
                RowsToMerge[0] = -1;

                //Merge first column at first
                for (int i = 0; i < dgvRoll.Rows.Count - 1; i++)
                {
                    //dgvRoll["Root", i].Value == dgvRoll["Root", i+1].Value
                    if (dgvRoll["Root", i].Value == dgvRoll["Root", i + 1].Value)
                    {
                        if (RowsToMerge[0] == -1)
                        {
                            RowsToMerge[0] = i;
                            RowsToMerge[1] = i + 1;
                        }
                        else
                        {
                            RowsToMerge[1] = i + 1;
                        }
                    }
                    else
                    {
                        MergeCells(RowsToMerge[0], RowsToMerge[1], dgvRoll.Columns["Root"].Index,
                                   isSelectedCell(RowsToMerge, dgvRoll.Columns["Root"].Index) ? true : false);
                        CollectMergedRowsInFirstColumn(RowsToMerge[0], RowsToMerge[1]);
                        RowsToMerge[0] = -1;
                    }
                    if (i == dgvRoll.Rows.Count - 2)
                    {
                        MergeCells(RowsToMerge[0], RowsToMerge[1], dgvRoll.Columns["Root"].Index,
                                   isSelectedCell(RowsToMerge, dgvRoll.Columns["Root"].Index) ? true : false);
                        CollectMergedRowsInFirstColumn(RowsToMerge[0], RowsToMerge[1]);
                        RowsToMerge[0] = -1;
                    }
                }
                if (RowsToMerge[0] != -1)
                {
                    MergeCells(RowsToMerge[0], RowsToMerge[1], dgvRoll.Columns["Root"].Index,
                               isSelectedCell(RowsToMerge, dgvRoll.Columns["Root"].Index) ? true : false);
                    RowsToMerge[0] = -1;
                }

                //merge all other columns
                for (int iColumn = 1; iColumn < dgvRoll.Columns.Count - 1; iColumn++)
                {
                    for (int iRow = 0; iRow < dgvRoll.Rows.Count - 1; iRow++)
                    {
                        if (((dgvRoll[iColumn, iRow] == dgvRoll[iColumn, iRow + 1]) &&
                             (isRowsHaveOneCellInFirstColumn(iRow, iRow + 1))))
                        {
                            if (RowsToMerge[0] == -1)
                            {
                                RowsToMerge[0] = iRow;
                                RowsToMerge[1] = iRow + 1;
                            }
                            else
                            {
                                RowsToMerge[1] = iRow + 1;
                            }
                        }
                        else
                        {
                            if (RowsToMerge[0] != -1)
                            {
                                MergeCells(RowsToMerge[0], RowsToMerge[1], iColumn,
                                           isSelectedCell(RowsToMerge, iColumn) ? true : false);
                                RowsToMerge[0] = -1;
                            }
                        }
                    }
                    if (RowsToMerge[0] != -1)
                    {
                        MergeCells(RowsToMerge[0], RowsToMerge[1], iColumn,
                                   isSelectedCell(RowsToMerge, iColumn) ? true : false);
                        RowsToMerge[0] = -1;
                    }
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Merge", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Merge", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Merge", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "Merge", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Merge", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Merge", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Merge", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Merge", exMessage);
            }
            #endregion Catch
        }

        private void MergeCells(int RowId1, int RowId2, int Column, bool isSelected)
        {
            try
            {
                Graphics g = dgvRoll.CreateGraphics();
                Pen gridPen = new Pen(dgvRoll.GridColor);

                //Cells Rectangles
                Rectangle CellRectangle1 = dgvRoll.GetCellDisplayRectangle(Column, RowId1, true);
                Rectangle CellRectangle2 = dgvRoll.GetCellDisplayRectangle(Column, RowId2, true);

                int rectHeight = 0;
                string MergedRows = String.Empty;

                for (int i = RowId1; i <= RowId2; i++)
                {
                    rectHeight += dgvRoll.GetCellDisplayRectangle(Column, i, false).Height;
                }

                Rectangle newCell = new Rectangle(CellRectangle1.X, CellRectangle1.Y, CellRectangle1.Width, rectHeight);

                g.FillRectangle(
                    new SolidBrush(isSelected
                                       ? dgvRoll.DefaultCellStyle.SelectionBackColor
                                       : dgvRoll.DefaultCellStyle.BackColor), newCell);

                g.DrawRectangle(gridPen, newCell);

                g.DrawString(dgvRoll.Rows[RowId1].Cells[Column].Value.ToString(), dgvRoll.DefaultCellStyle.Font,
                             new SolidBrush(isSelected
                                                ? dgvRoll.DefaultCellStyle.SelectionForeColor
                                                : dgvRoll.DefaultCellStyle.ForeColor), newCell.X + newCell.Width / 3,
                             newCell.Y + newCell.Height / 3);
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "MergeCells", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "MergeCells", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "MergeCells", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "MergeCells", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "MergeCells", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "MergeCells", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "MergeCells", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "MergeCells", exMessage);
            }
            #endregion Catch
        }

        

        private void CollectMergedRowsInFirstColumn(int RowId1, int RowId2)
        {
            try
            {
                string MergedRows = String.Empty;

                for (int i = RowId1; i <= RowId2; i++)
                {
                    MergedRows += i.ToString() + ";";
                }
                MergedRowsInFirstColumn.Add(MergedRows.Remove(MergedRows.Length - 1, 1));
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CollectMergedRowsInFirstColumn", exMessage);
            }
            #endregion Catch
        }

        private bool isSelectedCell(int[] Rows, int ColumnIndex)
        {
            try
            {
                if (dgvRoll.SelectedCells.Count > 0)
                {
                    for (int iCell = Rows[0]; iCell <= Rows[1]; iCell++)
                    {
                        for (int iSelCell = 0; iSelCell < dgvRoll.SelectedCells.Count; iSelCell++)
                        {
                            if (dgvRoll.Rows[iCell].Cells[ColumnIndex] == dgvRoll.SelectedCells[iSelCell])
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "isSelectedCell", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "isSelectedCell", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "isSelectedCell", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "isSelectedCell", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isSelectedCell", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isSelectedCell", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isSelectedCell", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isSelectedCell", exMessage);
            }

            #endregion Catch
            return false;

        }

        private bool isRowsHaveOneCellInFirstColumn(int RowId1, int RowId2)
        {
            try
            {
                foreach (string rowsCollection in MergedRowsInFirstColumn)
                {
                    string[] RowsNumber = rowsCollection.Split(';');

                    if ((isStringInArray(RowsNumber, RowId1.ToString())) &&
                        (isStringInArray(RowsNumber, RowId2.ToString())))
                    {
                        return true;
                    }
                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isRowsHaveOneCellInFirstColumn", exMessage);
            }
            #endregion Catch
            return false;
        }

        private bool isStringInArray(string[] Array, string value)
        {
            try
            {
                foreach (string item in Array)
                {
                    if (item == value)
                    {
                        return true;
                    }

                }
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "isStringInArray", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "isStringInArray", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "isStringInArray", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "isStringInArray", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isStringInArray", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isStringInArray", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isStringInArray", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "isStringInArray", exMessage);
            }
            #endregion Catch
            return false;
        }

        #endregion

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void FormUserRoll_Load(object sender, EventArgs e)
        {
            try
            {
                txtUserID.Text = Program.CurrentUserID;
                txtUserName.Text = Program.CurrentUser;
                GridLoad();
                UserRollSearch();
                ChangeData = false;

            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormUserRoll_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormUserRoll_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormUserRoll_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormUserRoll_Load", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_Load", exMessage);
            }
            #endregion Catch
        }

        private void dgvRoll_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dgvRoll_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvRoll.CurrentCell.ColumnIndex < 4)
                {
                    return;
                }

                if (dgvRoll[dgvRoll.CurrentCell.ColumnIndex, dgvRoll.CurrentRow.Index].Value == "Y")
                {
                    dgvRoll[dgvRoll.CurrentCell.ColumnIndex, dgvRoll.CurrentRow.Index].Value = "N";
                    return;
                }
                if (dgvRoll[dgvRoll.CurrentCell.ColumnIndex, dgvRoll.CurrentRow.Index].Value == "N")
                {
                    dgvRoll[dgvRoll.CurrentCell.ColumnIndex, dgvRoll.CurrentRow.Index].Value = "Y";
                    return;
                }
                else
                {
                    dgvRoll[dgvRoll.CurrentCell.ColumnIndex, dgvRoll.CurrentRow.Index].Value = "Y";
                    return;
                }
                ChangeData = true;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_DoubleClick", exMessage);
            }
            #endregion Catch

        }

        private void dgvRoll_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (txtUserID.Text.Trim() == "")
                {
                    MessageBox.Show("Please select the user");
                    return;
                }
                if (txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("Please select the user");
                    return;
                }


                AllSelect();

                userRollVMs = new List<UserRollVM>();

                for (int i = 0; i < dgvRoll.RowCount; i++)
                {
                    UserRollVM userRollVM = new UserRollVM();

                    userRollVM.UserID = txtUserID.Text.Trim();
                    userRollVM.FormID = dgvRoll.Rows[i].Cells["ID"].Value.ToString();
                    userRollVM.Access = dgvRoll.Rows[i].Cells["Access"].Value.ToString();
                    userRollVM.CreatedBy = Program.CurrentUser;
                    userRollVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    userRollVM.LastModifiedBy = Program.CurrentUser;
                    userRollVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    userRollVM.LineID = dgvRoll.Rows[i].Cells["Sl"].Value.ToString();
                    userRollVM.FormName = dgvRoll.Rows[i].Cells["Root"].Value.ToString() + "/" + dgvRoll.Rows[i].Cells["Child"].Value.ToString() + "/" + dgvRoll.Rows[i].Cells["ChildChild"].Value.ToString();
                    userRollVM.PostAccess = dgvRoll.Rows[i].Cells["Post"].Value.ToString();
                    userRollVM.AddAccess = dgvRoll.Rows[i].Cells["Add"].Value.ToString();
                    userRollVM.EditAccess = dgvRoll.Rows[i].Cells["Update"].Value.ToString();

                    

                    userRollVMs.Add(userRollVM);
                }
                progressBar1.Visible = true;
                btnSave.Enabled = false;
                backgroundWorkerSave.RunWorkerAsync();
                

               
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }
            #endregion Catch

        }
        private void backgroundWorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
              

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                UserInformationDAL userInformationDal = new UserInformationDAL();
                sqlResults = userInformationDal.InsertToUserRoll(userRollVMs, Program.DatabaseName);

                UPDATE_DOWORK_SUCCESS = true;

                //End DoWork
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                
                if (UPDATE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                }

                //End Complete
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
            finally
            {
                btnSave.Enabled = true;
                progressBar1.Visible = false;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnUserSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Me";
                string result = FormUserSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] UserInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtUserID.Text = UserInfo[0];
                    txtUserName.Text = UserInfo[1];
                }
                GridLoad();
                UserRollSearch(); // SOAP Service
                RollDetailsInfo();
                ChangeData = false;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnUserSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUserSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUserSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnUserSearch_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUserSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUserSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUserSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUserSearch_Click", exMessage);
            }
            #endregion Catch
            finally
            {
                ChangeData = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            UnCheckedAllAccess();
        }

        private void btnSellectAll_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            CheckedAllAccess();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Merge();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormUserRoll_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.Yes != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to close without saving?",
                        this.Text,

                        MessageBoxButtons.YesNo,

                        MessageBoxIcon.Question,

                        MessageBoxDefaultButton.Button2))
                    {
                        e.Cancel = true;
                        //UserRollSearchClose();
                    }

                }
                RollData = Program.CurrentUserID;
                Program.CurrentUserRollSearch(Program.CurrentUserID);
                
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormUserRoll_FormClosing", exMessage);
            }
            #endregion Catch
        }

        private void dgvRoll_Scroll(object sender, ScrollEventArgs e)
        {

            try
            {
                Rectangle rect = Rectangle.Union(
                    dgvRoll.GetCellDisplayRectangle(2, -1, true),
                    dgvRoll.GetCellDisplayRectangle(3, -1, true));
                dgvRoll.Invalidate(rect);
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvRoll_Scroll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvRoll_Scroll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvRoll_Scroll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "dgvRoll_Scroll", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_Scroll", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_Scroll", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_Scroll", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvRoll_Scroll", exMessage);
            }
            #endregion Catch

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            //No SOAP Service
            CheckedAllPost();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            UnCheckedAllPost();

        }

        #region backgroundWorker Event

       

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                
                UserInformationDAL userInformationDal = new UserInformationDAL();
                RollResult = userInformationDal.SearchUserRoll(RollData);

                //End DoWork
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                string decriptedProductData = Converter.DESDecrypt(PassPhrase, EnKey, RollResult);
                RollLines = decriptedProductData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                RollDetailsInfo();
                //End Complete
                ChangeData = false;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            #endregion Catch

            this.btnUserSearch.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void backgroundWorkerUserRollSearchClose_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
               
                UserInformationDAL userInformationDal = new UserInformationDAL();
                RollResult = userInformationDal.SearchUserRoll(RollData);

                //End DoWork
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_DoWork", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerUserRollSearchClose_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                string decriptedProductData = Converter.DESDecrypt(PassPhrase, EnKey, RollResult);
                Program.PublicRollLines = decriptedProductData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //End Complete
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUserRollSearchClose_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
        }


        #endregion

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void btnAllAdd_Click(object sender, EventArgs e)
        {
            CheckedAllAdd();
        }

        private void btnNoAdd_Click(object sender, EventArgs e)
        {
            UnCheckedAllAdd();
        }

        private void btnAllUpdate_Click(object sender, EventArgs e)
        {
            CheckedAllUpdate();
        }

        private void btnNoUpdate_Click(object sender, EventArgs e)
        {
            UnCheckedAllUpdate();
        }

    }
}

