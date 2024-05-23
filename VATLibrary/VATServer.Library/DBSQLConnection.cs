using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using VATViewModel.DTOs;
using DataTable = System.Data.DataTable;

namespace VATServer.Library
{
    public class DBSQLConnection
    {
        //private string ConnectionString = "";
        public DBSQLConnection()
        {

        }


        static int tt = 0;
        public SqlConnection GetConnectionNoPooling(SysDBInfoVMTemp connTemp = null)
        {
            string ConnectionString = "";
            if (connTemp != null)
            {
                SysDBInfoVM.SysdataSource = connTemp.SysdataSource;
                SysDBInfoVM.SysPassword = connTemp.SysPassword;
                SysDBInfoVM.SysUserName = connTemp.SysUserName;
                DatabaseInfoVM.DatabaseName = connTemp.SysDatabaseName;
            }

            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";connect Timeout=600; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
                  + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnection(SysDBInfoVMTemp connTemp = null)
        {
            string ConnectionString = "";
            if (connTemp != null)
            {
                //SysDBInfoVM.SysdataSource = connTemp.SysdataSource;
                //SysDBInfoVM.SysPassword = connTemp.SysPassword;
                //SysDBInfoVM.SysUserName = connTemp.SysUserName;
                //DatabaseInfoVM.DatabaseName = connTemp.SysDatabaseName;

                if (SysDBInfoVM.IsWindowsAuthentication)
                {
                    ConnectionString = "Data Source=" + connTemp.SysdataSource + ";trusted_Connection=True;Initial Catalog="
                        + connTemp.SysDatabaseName + ";connect Timeout=600; pooling=no;";
                }
                else
                {
                    ConnectionString = "Data Source=" + connTemp.SysdataSource + ";Initial Catalog=" + connTemp.SysDatabaseName
                        + ";user id=" + connTemp.SysUserName
                      + ";password=" + connTemp.SysPassword + ";connect Timeout=600; pooling=no;";
                }

            }
            else
            {
                if (SysDBInfoVM.IsWindowsAuthentication)
                {
                    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog="
                                       + DatabaseInfoVM.DatabaseName + ";connect Timeout=600; pooling=no;";
                }
                else
                {
                    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
                                       + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";
                }
            }
           

            //if (SysDBInfoVM.IsWindowsAuthentication)
            //{
            //    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog=" 
            //        + DatabaseInfoVM.DatabaseName + ";connect Timeout=600; pooling=no;";
            //}
            //else
            //{
            //    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
            //      + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";
            //}
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        
        public SqlConnection GetBataMiddlewareConnection(SysDBInfoVMTemp connTemp = null)
        {
            string ConnectionString = "";
            if (connTemp != null)
            {
                //SysDBInfoVM.SysdataSource = connTemp.SysdataSource;
                //SysDBInfoVM.SysPassword = connTemp.SysPassword;
                //SysDBInfoVM.SysUserName = connTemp.SysUserName;
                //DatabaseInfoVM.DatabaseName = connTemp.SysDatabaseName;

                if (SysDBInfoVM.IsWindowsAuthentication)
                {
                    ConnectionString = "Data Source=" + connTemp.SysdataSource + ";trusted_Connection=True;Initial Catalog=BataIntermediateDB;connect Timeout=600; pooling=no;";
                }
                else
                {
                    ConnectionString = "Data Source=" + connTemp.SysdataSource + ";Initial Catalog=BataIntermediateDB;user id=" + connTemp.SysUserName
                      + ";password=" + connTemp.SysPassword + ";connect Timeout=600; pooling=no;";
                }

            }
            else
            {
                if (SysDBInfoVM.IsWindowsAuthentication)
                {
                    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog=BataIntermediateDB;connect Timeout=600; pooling=no;";
                }
                else
                {
                    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=BataIntermediateDB;user id=" + SysDBInfoVM.SysUserName
                                       + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";
                }
            }
           

            //if (SysDBInfoVM.IsWindowsAuthentication)
            //{
            //    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog=" 
            //        + DatabaseInfoVM.DatabaseName + ";connect Timeout=600; pooling=no;";
            //}
            //else
            //{
            //    ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
            //      + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";
            //}
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnectionSCBL(SysDBInfoVMTemp connTemp = null)
        {
            string ConnectionString = "";
            if (connTemp != null)
            {
                SysDBInfoVM.SysdataSource = connTemp.SysdataSource;
                SysDBInfoVM.SysPassword = connTemp.SysPassword;
                SysDBInfoVM.SysUserName = connTemp.SysUserName;
                DatabaseInfoVM.DatabaseName = connTemp.SysDatabaseName;
            }
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog="
                                   + DatabaseInfoVM.DatabaseName + ";connect Timeout=600; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + "SCBL"
                                   + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnectionNoTimeOut(SysDBInfoVMTemp connTemp = null)
        {
            string ConnectionString = "";
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";connect Timeout=60000; pooling=no;";
            }

            else
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
                  + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=60000; pooling=no;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnectionForLogin(string DatabaseName)
        {
            string ConnectionString = "";
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;" +
                                         "Initial Catalog=" + DatabaseName + ";" +
                                         "connect Timeout=60;";
            }
            else
            {


                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";" +
                                        "Initial Catalog=" + DatabaseName + ";" +
                                        "user id=" + SysDBInfoVM.SysUserName + ";" +
                                        "password=" + SysDBInfoVM.SysPassword + ";" +
                                        "connect Timeout=60;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnectionLink3()
        {
            string ConnectionString = "Data Source= 192.168.15.1;" +
                                      "Initial Catalog=" + "Link3_Demo_DB" + ";" +
                                      "user id=" + "sa" + ";" +
                                      "password=" + "S123456_" + ";" +
                                      "connect Timeout=60;";
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public OdbcConnection GetJaphaConnection()
        {
            string ConnectionString = "{Tally ODBC Driver};server=10.108.1.11;Port=9999";

            OdbcConnection conn = new OdbcConnection(ConnectionString);


            return conn;
        }

        public OleDbConnection GetConnectionLink3OLEDB()
        {

            string ConnectionString = "Data Source= 192.168.15.1;" +
                                      "Initial Catalog=" + "Link3_Demo_DB" + ";" +
                                      "user id=" + "sa" + ";" +
                                      "password=" + "S123456_" + ";" +
                                      "connect Timeout=60;Provider=sqloledb";

            OleDbConnection conn = new OleDbConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetKohinoorConnection()
        {

            string ConnectionString = "Data Source=" + "192.168.4.5" + ";Initial Catalog=" + "DepotSales" + ";user id=" + "vatuser"
                + ";password=" + "vatuser" + ";connect Timeout=600; pooling=no;";

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }


        public SqlConnection GetSMCConnection()
        {
            string ConnectionString = "";
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + "192.168.4.5" + ";trusted_Connection=True;Initial Catalog=" + "DepotSales" + ";connect Timeout=600; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + "192.168.4.5" + ";Initial Catalog=" + "DepotSales" + ";user id=" + "vatuser"
                                        + ";password=" + "vatuser" + ";connect Timeout=600; pooling=no;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetDepoConnection(DataTable db)
        {
            string ConnectionString = "";
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ";trusted_Connection=True;Initial Catalog=" + db.Rows[0]["DbName"] + ";connect Timeout=600; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ";Initial Catalog=" + db.Rows[0]["DbName"] + ";user id=" + db.Rows[0]["Id"]
                  + ";password=" + db.Rows[0]["Pass"] + ";connect Timeout=600; pooling=no;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetSMCDSS_LiveConnection(DataTable db)
        {
            string ConnectionString = "";
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ";trusted_Connection=True;Initial Catalog=SMC2012_Demo_DB;connect Timeout=600; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ";Initial Catalog=SMC2012_Demo_DB;user id=" + db.Rows[0]["Id"]
                  + ";password=" + db.Rows[0]["Pass"] + ";connect Timeout=600; pooling=no;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }


        public SqlConnection GetAdhuriConnection(DataTable db)
        {
            string ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ",3341;Initial Catalog=" + db.Rows[0]["DbName"] + ";user id=" + db.Rows[0]["Id"]
                                      + ";password=" + db.Rows[0]["Pass"] + ";connect Timeout=600; pooling=no;";

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public Oracle.DataAccess.Client.OracleConnection GetSQRNewConnection(DataTable db)
        {
            //            var connectionString = @"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST="+ db.Rows[0]["IP"] +@")(PORT=1521))(CONNECT_DATA=(SID= "+db.Rows[0]["DbName"]+@")));
            //uid=" + db.Rows[0]["Id"] + ";pwd=" + db.Rows[0]["Pass"];

            string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" +
                                      db.Rows[0]["IP"] +
                                      @")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=" +
                                      db.Rows[0]["DbName"] + @")));
                                                User Id=" + db.Rows[0]["Id"] + ";Password=" +
                                      db.Rows[0]["Pass"] + ";";

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connectionString);



            return conn;
        }

   


        public OracleConnection GetSQRConnection(DataTable db)
        {
            var connectionString = @"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + db.Rows[0]["IP"] + @")(PORT=1521))(CONNECT_DATA=(SID= " + db.Rows[0]["DbName"] + @")));
uid=" + db.Rows[0]["Id"] + ";pwd=" + db.Rows[0]["Pass"];

            OracleConnection conn = new OracleConnection(connectionString);



            return conn;
        }



        public OracleConnection GetSQRConnectionFix()
        {
            var connectionString = @"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + "172.24.1.26"
                + @")(PORT=1521))(CONNECT_DATA=(SID= " + "STLDB1" + @")));
                uid=" + "stl_new_vat" + ";pwd=stlvat";

            OracleConnection conn = new OracleConnection(connectionString);



            return conn;
        }

        public Oracle.DataAccess.Client.OracleConnection GetDBHConnection(DataTable db)
        {
//            var connectionString = @"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + db.Rows[0]["IP"] + @")(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=dbhrac" + @")));
//uid=" + db.Rows[0]["Id"] + ";pwd=" + db.Rows[0]["Pass"];

            string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" +
                               db.Rows[0]["IP"] +
                @")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" +
                "dbhrac" + @")));
                                                User Id=" + db.Rows[0]["Id"] + ";Password=" +
                               db.Rows[0]["Pass"] + ";";

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connectionString);

            return conn;
        }



        public Oracle.DataAccess.Client.OracleConnection GetBataConnection()
        {

            string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" +
                                      "172.20.10.156" +
                                      @")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=" +
                                      "xe" + @")));
                                                User Id=" + "symphony" + ";Password=" +
                                      "symphony2020" + ";";

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connectionString);


            return conn;
        }



        public Oracle.DataAccess.Client.OracleConnection GetOracleConnection(DataTable db)
        {
            CommonDAL commonDal = new CommonDAL();
            string value  = commonDal.settings("Integration", "OracleServiceName");

            string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" +
                                      db.Rows[0]["IP"] +
                                      ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(@Service_Name=" +
                                      "nou" + @")));
                                                User Id=" + db.Rows[0]["Id"] + ";Password=" +
                                      db.Rows[0]["Pass"] + ";";

            connectionString = connectionString.Replace("@Service_Name", value == "Y" ? "Service_Name" : "SID");

            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connectionString);


            return conn;
        }



        public Oracle.DataAccess.Client.OracleConnection xxxGetOracleNourishConnection(DataTable db)
        {
            CommonDAL commonDal = new CommonDAL();
            string value  = commonDal.settings("Integration", "OracleServiceName");

            string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" +
                                      "172.16.25.94" +
                                      ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(Service_Name=" +
                                      "nou" + @")));
                                                User Id=" + db.Rows[0]["Id"] + ";Password=" +
                                      db.Rows[0]["Pass"] + ";";


            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connectionString);


            return conn;
        }

        public Oracle.DataAccess.Client.OracleConnection GetOracleNourishConnection(DataTable db)
        {
            CommonDAL commonDal = new CommonDAL();
            string value = commonDal.settings("Integration", "OracleServiceName");

            string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" +
                                      "172.16.25.243" +
                                      ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(Service_Name=" +
                                      "nou" + @")));
                                                User Id=" + db.Rows[0]["Id"] + ";Password=" +
                                      db.Rows[0]["Pass"] + ";";


            Oracle.DataAccess.Client.OracleConnection conn = new Oracle.DataAccess.Client.OracleConnection(connectionString);


            return conn;
        }



        // 172.16.25.229


        public SqlConnection GetConnectionSys(SysDBInfoVMTemp connTemp = null)
        {
            string ConnectionString = "";
            if (connTemp != null)
            {
                SysDBInfoVM.SysdataSource = connTemp.SysdataSource;
                SysDBInfoVM.SysPassword = connTemp.SysPassword;
                SysDBInfoVM.SysUserName = connTemp.SysUserName;
            }
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";trusted_Connection=True;" +
                                     "Initial Catalog=SymphonyVATSys;" +
                                     "connect Timeout=60;";
            }
            else
            {

                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";" +
                                        "Initial Catalog=SymphonyVATSys;" +
                                        "user id=" + SysDBInfoVM.SysUserName + ";" +
                                        "password=" + SysDBInfoVM.SysPassword + ";" +
                                        "connect Timeout=60;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionMaster()
        {
            string ConnectionString = "";
            if (SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=master;trusted_Connection=True;connect Timeout=60;";
            }
            else
            {
                ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=master; user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=60;";
            }
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public string ServerDateTime()
        {
            string result = "19800101";
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                #region open connection and transaction

                currConn = GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"use master";
                sqlText += @" SELECT CONVERT(VARCHAR(8), SYSDATETIME(), 112)";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                result = cmdIdExist.ExecuteScalar().ToString();

            }
            #region Catch

            catch (Exception ex)
            {
            }
            #endregion
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            return result;

        }

        public string ServerPrintDateTime()
        {
            string result = "19800101";
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                #region open connection and transaction

                currConn = GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"use master";
                sqlText += @" select convert(varchar, SYSDATETIME(), 0)";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                result = cmdIdExist.ExecuteScalar().ToString();

            }
            #region Catch

            catch (Exception ex)
            {
            }
            #endregion
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            return result;

        }


        public string GetConnectionString(DataTable db)
        {
            string ConnectionString = "";
            if (!SysDBInfoVM.IsWindowsAuthentication)
            {
                ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ";Initial Catalog=" +
                                         db.Rows[0]["DbName"] + ";user id=" + db.Rows[0]["Id"]
                                         + ";password=" + db.Rows[0]["Pass"] + ";connect Timeout=600; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + db.Rows[0]["IP"] + ";Initial Catalog=" +
                                   db.Rows[0]["DbName"] + ";trusted_Connection=true; connect Timeout=600;";
            }

            return ConnectionString;
        }
    }
}
