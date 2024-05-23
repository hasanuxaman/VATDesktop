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
   public class VendorGroupReport
    {
       public ReportDocument VendorGroupNew(string VendorGroupID, SysDBInfoVMTemp connVM = null)
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
               ReportResult = reportDsdal.VendorGroupNew(VendorGroupID,connVM);


               #region Complete
               if (ReportResult.Tables.Count <= 0)
               {
                   return null;
               }
               ReportResult.Tables[0].TableName = "DsVendorGroup";

               RptVendorGroup objrpt = new RptVendorGroup();
               objrpt.SetDataSource(ReportResult);
               objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
               objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
               objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
               //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
               objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
               objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
               objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
               objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
               objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
               //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
               objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
               FormulaFieldDefinitions crFormulaF;
               crFormulaF = objrpt.DataDefinition.FormulaFields;
               CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

               _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

               #endregion


               return objrpt;
           }
           catch(Exception ex)
           {
               throw ex;
           }
       }
    }
}
