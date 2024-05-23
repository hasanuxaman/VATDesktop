using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Windows.Forms;
using VATViewModel.DTOs;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using SymphonySofttech.Utilities;
using VATClient.ReportPages;
using VATClient.ReportPreview;
using VATServer.Library;
using VATViewModel.DTOs;
using VATClient.ModelDTO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management;
using System.Globalization;
using System.Net.NetworkInformation;


namespace VATClient
{
    public class CommonFormMethod
    {
        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName, string fieldValue)
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


        public DataGridViewRow ProductLoad(ProductVM vm)
        {
            DataGridViewRow selectedRow = new DataGridViewRow();

            try
            {

                string SqlText = @"   
Select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  
From Products p
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 
and p.ActiveStatus='Y'  
and @Filter";

                string FilterOverhead = "pc.IsRaw not in ('Overhead')";

                if (vm.IsRaw == "Overhead")
                {
                    FilterOverhead = "pc.IsRaw in ('Overhead')";
                }

                SqlText = SqlText.Replace("@Filter", FilterOverhead);


                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);



            }
            catch (Exception ex)
            {
                FileLogger.Log("ProductLoad", "ProductGroupLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductLoad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { }

            return selectedRow;
        }

        
    }
}
