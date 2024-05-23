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
using SymphonySofttech.Reports;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormVAT9_1FileUpload : Form
    {
        DataSet ds;
        DataTable dtTableResult = new DataTable();


        public FormVAT9_1FileUpload()
        {
            InitializeComponent();
        }

        private void btnload_Click(object sender, EventArgs e)
        {
            LoadGridData();
        }

        private void FormVAT9_1FileUpload_Load(object sender, EventArgs e)
        {
            LoadGridData();
        }

        private void LoadGridData()
        {
            try
            {
                IVAS_API dal = new IVAS_API();
                string periodId = txtPeriodId.Text.Trim();
                if (string.IsNullOrEmpty(periodId))
                {
                    periodId = "";
                }
                dtTableResult = dal.SelectAll(periodId, null, null, null);
                dgvSubForm.DataSource = dtTableResult;
                dgvSubForm.Columns["PeriodId"].Visible = false;
                dgvSubForm.Columns["FileLocation"].Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Program.ImportFileName = null;
            }
        }

        private void btnFileLoad_Click(object sender, EventArgs e)
        {
            try
            {
                Program.ImportFileName = null;
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Program.ImportFileName))
                {
                    BrowsFile();
                }
                else
                {
                    MessageBox.Show(this, "Please select the right file for import");
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Program.ImportFileName = null;
            }
        }

        private void BrowsFile()
        {
            #region try
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            try
            {
                string note = cmbNoteNo.Text.Trim();
                if (string.IsNullOrEmpty(note))
                {
                    MessageBox.Show(this, "Please select the note.");
                    return;
                }

                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";


                ////fdlg.Filter = "(*.doc; *.docx)|*.doc;*.docx;*.pdf|PDF files (*.pdf)|*.pdf|Excel files (*.xls; *.xlsx)|*.xls;*.xlsx|Text files (*.txt)|*.txt|Image files (*.jpg; *.png)|*.jpg;*.png|ZIP files (*.zip)|*.zip";
                fdlg.Filter = "(*.doc; *.docx;*.pdf;*.xls;*.xlsx;*.txt;*.jpg;*.png;*.zip)|*.doc;*.docx;*.pdf;*.xls;*.xlsx;*.txt;*.jpg;*.png;*.zip";


                fdlg.FilterIndex = 1;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }

                if (!string.IsNullOrWhiteSpace(Program.ImportFileName))
                {
                    string baseDirectory = Program.AppPath + "IVAS\\" + txtPeriodId.Text.Trim();
                    string archiveDirectory = Program.AppPath + "IVAS\\IVAS_Archive\\" + txtPeriodId.Text.Trim();

                    if (!Directory.Exists(baseDirectory))
                    {
                        Directory.CreateDirectory(baseDirectory);
                    }
                    if (!Directory.Exists(archiveDirectory))
                    {
                        Directory.CreateDirectory(archiveDirectory);
                    }

                    string uploadFileName = Path.GetFileName(Program.ImportFileName);

                    string fileName = string.Empty;
                    string fileType = string.Empty;

                    if (!string.IsNullOrEmpty(Program.ImportFileName) && Program.ImportFileName.Contains("."))
                    {
                        string[] parts = uploadFileName.Split('.');

                        fileType = parts.Last();
                    }

                    fileName = note + "." + fileType;
                    fileName = fileName.Replace("\\", "-").Replace("/", "-");
                    uploadFileName = fileName;

                    IVAS_API dal = new IVAS_API();
                    VAT9_1NBRApi_AttachmentVM paramVm = new VAT9_1NBRApi_AttachmentVM();
                    if (note == "Mushak6_1")
                    {
                        paramVm.DocType = "02";
                    }
                    else if (note == "Mushak6_2_1")
                    {
                        paramVm.DocType = "03";
                    }
                    else if (note == "Any Other Documents")
                    {
                        paramVm.DocType = "05";
                    }
                    else if (note == "Mushak6_10")
                    {
                        paramVm.DocType = "06";
                    }
                    else if (note == "Note24/29")
                    {
                        paramVm.DocType = "04";
                    }
                    else if (note == "Note-31")
                    {
                        paramVm.DocType = "07";
                    }
                    else if (note == "Note-26")
                    {
                        paramVm.DocType = "08";
                    }
                    else if (note == "Note-27")
                    {
                        paramVm.DocType = "09";
                    }
                    else if (note == "Note-32")
                    {
                        paramVm.DocType = "10";
                    }
                    paramVm.FileName = fileName;
                    paramVm.FileType = fileType;
                    paramVm.FileLocation = baseDirectory;
                    paramVm.PeriodId = txtPeriodId.Text.Trim();
                    paramVm.Notes = note;

                    retResults = dal.VAT9_1NBRApi_Attachment(paramVm, null, null, null);

                    if (retResults[0] == "Fail")
                    {
                        MessageBox.Show(this, retResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        string destinationPath = Path.Combine(baseDirectory, uploadFileName);
                        File.Copy(Program.ImportFileName, destinationPath, true);

                        string periodId = txtPeriodId.Text.Trim();
                        if (string.IsNullOrEmpty(periodId))
                        {
                            periodId = "";
                        }
                        dtTableResult = dal.SelectAll(periodId, null, null, null);
                        dgvSubForm.DataSource = dtTableResult;
                        MessageBox.Show(this, "File Upload Successful.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(this, "Please select the right file for import");
                    return;
                }
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Program.ImportFileName = null;
            }
            #endregion
        }

        private bool IsRowSelected()
        {
            DataGridView gd = dgvSubForm;
            DataTable dt = (DataTable)gd.DataSource;

            dtTableResult = new DataTable();
            ////adding column name
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                dtTableResult.Columns.Add(dt.Columns[j].ColumnName);
            }

            ////adding data rows
            for (int i = 0; i < gd.Rows.Count; i++)
            {
                if (Convert.ToBoolean(gd["Select", i].Value) == true)
                {
                    dtTableResult.Rows.Add(dt.Rows[i].ItemArray);
                }
            }

            return dtTableResult.Rows.Count > 0;
        }

        private void DeleteFiles(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        // Delete the file
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(this, "File does not exist.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Program.ImportFileName = null;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                string[] retResults = new string[4];
                retResults[0] = "Fail";
                retResults[1] = "Fail";

                if (!IsRowSelected())
                {
                    MessageBox.Show(this, "Please At least Select a Row");
                    return;
                }
                if (dtTableResult.Rows.Count > 1)
                {
                    MessageBox.Show(this, "Please Select One Row.");
                    return;
                }
                else
                {
                    DialogResult = MessageBox.Show("Do you want to delete this data?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DialogResult == DialogResult.Yes)
                    {
                        IVAS_API dal = new IVAS_API();
                        VAT9_1NBRApi_AttachmentVM paramVm = new VAT9_1NBRApi_AttachmentVM();
                        paramVm.FileName = dtTableResult.Rows[0]["FileName"].ToString();
                        paramVm.FileType = dtTableResult.Rows[0]["FileType"].ToString();
                        paramVm.DocType = dtTableResult.Rows[0]["DocType"].ToString();
                        paramVm.FileLocation = dtTableResult.Rows[0]["FileLocation"].ToString();
                        paramVm.PeriodId = dtTableResult.Rows[0]["PeriodId"].ToString();
                        paramVm.Notes = dtTableResult.Rows[0]["Notes"].ToString();

                        retResults = dal.Delete(paramVm, null, null, null);

                        if (retResults[0] == "Fail")
                        {
                            MessageBox.Show(this, retResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {

                            string archiveDirectory = Program.AppPath + "IVAS\\IVAS_Archive\\" + txtPeriodId.Text.Trim();
                            string file = paramVm.FileLocation + "\\" + paramVm.FileName;
                            string uploadFileName = Path.GetFileName(file);

                            if (!Directory.Exists(archiveDirectory))
                            {
                                Directory.CreateDirectory(archiveDirectory);
                            }

                            string destinationPath = Path.Combine(archiveDirectory, uploadFileName);
                            File.Copy(file, destinationPath, true);

                            #region Dispose & Delete
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            DeleteFiles(file);
                            #endregion

                            dtTableResult = dal.SelectAll(paramVm.PeriodId, null, null, null);
                            dgvSubForm.DataSource = dtTableResult;
                            MessageBox.Show(this, retResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.None;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }





    }
}
