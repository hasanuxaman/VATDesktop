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
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormCode : Form
    {
        #region Constructors

        public FormCode()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        #endregion

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        private DataSet CodeResult;
        private List<CodeVM> codeVMs;
        private IList<CodeVM> codeVMList;
        private bool ChangeData = false;

        private string preCellValue = string.Empty;

        private string prePrefixValue = string.Empty;
        private string preLenthValue = string.Empty;

        #region sql save, update, delete

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Methods

        private void GetLoad()
        {
            try
            {
                #region Statement

                this.progressBar1.Visible = true;

                backgroundWorkerLoad.RunWorkerAsync();

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GetLoad", exMessage);
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
                FileLogger.Log(this.Name, "GetLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GetLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GetLoad", exMessage);
            }
            #endregion
        }

       

        private void DataGridLoad(List<CodeVM> dgvCodeVms)
        {
            try
            {
                #region Code
                var codeDataTable = ConvertToDataTable(dgvCodeVms);

                dgvCode.Rows.Clear();
                int j = 0;
                foreach (DataRow item in codeDataTable.Rows)
                {
                    DataGridView dgv = new DataGridView();
                    dgvCode.Rows.Add(dgv);
                    dgvCode.Rows[j].Cells["CodeId"].Value = item["CodeId"].ToString();
                    dgvCode.Rows[j].Cells["CodeGroup"].Value = Program.AddSpacesToSentence(item["CodeGroup"].ToString());
                    dgvCode.Rows[j].Cells["CodeName"].Value = Program.AddSpacesToSentence(item["CodeName"].ToString());
                    dgvCode.Rows[j].Cells["prefix"].Value = item["prefix"].ToString();
                    dgvCode.Rows[j].Cells["prefixOld"].Value = item["prefixOld"].ToString();
                    dgvCode.Rows[j].Cells["Lenth"].Value = item["Lenth"].ToString();

                    j += 1;
                }

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "DataGridLoad", exMessage);
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
                FileLogger.Log(this.Name, "DataGridLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DataGridLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "DataGridLoad", exMessage);
            }

            #endregion
        }

        private static DataTable ConvertToDataTable(List<CodeVM> codeVMList)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(CodeVM));
            DataTable dataTable = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                dataTable.Columns.Add(prop.Name, prop.PropertyType);

            }

            object[] values = new object[props.Count];

            foreach (CodeVM item in codeVMList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        #endregion

        private void FormCode_Load(object sender, EventArgs e)
        {
            bgwSettingsValue.RunWorkerAsync();
            //GetLoad();
            //SetCodeGroup();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try statement
            try
            {
                this.progressBar1.Visible = true;
                this.btnUpdate.Enabled = false;

                codeVMs = new List<CodeVM>();

                for (int i = 0; i < dgvCode.RowCount; i++)
                {
                    CodeVM codeVM = new CodeVM();

                    codeVM.CodeId = dgvCode.Rows[i].Cells["CodeId"].Value.ToString();
                    codeVM.CodeGroup = dgvCode.Rows[i].Cells["CodeGroup"].Value.ToString().Trim().Replace(" ","");
                    codeVM.CodeName = dgvCode.Rows[i].Cells["CodeName"].Value.ToString().Trim().Replace(" ", "");
                    codeVM.prefix = dgvCode.Rows[i].Cells["prefix"].Value.ToString().ToUpper();
                    codeVM.Lenth = dgvCode.Rows[i].Cells["Lenth"].Value.ToString();
                    codeVM.prefixOld = dgvCode.Rows[i].Cells["prefixOld"].Value.ToString().ToUpper();
                    codeVMs.Add(codeVM);

                }//End For

                backgroundWorkerUpdate.RunWorkerAsync();
            }
            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try Statement
            try
            {
                CodeDAL codeDal = new CodeDAL();
                CodeResult = codeDal.SearchCodes(connVM);
            }
            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                dgvCode.Rows.Clear();
                int j = 0;
                //foreach (DataRow item in CodeResult.Tables[0].Rows)
                //{
                //    DataGridView dgv = new DataGridView();
                //    dgvCode.Rows.Add(dgv);
                //    dgvCode.Rows[j].Cells["CodeId"].Value = item["CodeId"].ToString();
                //    dgvCode.Rows[j].Cells["CodeGroup"].Value = Program.AddSpacesToSentence(item["CodeGroup"].ToString());
                //    dgvCode.Rows[j].Cells["CodeName"].Value = Program.AddSpacesToSentence(item["CodeName"].ToString());
                //    dgvCode.Rows[j].Cells["prefix"].Value = item["prefix"].ToString();
                //    dgvCode.Rows[j].Cells["Lenth"].Value = item["Lenth"].ToString();
                //    dgvCode.Rows[j].Cells["prefixOld"].Value = item["prefixOld"].ToString();

                //    j += 1;
                //}

                codeVMList = new List<CodeVM>();

                foreach (DataRow item in CodeResult.Tables[0].Rows)
                {
                    CodeVM codeVm = new CodeVM();

                    codeVm.CodeId = item["CodeId"].ToString();
                    codeVm.CodeGroup = item["CodeGroup"].ToString();
                    codeVm.CodeName = item["CodeName"].ToString();
                    codeVm.prefix = item["prefix"].ToString();
                    codeVm.prefixOld = item["prefixOld"].ToString();
                    codeVm.Lenth = item["Lenth"].ToString();

                    codeVMList.Add(codeVm);

                }
                foreach (DataRow item in CodeResult.Tables[1].Rows)
                {
                    cmbCodeGroup.Items.Add(Program.AddSpacesToSentence(item["CodeGroup"].ToString()));

                }
                cmbCodeGroup.Items.Insert(0, "Select One");
                cmbCodeGroup.SelectedIndex = 0;
               
            }

            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerLoad_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
            }
        }
        
        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try statement
            try
            {
                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                CodeDAL codeDal = new CodeDAL();
                sqlResults = codeDal.CodeUpdate(codeVMs,connVM);

                UPDATE_DOWORK_SUCCESS = true;
            }
            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try statement
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
                            throw new ArgumentNullException("backgroundWorkerUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }


                    }
                }
            }
            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }
        }

        private void dgvCode_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                //getting prefixValue from datagrid by rowindex and columindex
                string cellValueExiting = dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                int columnIndex = e.ColumnIndex;

                if (columnIndex == 3) // prefix
                {
                    prePrefixValue = string.Empty;
                    prePrefixValue = cellValueExiting.ToUpper();
                }
                else if (columnIndex == 4) // Lenth
                {
                    preLenthValue = string.Empty;
                    preLenthValue = cellValueExiting.ToUpper();
                }


            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", exMessage);
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
                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvCode_CellBeginEdit", exMessage);
            }
            #endregion
        }

        private void dgvCode_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region try

            try
            {
                //getting Cell Value from datagrid by rowindex and columindex
                string cellValue = dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                DataGridViewCellStyle cellStyleValid = new DataGridViewCellStyle();
                cellStyleValid.ForeColor = Color.Black;

                DataGridViewCellStyle cellStyleError = new DataGridViewCellStyle();
                cellStyleError.ForeColor = Color.Red;

                int columnIndex = e.ColumnIndex;

                if (columnIndex == 3) // prefix
                {

                    if (Program.formatCodePrefix(cellValue))
                    {
                        dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        return;
                    }
                    else
                    {
                        dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = prePrefixValue;
                        dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        return;
                    }
                }
                else if (columnIndex == 4) // Lenth
                {
                    if (Program.formatCodeLength(cellValue))
                    {
                        dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleValid;
                        return;
                    }
                    else
                    {
                        dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = preLenthValue;
                        dgvCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = cellStyleError;
                        return;
                    }
                }

            }

            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", exMessage);
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
                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvCode_CellEndEdit", exMessage);
            }
            #endregion
        }

        private void cmbCodeGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChangeData == true)
                {
                    DialogResult dialogResult = MessageBox.Show(@"Recent changes have not been saved ." + "\n" + " Want to close without saving?", @"", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        return;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        cmbCodeGroup.SelectedIndex = cmbCodeGroup.SelectedIndex;
                    }
                }

                #region Code

                string cmbCodeGroupText = cmbCodeGroup.Text.Trim().Replace(" ","");
                int cmbCodeGroupIndex = cmbCodeGroup.SelectedIndex;
                if (cmbCodeGroupIndex != 0)
                {
                    var tempCodeVms = codeVMList.Where(x => x.CodeGroup == cmbCodeGroupText).ToList();
                    DataGridLoad(tempCodeVms);
                }
                else if (cmbCodeGroupIndex == 0)
                {
                    var tempCodeVMs = codeVMList.ToList();
                    DataGridLoad(tempCodeVMs);
                }

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", exMessage);
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
                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbCodeGroup_SelectedIndexChanged", exMessage);
            }

            #endregion
        }

        private void FormCode_FormClosing(object sender, FormClosingEventArgs e)
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
                    }

                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            #endregion
        }

        private void bgwSettingsValue_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try statement
            try
            {

                UPDATE_DOWORK_SUCCESS = false;
                string sqlResultssettings = string.Empty;
                CodeDAL codeDal = new CodeDAL();

                ////sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollProductionConsumptions", "TPC", "4", null, null, connVM);
                ////sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollContInOuts", "TCIO", "4", null, null, connVM);
                ////sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClientInOuts", "TCLI", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4Backs", "TCLB", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4BacksWIP", "TCBW", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4BacksFG", "TCBF", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4Ins", "TCLI", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4InsWIP", "TCIW", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4Outs", "TCLO", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4OutWIP", "TCOW", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClient6_4OutFG", "TCOF", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClientConsumptions", "TCLC", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClientConsumptionsWIP", "TCCW", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollClientConsumptionsFG", "TCCF", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollCont6_4Backs", "TCOB", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollCont6_4Ins", "TCOI", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollCont6_4Outs", "TCOO", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollContConsumptions", "TCOC", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "TollContProductions", "TCOP", "4", null, null, connVM);


                sqlResultssettings = codeDal.CodeDataInsert("Sale", "SA-01", "SA01", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "SA-02", "SA02", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "SA-04", "SA04", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "IN-43", "IN43", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "IN-44", "IN44", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("DayEnd", "Other", "DAE", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "TradeChallan", "TRC", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "BankDepositSA1", "BDPSA1", "5", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "BankDepositSA2", "BDPSA2", "5", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "BankPaymentReceive", "BPR", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Sale", "CRInvoice", "CRI", "4", null, null, connVM);

                sqlResultssettings = codeDal.CodeDataInsert("BillInvoice", "BillInvoice", "BIN", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Dispose", "Trading", "DST", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("TollFGReceiveWithoutBOM", "TollFGReceiveWithoutBOM", "TFW", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("TollIssueWithoutBOM", "TollIssueWithoutBOM", "TIW", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("CustomerItem", "Other", "INV", "4", null, null, connVM);


                sqlResultssettings = codeDal.CodeDataInsert("TransferCTC", "TransferRaw", "RTR", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("TransferCTC", "TransferWastage", "WTR", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("TransferCTC", "TransferFinish", "FTR", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Sale", "TollSale", "TSI", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Sale", "DisposeRaw", "DPR", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "DisposeFinish", "DPF", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Sale", "RawCredit", "RCN", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Toll", "Invoice6_3", "TIV", "4", null, null,connVM);

                //06 Dec 2020
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "RawIssue", "CRI", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "RawReceive", "CRR", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "FGReceive", "CFR", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Toll", "Client6_3", "CIV", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("VDB", "VDB", "VDB", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("SDB", "SDB", "VDB", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Sale", "RawSale", "RIN", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "CommercialImporter", "SCI", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Purchase", "CommercialImporter", "PCI", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("SaleExport", "SaleExport", "SNX", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "BTB", "BTB", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "61Out", "T1O", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "62Out", "T2O", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "61In", "T1I", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Transfer", "62In", "T2I", "4", null, null,connVM);

                sqlResultssettings = codeDal.CodeDataInsert("DDB", "DDB", "DDB", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Purchase", "PurchaseDN", "PDN", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Purchase", "PurchaseCN", "PCN", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "ServiceNS", "SNS", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "ExportServiceNS", "ENS", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "ExportServiceNSCredit", "ESC", "4", null, null,connVM);
                #region Wastage
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "SaleWastage", "SLW", "4", null, null);
                sqlResultssettings = codeDal.CodeDataInsert("Issue", "IssueWastage", "ISW", "4", null, null);
                sqlResultssettings = codeDal.CodeDataInsert("Issue", "ContractorIssueWoBOM", "CTI", "4", null, null);
                #endregion

                sqlResultssettings = codeDal.CodeDataInsert("Purchase", "ServiceNS", "PSN", "4", null, null, connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Purchase", "PurchaseTollcharge", "PTC", "4", null, null, connVM);
                 
                    sqlResultssettings = codeDal.CodeDataInsert("Purchase", "Service", "PSE", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("VAT11GaGa", "VAT11GaGa", "VGA", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "AdjCashPayble", "ACP", "4", null, null,connVM);
                
                sqlResultssettings = codeDal.CodeDataInsert("Receive", "Package", "RPK", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "Package", "SPK", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Sale", "Delivery", "SDC", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("VAT7", "VAT7", "VT7", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("TDS", "TDS", "TDS", "4", null, null,connVM);

                #region Demand 
                sqlResultssettings = codeDal.CodeDataInsert("Demand", "Other", "DEM", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("Demand", "Receive", "REC", "4", null, null,connVM);
                sqlResultssettings = codeDal.CodeDataInsert("TransferRaw", "TransferRaw", "TRR", "4", null, null,connVM);
                
                #endregion
                #region ReverseAdjustment
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "Treasury-Credit", "DCN", "4", null, null);
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "VDSSale", "SVD", "4", null, null);
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "VDS-Credit", "VCN", "4", null, null);
                sqlResultssettings = codeDal.CodeDataInsert("SDDeposit", "Treasury-Credit", "SCN", "4", null, null);
                sqlResultssettings = codeDal.CodeDataInsert("Deposit", "AdjCashPayble-Credit", "ACN", "4", null, null);
            #endregion

                UPDATE_DOWORK_SUCCESS = true;
            }
            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSettingsValue_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSettingsValue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try statement
            try
            {
                if (UPDATE_DOWORK_SUCCESS)
                {
                    GetLoad();
                }
            }
            #endregion
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSettingsValue_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {

                this.progressBar1.Visible = false;
                this.btnUpdate.Enabled = true;
            }
        }

        private void dgvCode_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim().ToLower().Replace(" ", "");
                    var tempCodeVms = codeVMList.Where(x =>
                        x.prefix.ToLower().Contains(search)
                        || x.CodeGroup.ToLower().Contains(search)
                        || x.CodeName.ToLower().Contains(search)
                        
                        ).ToList();
                    DataGridLoad(tempCodeVms);

        }

    }
}
