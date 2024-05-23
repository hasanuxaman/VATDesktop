using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATServer.License;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace SymphonySofttech.Reports
{
    public class ProductReport
    {
        public ReportDocument ProductNew(string ItemNo, string CategoryID, string IsRaw, SysDBInfoVMTemp connVM = null, string ProductCode = "")
        {
            try
            {
                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.ProductNew(ItemNo, CategoryID, IsRaw, connVM, ProductCode);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DsProduct";

                RptProductListing objrpt = new RptProductListing();

                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Product Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + OrdinaryVATDesktop.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReportDocument ReportBOMAnnexure(ParameterVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ReportDocument rptDoc = new ReportDocument();
            DataSet ds = new DataSet();
            ReportDSDAL _ReportDSDAL = new ReportDSDAL();


            #endregion

            try
            {
               
                #region Data Call
                
                ds = _ReportDSDAL.ReportBOMAnnexure(paramVM,connVM);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("Annexure is not Available for this Product!");
                }

                #endregion

                #region Table Name
                
                ds.Tables[0].TableName = "dtBOMAnnexure";

                #endregion

                #region Set Data Source

                rptDoc = new RptBOMAnnexure();
                ////rptDoc.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptBOMAnnexure.rpt");




                rptDoc.SetDataSource(ds);

                #endregion

            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            finally { }

            return rptDoc;

        }


    }






    public class CommonFormMethod
    {
        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName, string fieldValue, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                FormulaFieldDefinition fs;
                fs = crFormulaF[fieldName];
                objrpt.DataDefinition.FormulaFields[fieldName].Text = "'" + fieldValue + "'";
            }
            catch (Exception ex)
            {


            }


        }

    }
}
