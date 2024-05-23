using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormPurchaseAdjustment : Form
    {

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private bool ChangeData = false;
        private string NextID = string.Empty;
        public string VFIN = "151";
        private bool IsUpdate = false;
        private string SelectedValue = string.Empty;
        private bool IsNull = false;
        bool ApplyAll = false;


        #region Global Variables for BackgroundWorker

        private string result = string.Empty;

        #endregion



        #endregion

        public FormPurchaseAdjustment()
        {
            InitializeComponent();
        }

        private void FormPurchaseAdjustment_Load(object sender, EventArgs e)
        {

        }

        public static string SelectOne(DataGridViewRow row)
        {
            string SearchValue = string.Empty;

            FormPurchaseAdjustment frmPur = new FormPurchaseAdjustment();
            try
            {
                string ReasonOfReturn = "";

                if (row.Cells["ReasonOfReturn"].Value != null)
                {
                    frmPur.txtReason.Text = row.Cells["ReasonOfReturn"].Value.ToString();
                }
                frmPur.txtProductName.Text = row.Cells["ItemName"].Value.ToString();
                frmPur.txtCode.Text = row.Cells["PCode"].Value.ToString();

                frmPur.ShowDialog();
                SearchValue = frmPur.SelectedValue;
                if (SearchValue == "")
                {

                    SearchValue = row.Cells["ReasonOfReturn"].Value.ToString();
                }
            }

            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormPurchaseAdjustment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormPurchaseAdjustment", "SelectOne", exMessage);
            }

            #endregion Catch

            return SearchValue;

        }

        private void NullCheck()
        {

            IsNull = false;

            if (txtReason.Text.Trim() == "")
            {
                MessageBox.Show("Please Input Reason Of Return ");
                txtUom.Focus();
                IsNull = true;
            }

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyAll = false;

                NullCheck();
                string results = String.Empty;

                results = txtReason.Text.ToString() + FieldDelimeter + chkReturn.Checked + FieldDelimeter + ApplyAll;

                SelectedValue = results;
                if (IsNull != true)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnApplyAll_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyAll = true;
                NullCheck();
                //////if (chkReturn.Checked == false && chkInvoiceNo.Checked == false && chkInvoiceDate.Checked == false)
                //////{
                //////    MessageBox.Show("Please Select At least One Check Box  ");
                //////    IsNull = true;
                //////}
                string results = String.Empty;

                results = txtReason.Text.ToString() + FieldDelimeter + chkReturn.Checked + FieldDelimeter + ApplyAll;

                SelectedValue = results;

                if (IsNull != true)
                {
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
