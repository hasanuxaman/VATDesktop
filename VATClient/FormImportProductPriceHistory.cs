using Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormImportProductPriceHistory : Form
    {

        private DataTable dtProductPriceHistory = new DataTable();
        List<ErrorMessage> errormessage = new List<ErrorMessage>();
        private string[] sqlResults;

        public FormImportProductPriceHistory()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                btnImport.Enabled = false;
                progressBar1.Visible = true;
                btnSave.Enabled = true;

                if (chkSame.Checked == false)
                {
                    BrowsFile();
                }
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return;
                }

                ProductImport();
                
                if (dataGridView1.Rows.Count > 0)
                {
                    chkSame.Checked = true;
                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
            }

            #endregion

            finally
            {
                btnImport.Enabled = true;
                progressBar1.Visible = false;

                lbCount.Text = "Total Record(s): " + dataGridView1.Rows.Count.ToString();

            }

        }

        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*";
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    Program.ImportFileName = fdlg.FileName;
                }
            }
            #endregion

            #region catch
            
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

            #endregion
        }

        private void ProductImport()
        {
            bool IsError = false;
            DataSet ds = new DataSet();

            #region try

            try
            {
                string fileName = Program.ImportFileName;
                ds = LoadDataSetFromStream(fileName);

                if (ds.Tables.Contains("ProductPriceHistory"))
                {

                    dtProductPriceHistory = ds.Tables["ProductPriceHistory"];

                    OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtProductPriceHistory, errormessage);

                    if (errormessage.Count > 0)
                    {
                        FormErrorMessage.ShowDetails(errormessage);
                        btnSave.Enabled = false;
                    }
                    errormessage.Clear();

                    DataView view = new DataView(dtProductPriceHistory);

                    dtProductPriceHistory = view.ToTable(false, "ProductGroup", "ProductCode", "ProductName", "EffectDate", "VatablePrice");

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = dtProductPriceHistory;

                }
                else
                {
                    MessageBox.Show("SheetName Not Correct");
                    return;
                }


            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", exMessage);
            }

            #endregion
        }

        private DataSet LoadDataSetFromStream(string fileName)
        {
            try
            {

                DataSet ds = new DataSet();
                FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                reader.Close();

                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                #region NullCheck
                if (dataGridView1.RowCount <= 0)
                {
                    MessageBox.Show("There are no data to save!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
              
                #endregion NullCheck

                ProductSave();

            }
            #endregion

            #region catch
            
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion
        }

        private void ProductSave()
        {

            try
            {

                if (dtProductPriceHistory.Columns.Contains("CreatedBy"))
                {
                    dtProductPriceHistory.Columns.Remove("CreatedBy");
                }
                if (dtProductPriceHistory.Columns.Contains("CreatedOn"))
                {
                    dtProductPriceHistory.Columns.Remove("CreatedOn");
                }

                var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = Program.CurrentUser };
                var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

                dtProductPriceHistory.Columns.Add(CreatedBy);
                dtProductPriceHistory.Columns.Add(CreatedOn);

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                bgwSave.RunWorkerAsync();
            }
            #region catch
           
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSave", exMessage);
            }
            #endregion

        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region statement

                sqlResults = new string[2];

                ImportDAL importDal = new ImportDAL();

                sqlResults = importDal.ImportProductPriceHistory(dtProductPriceHistory);

                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];

                    if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwProduct_RunWorkerCompleted", exMessage);
            }
            #endregion
            #region Final
            finally
            {
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
            #endregion Final
        }

    }
}
