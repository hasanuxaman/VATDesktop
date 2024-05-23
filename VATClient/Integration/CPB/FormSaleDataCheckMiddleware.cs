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
using VATServer.Ordinary;
using VATViewModel.DTOs;
using System.Xml;

namespace VATClient.Integration.CPB
{
    public partial class FormSaleDataCheckMiddleware : Form
    {
        public FormSaleDataCheckMiddleware()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        string folderLocation = @"E:\Old Data\Clients\CPB\CP Log 18July2021\";
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private void FormSaleDataCheckMiddleware_Load(object sender, EventArgs e)
        {

        }
        private void DataCheck( string refNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refNo))
                {
                    MessageBox.Show("Please input Ref No");
                    return;
                }

                string rootDirectory = folderLocation + refNo + ".txt";
                DataSet dataSet = new DataSet();

                if (System.IO.File.Exists(rootDirectory))
                {
                  string text= File.ReadAllText(rootDirectory);
                  StringReader reader = new StringReader(text);

                  //XmlReaderSettings settings = new XmlReaderSettings();
                  //settings.DtdProcessing = DtdProcessing.Ignore;

                  //using (XmlReader readerx = XmlReader.Create(text, settings))
                  //{
                      dataSet.ReadXml(reader);
                  //}

                  var table = dataSet.Tables[0];

                 //dataSet= OrdinaryVATDesktop.GetDataSetFromXML(text);
                    //StringReader reader = new StringReader(rootDirectory);

                    //dataSet.ReadXml(reader);

                    //var table = dataSet.Tables[0];

                    //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    //doc.Load(rootDirectory);
                    //DataSet ds = new DataSet();
                    //ds.ReadXml(rootDirectory);
                    //dgvHistory.DataSource = ds;
                }
                else
                {
                    MessageBox.Show("Data not found with the Ref No");
                    return;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            DataCheck(txtRefNo.Text.Trim());
        }
    }
}
