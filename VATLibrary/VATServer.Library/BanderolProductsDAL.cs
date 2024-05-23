using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Interface;


namespace VATServer.Library
{
    public class BanderolProductsDAL : IBanderolProducts 
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //
        public string[] InsertToBanderolProducts(string BandProductId, string ItemNo, string BanderolID, string PackagingId, decimal BUsedQty, string ActiveStatus,
            string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string WastageQty, decimal OpeningQty, string OpeningDate, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(BandProductId))
                {
                    throw new ArgumentNullException("InsertToBanderolProducts","Please enter Product name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToBanderolProducts");

                #endregion open connection and transaction
                #region check in product table
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    //sqlText = "select count(distinct ItemNo) from Products where ItemNo='" + ItemNo + "'";
                    sqlText = "select count(distinct ItemNo) from Products where ItemNo = @ItemNo";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                    //BugsBD
                    SqlParameter parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                    parameter.Value = ItemNo;
                    cmdIdExist.Parameters.Add(parameter);

                    cmdIdExist.Transaction = transaction;
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToBanderolProducts", "Product information does not exist in Products list.");
                    }

                }
                #endregion

                #region Insert Product Information

                //sqlText = "select count(distinct ItemNo) from BanderolProducts where  ItemNo = '" + ItemNo + "'";
                sqlText = "select count(distinct ItemNo) from BanderolProducts where  ItemNo = @ItemNo";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);

                //BugsBD
                SqlParameter parameter2 = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                parameter2.Value = ItemNo;
                cmdNameExist.Parameters.Add(parameter2);


                cmdNameExist.Transaction = transaction;
                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("InsertToBanderolProducts", "Requested Product is already exist");
                }

                sqlText = "select isnull(max(cast(BandProductId as int)),0)+1 FROM  BanderolProducts";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Banderol ProductId";
                    throw new ArgumentNullException("InsertToBanderolProducts","Unable to create new Banderol Product information Id");
                }

                sqlText = "";
                sqlText += "insert into BanderolProducts";
                sqlText += "(";
                sqlText += "BandProductId,";
                sqlText += "ItemNo,";
                sqlText += "BanderolId,";
                sqlText += "PackagingId,";
                sqlText += "BUsedQty,";
                sqlText += "ActiveStatus,";
                sqlText += "WastageQty,";
                sqlText += "OpeningQty,";
                sqlText += "OpeningDate,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";

                sqlText += "@nextId,";
                sqlText += "@ItemNo,";
                sqlText += "@BanderolID,";
                sqlText += "@PackagingId,";
                sqlText += "@BUsedQty,";
                sqlText += "@ActiveStatus,";
                sqlText += "@WastageQty,";
                sqlText += "@OpeningQty,";
                sqlText += "@OpeningDate,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn";

                //sqlText += "'" + nextId + "',";
                //sqlText += "'" + ItemNo + "',";
                //sqlText += "'" + BanderolID + "',";
                //sqlText += "'" + PackagingId + "',";
                //sqlText += "'" + BUsedQty + "',";
                //sqlText += "'" + ActiveStatus + "',";
                //sqlText += "'" + WastageQty + "',";
                //sqlText += "'" + OpeningQty + "',";
                //sqlText += "'" + OpeningDate + "',";
                //sqlText += "'" + CreatedBy + "',";
                //sqlText += "'" + CreatedOn + "',";
                //sqlText += "'" + LastModifiedBy + "',";
                //sqlText += "'" + LastModifiedOn + "'";

                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                //BugsBD
                SqlParameter parameter3 = new SqlParameter("@nextId", SqlDbType.Int, 250);
                parameter3.Value = nextId;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                parameter3.Value = ItemNo;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@BanderolID", SqlDbType.VarChar, 250);
                parameter3.Value = BanderolID;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@PackagingId", SqlDbType.VarChar, 250);
                parameter3.Value = PackagingId;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@BUsedQty", SqlDbType.Decimal, 250);
                parameter3.Value = BUsedQty;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@ActiveStatus", SqlDbType.VarChar, 250);
                parameter3.Value = ActiveStatus;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@WastageQty", SqlDbType.VarChar, 250);
                parameter3.Value = WastageQty;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@OpeningQty", SqlDbType.Decimal, 250);
                parameter3.Value = OpeningQty;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@OpeningDate", SqlDbType.VarChar, 250);
                parameter3.Value = OpeningDate;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@CreatedBy", SqlDbType.VarChar, 250);
                parameter3.Value = CreatedBy;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@CreatedOn", SqlDbType.VarChar, 250);
                parameter3.Value = CreatedOn;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@LastModifiedBy", SqlDbType.VarChar, 250);
                parameter3.Value = LastModifiedBy;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@LastModifiedOn", SqlDbType.VarChar, 250);
                parameter3.Value = LastModifiedOn;
                cmdInsert.Parameters.Add(parameter3);



                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested Banderol Product Information successfully added";
                retResults[2] = "" + nextId;
            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("BanderolProductsDAL", "InsertToBanderolProducts", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BanderolProductsDAL", "InsertToBanderolProducts", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion 

            #region finally

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
            #endregion

            return retResults;

        }

        //
        public string[] UpdateBanderolProduct(string BandProductId, string ItemNo, string BanderolID, string PackagingId, decimal BUsedQty, string ActiveStatus,
            string LastModifiedBy, string LastModifiedOn, string WastageQty, decimal OpeningQty, string OpeningDate, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            #endregion

            #region try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(BandProductId))
                {
                    throw new ArgumentNullException("UpdateBanderolProduct","Please enter Product name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateBanderolProduct");
              
                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(BandProductId))
                {
                    //sqlText = "select count(BandProductId) from BanderolProducts where  BandProductId='" + BandProductId + "'";
                    sqlText = "select count(BandProductId) from BanderolProducts where  BandProductId = @BandProductId";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                    //BugsBD
                    SqlParameter parameter = new SqlParameter("@BandProductId", SqlDbType.VarChar, 250);
                    parameter.Value = BandProductId;
                    cmdIdExist.Parameters.Add(parameter);


                    cmdIdExist.Transaction = transaction;
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("UpdateBanderolProduct", "Could not find requested product information ");
                    }

                }

                #region check in product table
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    //sqlText = "select count(distinct ItemNo) from Products where ItemNo='" + ItemNo + "'";
                    sqlText = "select count(distinct ItemNo) from Products where ItemNo = @ItemNo";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);

                    //BugsBD
                    SqlParameter parameter2 = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                    parameter2.Value = ItemNo;
                    cmdIdExist.Parameters.Add(parameter2);


                    cmdIdExist.Transaction = transaction;
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToBanderolProducts", "Product information does not exist in Products list.");
                    }

                }
                #endregion

                #region Update Banderol Product Information

                sqlText = "";
                sqlText += "UPDATE BanderolProducts SET ";
                sqlText += " ItemNo = @ItemNo,";
                sqlText += " BanderolId = @BanderolID,";
                sqlText += " PackagingId = @PackagingId,";
                sqlText += " BUsedQty = @BUsedQty,";
                sqlText += " ActiveStatus = @ActiveStatus,";
                sqlText += " WastageQty = @WastageQty,";
                sqlText += " OpeningQty = @OpeningQty,";
                sqlText += " OpeningDate = @OpeningDate,";
                sqlText += " LastModifiedBy = @LastModifiedBy,";
                sqlText += " LastModifiedOn = @LastModifiedOn";

                sqlText += " where BandProductId = @BandProductId";

                //sqlText += " ItemNo='" + ItemNo + "',";
                //sqlText += " BanderolId='" + BanderolID + "',";
                //sqlText += " PackagingId='" + PackagingId + "',";
                //sqlText += " BUsedQty='" + BUsedQty + "',";
                //sqlText += " ActiveStatus='" + ActiveStatus + "',";
                //sqlText += " WastageQty='" + WastageQty + "',";
                //sqlText += " OpeningQty='" + OpeningQty + "',";
                //sqlText += " OpeningDate='" + OpeningDate + "',";
                //sqlText += " LastModifiedBy='" + LastModifiedBy + "',";
                //sqlText += " LastModifiedOn='" + LastModifiedOn + "'";
                //sqlText += " where BandProductId='" + BandProductId + "'";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                //BugsBD
                SqlParameter parameter3 = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                parameter3.Value = ItemNo;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@BanderolID", SqlDbType.VarChar, 250);
                parameter3.Value = BanderolID;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@PackagingId", SqlDbType.VarChar, 250);
                parameter3.Value = PackagingId;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@BUsedQty", SqlDbType.Decimal, 250);
                parameter3.Value = BUsedQty;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@ActiveStatus", SqlDbType.VarChar, 250);
                parameter3.Value = ActiveStatus;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@WastageQty", SqlDbType.VarChar, 250);
                parameter3.Value = WastageQty;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@OpeningQty", SqlDbType.Decimal, 250);
                parameter3.Value = OpeningQty;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@OpeningDate", SqlDbType.VarChar, 250);
                parameter3.Value = OpeningDate;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@LastModifiedBy", SqlDbType.VarChar, 250);
                parameter3.Value = LastModifiedBy;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@LastModifiedOn", SqlDbType.VarChar, 250);
                parameter3.Value = LastModifiedOn;
                cmdInsert.Parameters.Add(parameter3);
                parameter3 = new SqlParameter("@BandProductId", SqlDbType.VarChar, 250);
                parameter3.Value = BandProductId;
                cmdInsert.Parameters.Add(parameter3);


                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Banderol Product Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Banderol Product Information successfully Updated";

                    }

                }

                #endregion Commit
            }
            #endregion

            #region Catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("BanderolProductsDAL", "UpdateBanderolProduct", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BanderolProductsDAL", "UpdateBanderolProduct", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally

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
            #endregion

            return retResults;
        }

        public string[] DeleteBanderolProduct(string BandProductId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = BandProductId;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(BandProductId.ToString()))
                {
                    throw new ArgumentNullException("DeleteBanderolProduct",
                                "Could not find requested Banderol Product.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                //sqlText = "select count(BandProductId) from BanderolProducts where BandProductId='" + BandProductId + "'";
                sqlText = "select count(BandProductId) from BanderolProducts where BandProductId = @BandProductId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);

                //BugsBD
                SqlParameter parameter = new SqlParameter("@BandProductId", SqlDbType.VarChar, 250);
                parameter.Value = BandProductId;
                cmdExist.Parameters.Add(parameter);


                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Banderol Product Information.";
                    return retResults;
                }

                //sqlText = "delete BanderolProducts where BandProductId='" + BandProductId + "'";
                sqlText = "delete BanderolProducts where BandProductId = @BandProductId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);

                SqlParameter parameter2 = new SqlParameter("@BandProductId", SqlDbType.VarChar, 250);
                parameter2.Value = BandProductId;
                cmdExist.Parameters.Add(parameter2);


                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Banderol Product Information successfully deleted";
                }


            }

            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BanderolProductsDAL", "DeleteBanderolProduct", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BanderolProductsDAL", "DeleteBanderolProduct", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

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

            #endregion

            return retResults;
        }

        ////
        public DataTable SearchBanderolProducts(string ProductName, string ProductCode, string BanderolId, string BanderolName, string PackagingId, string PackagingNature, string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BanderolProducts");

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                sqlText = 
                           @" SELECT isnull(NULLIF(bp.BandProductId,''),'')BandProductId,
                            isnull(NULLIF(bp.ItemNo,''),0)ItemNo, 
                            isnull(NULLIF(p.ProductCode,''),'')ProductCode,
                            isnull(NULLIF(p.ProductName,''),'')ProductName,
                            isnull(NULLIF(bp.BanderolId,''),'')BanderolId,
                            isnull(NULLIF(b.BanderolName,''),'')BanderolName,
                            isnull(NULLIF(b.BanderolSize,''),'')BanderolSize,
                            isnull(NULLIF(b.UOM,''),'')BanderolUom,
                            isnull(NULLIF(bp.PackagingId,''),'')PackagingId,
                            isnull(NULLIF(pii.PackagingNature,''),'')PackagingName,
                            isnull(NULLIF(pii.PackagingCapacity,''),'')PackagingSize,
                            isnull(NULLIF(pii.UOM,''),'')PackagingUom,
                            isnull(NULLIF(bp.BUsedQty,0),0)BUsedQty,
                            isnull(NULLIF(bp.WastageQty,0),0)WastageQty,
                            isnull(NULLIF(bp.ActiveStatus,''),'')ActiveStatus,
                            isnull(NULLIF(bp.OpeningQty,0),0)OpeningQty,
                            convert (varchar,isnull (bp.OpeningDate,GETDATE()),120)OpeningDate
                            FROM BanderolProducts bp Left Outer Join Products p
							on bp.ItemNo=p.ItemNo Left outer Join Banderols b
							on bp.BanderolId=b.BanderolID Left outer join PackagingInformations pii
							on bp.PackagingId=pii.PackagingId 
                 
                            WHERE 
                                (p.ProductCode  LIKE '%' +  @ProductCode  + '%' OR @ProductCode IS NULL) 
                            AND (p.ProductName LIKE '%' + @ProductName + '%' OR @ProductName IS NULL)
                            AND (bp.BanderolId LIKE '%' + @BanderolId + '%' OR @BanderolId IS NULL)
                            AND (b.BanderolName LIKE '%' + @BanderolName + '%' OR @BanderolName IS NULL)
							AND (bp.PackagingId  LIKE '%' +  @PackagingId  + '%' OR @PackagingId IS NULL) 
                            AND (pii.PackagingNature LIKE '%' + @PackagingNature + '%' OR @PackagingNature IS NULL)
                            AND (bp.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)       
                            order by
                bp.BandProductId";
                            
                SqlCommand objCommBanderolProduct = new SqlCommand();
                objCommBanderolProduct.Connection = currConn;

                objCommBanderolProduct.CommandText = sqlText;
                objCommBanderolProduct.CommandType = CommandType.Text;


                if (!objCommBanderolProduct.Parameters.Contains("@ProductName"))
                { objCommBanderolProduct.Parameters.AddWithValue("@ProductName", ProductName); }
                else { objCommBanderolProduct.Parameters["@ProductName"].Value = ProductName; }
                if (!objCommBanderolProduct.Parameters.Contains("@ProductCode"))
                { objCommBanderolProduct.Parameters.AddWithValue("@ProductCode", ProductCode); }
                else { objCommBanderolProduct.Parameters["@ProductCode"].Value = ProductCode; }
                if (!objCommBanderolProduct.Parameters.Contains("@BanderolId"))
                { objCommBanderolProduct.Parameters.AddWithValue("@BanderolId", BanderolId); }
                else { objCommBanderolProduct.Parameters["@BanderolId"].Value = BanderolId; }

                if (BanderolName == "")
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@BanderolName"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@BanderolName", System.DBNull.Value); }
                    else { objCommBanderolProduct.Parameters["@BanderolName"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@BanderolName"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@BanderolName", BanderolName); }
                    else { objCommBanderolProduct.Parameters["@BanderolName"].Value = BanderolName; }
                }
                if (!objCommBanderolProduct.Parameters.Contains("@PackagingId"))
                { objCommBanderolProduct.Parameters.AddWithValue("@PackagingId", PackagingId); }
                else { objCommBanderolProduct.Parameters["@PackagingId"].Value = PackagingId; }

                if (PackagingNature == "")
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@PackagingNature"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@PackagingNature", System.DBNull.Value); }
                    else { objCommBanderolProduct.Parameters["@PackagingNature"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@PackagingNature"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@PackagingNature", PackagingNature); }
                    else { objCommBanderolProduct.Parameters["@PackagingNature"].Value = PackagingNature; }
                }

                

                if (!objCommBanderolProduct.Parameters.Contains("@ActiveStatus"))
                { objCommBanderolProduct.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommBanderolProduct.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBanderolProduct);

                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BanderolProductsDAL", "SearchBanderolProducts", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BanderolProductsDAL", "SearchBanderolProducts", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

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

            #endregion

            return dataTable;

        }

        //
        public DataTable SearchBanderol(string BandProductId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables   

            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Banderol");

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                sqlText = "";
                sqlText += " Select b.BanderolId,b.BanderolName,b.BanderolSize,b.UOM,bp.BandProductId ";
                sqlText += " from BanderolProducts bp left outer join Banderols b on bp.BanderolID=b.BanderolID ";
                sqlText += " where bp.BandProductId = '" + BandProductId + "'";

                SqlCommand cmdBande = new SqlCommand(sqlText, currConn);
                SqlDataAdapter adptBande = new SqlDataAdapter(cmdBande);
                adptBande.Fill(dataTable);


            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BanderolProductsDAL", "SearchBanderol", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BanderolProductsDAL", "SearchBanderol", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

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

            #endregion

            return dataTable;
        }
    }
}
