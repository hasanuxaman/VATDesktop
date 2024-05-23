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
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormRegularProcess : Form
    {
        public FormRegularProcess()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                lblCurrentProcess.Visible = true;
                progressBar1.Value = 0;

                bgwProcess.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        
        private void bgwProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Comments

            //ProcessVM argument = (ProcessVM)e.Argument;
            //IssueDAL issueDal = new IssueDAL();

            //AVGPriceVm priceVm = new AVGPriceVm();

            //ProductDAL productDal = new ProductDAL();
            //DataTable dtProductCount = productDal.GetUprocessCount();


            //SetLabel("Avg price is processing");

            //priceVm.AvgDateTime = argument.VAT6_1Model.StartDate;
            //ResultVM resultVm = issueDal.UpdateAvgPrice_New_Refresh(priceVm);

            //SetLabel(dtProductCount.Rows[0][0] + " products of VAT6.1 is processing");

            //issueDal.SaveVAT6_1_Permanent(argument.VAT6_1Model);
            //e.Result = issueDal.SaveVAT6_1_Permanent_Branch(argument.VAT6_1Model);

            //SetLabel(dtProductCount.Rows[0][1] + " products of VAT6.2 is processing");
            //issueDal.SaveVAT6_2_Permanent(argument.VAT6_2Model);
            //e.Result = issueDal.SaveVAT6_2_Permanent_Branch(argument.VAT6_2Model);

            //SetLabel(dtProductCount.Rows[0][2] + " products of VAT6.2_1 is processing");
            //issueDal.SaveVAT6_2_1_Permanent(argument.VAT6_2_1Model);
            //e.Result = issueDal.SaveVAT6_2_1_Permanent_Branch(argument.VAT6_2_1Model);


            //CommonDAL commonDal = new CommonDAL();
            //commonDal.settingsUpdateMaster("DayEnd", "DayEndProcess", "N");
            //commonDal.settingsUpdateMaster("DayEnd", "VAT6_1ProcessDate", DateTime.Now.ToString("yyy-MM-dd"));
            //commonDal.settingsUpdateMaster("DayEnd", "VAT6_2ProcessDate", DateTime.Now.ToString("yyy-MM-dd"));
            //commonDal.settingsUpdateMaster("DayEnd", "VAT6_2_1ProcessDate", DateTime.Now.ToString("yyy-MM-dd"));
            //commonDal.UpdateProcessFlag();

            #endregion

            ProductDAL productdal = new ProductDAL();

            IntegrationParam processParam = new IntegrationParam
            {
                CurrentUserId = Program.CurrentUserID,
                CurrentUserName = Program.CurrentUser,
                IsBureau = Program.IsBureau,
                SetLabel = SetLabel
            };

            ResultVM resultVM = productdal.DayEndProcess(processParam);

        }

        private void bgwProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data Successfully Processed !" : e.Error.Message);
            }
            finally
            {
                progressBar1.Visible = false;
                lblCurrentProcess.Visible = false;
            }
        }


        private void SetLabel(string text)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() =>
                    {
                        lblCurrentProcess.Text = text;
                        progressBar1.Value += 1;
                    }
                ));

        }

        private void FormRegularProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwProcess.IsBusy)
            {
                bgwProcess.CancelAsync();
            }
        }
    }
}
