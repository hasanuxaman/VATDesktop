using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class VATRegistersDAL
    {
        CommonDAL _cDAL = new CommonDAL();
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #region 6.2 process

        public string[] SaveVAT6_2_Permanent(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string json = "";
            String storedProcedureName = "";
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {

                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                SqlCommand cmd = new SqlCommand();

                string vExportInBDT = commonDal.settings("VAT9_1", "ExportInBDT", currConn, transaction, connVM);
                string PDesc = commonDal.settings("VAT6_2", "ProductDescription", currConn, transaction, connVM);
                bool ProductPriceHistory = commonDal.settings("VAT6_2", "ProductPriceHistorys", null, null, connVM) == "Y";
                vm.PermanentProcess = true;
                vm.PDesc = PDesc;

                vm.IsExport = "No";
                if (vExportInBDT == "N")
                {
                    sqlText = "Select CASE WHEN pc.IsRaw = 'Export' THEN 'Yes' ELSE 'No' END AS IsExport ";
                    sqlText += " from ProductCategories pc join Products p on pc.CategoryID = p.CategoryID ";
                    sqlText += " where p.ItemNo = '" + vm.ItemNo + "'";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    object objItemNo = cmd.ExecuteScalar();
                    if (objItemNo == null)
                        vm.IsExport = "No";
                    else
                        vm.IsExport = objItemNo.ToString();
                }

                string DECLARE = @"
DECLARE @StartDate DATETIME;
DECLARE @EndDate DATETIME;
DECLARE @post1 VARCHAR(2);
DECLARE @post2 VARCHAR(2);
DECLARE @ItemNo VARCHAR(20);
DECLARE @IsExport VARCHAR(20);
DECLARE @BranchId  as int;
DECLARE @UserID as varchar(100);
DECLARE @PeriodId as varchar(100);
--SET @PeriodId ='vvPeriodId';
--SET @UserID ='vvUserID';
--SET @BranchId ='vvBranchId';
--SET @IsExport ='vvIsExport';
--SET @Itemno='vvItemno';
--SET @post1='vvpost1';
--SET @post2='vvpost2';
--SET @StartDate='vvStartDate';
--SET @EndDate= 'vvEndDate';
";
                #region Opening

                sqlText = @"";
                sqlText = sqlText + " " + Get6_2OpeningQuery(ProcessConfig.Permanent);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }
                string deleteText = @" 


                    delete from VAT6_2_Permanent where 1=1  and TransType != 'Opening'
                    and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
                    @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_2' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                sqlText = sqlText + " " + deleteText;

                if (!vm.FromSP)
                {
                    vm.SPSQLText = sqlText;
                    json = JsonConvert.SerializeObject(vm);
                    storedProcedureName = "SP_SaveVAT6_2_Permanent";
                    cmd = new SqlCommand(storedProcedureName, currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JsonParameters", json);
                }
                //cmd.ExecuteScalar();

                #endregion

                //sqlText = "";
                string Save6_2_SPText = Save6_2_SP(vm);

                sqlText = sqlText + " " + Save6_2_SPText;

                if (!vm.FromSP)
                {
                    vm.SPSQLText = sqlText;
                    json = JsonConvert.SerializeObject(vm);
                    storedProcedureName = "SP_SaveVAT6_2_Permanent";
                    cmd = new SqlCommand(storedProcedureName, currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JsonParameters", json);
                    //cmd.ExecuteScalar();
                }
                #region SQL Command

                #endregion
                //sqlText = "";

                string updateMasterItem = @"
update VAT6_2
set ItemNo = P.MasterProductItemNo
from Products p inner join VAT6_2 vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

";
                sqlText = sqlText + " " + updateMasterItem;

                #region VAT 6.2 Permanent Update Query

                string insertToPermanent = Get6_2PartitionQuery(ProcessConfig.Permanent, ProductPriceHistory);

                #endregion

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT6_2_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }
                sqlText = sqlText + " " + insertToPermanent;
                if (!vm.FromSP)
                {
                    vm.SPSQLText = sqlText;
                    json = JsonConvert.SerializeObject(vm);
                    storedProcedureName = "SP_SaveVAT6_2_Permanent";
                    cmd = new SqlCommand(storedProcedureName, currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JsonParameters", json);
                    //cmd.ExecuteScalar();
                }
                if (!vm.FromSP)
                {
                    cmd = new SqlCommand(sqlText, currConn, transaction);

                    #region Parameter
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PDesc", PDesc);
                    //cmd.Parameters.AddWithValueAndParamCheck("@ProdutType", vm.ProdutType);

                    if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ProdutType", vm.ProdutType);

                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                    {
                        //if (vm.ProdutCategoryLike == true)
                        //{
                        //    cmd.Parameters.AddWithValueAndParamCheck("@ProdutGroupName", vm.ProdutGroupName);
                        //}
                        //else
                        //{
                        cmd.Parameters.AddWithValueAndParamCheck("@ProdutCategoryId", vm.ProdutCategoryId);
                        //}
                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.Parameters.AddWithValueAndParamCheck("@IsExport", vm.IsExport);
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", vm.EndDate);
                    cmd.Parameters.AddWithValueAndParamCheck("@Post1", vm.Post1);
                    cmd.Parameters.AddWithValueAndParamCheck("@Post2", vm.Post2);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));

                    #endregion Parameter

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    sqlText = DECLARE + "" + sqlText;

                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@BranchId", vm.BranchId != 0 ? "'" + vm.BranchId + "'" : "0");
                    vm.SPSQLText = vm.SPSQLText.Replace("@PeriodId", !string.IsNullOrWhiteSpace(vm.PeriodId) ? "'" + vm.PeriodId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@IsExport", !string.IsNullOrWhiteSpace(vm.IsExport) ? "'" + vm.IsExport + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@Post1", !string.IsNullOrWhiteSpace(vm.Post1) ? "'" + vm.Post1 + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@Post2", !string.IsNullOrWhiteSpace(vm.Post2) ? "'" + vm.Post2 + "'" : "");

                    vm.SPSQLText = vm.SPSQLText.Replace("@post1", !string.IsNullOrWhiteSpace(vm.Post1) ? "'" + vm.Post1 + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@post2", !string.IsNullOrWhiteSpace(vm.Post2) ? "'" + vm.Post2 + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();


                    //OrdinaryVATDesktop ord = new OrdinaryVATDesktop();
                    //VAT6_2SPParamVM vmSP = new VAT6_2SPParamVM();
                    //OrdinaryVATDesktop.TransferData(vm, vmSP);

                    //vm.SQLQuery = "";
                    //storedProcedureName = "SP_VAT_Process";
                    //cmd = new SqlCommand(storedProcedureName, currConn, transaction);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValueAndParamCheck("@SQLQuery", sqlText);
                    //cmd.ExecuteNonQuery();

                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }

            #endregion

            #region Catch and Finall
            catch (SqlException ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {

                    }
                    //transaction.Commit();
                }
                FileLogger.Log("VATRegistersDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            #region Results
            return retResults;
            #endregion
        }

        public string Get6_2PartitionQuery(string flag, bool ProductPriceHistory)
        {

            #region Partition Query

            string partitionQuery = @"

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime, ReferenceNo varchar(200))
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6), StartDatetime datetime
, SerialNo varchar(10))

update VAT6_2 set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2.TransID
@itemCondition1

--insert into #NBRPrive ---(ItemNo, CustomerId, Rate, EffectDate, ToDate, ReferenceNo)
--select itemNo, '' CustomerId ,ISNULL(NBRPrice,0) NBRPrice, '1900/01/01'EffectDate ,null ToDate,null from products
--where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1) 

--insert into #NBRPrive
--select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,null from ProductPriceHistorys
--where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 )
--order by EffectDate
--
--insert into #NBRPrive
--select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,ReferenceNo from BOMs
--where FinishItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 )  and Post = 'Y'
--order by EffectDate

@itemConditionNBRPrice

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  @itemCondition2

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0 
)RT
where RT.Id=#NBRPrive.Id and isnull(nullif(customerid,''),0)<=0  @itemCondition2

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

delete from VAT6_2 where  VAT6_2.TransType='Opening' @itemCondition1

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,StartDateTime,SerialNo)
select Id,ItemNo,TransType,Quantity,UnitCost,StartDateTime,SerialNo from VAT6_2 
where 1=1 and VAT6_2.UserId = @UserId @itemCondition1
order by ItemNo,StartDateTime,SerialNo,Id

update VAT6_2 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY  ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2.Id @itemCondition1


update VAT6_2 set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
FROM #Temp)RT
where RT.Id=VAT6_2.Id @itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1

----######################----------------
 update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=0
----######################----------------


update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and nullif( VAT6_2.CustomerID,'') is null
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID>0
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1


update VAT6_2 set   RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition1

--select * from #NBRPrive order by ItemNo,CustomerId,EffectDate
--select * from #Temp order by sl 

update VAT6_2 set AdjustmentValue=0 where 1=1 @itemCondition1
update VAT6_2 set AdjustmentValue=   (DeclaredPrice-UnitRate)*Quantity where 1=1 @itemCondition1
 

 
delete from  #Temp
delete from   #NBRPrive

DBCC CHECKIDENT ('#Temp', RESEED, 0);
DBCC CHECKIDENT ('#NBRPrive', RESEED, 0);



insert into VAT6_2_Permanent(
[SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]  ,[Address1] ,[Address2]
      ,[Address3] ,[VATRegistrationNo]
      ,[ProductName] ,[ProductCode]
      ,[UOM],[HSCodeNo]
      ,[Quantity] ,[VATRate]   ,[SD]   ,[UnitCost]
      ,[Remarks]   ,[CreatedDateTime]   ,[UnitRate]   ,[ItemNo]   ,[AdjustmentValue]
      ,[UserId]   ,[BranchId]    ,[CustomerID]  ,[ProductDesc]    ,[ClosingRate]    ,[DeclaredPrice]    ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	  ,PeriodId
	  )

SELECT [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	  ,@PeriodId
  FROM VAT6_2 
where UserId = @UserId @itemCondition1
 order by ItemNo,StartDateTime,SerialNo


--create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime)
--create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6))

update VAT6_2_Permanent set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2_Permanent.TransID @itemCondition3

--insert into #NBRPrive
--select itemNo, '' CustomerId ,
--(
--case 
--when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
--end
--) NBRPrice, '1900/01/01'EffectDate ,null ToDate, null from products
--where ItemNo in(select distinct Itemno from VAT6_2_Permanent where 1=1 @itemCondition3) 
 

--insert into #NBRPrive
--select ItemNo, ''customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,null ReferenceNo from ProductPriceHistorys
--where ItemNo in(select distinct Itemno from VAT6_2_Permanent where 1=1 @itemCondition3)  
--order by EffectDate
--
--insert into #NBRPrive
--select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate, ReferenceNo from BOMs
--where FinishItemNo in(select distinct Itemno from VAT6_2_Permanent where 1=1 @itemCondition3) and Post = 'Y'
--order by EffectDate

@itemCondition3NBRPrice

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2


update #NBRPrive set  ToDate=RT.RunningTotal
from ( SELECT id,Customerid, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0 
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null
@itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0 
)RT
where RT.Id=#NBRPrive.Id  
and isnull(nullif(customerid,''),0)<=0
and ToDate is null
@itemCondition2


update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,StartDateTime,SerialNo)
select Id,ItemNo,TransType,Quantity,UnitCost,StartDateTime,SerialNo from VAT6_2_Permanent 
where 1=1 @itemCondition3
order by ItemNo,StartDateTime,SerialNo,Id

update VAT6_2_Permanent set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY  ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2_Permanent.Id
@itemCondition3

update VAT6_2_Permanent set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
FROM #Temp)RT
where RT.Id=VAT6_2_Permanent.Id
@itemCondition3

update VAT6_2_Permanent set DeclaredPrice =0

update VAT6_2_Permanent set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_Permanent.ItemNo
and VAT6_2_Permanent.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_Permanent.StartDateTime<#NBRPrive.ToDate
and VAT6_2_Permanent.CustomerID=#NBRPrive.CustomerId
and isnull(VAT6_2_Permanent.DeclaredPrice,0)=0
@itemCondition3


declare @refNo varchar(200)  = (select SettingValue from Settings
where SettingGroup = 'VAT6_2' and SettingName = 'DefaultRef')

if @refNo != 'NA'
begin
	update VAT6_2_Permanent set DeclaredPrice =NBRPrive.Rate
    from (	
		SELECT id, ItemNo,ReferenceNo,Rate,EffectDate
		,LEAD(EffectDate,1,'2199-12-31 00:00:00.000') 
		OVER (PARTITION BY ItemNo,ReferenceNo ORDER BY id) AS ToDate
		FROM #NBRPrive
		where isnull(nullif(customerid,''),0)<=0 
	)NBRPrive
    where NBRPrive.ItemNo=VAT6_2_Permanent.ItemNo
    and VAT6_2_Permanent.StartDateTime >=NBRPrive.EffectDate and VAT6_2_Permanent.StartDateTime<NBRPrive.ToDate
    and isnull(VAT6_2_Permanent.DeclaredPrice,0)=0
    and NBRPrive.ReferenceNo=@refNo
    @itemCondition3
end

update VAT6_2_Permanent set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_Permanent.ItemNo
and VAT6_2_Permanent.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_Permanent.StartDateTime<#NBRPrive.ToDate
and isnull(VAT6_2_Permanent.DeclaredPrice,0)=0
@itemCondition3

update VAT6_2_Permanent set AdjustmentValue=0 where 1=1 @itemCondition3
update VAT6_2_Permanent set   RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition3

update VAT6_2_Permanent set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 @itemCondition3

drop table #Temp
drop table #NBRPrive";

            #endregion

            #region Partition Query Branch

            string partitionQueryBranch =
                @"
create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime, BranchId int, ReferenceNo varchar(200))
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6),BranchId int, StartDatetime datetime
, SerialNo varchar(10))

update VAT6_2_Permanent_Branch set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2_Permanent_Branch.TransID @itemCondition3

--insert into #NBRPrive
--select itemNo, '' CustomerId ,
--(
--case 
--when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
--end
--) NBRPrice, '1900/01/01'EffectDate ,null,BranchId ToDate, null from products
--where ItemNo in(select distinct Itemno from VAT6_2_Permanent_Branch where 1=1 @itemCondition3) 

--insert into #NBRPrive
--select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,0 BranchId,null from ProductPriceHistorys
--where ItemNo in(select distinct Itemno from VAT6_2_Permanent_Branch where 1=1 @itemCondition3)
--order by EffectDate
--
--insert into #NBRPrive
--select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,BranchId,ReferenceNo from BOMs
--where FinishItemNo in(select distinct Itemno from VAT6_2_Permanent_Branch where 1=1 @itemCondition3) and Post = 'Y'
--order by EffectDate

@itemConditionNBRPrice

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

----######################----------------
update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT Customerid,id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null  @itemCondition2

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0
)RT
where RT.Id=#NBRPrive.Id  
and ToDate is null and isnull(nullif(customerid,''),0)<=0  @itemCondition2
----######################----------------

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId,StartDateTime,SerialNo)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId,StartDateTime,SerialNo from VAT6_2_Permanent_Branch 
where 1=1 @itemCondition3
order by BranchId,ItemNo,StartDateTime,SerialNo


update VAT6_2_Permanent_Branch set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
FROM #Temp)RT
where 
RT.Id=VAT6_2_Permanent_Branch.Id
and RT.BranchId=VAT6_2_Permanent_Branch.BranchId
@itemCondition3

update VAT6_2_Permanent_Branch set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=VAT6_2_Permanent_Branch.Id
and RT.BranchId=VAT6_2_Permanent_Branch.BranchId
@itemCondition3

update VAT6_2_Permanent_Branch set DeclaredPrice =0

update VAT6_2_Permanent_Branch set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_Permanent_Branch.ItemNo
and VAT6_2_Permanent_Branch.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_Permanent_Branch.StartDateTime<#NBRPrive.ToDate
and VAT6_2_Permanent_Branch.CustomerID=#NBRPrive.CustomerId
and isnull(VAT6_2_Permanent_Branch.DeclaredPrice,0)=0
@itemCondition3


declare @refNo varchar(200)  = (select SettingValue from Settings
where SettingGroup = 'VAT6_2' and SettingName = 'DefaultRef')

if @refNo != 'NA'
begin
	update VAT6_2_Permanent set DeclaredPrice =NBRPrive.Rate
    from (	
		SELECT id, ItemNo,ReferenceNo,Rate,EffectDate
		,LEAD(EffectDate,1,'2199-12-31 00:00:00.000') 
		OVER (PARTITION BY ItemNo,ReferenceNo ORDER BY id) AS ToDate
		FROM #NBRPrive
		where isnull(nullif(customerid,''),0)<=0 
	)NBRPrive
    where NBRPrive.ItemNo=VAT6_2_Permanent.ItemNo
    and VAT6_2_Permanent.StartDateTime >=NBRPrive.EffectDate and VAT6_2_Permanent.StartDateTime<NBRPrive.ToDate
    and isnull(VAT6_2_Permanent.DeclaredPrice,0)=0
    and NBRPrive.ReferenceNo=@refNo
    @itemCondition3
end

update VAT6_2_Permanent_Branch set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_Permanent_Branch.ItemNo
and VAT6_2_Permanent_Branch.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_Permanent_Branch.StartDateTime<#NBRPrive.ToDate
and isnull(VAT6_2_Permanent_Branch.DeclaredPrice,0)=0
@itemCondition3



update VAT6_2_Permanent_Branch set  RunningOpeningQuantityFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM VAT6_2_Permanent_Branch
)RT
where 
RT.Id=VAT6_2_Permanent_Branch.Id
and RT.BranchId=VAT6_2_Permanent_Branch.BranchId
@itemCondition3

 update VAT6_2_Permanent_Branch set  RunningOpeningValueFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotalValueFinal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM VAT6_2_Permanent_Branch
)RT
where 
RT.Id=VAT6_2_Permanent_Branch.Id
and RT.BranchId=VAT6_2_Permanent_Branch.BranchId
@itemCondition3


update VAT6_2_Permanent_Branch set   RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition3
update VAT6_2_Permanent_Branch set AdjustmentValue=0 where 1=1 @itemCondition3
update VAT6_2_Permanent_Branch set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 @itemCondition3


drop table #Temp
drop table #NBRPrive
";

            #endregion

            #region Temp

            string partitionQueryTemp = @"

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime, ReferenceNo varchar(200))
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6), StartDatetime datetime
, SerialNo varchar(10))

update VAT6_2 set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2.TransID and VAT6_2.UserId = @UserId

--insert into #NBRPrive
--select itemNo, '' CustomerId ,
--(
--case 
--when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
--end
--) NBRPrice, '1900/01/01'EffectDate ,null ToDate, null from products
--where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)

--insert into #NBRPrive
--select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,null from ProductPriceHistorys
--where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)
--order by EffectDate
--
--insert into #NBRPrive
--select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,ReferenceNo from BOMs
--where FinishItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId) and Post = 'Y'
--order by EffectDate

--@itemConditionNBRPrice

update #NBRPrive set  ToDate=null 

----######################----------------
update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT Customerid,id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0 
)RT
where RT.Id=#NBRPrive.Id  
and ToDate is null
and isnull(nullif(customerid,''),0)<=0
----######################----------------


update #NBRPrive set  ToDate='2199/12/31' where ToDate is null

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,StartDateTime,SerialNo)
select Id,ItemNo,TransType,Quantity,UnitCost,StartDateTime,SerialNo from VAT6_2 
where VAT6_2.UserId = @UserId
order by ItemNo,StartDateTime,SerialNo


update VAT6_2 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY  ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2.Id and VAT6_2.UserId = @UserId


update VAT6_2 set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
FROM #Temp)RT
where RT.Id=VAT6_2.Id and VAT6_2.UserId = @UserId

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
and VAT6_2.UserId = @UserId
and isnull(VAT6_2.DeclaredPrice,0)=0


declare @refNo varchar(200)  = (select SettingValue from Settings
where SettingGroup = 'VAT6_2' and SettingName = 'DefaultRef')

if @refNo != 'NA'
begin

	update VAT6_2_Permanent set DeclaredPrice =NBRPrive.Rate
    from (	
		SELECT id, ItemNo,ReferenceNo,Rate,EffectDate
		,LEAD(EffectDate,1,'2199-12-31 00:00:00.000') 
		OVER (PARTITION BY ItemNo,ReferenceNo ORDER BY id) AS ToDate
		FROM #NBRPrive
		where isnull(nullif(customerid,''),0)<=0 
	)NBRPrive
    where NBRPrive.ItemNo=VAT6_2_Permanent.ItemNo
    and VAT6_2_Permanent.StartDateTime >=NBRPrive.EffectDate and VAT6_2_Permanent.StartDateTime<NBRPrive.ToDate
    and isnull(VAT6_2_Permanent.DeclaredPrice,0)=0
    and NBRPrive.ReferenceNo=@refNo

end



update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.UserId = @UserId
and isnull(VAT6_2.DeclaredPrice,0)=0



drop table #Temp
drop table #NBRPrive
";

            #endregion

            #region NBRPriceHistoryPartitionQuery

            string NBRPriceHistoryPartitionQueryitemCondition1 = @"

insert into #NBRPrive
select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,null from ProductPriceHistorys
where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 )
order by EffectDate

";
            string NBRPriceHistoryPartitionQueryitemCondition3 = @"

insert into #NBRPrive
select ItemNo, ''customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,null ReferenceNo from ProductPriceHistorys
where ItemNo in(select distinct Itemno from VAT6_2_Permanent where 1=1 @itemCondition3)  
order by EffectDate

";


            string NBRPriceBOMsPartitionQueryitemCondition1 = @"

insert into #NBRPrive ---(ItemNo, CustomerId, Rate, EffectDate, ToDate, ReferenceNo)
select itemNo, '' CustomerId ,ISNULL(NBRPrice,0) NBRPrice, '1900/01/01'EffectDate ,null ToDate,null from products
where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,ReferenceNo from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 )  and Post = 'Y'
order by EffectDate
";

            string NBRPriceBOMsPartitionQueryitemCondition3 = @"

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null ToDate, null from products
where ItemNo in(select distinct Itemno from VAT6_2_Permanent where 1=1 @itemCondition3) 
 
insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate, ReferenceNo from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2_Permanent where 1=1 @itemCondition3) and Post = 'Y'
order by EffectDate

";

            #endregion

            #region NBRPriceHistorypartitionQueryBranch

            string NBRPriceHistorypartitionQueryBranch = @"

insert into #NBRPrive
select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,0 BranchId,null from ProductPriceHistorys
where ItemNo in(select distinct Itemno from VAT6_2_Permanent_Branch where 1=1 @itemCondition3)
order by EffectDate



";
            string NBRPriceBOMspartitionQueryBranch = @"

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null,BranchId ToDate, null from products
where ItemNo in(select distinct Itemno from VAT6_2_Permanent_Branch where 1=1 @itemCondition3) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,BranchId,ReferenceNo from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2_Permanent_Branch where 1=1 @itemCondition3) and Post = 'Y'
order by EffectDate

";

            #endregion

            #region NBRPriceHistorypartitionQueryTemp

            string NBRPriceHistorypartitionQueryTemp = @"

insert into #NBRPrive
select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,null from ProductPriceHistorys
where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)
order by EffectDate

";
            string NBRPriceBOMspartitionQueryTemp = @"

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null ToDate, null from products
where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,ReferenceNo from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId) and Post = 'Y'
order by EffectDate

";

            #endregion

            if (flag == ProcessConfig.Permanent)
            {
                ////partitionQuery = partitionQuery.Replace("@itemConditionNBRPrice", NBRPriceHistory);

                if (ProductPriceHistory)
                {
                    partitionQuery = partitionQuery.Replace("@itemConditionNBRPrice", NBRPriceHistoryPartitionQueryitemCondition1);
                    partitionQuery = partitionQuery.Replace("@itemCondition3NBRPrice", NBRPriceHistoryPartitionQueryitemCondition3);
                }
                else
                {
                    partitionQuery = partitionQuery.Replace("@itemConditionNBRPrice", NBRPriceBOMsPartitionQueryitemCondition1);
                    partitionQuery = partitionQuery.Replace("@itemCondition3NBRPrice", NBRPriceBOMsPartitionQueryitemCondition3);
                }

                return partitionQuery;
            }
            else if (flag == ProcessConfig.Permanent_Branch)
            {
                //partitionQueryBranch = partitionQueryBranch.Replace("@itemConditionNBRPrice", NBRPriceHistory);

                if (ProductPriceHistory)
                {
                    partitionQueryBranch = partitionQueryBranch.Replace("@itemConditionNBRPrice", NBRPriceHistorypartitionQueryBranch);
                }
                else
                {
                    partitionQueryBranch = partitionQueryBranch.Replace("@itemConditionNBRPrice", NBRPriceBOMspartitionQueryBranch);
                }

                return partitionQueryBranch;
            }
            else if (flag == ProcessConfig.Temp)
            {
                ////partitionQueryTemp = partitionQueryTemp.Replace("@itemConditionNBRPrice", NBRPriceHistory);

                if (ProductPriceHistory)
                {
                    partitionQueryTemp = partitionQueryTemp.Replace("@itemConditionNBRPrice", NBRPriceHistorypartitionQueryTemp);
                }
                else
                {
                    partitionQueryTemp = partitionQueryTemp.Replace("@itemConditionNBRPrice", NBRPriceBOMspartitionQueryTemp);
                }

                return partitionQueryTemp;

            }

            return "";
        }

        public DataSet VAT6_2(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet dataSet = new DataSet("ReportVAT17");
            string PDesc = "N";
            #endregion

            #region Try

            try
            {

                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Settings Value

                string vExportInBDT = "";
                CommonDAL commonDal = new CommonDAL();
                vExportInBDT = commonDal.settings("VAT9_1", "ExportInBDT", currConn, transaction, connVM);
                PDesc = commonDal.settings("VAT6_2", "ProductDescription", currConn, transaction, connVM);
                bool Permanent6_2 = true;//commonDal.settings("VAT6_2", "6_2Permanent") == "Y";

                #endregion

                string[] result = { };
                if (Permanent6_2 && !vm.PermanentProcess && !vm.MainProcess)
                {
                    if (vm.BranchWise)
                    {
                        result = Save6_2_FromPermanent_Branch(vm, vExportInBDT, currConn, transaction, PDesc, connVM);

                    }
                    else
                    {
                        result = Save6_2_FromPermanent(vm, vExportInBDT, currConn, transaction, PDesc, connVM);
                        FileLogger.Log("Save6_2_FromPermanent", "Save6_2_FromPermanent Call End", DateTime.Now.ToString());

                    }


                }
                else
                {
                    //result = Save6_2(vm, vExportInBDT, currConn, transaction, PDesc, connVM);
                }
                sqlText = GetVAT6_2SelectQuery();

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 1000;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataSet);


                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                //FileLogger.Log("Save6_2", "GetVAT6_2SelectQuery Call End", DateTime.Now.ToString());


            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                //if (Vtransaction == null)
                //{
                //    transaction.Rollback();
                //}
                FileLogger.Log("ReportDSDAL", "VAT6_2", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ReportDSDAL", "VAT6_2", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

            }

            #endregion

            return dataSet;
        }


        public string[] Save6_2(VAT6_2ParamVM vm, string vExportInBDT, SqlConnection currConn, SqlTransaction transaction
    , string PDesc, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = ""; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText;
                CommonDAL commonDal = new CommonDAL();
                SqlCommand objCommVAT17 = new SqlCommand();
                bool Permanent6_2 = vm.PermanentProcess;

                if (Permanent6_2)
                {
                    string maxDate = @"select dateadd(d,1,max(StartDatetime)) from VAT6_2_Permanent";

                    SqlCommand dateCmd = new SqlCommand(maxDate, currConn, transaction);
                    string date = dateCmd.ExecuteScalar().ToString();

                }

                #region Checkpoint

                string IsExport = "No";

                if (vExportInBDT == "N")
                {
                    sqlText = "Select CASE WHEN pc.IsRaw = 'Export' THEN 'Yes' ELSE 'No' END AS IsExport ";
                    sqlText += " from ProductCategories pc join Products p on pc.CategoryID = p.CategoryID ";
                    sqlText += " where p.ItemNo = '" + vm.ItemNo + "'";

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    object objItemNo = cmd.ExecuteScalar();
                    if (objItemNo == null)
                        IsExport = "No";
                    else
                        IsExport = objItemNo.ToString();
                }

                #endregion

                sqlText = " ";

                #region SQL Text

                #region Begining

                sqlText += @"
                
------DECLARE @StartDate DATETIME;
------DECLARE @EndDate DATETIME;
------DECLARE @post1 VARCHAR(2);
------DECLARE @post2 VARCHAR(2);
------DECLARE @ItemNo VARCHAR(20);
------DECLARE @IsExport VARCHAR(20);
------SET @IsExport ='No';
------SET @Itemno='24';
------SET @post1='Y';
------SET @post2='N';
------SET @StartDate='2014-04-01';
------SET @EndDate= '2014-04-27';
------DECLARE @BranchId  as int = 1;


declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;


CREATE TABLE #VAT_17(
ID int identity(1,1),SerialId int NULL,SerialNo  varchar (10) NULL,	 ItemNo   varchar (200) NULL,
 StartDateTime   datetime  NULL,	 StartingQuantity   decimal (25, 9) NULL,
 StartingAmount   decimal (25, 9) NULL,	 CustomerID   varchar (200) NULL,
 SD   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 Quantity   decimal (25, 9) NULL,
 UnitCost   decimal (25, 9) NULL,	 TransID   varchar (200) NULL,	 TransType   varchar (200) NULL,Remarks VARCHAR(200)
,CreatedDateTime   datetime  NULL, UnitRate decimal(25,9),ProductDesc varchar(1000), ClosingRate decimal(25,9),AdjustmentValue decimal(25,9)
)

CREATE TABLE #VATTemp_17(SerialNo  varchar (10) NULL,	 Dailydate   datetime  NULL,	 TransID   varchar (200) NULL,
 TransType   varchar (200) NULL,	 ItemNo   varchar (200) NULL,	 UnitCost   decimal (25, 9) NULL,
 Quantity   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 SD   decimal (25, 9) NULL,Remarks VARCHAR(200),CreatedDateTime   datetime  NULL, UnitRate decimal(25,9),AdjustmentValue decimal(25,9)) 
 
";
                sqlText += @"  
select * into #ProductReceive from   ( 
select Products.ItemNo,0 OpeningRate,0 ClosingRate from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1
--and Products.BranchId=@BranchId

";

                #endregion

                #region Conditions
                if (vm.PermanentProcess && vm.FilterProcessItems)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_2','VAT6_1_And_6_2')  and Products.ProcessFlag='Y' ";

                }
                else if (vm.PermanentProcess && !vm.MainProcess)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_2','VAT6_1_And_6_2') ";
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        sqlText += " and ItemNo in ('" + vm.ItemNo + "') ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                    else if (vm.Flag == "SCBL_Finish")
                    {
                        sqlText += @"  and IsRaw in('Finish','Export')";
                        sqlText += @"  and Products.ActiveStatus='Y'";

                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }
                if (vm.VAT6_2_1)
                {
                    sqlText += @"  and products.ReportType in('VAT6_2_1')";

                }
                else
                {
                    sqlText += @"  and products.ReportType in('VAT6_2','VAT6_1_And_6_2')";
                }
                sqlText += @"  ) as a

";

                #endregion

                #region Transaction Data

                if (vm.Opening == false)
                {
                    sqlText += @"";

                    #region VAT 6.2.1 False / Receive Data

                    if (vm.VAT6_2_1 == false)
                    {
                        #region Receive Data

                        sqlText += @"
-------------------------------------------------- Start Receive --------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
";

                        #region 'Other','Tender','PackageProduction' ,'Wastage','SaleWastage','Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService' ,'TradingImport'

                        sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1',rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
isnull(rd.SubTotal,0) AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType--'Receive'
,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD 
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('Other','Tender','PackageProduction' 
,'Wastage' ,'Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService'
,'TradingImport'
)
AND rd.BranchId=@BranchId
";
                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A2',rd.TransferDate,h.TransferCode,'Receive',rd.ToItemNo,
isnull(rd.ReceivePrice,0) AS SubTotal, 
isnull(NULLIF(ToQuantity,0),isnull(ToQuantity,0) ),0 VATAmount,0 SDAmount,rd.TransactionType--'Receive'
,rd.TransferDate,rd.ReceivePrice,0 AdjustmentValue
from ProductTransfersDetails RD 
left outer join ProductTransfers h on h.Id =rd.ProductTransferId
where 1=1
and rd.TransferDate >= @StartDate and rd.TransferDate <DATEADD(d,1,@EndDate) 
and rd.ToItemNo in(select distinct ItemNo from #ProductReceive)
AND (ToQuantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('FinishCTC')
AND rd.BranchId=@BranchId
";


                        #endregion

                        #region Comments Nov-01-2020

                        if (vm.StockMovement == false)
                        {
                            sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'A3', InvoiceDateTime,SalesInvoiceNo,'Receive',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN( 'RawSale')
AND BranchId=@BranchId

";
                        }

                        #endregion

                        #region 'TollReceive'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1',rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
CASE WHEN @IsExport='Yes' THEN isnull(rd.DollerValue,0) ELSE isnull(rd.SubTotal,0) END AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType--'Receive'
,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('TollReceive')
and pc.IsRaw in('finish')
AND rd.BranchId=@BranchId



";

                        #endregion



                        #region 'ReceiveReturn'

                        sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1', rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
-CASE WHEN @IsExport='Yes' THEN isnull(rd.DollerValue,0) ELSE isnull(rd.SubTotal,0) END AS SubTotal, 
-isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),-rd.VATAmount
,-rd.SDAmount,RD.TransactionType,rd.CreatedOn,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND RD.TransactionType IN('ReceiveReturn')
AND rd.BranchId=@BranchId
-------------------------------------------------- End Receive --------------------------------------------------
-----------------------------------------------------------------------------------------------------------------
						";

                        #endregion

                        #endregion

                        #region Purchase Data

                        #region 'ClientFGReceiveWOBOM'

                        sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'A5', ReceiveDate,PurchaseInvoiceNo,'Receive',ItemNo, SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ) Quantity,VATAmount,SDAmount, 'Client FG Receive without BOM'
,ReceiveDate,CostPrice
from PurchaseInvoiceDetails rd 
where rd.ReceiveDate >= @StartDate and rd.ReceiveDate <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('ClientFGReceiveWOBOM')
AND rd.BranchId=@BranchId
";

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Sale Data

                    #region 'Other','RawSale','PackageSale','Wastage', 'SaleWastage','CommercialImporter','ServiceNS','Export','ExportServiceNS','ExportTender','Tender',

                    //'ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TollSale'
                    if (vm.StockMovement == false)
                    {
                        sqlText += @"

-------------------------------------------------- Start Sale --------------------------------------------------
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B10', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('RawSale' )
AND BranchId=@BranchId

";
                    }

                    sqlText += @"

-------------------------------------------------- Start Sale --------------------------------------------------
---------------------------------------------------------------------------------------------------------------- 
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Other','PackageSale','Wastage',  'CommercialImporter','ServiceNS','Export','ExportServiceNS','ExportTender',
'Tender','ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TollSale')
AND BranchId=@BranchId

";

                    sqlText += @"

-------------------------------------------------- Start Sale --------------------------------------------------
---------------------------------------------------------------------------------------------------------------- 
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B2', rd.TransferDate,h.TransferCode,'Sale',FromItemNo
, IssuePrice AS SubTotal
,isnull(NULLIF(FromQuantity,0),isnull(FromQuantity,0) )Quantity
,0 VATAmount
,0 SDAmount
,rd.TransactionType--'Sale'
,rd.TransferDate,IssuePrice,0 AdjustmentValue
from ProductTransfersDetails rd
left outer join ProductTransfers h on h.Id =rd.ProductTransferId
where rd.TransferDate >= @StartDate and 
 rd.TransferDate < DATEADD(d,1,@EndDate) and FromItemNo in(select distinct ItemNo from #ProductReceive)
AND (FromQuantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('FinishCTC')
AND rd.BranchId=@BranchId

";
                    if (vm.StockMovement == false)
                    {
                        sqlText += @"

-------------------------------------------------- Start Sale --------------------------------------------------
---------------------------------------------------------------------------------------------------------------- 
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN( 'SaleWastage' )
AND BranchId=@BranchId

";
                    }

                    #endregion

                    #region 'Debit'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, CurrencyValue AS SubTotal
,(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end  )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Debit')
AND BranchId=@BranchId

";

                    #endregion

                    #region 'DisposeFinish'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,'Dispose Finish Sale' TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where 1=1 
and InvoiceDateTime >= @StartDate and InvoiceDateTime < DATEADD(d,1,@EndDate) 
and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('DisposeFinish')
AND BranchId=@BranchId

";

                    #endregion

                    if (vm.PermanentProcess && vm.BranchId != 0)
                    {
                        #region 'TransferReceive'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A4',rd.TransactionDateTime,rd.TransferReceiveNo,'Receive',rd.ItemNo,
 isnull(rd.SubTotal,0)  AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,'Transfer Receive'
,rd.TransactionDateTime,rd.CostPrice,0 AdjustmentValue
from TransferReceiveDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.TransactionDateTime >= @StartDate and rd.TransactionDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('62in')
and pc.IsRaw in('finish','Trading')
AND rd.BranchId=@BranchId

";

                        #endregion

                    }
                    if (vm.PermanentProcess && vm.BranchId != 0)
                    {
                        #region 'Transfer Issue'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B3',rd.TransactionDateTime,rd.TransferIssueNo,'Sale',rd.ItemNo,
 isnull(rd.SubTotal,0)  AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,'Transfer Issue'
,rd.TransactionDateTime,rd.CostPrice,0 AdjustmentValue
from TransferIssueDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.TransactionDateTime >= @StartDate and rd.TransactionDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('62out')
and pc.IsRaw in('finish','Trading')
AND rd.BranchId=@BranchId

";

                        #endregion

                    }

                    #region 'Credit','RawCredit'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo,
----------CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal, 
-CurrencyValue AS SubTotal,
- (  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end  )Quantity,-VATAmount,-SDAmount,TransactionType,CreatedOn,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Credit','RawCredit')
AND BranchId=@BranchId

";

                    #endregion

                    #endregion

                    #region Dispose Finish Data

                    #region 'Other'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'B4', TransactionDateTime,DisposeNo,'Sale',FinishItemNo ItemNo, (isnull(Quantity,0) * ISNULL(UnitPrice,0)) AS SubTotal
,isnull(Quantity,0) Quantity,0 VATAmount,0 SDAmount, 'Finish Dispose'
,TransactionDateTime,0 NBRPrice
from DisposeFinishs
where 1=1
AND TransactionDateTime >= @StartDate and  TransactionDateTime < DATEADD(d,1,@EndDate) 
and FinishItemNo in(select distinct ItemNo from #ProductReceive)
AND ISNULL(IsSaleable,'N')='N'
AND (Quantity>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Other')
AND BranchId=@BranchId

";

                    #endregion

                    #region Dispose Trading

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'B4', TransactionDateTime,DisposeNo,'Sale',FinishItemNo ItemNo, (isnull(Quantity,0) * ISNULL(UnitPrice,0)) AS SubTotal
,isnull(Quantity,0) Quantity,0 VATAmount,0 SDAmount, 'Trading Dispose'
,TransactionDateTime,0 NBRPrice
from DisposeFinishs
where 1=1
AND TransactionDateTime >= @StartDate and  TransactionDateTime < DATEADD(d,1,@EndDate) 
and FinishItemNo in(select distinct ItemNo from #ProductReceive)
AND ISNULL(IsSaleable,'N')='N'
AND (Quantity>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('DisposeTrading')
AND BranchId=@BranchId

";

                    #endregion


                    #endregion
                }

                #endregion

                #region Opening Data

                sqlText += @"
------select @OpeningDate = p.OpeningDate from Products p
------WHERE ItemNo=@ItemNo
------
------IF(@OpeningDate<@StartDate)

set @OpeningDate=@StartDate
insert into #VATTemp_17(SerialNo,Dailydate,TransID,VATRate,SD,remarks,TransType,ItemNo,Quantity,UnitCost)

SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo,
 SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost
	FROM (
SELECT distinct  ItemNo, 0 Quantity, 0 Amount  
FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId='1'

";

                #region Opening 6.2 False / Value from Product

                if (vm.Opening6_2 == false)
                {
                    if (vm.Opening == false)
                    {
                        if (vm.BranchId > 1)
                        {
                            sqlText += @"		 
UNION ALL 
SELECT distinct  ItemNo, isnull(StockQuantity,0) Quantity, isnull(p.StockValue,0) Amount  
FROM ProductStocks p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND BranchId>1
--group by ItemNo

";
                        }
                        else
                        {
                            sqlText += @"		 
UNION ALL 
SELECT distinct  ItemNo, isnull(OpeningBalance,0) Quantity, isnull(p.OpeningTotalCost,0) Amount  
FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
--AND BranchId='1'
------group by ItemNo

 ";



                        }
                    }
                }

                #endregion

                sqlText += @"		
";

                #region Rceives Data

                if (vm.VAT6_2_1 == false)
                {
                    #region Receive Data

                    #region 'Other','Tender','PackageProduction','Wastage','SaleWastage','Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TradingImport'

                    sqlText += @" 
UNION ALL 
(SELECT distinct  ItemNo,isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) Quantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS Amount
 FROM ReceiveDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND ReceiveDateTime>= '07/01/2019' and ReceiveDateTime < @StartDate  
AND TransactionType IN('Other','Tender','PackageProduction'  
,'Wastage' ,'Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService'
,'TradingImport'
)AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 
";

                    #endregion

                    #region FinishCTC
                    sqlText += @" 
UNION ALL 
(SELECT distinct  ToItemNo,isnull(sum(isnull(ToQuantity,isnull(ToQuantity,0))),0) Quantity,
    isnull(sum(isnull(ReceivePrice,0)),0)  Amount
 FROM ProductTransfersDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND TransferDate>= '07/01/2019' and TransferDate < @StartDate  
AND TransactionType IN('FinishCTC')AND ToItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ToItemNo
) 
";
                    #endregion FinishCTC

                    #region 'TollReceive'

                    sqlText += @" 
UNION ALL 
(SELECT distinct  RD.ItemNo,isnull(sum(isnull(RD.UOMQty,isnull(RD.Quantity,0))),0) ReceiveQuantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(RD.DollerValue,0)),0) ELSE isnull(sum(isnull(RD.SubTotal,0)),0) END AS SubTotal
 FROM ReceiveDetails  RD
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
WHERE 1=1
 AND (rd.Post =@post1 or rd.Post= @post2)   
 AND RD.ReceiveDateTime>= '07/01/2019' and RD.ReceiveDateTime < @StartDate  
AND rd.TransactionType IN('TollReceive')
and pc.IsRaw in('finish')
AND RD.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND rd.BranchId=@BranchId
group by RD.ItemNo
) 
";

                    #endregion

                    #endregion

                    #region Purchase Data

                    #region 'ClientFGReceiveWOBOM'

                    sqlText += @" 
UNION ALL 
(
SELECT  distinct ItemNo,isnull(sum(Quantity),0) Quantity, isnull(sum(SubTotal),0)   AS SubTotal
FROM PurchaseInvoiceDetails   WHERE 1=1 
AND (Post =@post1 or Post= @post2)  
AND ReceiveDate>= '07/01/2019' and ReceiveDate < @StartDate  
AND TransactionType IN('ClientFGReceiveWOBOM')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                    #endregion

                    #endregion
                }

                #region 'ReceiveReturn'

                sqlText += @"
UNION ALL
(SELECT distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM ReceiveDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2)    AND ReceiveDateTime>= '07/01/2019' and ReceiveDateTime < @StartDate  
 and TransactionType IN('ReceiveReturn') AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 

";

                #endregion

                #endregion

                #region Sales Data

                #region 'Other','PackageSale','Wastage','SaleWastage','CommercialImporter','ServiceNS','Export','ExportServiceNS','ExportTender','Tender',

                //'ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TollSale'

                sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN(
'Other'
,'PackageSale','Wastage', 'CommercialImporter','ServiceNS','Export','ExportServiceNS'
,'ExportTender','Tender','ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender'
,'InternalIssue','Service','ExportService','TollSale')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";


                sqlText += @" 
UNION ALL 
(SELECT distinct  FromItemNo,-1*isnull(sum(isnull(ToQuantity,isnull(ToQuantity,0))),0) Quantity,
   -1* isnull(sum(isnull(ReceivePrice,0)),0)  Amount
 FROM ProductTransfersDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND TransferDate>= '07/01/2019' and TransferDate < @StartDate  
AND TransactionType IN('FinishCTC')AND FromItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by FromItemNo
) 
";

                if (vm.StockMovement == false)
                {
                    sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
--CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN( 'SaleWastage' )
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";
                }

                #endregion

                #region 'Debit'

                sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo
,-isnull(sum( case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleNewQuantity
,-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   
WHERE 1=1
AND (Post =@post1 or Post= @post2)  
AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN('Debit')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                #endregion

                #region 'DisposeFinish'

                sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN('DisposeFinish')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                #endregion

                #region 'Credit','RawCredit'

                sqlText += @" 
UNION ALL  
(SELECT distinct  ItemNo
,isnull(sum( case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity
----------,CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
,isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
 AND TransactionType in( 'Credit','RawCredit') AND ItemNo  in(select distinct ItemNo from #ProductReceive)

AND BranchId=@BranchId
group by ItemNo

)
";

                #endregion

                #endregion

                #region Dispose Finish Data

                #region 'Other'

                sqlText += @" 
UNION ALL 
(
SELECT  distinct FinishItemNo ItemNo,-isnull(sum(Quantity),0) DisposeQuantity, -0   AS SubTotal
FROM DisposeFinishs   WHERE 1=1 
 AND (Post =@post1 or Post= @post2)  
 AND TransactionDateTime>= '07/01/2019' and TransactionDateTime < @StartDate  
AND TransactionType IN('Other')
AND FinishItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND ISNULL(IsSaleable,'N')='N'
group by FinishItemNo
)
";

                #endregion

                #endregion


                sqlText += @" 
) AS a GROUP BY a.ItemNo


";

                #endregion

                #region Insert, Update, Select VAT 6.2

                sqlText += @"

insert into #VAT_17(SerialNo,ItemNo,StartDateTime,StartingQuantity,StartingAmount,
CustomerID,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
select SerialNo,ItemNo,Dailydate,0,0,0,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime, UnitRate,AdjustmentValue  
from #VATTemp_17
order by dailydate,SerialNo;


update #VAT_17 set StartDateTime=@StartDate where SerialNo='A' 


update #VAT_17 set 
CustomerID=SalesInvoiceHeaders.CustomerID,
SerialId = SalesInvoiceHeaders.Id
from SalesInvoiceHeaders
where SalesInvoiceHeaders.SalesInvoiceNo=#VAT_17.TransID 
and #VAT_17.TransType='Sale'
AND (SalesInvoiceHeaders.Post =@post1 or SalesInvoiceHeaders.Post= @post2)
AND BranchId=@BranchId 



update #VAT_17 set 
CustomerID=TransferIssueDetails.TransferTo,
SerialId = TransferIssueDetails.Id
from TransferIssueDetails
where TransferIssueDetails.TransferIssueNo=#VAT_17.TransID 
and #VAT_17.Remarks='Transfer Issue'
AND (TransferIssueDetails.Post =@post1 or TransferIssueDetails.Post= @post2)
AND BranchId=@BranchId 




update #VAT_17 set 
SerialId = ReceiveHeaders.Id
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=#VAT_17.TransID 
and #VAT_17.TransType='Receive'
AND (ReceiveHeaders.Post =@post1 or ReceiveHeaders.Post= @post2)
AND BranchId=@BranchId


update #VAT_17 set 
SerialId = ProductTransfers.Id
from ProductTransfers
where ProductTransfers.TransferCode=#VAT_17.TransID 
AND (ProductTransfers.Post =@post1 or ProductTransfers.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null

update #VAT_17 set 
SerialId = TransferReceives.Id
from TransferReceives
where TransferReceives.TransferReceiveNo=#VAT_17.TransID 
AND (TransferReceives.Post =@post1 or TransferReceives.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null


update #VAT_17 set 
SerialId = PurchaseInvoiceHeaders.Id
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_17.TransID 
AND (PurchaseInvoiceHeaders.Post =@post1 or PurchaseInvoiceHeaders.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null

update #VAT_17 set 
SerialId = TransferIssues.Id
from TransferIssues
where TransferIssues.TransferIssueNo=#VAT_17.TransID 
AND (TransferIssues.Post =@post1 or TransferIssues.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null


update #VAT_17 set 
SerialId = DisposeFinishs.Id
from DisposeFinishs
where DisposeFinishs.DisposeNo=#VAT_17.TransID 
AND (DisposeFinishs.Post =@post1 or DisposeFinishs.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null



update #VAT_17 set #VAT_17.ProductDesc=Products.productName
	 from Products where Products.itemNo=#VAT_17.ItemNo
";

                if (PDesc.ToLower() == "y")
                {
                    sqlText += @"  
update #VAT_17 set #VAT_17.ProductDesc=SalesInvoiceDetails.ProductDescription
from SalesInvoiceDetails where SalesInvoiceDetails.itemNo=#VAT_17.ItemNo and SalesInvoiceDetails.SalesInvoiceNo=#VAT_17.TransID
";
                }

                // update #VAT_17 set UnitCost=#ProductReceive.OpeningRate*Quantity where TransType='opening' and item no =ProductReceive.itemNo
                // update #VAT_17 set ClosingRate=ProductReceive.ClosingRate where   item no =ProductReceive.itemNo


                sqlText += @" 
--select #VAT_17.SerialNo,convert (varchar,#VAT_17.StartDateTime,120)StartDateTime,
--#VAT_17.StartingQuantity,#VAT_17.StartingAmount,
--#VAT_17.TransID,#VAT_17.TransType,
--isnull(c.CustomerName,'-')CustomerName,
--isnull(c.Address1,'-')Address1,isnull(c.Address2,'-')Address2,
--isnull(c.Address3,'-')Address3,
--isnull(c.VATRegistrationNo,'-')VATRegistrationNo
--,#VAT_17.ProductDesc ProductName 
--,p.ProductCode
--,p.UOM
--,isnull(p.HSCodeNo,'NA')HSCodeNo,#VAT_17.Quantity,#VAT_17.VATRate
--,#VAT_17.SD,#vat_17.UnitCost,#VAT_17.remarks
--,isnull(#vat_17.CreatedDateTime,@StartDate)CreatedDateTime, isnull(#vat_17.UnitRate ,0)UnitRate ,#VAT_17.ItemNo,#VAT_17.AdjustmentValue
--from #VAT_17  left outer JOIN 
--Customers as C on #VAT_17.CustomerID=c.CustomerID 
--left outer join Products P on #VAT_17.ItemNo=p.ItemNo
--
--
--order by #VAT_17.StartDateTime,#VAT_17.SerialNo

";

                if (vm.SkipOpening)
                {
                    sqlText = sqlText + @" delete from #VAT_17 where TransType = 'Opening' and  UserId = @UserId";
                }
                else
                {
                    sqlText = sqlText + @" delete from VAT6_2 where UserId = @UserId";
                }

                sqlText = sqlText + @" 
insert into VAT6_2(
SerialId
,SerialNo
,ItemNo
,StartDateTime
,StartingQuantity
,StartingAmount
,CustomerID
,SD
,VATRate
,Quantity
,UnitCost
,TransID
,TransType
,Remarks
,CreatedDateTime
,UnitRate
,ProductDesc
,ClosingRate
,AdjustmentValue
,UserId
,BranchId)

select SerialId
,SerialNo
,ItemNo
,StartDateTime
,StartingQuantity
,StartingAmount
,CustomerID
,SD
,VATRate
,Quantity
,UnitCost
,TransID
,TransType
,Remarks
,CreatedDateTime
,UnitRate
,ProductDesc
,ClosingRate
,AdjustmentValue
,@UserId,@BranchId 
from #VAT_17
order by ItemNo,StartDateTime,SerialNo,SerialId


DROP TABLE #VAT_17
DROP TABLE #VATTemp_17
DROP TABLE #ProductReceive
 ";


                #endregion

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion SQL

                #region SQL Command
                objCommVAT17 = new SqlCommand(sqlText, currConn, transaction);

                #region Parameter

                objCommVAT17.Parameters.AddWithValue("@PDesc", PDesc);
                objCommVAT17.Parameters.AddWithValue("@BranchId", vm.BranchId);
                objCommVAT17.Parameters.AddWithValue("@UserId", vm.UserId);

                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    objCommVAT17.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        objCommVAT17.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                    }
                    else
                    {
                        objCommVAT17.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                if (!objCommVAT17.Parameters.Contains("@IsExport"))
                {
                    objCommVAT17.Parameters.AddWithValue("@IsExport", IsExport);
                }
                else
                {
                    objCommVAT17.Parameters["@IsExport"].Value = IsExport;
                }

                if (!objCommVAT17.Parameters.Contains("@StartDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@StartDate", vm.StartDate);
                }
                else
                {
                    objCommVAT17.Parameters["@StartDate"].Value = vm.StartDate;
                }

                if (!objCommVAT17.Parameters.Contains("@EndDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@EndDate", vm.EndDate);
                }
                else
                {
                    objCommVAT17.Parameters["@EndDate"].Value = vm.EndDate;
                }

                if (!objCommVAT17.Parameters.Contains("@post1"))
                {
                    objCommVAT17.Parameters.AddWithValue("@post1", vm.Post1);
                }
                else
                {
                    objCommVAT17.Parameters["@post1"].Value = vm.Post1;
                }

                if (!objCommVAT17.Parameters.Contains("@post2"))
                {
                    objCommVAT17.Parameters.AddWithValue("@post2", vm.Post2);
                }
                else
                {
                    objCommVAT17.Parameters["@post2"].Value = vm.Post2;
                }

                #endregion Parameter
                objCommVAT17.ExecuteNonQuery();

                retResults[0] = "success";


                #endregion


                return retResults;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string Save6_2_SP(VAT6_2ParamVM vm)
        {

            try
            {
                string sqlText;
                CommonDAL commonDal = new CommonDAL();
                SqlCommand objCommVAT17 = new SqlCommand();
                bool Permanent6_2 = vm.PermanentProcess;



                sqlText = " ";

                #region SQL Text

                #region Begining

                sqlText += @"
                
------DECLARE @StartDate DATETIME;
------DECLARE @EndDate DATETIME;
------DECLARE @post1 VARCHAR(2);
------DECLARE @post2 VARCHAR(2);
------DECLARE @ItemNo VARCHAR(20);
------DECLARE @IsExport VARCHAR(20);
------SET @IsExport ='No';
------SET @Itemno='24';
------SET @post1='Y';
------SET @post2='N';
------SET @StartDate='2014-04-01';
------SET @EndDate= '2014-04-27';
------DECLARE @BranchId  as int = 1;


declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;


CREATE TABLE #VAT_17(
ID int identity(1,1),SerialId int NULL,SerialNo  varchar (10) NULL,	 ItemNo   varchar (200) NULL,
 StartDateTime   datetime  NULL,	 StartingQuantity   decimal (25, 9) NULL,
 StartingAmount   decimal (25, 9) NULL,	 CustomerID   varchar (200) NULL,
 SD   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 Quantity   decimal (25, 9) NULL,
 UnitCost   decimal (25, 9) NULL,	 TransID   varchar (200) NULL,	 TransType   varchar (200) NULL,Remarks VARCHAR(200)
,CreatedDateTime   datetime  NULL, UnitRate decimal(25,9),ProductDesc varchar(1000), ClosingRate decimal(25,9),AdjustmentValue decimal(25,9)
)

CREATE TABLE #VATTemp_17(SerialNo  varchar (10) NULL,	 Dailydate   datetime  NULL,	 TransID   varchar (200) NULL,
 TransType   varchar (200) NULL,	 ItemNo   varchar (200) NULL,	 UnitCost   decimal (25, 9) NULL,
 Quantity   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 SD   decimal (25, 9) NULL,Remarks VARCHAR(200),CreatedDateTime   datetime  NULL, UnitRate decimal(25,9),AdjustmentValue decimal(25,9)) 
 
";
                sqlText += @"  
select * into #ProductReceive from   ( 
select Products.ItemNo,0 OpeningRate,0 ClosingRate from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1
--and Products.BranchId=@BranchId

";

                #endregion

                #region Conditions
                if (vm.PermanentProcess && vm.FilterProcessItems)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_2','VAT6_1_And_6_2')  and Products.ProcessFlag='Y' ";

                }
                else if (vm.PermanentProcess && !vm.MainProcess)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_2','VAT6_1_And_6_2') ";
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        sqlText += " and ItemNo in ('" + vm.ItemNo + "') ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                    else if (vm.Flag == "SCBL_Finish")
                    {
                        sqlText += @"  and IsRaw in('Finish','Export')";
                        sqlText += @"  and Products.ActiveStatus='Y'";

                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }
                if (vm.VAT6_2_1)
                {
                    sqlText += @"  and products.ReportType in('VAT6_2_1')";

                }
                else
                {
                    sqlText += @"  and products.ReportType in('VAT6_2','VAT6_1_And_6_2')";
                }
                sqlText += @"  ) as a

";

                #endregion

                #region Transaction Data

                if (vm.Opening == false)
                {
                    sqlText += @"";

                    #region VAT 6.2.1 False / Receive Data

                    if (vm.VAT6_2_1 == false)
                    {
                        #region Receive Data

                        sqlText += @"
-------------------------------------------------- Start Receive --------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
";

                        #region 'Other','Tender','PackageProduction' ,'Wastage','SaleWastage','Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService' ,'TradingImport'

                        sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1',rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
isnull(rd.SubTotal,0) AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType--'Receive'
,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD 
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('Other','Tender','PackageProduction' 
,'Wastage' ,'Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService'
,'TradingImport'
)
AND rd.BranchId=@BranchId
";
                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A2',rd.TransferDate,h.TransferCode,'Receive',rd.ToItemNo,
isnull(rd.ReceivePrice,0) AS SubTotal, 
isnull(NULLIF(ToQuantity,0),isnull(ToQuantity,0) ),0 VATAmount,0 SDAmount,rd.TransactionType--'Receive'
,rd.TransferDate,rd.ReceivePrice,0 AdjustmentValue
from ProductTransfersDetails RD 
left outer join ProductTransfers h on h.Id =rd.ProductTransferId
where 1=1
and rd.TransferDate >= @StartDate and rd.TransferDate <DATEADD(d,1,@EndDate) 
and rd.ToItemNo in(select distinct ItemNo from #ProductReceive)
AND (ToQuantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('FinishCTC')
AND rd.BranchId=@BranchId
";


                        #endregion

                        #region Comments Nov-01-2020

                        if (vm.StockMovement == false)
                        {
                            sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'A3', InvoiceDateTime,SalesInvoiceNo,'Receive',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN( 'RawSale')
AND BranchId=@BranchId

";
                        }

                        #endregion

                        #region 'TollReceive'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1',rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
CASE WHEN @IsExport='Yes' THEN isnull(rd.DollerValue,0) ELSE isnull(rd.SubTotal,0) END AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType--'Receive'
,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('TollReceive')
and pc.IsRaw in('finish')
AND rd.BranchId=@BranchId



";

                        #endregion

                        #region 'TollClient6_4Ins','TollClient6_4BacksFG'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1',rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
isnull(rd.SubTotal,0) AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType
,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('TollClient6_4Ins','TollClient6_4BacksFG')
and pc.IsRaw in('finish')
AND rd.BranchId=@BranchId



";

                        #endregion


                        #region 'ReceiveReturn'

                        sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A1', rd.ReceiveDateTime,rd.ReceiveNo,'Receive',rd.ItemNo,
-CASE WHEN @IsExport='Yes' THEN isnull(rd.DollerValue,0) ELSE isnull(rd.SubTotal,0) END AS SubTotal, 
-isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),-rd.VATAmount
,-rd.SDAmount,RD.TransactionType,rd.CreatedOn,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND RD.TransactionType IN('ReceiveReturn')
AND rd.BranchId=@BranchId
-------------------------------------------------- End Receive --------------------------------------------------
-----------------------------------------------------------------------------------------------------------------
						";

                        #endregion

                        #endregion

                        #region Purchase Data

                        #region 'ClientFGReceiveWOBOM'

                        sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'A5', ReceiveDate,PurchaseInvoiceNo,'Receive',ItemNo, SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ) Quantity,VATAmount,SDAmount, 'Client FG Receive without BOM'
,ReceiveDate,CostPrice
from PurchaseInvoiceDetails rd 
where rd.ReceiveDate >= @StartDate and rd.ReceiveDate <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('ClientFGReceiveWOBOM')
AND rd.BranchId=@BranchId
";

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Sale Data

                    #region 'Other','RawSale','PackageSale','Wastage', 'SaleWastage','CommercialImporter','ServiceNS','Export','ExportServiceNS','ExportTender','Tender',

                    //'ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TollSale'
                    if (vm.StockMovement == false)
                    {
                        sqlText += @"

-------------------------------------------------- Start Sale --------------------------------------------------
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B10', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('RawSale' )
AND BranchId=@BranchId

";
                    }

                    sqlText += @"


insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Other','PackageSale','Wastage',  'CommercialImporter','ServiceNS','Export','ExportServiceNS','ExportTender',
'Tender','ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TollSale')
AND BranchId=@BranchId

";

                    sqlText += @"


insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B2', rd.TransferDate,h.TransferCode,'Sale',FromItemNo
, IssuePrice AS SubTotal
,isnull(NULLIF(FromQuantity,0),isnull(FromQuantity,0) )Quantity
,0 VATAmount
,0 SDAmount
,rd.TransactionType--'Sale'
,rd.TransferDate,IssuePrice,0 AdjustmentValue
from ProductTransfersDetails rd
left outer join ProductTransfers h on h.Id =rd.ProductTransferId
where rd.TransferDate >= @StartDate and 
 rd.TransferDate < DATEADD(d,1,@EndDate) and FromItemNo in(select distinct ItemNo from #ProductReceive)
AND (FromQuantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('FinishCTC')
AND rd.BranchId=@BranchId

";

                    #region TollClient6_4OutFG

                    sqlText += @"


insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, subtotal AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('TollClient6_4OutFG')
AND BranchId=@BranchId

";

                    #endregion

                    if (vm.StockMovement == false)
                    {
                        sqlText += @"

-------------------------------------------------- Start Sale --------------------------------------------------
---------------------------------------------------------------------------------------------------------------- 
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN( 'SaleWastage' )
AND BranchId=@BranchId

";
                    }

                    #endregion

                    #region 'Debit'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, CurrencyValue AS SubTotal
,(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end  )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Debit')
AND BranchId=@BranchId

";

                    #endregion

                    #region 'DisposeFinish'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,'Dispose Finish Sale' TransactionType--'Sale'
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where 1=1 
and InvoiceDateTime >= @StartDate and InvoiceDateTime < DATEADD(d,1,@EndDate) 
and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('DisposeFinish')
AND BranchId=@BranchId

";

                    #endregion

                    if (vm.PermanentProcess && vm.BranchId != 0)
                    {
                        #region 'TransferReceive'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'A4',rd.TransactionDateTime,rd.TransferReceiveNo,'Receive',rd.ItemNo,
 isnull(rd.SubTotal,0)  AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,'Transfer Receive'
,rd.TransactionDateTime,rd.CostPrice,0 AdjustmentValue
from TransferReceiveDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.TransactionDateTime >= @StartDate and rd.TransactionDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('62in')
and pc.IsRaw in('finish','Trading')
AND rd.BranchId=@BranchId

";

                        #endregion

                    }
                    if (vm.PermanentProcess && vm.BranchId != 0)
                    {
                        #region 'Transfer Issue'

                        sqlText += @"
insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select 'B3',rd.TransactionDateTime,rd.TransferIssueNo,'Sale',rd.ItemNo,
 isnull(rd.SubTotal,0)  AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,'Transfer Issue'
,rd.TransactionDateTime,rd.CostPrice,0 AdjustmentValue
from TransferIssueDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.TransactionDateTime >= @StartDate and rd.TransactionDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN('62out')
and pc.IsRaw in('finish','Trading')
AND rd.BranchId=@BranchId

";

                        #endregion

                    }

                    #region 'Credit','RawCredit'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
select 'B1', InvoiceDateTime,SalesInvoiceNo,'Sale',ItemNo,
----------CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal, 
-CurrencyValue AS SubTotal,
- (  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end  )Quantity,-VATAmount,-SDAmount,TransactionType,CreatedOn,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Credit','RawCredit')
AND BranchId=@BranchId

";

                    #endregion

                    #endregion

                    #region Dispose Finish Data

                    #region 'Other'

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'B4', TransactionDateTime,DisposeNo,'Sale',FinishItemNo ItemNo, (isnull(Quantity,0) * ISNULL(UnitPrice,0)) AS SubTotal
,isnull(Quantity,0) Quantity,0 VATAmount,0 SDAmount, 'Finish Dispose'
,TransactionDateTime,0 NBRPrice
from DisposeFinishs
where 1=1
AND TransactionDateTime >= @StartDate and  TransactionDateTime < DATEADD(d,1,@EndDate) 
and FinishItemNo in(select distinct ItemNo from #ProductReceive)
AND ISNULL(IsSaleable,'N')='N'
AND (Quantity>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('Other')
AND BranchId=@BranchId

";

                    #endregion

                    #region Dispose Trading

                    sqlText += @"

insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'B4', TransactionDateTime,DisposeNo,'Sale',FinishItemNo ItemNo, (isnull(Quantity,0) * ISNULL(UnitPrice,0)) AS SubTotal
,isnull(Quantity,0) Quantity,0 VATAmount,0 SDAmount, 'Trading Dispose'
,TransactionDateTime,0 NBRPrice
from DisposeFinishs
where 1=1
AND TransactionDateTime >= @StartDate and  TransactionDateTime < DATEADD(d,1,@EndDate) 
and FinishItemNo in(select distinct ItemNo from #ProductReceive)
AND ISNULL(IsSaleable,'N')='N'
AND (Quantity>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN('DisposeTrading')
AND BranchId=@BranchId

";

                    #endregion


                    #endregion
                }

                #endregion

                #region Opening Data

                sqlText += @"
------select @OpeningDate = p.OpeningDate from Products p
------WHERE ItemNo=@ItemNo
------
------IF(@OpeningDate<@StartDate)

set @OpeningDate=@StartDate
insert into #VATTemp_17(SerialNo,Dailydate,TransID,VATRate,SD,remarks,TransType,ItemNo,Quantity,UnitCost)

SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo,
 SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost
	FROM (
SELECT distinct  ItemNo, 0 Quantity, 0 Amount  
FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId='1'

";

                #region Opening 6.2 False / Value from Product

                if (vm.Opening6_2 == false)
                {
                    if (vm.Opening == false)
                    {
                        if (vm.BranchId > 1)
                        {
                            sqlText += @"		 
UNION ALL 
SELECT distinct  ItemNo, isnull(StockQuantity,0) Quantity, isnull(p.StockValue,0) Amount  
FROM ProductStocks p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND BranchId>1
--group by ItemNo

";
                        }
                        else
                        {
                            sqlText += @"		 
UNION ALL 
SELECT distinct  ItemNo, isnull(OpeningBalance,0) Quantity, isnull(p.OpeningTotalCost,0) Amount  
FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
--AND BranchId='1'
------group by ItemNo

 ";



                        }
                    }
                }

                #endregion

                sqlText += @"		
";

                #region Rceives Data

                if (vm.VAT6_2_1 == false)
                {
                    #region Receive Data

                    #region 'Other','Tender','PackageProduction','Wastage','SaleWastage','Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TradingImport'

                    sqlText += @" 
UNION ALL 
(SELECT distinct  ItemNo,isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) Quantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS Amount
 FROM ReceiveDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND ReceiveDateTime>= '07/01/2019' and ReceiveDateTime < @StartDate  
AND TransactionType IN('Other','Tender','PackageProduction'  
,'Wastage' ,'Trading','TradingAuto','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService'
,'TradingImport'
)AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 
";

                    #endregion

                    #region FinishCTC
                    sqlText += @" 
UNION ALL 
(SELECT distinct  ToItemNo,isnull(sum(isnull(ToQuantity,isnull(ToQuantity,0))),0) Quantity,
    isnull(sum(isnull(ReceivePrice,0)),0)  Amount
 FROM ProductTransfersDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND TransferDate>= '07/01/2019' and TransferDate < @StartDate  
AND TransactionType IN('FinishCTC')AND ToItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ToItemNo
) 
";
                    #endregion FinishCTC

                    #region 'TollReceive'

                    sqlText += @" 
UNION ALL 
(SELECT distinct  RD.ItemNo,isnull(sum(isnull(RD.UOMQty,isnull(RD.Quantity,0))),0) ReceiveQuantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(RD.DollerValue,0)),0) ELSE isnull(sum(isnull(RD.SubTotal,0)),0) END AS SubTotal
 FROM ReceiveDetails  RD
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
WHERE 1=1
 AND (rd.Post =@post1 or rd.Post= @post2)   
 AND RD.ReceiveDateTime>= '07/01/2019' and RD.ReceiveDateTime < @StartDate  
AND rd.TransactionType IN('TollReceive')
and pc.IsRaw in('finish')
AND RD.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND rd.BranchId=@BranchId
group by RD.ItemNo
) 
";

                    #endregion

                    #endregion

                    #region Purchase Data

                    #region 'ClientFGReceiveWOBOM'

                    sqlText += @" 
UNION ALL 
(
SELECT  distinct ItemNo,isnull(sum(Quantity),0) Quantity, isnull(sum(SubTotal),0)   AS SubTotal
FROM PurchaseInvoiceDetails   WHERE 1=1 
AND (Post =@post1 or Post= @post2)  
AND ReceiveDate>= '07/01/2019' and ReceiveDate < @StartDate  
AND TransactionType IN('ClientFGReceiveWOBOM')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                    #endregion

                    #endregion
                }

                #region 'ReceiveReturn'

                sqlText += @"
UNION ALL
(SELECT distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM ReceiveDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2)    AND ReceiveDateTime>= '07/01/2019' and ReceiveDateTime < @StartDate  
 and TransactionType IN('ReceiveReturn') AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 

";

                #endregion

                #endregion

                #region Sales Data

                #region 'Other','PackageSale','Wastage','SaleWastage','CommercialImporter','ServiceNS','Export','ExportServiceNS','ExportTender','Tender',

                //'ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender','InternalIssue','Service','ExportService','TollSale'

                sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN(
'Other'
,'PackageSale','Wastage', 'CommercialImporter','ServiceNS','Export','ExportServiceNS'
,'ExportTender','Tender','ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender'
,'InternalIssue','Service','ExportService','TollSale')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";


                sqlText += @" 
UNION ALL 
(SELECT distinct  FromItemNo,-1*isnull(sum(isnull(ToQuantity,isnull(ToQuantity,0))),0) Quantity,
   -1* isnull(sum(isnull(ReceivePrice,0)),0)  Amount
 FROM ProductTransfersDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND TransferDate>= '07/01/2019' and TransferDate < @StartDate  
AND TransactionType IN('FinishCTC')AND FromItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by FromItemNo
) 
";

                if (vm.StockMovement == false)
                {
                    sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
--CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN( 'SaleWastage' )
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";
                }

                #endregion

                #region 'Debit'

                sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo
,-isnull(sum( case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleNewQuantity
,-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   
WHERE 1=1
AND (Post =@post1 or Post= @post2)  
AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN('Debit')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                #endregion

                #region 'DisposeFinish'

                sqlText += @" 
UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN('DisposeFinish')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                #endregion

                #region 'Credit','RawCredit'

                sqlText += @" 
UNION ALL  
(SELECT distinct  ItemNo
,isnull(sum( case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity
----------,CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
,isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
 AND TransactionType in( 'Credit','RawCredit') AND ItemNo  in(select distinct ItemNo from #ProductReceive)

AND BranchId=@BranchId
group by ItemNo

)
";

                #endregion

                #endregion

                #region Dispose Finish Data

                #region 'Other'

                sqlText += @" 
UNION ALL 
(
SELECT  distinct FinishItemNo ItemNo,-isnull(sum(Quantity),0) DisposeQuantity, -0   AS SubTotal
FROM DisposeFinishs   WHERE 1=1 
 AND (Post =@post1 or Post= @post2)  
 AND TransactionDateTime>= '07/01/2019' and TransactionDateTime < @StartDate  
AND TransactionType IN('Other')
AND FinishItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND ISNULL(IsSaleable,'N')='N'
group by FinishItemNo
)
";

                #endregion

                #endregion


                sqlText += @" 
) AS a GROUP BY a.ItemNo


";

                #endregion

                #region Insert, Update, Select VAT 6.2

                sqlText += @"

insert into #VAT_17(SerialNo,ItemNo,StartDateTime,StartingQuantity,StartingAmount,
CustomerID,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
select SerialNo,ItemNo,Dailydate,0,0,0,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime, UnitRate,AdjustmentValue  
from #VATTemp_17
order by dailydate,SerialNo;


update #VAT_17 set StartDateTime=@StartDate where SerialNo='A' 


update #VAT_17 set 
CustomerID=SalesInvoiceHeaders.CustomerID,
SerialId = SalesInvoiceHeaders.Id
from SalesInvoiceHeaders
where SalesInvoiceHeaders.SalesInvoiceNo=#VAT_17.TransID 
and #VAT_17.TransType='Sale'
AND (SalesInvoiceHeaders.Post =@post1 or SalesInvoiceHeaders.Post= @post2)
AND BranchId=@BranchId 



update #VAT_17 set 
CustomerID=TransferIssueDetails.TransferTo,
SerialId = TransferIssueDetails.Id
from TransferIssueDetails
where TransferIssueDetails.TransferIssueNo=#VAT_17.TransID 
and #VAT_17.Remarks='Transfer Issue'
AND (TransferIssueDetails.Post =@post1 or TransferIssueDetails.Post= @post2)
AND BranchId=@BranchId 




update #VAT_17 set 
SerialId = ReceiveHeaders.Id
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=#VAT_17.TransID 
and #VAT_17.TransType='Receive'
AND (ReceiveHeaders.Post =@post1 or ReceiveHeaders.Post= @post2)
AND BranchId=@BranchId


update #VAT_17 set 
SerialId = ProductTransfers.Id
from ProductTransfers
where ProductTransfers.TransferCode=#VAT_17.TransID 
AND (ProductTransfers.Post =@post1 or ProductTransfers.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null

update #VAT_17 set 
SerialId = TransferReceives.Id
from TransferReceives
where TransferReceives.TransferReceiveNo=#VAT_17.TransID 
AND (TransferReceives.Post =@post1 or TransferReceives.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null


update #VAT_17 set 
SerialId = PurchaseInvoiceHeaders.Id
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_17.TransID 
AND (PurchaseInvoiceHeaders.Post =@post1 or PurchaseInvoiceHeaders.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null

update #VAT_17 set 
SerialId = TransferIssues.Id
from TransferIssues
where TransferIssues.TransferIssueNo=#VAT_17.TransID 
AND (TransferIssues.Post =@post1 or TransferIssues.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null


update #VAT_17 set 
SerialId = DisposeFinishs.Id
from DisposeFinishs
where DisposeFinishs.DisposeNo=#VAT_17.TransID 
AND (DisposeFinishs.Post =@post1 or DisposeFinishs.Post= @post2)
AND BranchId=@BranchId 
AND SerialId is null



update #VAT_17 set #VAT_17.ProductDesc=Products.productName
	 from Products where Products.itemNo=#VAT_17.ItemNo
";


                sqlText += @"  
update #VAT_17 set #VAT_17.ProductDesc=SalesInvoiceDetails.ProductDescription
from SalesInvoiceDetails where SalesInvoiceDetails.itemNo=#VAT_17.ItemNo and SalesInvoiceDetails.SalesInvoiceNo=#VAT_17.TransID
";




                if (vm.SkipOpening)
                {
                    sqlText = sqlText + @" delete from #VAT_17 where TransType = 'Opening' and  UserId = @UserId";
                }
                else
                {
                    sqlText = sqlText + @" delete from VAT6_2 where UserId = @UserId";

                    //if (vm.BranchId==0)
                    //{
                    //    sqlText = sqlText + @" delete from VAT6_2 where UserId = @UserId";
                    //}
                    //else
                    //    {
                    //        sqlText = sqlText + @" delete from VAT6_2 where UserId = @UserId and BranchId=@BranchId";
                    //    }
                }

                sqlText = sqlText + @" 
insert into VAT6_2(
SerialId
,SerialNo
,ItemNo
,StartDateTime
,StartingQuantity
,StartingAmount
,CustomerID
,SD
,VATRate
,Quantity
,UnitCost
,TransID
,TransType
,Remarks
,CreatedDateTime
,UnitRate
,ProductDesc
,ClosingRate
,AdjustmentValue
,UserId
,BranchId)

select SerialId
,SerialNo
,ItemNo
,StartDateTime
,StartingQuantity
,StartingAmount
,CustomerID
,SD
,VATRate
,Quantity
,UnitCost
,TransID
,TransType
,Remarks
,CreatedDateTime
,UnitRate
,ProductDesc
,ClosingRate
,AdjustmentValue
,@UserId,@BranchId 
from #VAT_17
order by ItemNo,StartDateTime,SerialNo,SerialId


DROP TABLE #VAT_17
DROP TABLE #VATTemp_17
DROP TABLE #ProductReceive
 ";


                #endregion

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion SQL






                return sqlText;

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private string GetVAT6_2SelectQuery()
        {
            return @"

select  VAT6_2.SerialNo
,convert (varchar,VAT6_2.StartDateTime,120)StartDateTime
,convert (varchar,VAT6_2.StartDateTime,120)Day,
VAT6_2.StartingQuantity,VAT6_2.StartingAmount,
VAT6_2.TransID,VAT6_2.TransType,
(case when VAT6_2.Remarks = 'Transfer Issue' then isnull(bp.BranchName,'-') else isnull(c.CustomerName,'-') end )CustomerName,
(case when VAT6_2.Remarks = 'Transfer Issue' then isnull(bp.Address,'-') else isnull(c.Address1,'-') end )Address1,
--isnull(c.Address1,'-')Address1,
isnull(c.Address2,'-')Address2,
isnull(c.Address3,'-')Address3,
isnull(c.VATRegistrationNo,'-')VATRegistrationNo
,case when p.ProductDescription in('N/A','-','') then  p.ProductName else VAT6_2.ProductDesc end  ProductName 
,p.ProductCode
,p.UOM
,isnull(p.HSCodeNo,'NA')HSCodeNo,VAT6_2.Quantity
,isnull(VAT6_2.VATRate,'0')VATRate
,isnull(VAT6_2.SD,0)SD,VAT6_2.UnitCost,VAT6_2.remarks
,isnull(VAT6_2.CreatedDateTime,'1900-01-01')CreatedDateTime, isnull(VAT6_2.UnitRate ,0)UnitRate ,VAT6_2.ItemNo,isnull(VAT6_2.AdjustmentValue,0)AdjustmentValue
,
isnull(ClosingRate,0)ClosingRate,	 
isnull(DeclaredPrice,0)DeclaredPrice ,
isnull(RunningTotal,0)RunningTotal,
isnull(RunningTotalValue,0)RunningTotalValue,
isnull(RunningTotalValueFinal,0)RunningTotalValueFinal,
isnull(RunningOpeningValueFinal,0)RunningOpeningValueFinal,
isnull(RunningOpeningQuantityFinal,0)RunningOpeningQuantityFinal
,(select distinct isnull(ReturnTransactionType,'')ReturnTransactionType from salesinvoicedetails where ItemNo = VAT6_2.ItemNo and SalesinvoiceNo = VAT6_2.TransId)  ReturnTransactionType
--,sd.ReturnTransactionType
--,VAT6_2.ID
from VAT6_2  
left outer JOIN Customers as C on VAT6_2.CustomerID=c.CustomerID  
--and VAT6_2.TransType='Sale'
and (VAT6_2.TransType='Sale' or (VAT6_2.TransType='Receive' and Remarks in('TollClient6_4Ins','TollClient6_4BacksFG','TollClient6_4OutFG')))
left outer JOIN BranchProfiles as BP on VAT6_2.CustomerID=BP.BranchId  and VAT6_2.Remarks='Transfer Issue'
left outer join Products P on VAT6_2.ItemNo=p.ItemNo
--left outer join salesinvoicedetails sd on sd.ItemNo = VAT6_2.ItemNo and sd.SalesinvoiceNo = VAT6_2.TransId
where VAT6_2.UserId =@UserId
order by VAT6_2.StartDateTime,VAT6_2.SerialNo,VAT6_2.ID";
        }

        private string[] Save6_2_FromPermanent(VAT6_2ParamVM vm, string vExportInBDT, SqlConnection currConn, SqlTransaction transaction, string PDesc, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText;
                CommonDAL commonDal = new CommonDAL();

                bool Permanent6_2 = commonDal.settings("VAT6_2", "6_2Permanent", currConn, transaction, connVM) == "Y";

                //if (Permanent6_2)
                //{
                //    string maxDate = @"select dateadd(d,1,max(StartDatetime)) from VAT6_2_Permanent";

                //    SqlCommand dateCmd = new SqlCommand(maxDate, currConn, transaction);
                //    string date = dateCmd.ExecuteScalar().ToString();


                //}


                sqlText = " ";

                #region SQL Text

                #region Begining

                sqlText += @"  
select * into #ProductReceive from   ( 
select Products.ItemNo,0 OpeningRate,0 ClosingRate from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1
--and Products.BranchId=@BranchId


";
                #endregion

                #region Conditions
                // vm.ProdutType = "Finish";
                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";
                        sqlText += @"  and Products.ActiveStatus='Y'";

                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                        sqlText += @"  and Products.ActiveStatus='Y'";

                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  ) as a

";
                sqlText += @"

 
delete from VAT6_2 where UserId = @UserId

insert into VAT6_2([SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,UserId
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal])


select [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,@UserId
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]   
	  
from   ( 

select * from VAT6_2_Permanent where
 TransType in  ('Opening')
and ItemNo in ( select distinct ItemNo from #ProductReceive)

union all

select * from VAT6_2_Permanent where
 TransType not in  ('Opening')
and ItemNo in (select distinct ItemNo from #ProductReceive)
and  StartDateTime >= @StartDate and 
StartDateTime < DATEADD(d,1,@EndDate) 

 ) as a
order by ItemNo, StartDateTime, SerialNo, Id

 update VAT6_2 set  Quantity=a.ClosingQty
,UnitCost=a.RunningTotalValue, StartDateTime = @StartDate,
                    RunningTotal=a.ClosingQty
,RunningTotalValue=a.ClosingValue
,RunningTotalValueFinal =a.ClosingValue
,DeclaredPrice=a.DeclaredPrice,UnitRate=a.UnitRate,AdjustmentValue=0

from  (
select VAT6_2_Permanent.Id,VAT6_2_Permanent.ItemNo
,RunningTotal ClosingQty
,RunningTotalValueFinal ClosingValue
,DeclaredPrice
,UnitRate
,RunningTotalValue

from VAT6_2_Permanent
right outer join (
select distinct ItemNo, MAX(Id)Id from VAT6_2_Permanent
where StartDateTime <@StartDate and ItemNo in (select distinct ItemNo from #ProductReceive)
group by ItemNo) as a
on   a.Id=VAT6_2_Permanent.ID) as a
where a.ItemNo=VAT6_2.ItemNo and VAT6_2.TransType='Opening'

select * from VAT6_2 where UserId = @UserId
";
                #endregion

                //if (vm.BranchId == 0)
                //{
                //    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                //}

                #endregion SQL

                #region SQL Command


                SqlCommand objCommVAT17 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT17.CommandTimeout = 1000;
                objCommVAT17.Parameters.AddWithValue("@UserId", vm.UserId);

                #region Parameter

                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    objCommVAT17.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        objCommVAT17.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                    }
                    else
                    {
                        objCommVAT17.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                if (!objCommVAT17.Parameters.Contains("@StartDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@StartDate", vm.StartDate);
                }
                else
                {
                    objCommVAT17.Parameters["@StartDate"].Value = vm.StartDate;
                }

                if (!objCommVAT17.Parameters.Contains("@EndDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@EndDate", vm.EndDate);
                }
                else
                {
                    objCommVAT17.Parameters["@EndDate"].Value = vm.EndDate;
                }

                #endregion Parameter

                #endregion

                int rows = objCommVAT17.ExecuteNonQuery();

                string[] result = new[] { "success" };

                return result;

            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public string Get6_2OpeningQuery(string flag)
        {
            string parmanentOpening = @"

Insert into VAT6_2_Permanent(
[SerialNo],[StartDateTime]
      ,[StartingQuantity],[StartingAmount]
      ,[TransID]
      ,[TransType] ,[Quantity]
      ,[SD],[UnitCost]    ,[Remarks] ,[ItemNo] 
   ,[UserId]   ,[BranchId]  ,[CustomerID]
      ,[ProductDesc]
,RunningOpeningQuantityFinal
,RunningOpeningValueFinal
,UnitRate
,AdjustmentValue
      )


SELECT distinct 'A'SerialNo, '1900-01-01' StartDateTime, 0 StartingQuantity, 0 StartingAmount,
0 TransID, 'Opening' TransType,
sum(isnull(p.StockQuantity,0)) Quantity,0 SD, sum(isnull(p.StockValue,0)) UnitCost, 'Opening'Remarks, pd.ItemNo
, @UserId
,0 BranchId, 0 CustomerID,pd.ProductName,sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost, sum(isnull(p.StockValue,0))/(case when sum(isnull(StockQuantity,0)) =0 then 1 else sum(isnull(StockQuantity,0)) end) UnitRate,0

FROM ProductStocks p  left outer join Products pd on pd.ItemNo = p.ItemNo
WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_2','VAT6_1_And_6_2'))
and p.ItemNo not in (select ItemNo from VAT6_2_Permanent where transType = 'Opening')
@itemCondition
group by pd.ItemNo,pd.ProductName
";

            string permanent_BranchOpening = @"

Insert into VAT6_2_Permanent_Branch(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType] 
,[Quantity]
,[SD]
,[UnitCost]   
,[Remarks] 
,[ItemNo] 
,[UserId]   
,[BranchId]  
,[CustomerID]
,[ProductDesc]
,RunningOpeningQuantityFinal
,RunningOpeningValueFinal
,UnitRate

      )


SELECT distinct 'A'SerialNo, '1900-01-01' StartDateTime, 0 StartingQuantity, 0 StartingAmount,
0 TransID, 'Opening' TransType,
sum(isnull(p.StockQuantity,0)) Quantity,0 SD, sum(isnull(p.StockValue,0)) UnitCost, 'Opening'Remarks, p.ItemNo
, @UserId
,p.BranchId, 0 CustomerID,pd.ProductName,sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost, sum(isnull(p.StockValue,0))/(case when sum(isnull(StockQuantity,0)) =0 then 1 else sum(isnull(StockQuantity,0)) end) UnitRate

FROM ProductStocks p  left outer join Products pd on pd.ItemNo = p.ItemNo
WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_2','VAT6_1_And_6_2'))
and p.ItemNo not in (select ItemNo from VAT6_2_Permanent_Branch where transType = 'Opening')
@itemCondition
group by p.ItemNo,pd.ProductName,p.BranchId

";

            if (flag == ProcessConfig.Permanent_Branch)
            {
                return permanent_BranchOpening;
            }
            else if (flag == ProcessConfig.Permanent)
            {
                return parmanentOpening;
            }

            return "";
        }

        public string Get6_2UpdateFinal()
        {
            string parmanentOpening = @"
update VAT6_2 set RunningTotalValue= 0, RunningTotalValueFinal=0
from (
select distinct ItemNo,min(StartDateTime)StartDateTime  from VAT6_2_Permanent 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
--and VAT6_2.UserId =@UserId
group by ItemNo
) a
where VAT6_2.ItemNo = a.ItemNo and VAT6_2.StartDateTime>= a.StartDateTime
and VAT6_2.UserId =@UserId
";



            return parmanentOpening;
        }




        private string GetVAT6_2PartitionQuery(SysDBInfoVMTemp connVM = null)
        {
            CommonDAL commonDal = new CommonDAL();

            bool ProductPriceHistory = commonDal.settings("VAT6_2", "ProductPriceHistorys", null, null, connVM) == "Y";

            string sqlText = @"";

            sqlText += @"

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6),StartDatetime datetime,SerialNo varchar(10))

update VAT6_2 set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2.TransID and VAT6_2.UserId = @UserId

--insert into #NBRPrive
--select itemNo, '' CustomerId ,
--(
--case 
--when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
--end
--) NBRPrice, '1900/01/01'EffectDate ,null ToDate from products
--where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)


--insert into #NBRPrive
--select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate from ProductPriceHistorys
--where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)
--order by EffectDate
--
--insert into #NBRPrive
--select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate from BOMs
--where FinishItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)
--order by EffectDate

";
            if (ProductPriceHistory)
            {
                sqlText += @"

--insert into #NBRPrive
--select itemNo, '' CustomerId ,
--(
--case 
--when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
--end
--) NBRPrice, '1900/01/01'EffectDate ,null ToDate from products
--where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)

insert into #NBRPrive
select ItemNo,'' customerId, ISNULL(VatablePrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate from ProductPriceHistorys
where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)
order by EffectDate

";
            }
            else
            {
                sqlText += @"

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null ToDate from products
where ItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2 where VAT6_2.UserId = @UserId)
order by EffectDate

";

            }

            sqlText += @"

update #NBRPrive set  ToDate=null 

--update #NBRPrive set  ToDate=RT.RunningTotal
--from (SELECT id, ItemNo,
--LEAD(EffectDate) 
--OVER (PARTITION BY customerId,ItemNo ORDER BY id) AS RunningTotal
--FROM #NBRPrive)RT
--where RT.Id=#NBRPrive.Id

----######################----------------
update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT Customerid,id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0 
)RT
where RT.Id=#NBRPrive.Id  
and ToDate is null
and isnull(nullif(customerid,''),0)<=0
----######################----------------


update #NBRPrive set  ToDate='2199/12/31' where ToDate is null

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,StartDateTime,SerialNo)
select Id,ItemNo,TransType,Quantity,UnitCost,StartDateTime,SerialNo from VAT6_2 
where VAT6_2.UserId = @UserId
order by ItemNo,StartDateTime,SerialNo,Id

--delete from VAT6_2 where  VAT6_2.TransType='Opening'

update VAT6_2 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY  ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2.Id and VAT6_2.UserId = @UserId


update VAT6_2 set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
FROM #Temp)RT
where RT.Id=VAT6_2.Id and VAT6_2.UserId = @UserId

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
and VAT6_2.UserId = @UserId
and isnull(VAT6_2.DeclaredPrice,0)=0


update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.UserId = @UserId
and isnull(VAT6_2.DeclaredPrice,0)=0

update VAT6_2 set   DeclaredPrice= Unitcost/Quantity
where  VAT6_2.UserId = @UserId
 and TransType = 'Opening' and DeclaredPrice = 0
and Quantity != 0

update VAT6_2 set   RunningTotalValueFinal= DeclaredPrice*RunningTotal
where  VAT6_2.UserId = @UserId

 
update VAT6_2 set AdjustmentValue=0 where 1=1 and  VAT6_2.UserId = @UserId
update VAT6_2 set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 and  VAT6_2.UserId = @UserId
  

drop table #Temp
drop table #NBRPrive
";

            return sqlText;

        }

        private string[] Save6_2_FromPermanent_Branch(VAT6_2ParamVM vm, string vExportInBDT, SqlConnection currConn, SqlTransaction transaction, string PDesc, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText;
                CommonDAL commonDal = new CommonDAL();

                bool Permanent6_2 = commonDal.settings("VAT6_2", "6_2Permanent", currConn, transaction, connVM) == "Y";

                sqlText = " ";

                #region SQL Text

                #region Begining

                sqlText += @"  
select * into #ProductReceive from   ( 
select Products.ItemNo,0 OpeningRate,0 ClosingRate from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1
--and Products.BranchId=@BranchId

";
                #endregion

                #region Conditions
                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                        sqlText += @"  and Products.ActiveStatus='Y'";

                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  ) as a

";
                sqlText += @"

 
delete from VAT6_2 where UserId = @UserId

insert into VAT6_2([SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,UserId
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal])


select [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,@UserId
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]   
	  
from   ( 

select * from VAT6_2_Permanent_Branch where
 TransType in  ('Opening')
and ItemNo in ( select distinct ItemNo from #ProductReceive)
and BranchId=@BranchId


union all

select * from VAT6_2_Permanent_Branch where
 TransType not in  ('Opening')
and ItemNo in (select distinct ItemNo from #ProductReceive)
and  StartDateTime >= @StartDate 
and StartDateTime < DATEADD(d,1,@EndDate) 
and BranchId=@BranchId

 ) as a
order by ItemNo, StartDateTime, SerialNo, Id


update VAT6_2 set  Quantity=a.ClosingQty,UnitCost=a.RunningTotalValue, StartDateTime = @StartDate,
                    RunningTotal=a.ClosingQty,RunningTotalValue=a.ClosingValue,RunningTotalValueFinal =a.ClosingValue,DeclaredPrice=a.DeclaredPrice,UnitRate=a.UnitRate,AdjustmentValue=0

from  (
select VAT6_2_Permanent_Branch.Id,VAT6_2_Permanent_Branch.ItemNo,RunningTotal ClosingQty,RunningTotalValueFinal ClosingValue,DeclaredPrice,UnitRate,RunningTotalValue

from VAT6_2_Permanent_Branch
right outer join (
select distinct ItemNo,BranchId, MAX(Id)Id from VAT6_2_Permanent_Branch
where StartDateTime <@StartDate and ItemNo in (select distinct ItemNo from #ProductReceive) and BranchId=@BranchId
group by ItemNo,BranchId) as a
on   a.Id=VAT6_2_Permanent_Branch.ID and a.BranchId=VAT6_2_Permanent_Branch.BranchId
where VAT6_2_Permanent_Branch.BranchId=@BranchId
) as a
where a.ItemNo=VAT6_2.ItemNo and VAT6_2.TransType='Opening' 

select * from VAT6_2 where UserId = @UserId 
";
                #endregion

                #endregion SQL

                #region SQL Command

                SqlCommand objCommVAT17 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT17.CommandTimeout = 1000;
                objCommVAT17.Parameters.AddWithValue("@UserId", vm.UserId);
                objCommVAT17.Parameters.AddWithValueAndParamCheck("@BranchId", vm.BranchId);

                #region Parameter

                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    objCommVAT17.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        objCommVAT17.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                    }
                    else
                    {
                        objCommVAT17.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                if (!objCommVAT17.Parameters.Contains("@StartDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@StartDate", vm.StartDate);
                }
                else
                {
                    objCommVAT17.Parameters["@StartDate"].Value = vm.StartDate;
                }

                if (!objCommVAT17.Parameters.Contains("@EndDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@EndDate", vm.EndDate);
                }
                else
                {
                    objCommVAT17.Parameters["@EndDate"].Value = vm.EndDate;
                }

                #endregion Parameter

                #endregion

                int rows = objCommVAT17.ExecuteNonQuery();

                string[] result = new[] { "success" };

                return result;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] SaveVAT6_2_Permanent_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand();
                bool ProductPriceHistory = commonDal.settings("VAT6_2", "ProductPriceHistorys", null, null, connVM) == "Y";

                #region Opening

                sqlText = Get6_2OpeningQuery(ProcessConfig.Permanent_Branch);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }

                if (!vm.FromSP)
                {
                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", vm.BranchId);
                    cmd.CommandTimeout = 500;
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    sqlText = sqlText.Replace("@UserId", "'" + vm.UserId + "'");
                    sqlText = sqlText.Replace("@BranchId", vm.BranchId.ToString());
                    sqlText = sqlText.Replace("@ItemNo", "'" + vm.ItemNo + "'");
                    vm.SPSQLText = sqlText;

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();

                }
                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_Permanent_Branch where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
                                @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_2' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }
                if (!vm.FromSP)
                {
                    cmd.CommandText = deleteText;
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    deleteText = deleteText.Replace("@StartDate", "'" + vm.StartDate + "'");
                    deleteText = deleteText.Replace("@EndDate", "'" + vm.EndDate + "'");
                    deleteText = deleteText.Replace("@ItemNo", "'" + vm.ItemNo + "'");

                    vm.SPSQLText = deleteText;
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();


                }
                #endregion

                string insertToPermanentTable = @"

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime, BranchId int)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6), BranchId int)

update VAT6_2 set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2.TransID
@itemCondition1

insert into #NBRPrive
select itemNo, 0 CustomerId ,ISNULL(NBRPrice,0) NBRPrice, '1900/01/01'EffectDate ,null ToDate,0 BranchId  from products
where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,0 BranchId  from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 ) 
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,customerId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive)RT
where RT.Id=#NBRPrive.Id 
@itemCondition2


update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  @itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive

)RT
where RT.Id=#NBRPrive.Id @itemCondition2

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

delete from VAT6_2 where  VAT6_2.TransType='Opening' @itemCondition1

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT6_2 
where 1=1 @itemCondition1
order by BranchId,ItemNo,StartDateTime,SerialNo

update VAT6_2 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2.Id 
and RT.BranchId = VAT6_2.BranchId
@itemCondition1


update VAT6_2 set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=VAT6_2.Id
and RT.BranchId=VAT6_2.BranchId
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1


----######################----------------
 update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=0
@itemCondition1
------######################----------------
--
--
update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and nullif( VAT6_2.CustomerID,'') is null
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID>0
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1

UPDATE VAT6_2
SET RunningTotalValueFinal = CAST(DeclaredPrice  * RunningTotal AS decimal(18,4))
 where 1=1 @itemCondition1

update VAT6_2 set AdjustmentValue=0 where 1=1  @itemCondition1
update VAT6_2 set AdjustmentValue=RunningTotalValue-RunningTotalValueFinal where 1=1  @itemCondition1

 ----######################----------------

--delete from  #Temp
--delete from   #NBRPrive

--DBCC CHECKIDENT ('#Temp', RESEED, 0);
--DBCC CHECKIDENT ('#NBRPrive', RESEED, 0);

drop table #Temp
drop table #NBRPrive

insert into VAT6_2_Permanent_Branch(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType]
,[CustomerName]  
,[Address1] 
,[Address2]
,[Address3] 
,[VATRegistrationNo]
,[ProductName] 
,[ProductCode]
,[UOM]
,[HSCodeNo]
,[Quantity] 
,[VATRate]   
,[SD]   
,[UnitCost]
,[Remarks]   
,[CreatedDateTime]   
,[UnitRate]   
,[ItemNo]   
,[AdjustmentValue]
,[UserId]   
,[BranchId]    
,[CustomerID]  
,[ProductDesc]    
,[ClosingRate]    
,[DeclaredPrice]    
,[RunningTotal]
,[RunningTotalValue]
,[RunningTotalValueFinal]
,[RunningOpeningValueFinal]
,[RunningOpeningQuantityFinal]
,PeriodId
	  )

SELECT [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	  ,Format(StartDateTime,'MMyyyy')PeriodId
  FROM VAT6_2 
where UserId = @UserId @itemCondition1
 order by ItemNo,StartDateTime,SerialNo

";

                #region VAT 6.2 Insert Condition

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition3", " and VAT6_2_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition1", "");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition2", "");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition3", "");
                }
                if (!vm.FromSP)
                {
                    cmd.CommandText = insertToPermanentTable;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                }

                #endregion

                string updateMasterItem = @"
update VAT6_2
set ItemNo = isnull(P.MasterProductItemNo,P.ItemNo)
from Products p inner join VAT6_2 vp
on p.ItemNo = vp.ItemNo
where UserId = @UserId

";

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);
                vm.PermanentProcess = true;

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();
                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList();

                foreach (BranchProfileVM branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;

                    //DataSet dsResult = VAT6_2(vm, currConn, transaction, connVM);
                    string Save6_2_SPText = Save6_2_SP(vm);
                    Save6_2_SPText = Save6_2_SPText.Replace("@StartDate", "'" + vm.StartDate.ToString().ToLower() + "'");
                    Save6_2_SPText = Save6_2_SPText.Replace("@EndDate", "'" + vm.EndDate.ToString().ToLower() + "'");
                    Save6_2_SPText = Save6_2_SPText.Replace("@post1", "'" + vm.Post1.ToString().ToLower() + "'");
                    Save6_2_SPText = Save6_2_SPText.Replace("@post2", "'" + vm.Post2.ToString().ToLower() + "'");
                    Save6_2_SPText = Save6_2_SPText.Replace("@BranchId", vm.BranchId.ToString().ToLower());

                    Save6_2_SPText = Save6_2_SPText.Replace("@IsExport", "'" + vm.IsExport + "'");
                    Save6_2_SPText = Save6_2_SPText.Replace("@UserId", "'" + vm.UserId.ToString().ToLower() + "'");

                    vm.SPSQLText = Save6_2_SPText;

                    if (!vm.FromSP)
                    {
                        cmd.CommandText = vm.SPSQLText;
                        cmd.ExecuteNonQuery();

                    }
                    else
                    {

                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();

                    }

                    if (!vm.FromSP)
                    {
                        cmd.CommandText = updateMasterItem;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = insertToPermanentTable;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        updateMasterItem = updateMasterItem.Replace("@BranchId", vm.BranchId.ToString().ToLower());
                        updateMasterItem = updateMasterItem.Replace("@UserId", "'" + vm.UserId.ToLower() + "'");

                        vm.SPSQLText = updateMasterItem;
                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();


                        insertToPermanentTable = insertToPermanentTable.Replace("@BranchId", vm.BranchId.ToString().ToLower());
                        insertToPermanentTable = insertToPermanentTable.Replace("@UserId", "'" + vm.UserId.ToLower() + "'");
                        insertToPermanentTable = insertToPermanentTable.Replace("@PeriodId", "'" + Convert.ToDateTime(vm.StartDate).ToString("MMyyyy").ToLower() + "'");
                        insertToPermanentTable = insertToPermanentTable.Replace("@StartDate", "'" + vm.StartDate.ToLower() + "'");
                        if (!string.IsNullOrEmpty(vm.ItemNo))
                        {
                            insertToPermanentTable = insertToPermanentTable.Replace("@ItemNo", "'" + vm.ItemNo.ToLower() + "'");
                        }

                        vm.SPSQLText = insertToPermanentTable;
                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();



                    }

                }

                #region VAT 6.2 Permanent Update Query

                string updatePermanentTable = Get6_2PartitionQuery(ProcessConfig.Permanent_Branch, ProductPriceHistory);

                #endregion

                #region VAT 6.2 Update Condition

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition3", " and VAT6_2_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition1", "");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition2", "");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition3", "");
                }
                if (!vm.FromSP)
                {
                    cmd.CommandText = updatePermanentTable;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    updatePermanentTable = updatePermanentTable.Replace("@UserId", vm.UserId);
                    updatePermanentTable = updatePermanentTable.Replace("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    updatePermanentTable = updatePermanentTable.Replace("@StartDate", vm.StartDate);

                    vm.SPSQLText = updatePermanentTable;
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();


                }
                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_2_Permanent_BranchBackup(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            bool SPProcess = true;
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand();

                string vExportInBDT = commonDal.settings("VAT9_1", "ExportInBDT", currConn, transaction, connVM);
                string PDesc = commonDal.settings("VAT6_2", "ProductDescription", currConn, transaction, connVM);
                bool ProductPriceHistory = commonDal.settings("VAT6_2", "ProductPriceHistorys", null, null, connVM) == "Y";
                vm.IsExport = "No";
                vm.PDesc = PDesc;
                if (vExportInBDT == "N")
                {
                    sqlText = "Select CASE WHEN pc.IsRaw = 'Export' THEN 'Yes' ELSE 'No' END AS IsExport ";
                    sqlText += " from ProductCategories pc join Products p on pc.CategoryID = p.CategoryID ";
                    sqlText += " where p.ItemNo = '" + vm.ItemNo + "'";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    object objItemNo = cmd.ExecuteScalar();
                    if (objItemNo == null)
                        vm.IsExport = "No";
                    else
                        vm.IsExport = objItemNo.ToString();
                }


                #region Opening


                sqlText = "";
                cmd = new SqlCommand(sqlText, currConn, transaction);


                sqlText = "";


                sqlText = Get6_2OpeningQuery(ProcessConfig.Permanent_Branch);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }

                #endregion

                #region Delete Existing Data

                string deleteText = @" 

                                delete from VAT6_2_Permanent_Branch where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
                                @itemCondition";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_2' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }


                if (SPProcess == false)
                {
                    cmd.CommandText = deleteText;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", vm.EndDate);
                    if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    deleteText = deleteText.Replace("@UserID", vm.UserId);
                    deleteText = deleteText.Replace("@StartDate", vm.StartDate);
                    deleteText = deleteText.Replace("@EndDate", vm.EndDate);
                    deleteText = deleteText.Replace("@ItemNo", vm.ItemNo);

                    cmd = new SqlCommand("SP_VAT_Process", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@SQLQuery", deleteText);

                    cmd.ExecuteNonQuery();
                }



                #endregion
                #region  insertToPermanentTable
                string insertToPermanentTable = @"

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime, BranchId int)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6), BranchId int)

update VAT6_2 set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2.TransID
@itemCondition1

insert into #NBRPrive
select itemNo, 0 CustomerId ,ISNULL(NBRPrice,0) NBRPrice, '1900/01/01'EffectDate ,null ToDate,0 BranchId  from products
where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,0 BranchId  from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 ) 
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,customerId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive)RT
where RT.Id=#NBRPrive.Id 
@itemCondition2


update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  @itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive

)RT
where RT.Id=#NBRPrive.Id @itemCondition2

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

delete from VAT6_2 where  VAT6_2.TransType='Opening' @itemCondition1

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT6_2 
where 1=1 @itemCondition1
order by BranchId,ItemNo,StartDateTime,SerialNo

update VAT6_2 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2.Id 
and RT.BranchId = VAT6_2.BranchId
@itemCondition1


update VAT6_2 set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=VAT6_2.Id
and RT.BranchId=VAT6_2.BranchId
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1


----######################----------------
 update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=0
@itemCondition1
------######################----------------
--
--
update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and nullif( VAT6_2.CustomerID,'') is null
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID>0
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1

UPDATE VAT6_2
SET RunningTotalValueFinal = CAST(DeclaredPrice  * RunningTotal AS decimal(18,4))
 where 1=1 @itemCondition1

update VAT6_2 set AdjustmentValue=0 where 1=1  @itemCondition1
update VAT6_2 set AdjustmentValue=RunningTotalValue-RunningTotalValueFinal where 1=1  @itemCondition1

 ----######################----------------

--delete from  #Temp
--delete from   #NBRPrive

--DBCC CHECKIDENT ('#Temp', RESEED, 0);
--DBCC CHECKIDENT ('#NBRPrive', RESEED, 0);

drop table #Temp
drop table #NBRPrive

insert into VAT6_2_Permanent_Branch(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType]
,[CustomerName]  
,[Address1] 
,[Address2]
,[Address3] 
,[VATRegistrationNo]
,[ProductName] 
,[ProductCode]
,[UOM]
,[HSCodeNo]
,[Quantity] 
,[VATRate]   
,[SD]   
,[UnitCost]
,[Remarks]   
,[CreatedDateTime]   
,[UnitRate]   
,[ItemNo]   
,[AdjustmentValue]
,[UserId]   
,[BranchId]    
,[CustomerID]  
,[ProductDesc]    
,[ClosingRate]    
,[DeclaredPrice]    
,[RunningTotal]
,[RunningTotalValue]
,[RunningTotalValueFinal]
,[RunningOpeningValueFinal]
,[RunningOpeningQuantityFinal]
,PeriodId
	  )

SELECT [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	  ,Format(StartDateTime,'MMyyyy')PeriodId
  FROM VAT6_2 
where UserId = @UserId @itemCondition1
 order by ItemNo,StartDateTime,SerialNo

";
                #endregion  insertToPermanentTable

                #region VAT 6.2 Insert Condition

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition3", " and VAT6_2_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition1", "");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition2", "");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition3", "");
                }
                sqlText = sqlText + "" + insertToPermanentTable;


                #region Parameter
                /*
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PDesc", PDesc);

                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ProdutType", vm.ProdutType);
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ProdutCategoryId", vm.ProdutCategoryId);
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }
                cmd.Parameters.AddWithValueAndParamCheck("@IsExport", vm.IsExport);
                cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValueAndParamCheck("@EndDate", vm.EndDate);
                cmd.Parameters.AddWithValueAndParamCheck("@Post1", vm.Post1);
                cmd.Parameters.AddWithValueAndParamCheck("@Post2", vm.Post2);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                */
                #endregion Parameter


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    //cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                #endregion

                string updateMasterItem = @"
update VAT6_2
set ItemNo = isnull(P.MasterProductItemNo,P.ItemNo)
from Products p inner join VAT6_2 vp
on p.ItemNo = vp.ItemNo
where UserId = @UserId

";


                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);
                vm.PermanentProcess = true;

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();
                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList();

                foreach (BranchProfileVM branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;
                    string Save6_2_SPText = Save6_2_SP(vm);
                    string SQLQuery = "";
                    SQLQuery = Save6_2_SPText + " " + updateMasterItem + " " + sqlText;
                    #region Replace
                    SQLQuery = SQLQuery.Replace("@BranchId", vm.BranchId.ToString().Trim());
                    SQLQuery = SQLQuery.Replace("@UserId", vm.UserId.ToString().Trim());
                    if (!string.IsNullOrWhiteSpace(vm.PDesc))

                        if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                        {
                            SQLQuery = SQLQuery.Replace("@PDesc", vm.PDesc.ToString().Trim());
                        }
                        else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                        {
                            SQLQuery = SQLQuery.Replace("@ProdutCategoryId", vm.ProdutCategoryId.ToString().Trim());
                        }
                        else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                        {
                            SQLQuery = SQLQuery.Replace("@ItemNo", vm.ItemNo.ToString().Trim());
                        }

                    SQLQuery = SQLQuery.Replace("@IsExport", vm.IsExport.ToString().Trim());
                    SQLQuery = SQLQuery.Replace("@StartDate", vm.StartDate.ToString().Trim());
                    SQLQuery = SQLQuery.Replace("@EndDate", vm.EndDate.ToString().Trim());
                    SQLQuery = SQLQuery.Replace("@Post1", vm.Post1.ToString().Trim());
                    SQLQuery = SQLQuery.Replace("@Post2", vm.Post2.ToString().Trim());
                    SQLQuery = SQLQuery.Replace("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    #endregion Replace

                    //cmd.CommandText = SQLQuery;
                    //cmd.Parameters.AddWithValueAndParamCheck("@BranchId", vm.BranchId);

                    cmd = new SqlCommand("SP_VAT_Process", currConn, transaction);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@SQLQuery", SQLQuery);


                    //  SqlParameter resultParameter = new SqlParameter("@Result", SqlDbType.Int);
                    //  resultParameter.Direction = ParameterDirection.Output;
                    //  cmd.Parameters.Add(resultParameter);

                    cmd.ExecuteNonQuery();



                }

                #region VAT 6.2 Permanent Update Query

                string updatePermanentTable = Get6_2PartitionQuery(ProcessConfig.Permanent_Branch, ProductPriceHistory);

                #endregion

                #region VAT 6.2 Update Condition

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition3", " and VAT6_2_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition1", "");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition2", "");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition3", "");
                }
                if (false)
                {
                    cmd.CommandText = updatePermanentTable;

                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    updatePermanentTable = updatePermanentTable.Replace("@UserId", vm.UserId);
                    updatePermanentTable = updatePermanentTable.Replace("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    updatePermanentTable = updatePermanentTable.Replace("@StartDate", vm.StartDate);
                    updatePermanentTable = updatePermanentTable.Replace("@ItemNo", vm.ItemNo);

                    cmd = new SqlCommand("SP_VAT_Process", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@SQLQuery", updatePermanentTable);
                    cmd.ExecuteNonQuery();
                }

                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("VATRegistersDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            #region Results
            return retResults;
            #endregion
        }

        #endregion

        #region 6.1 Process

        public string GetOpeningQuery(string flag)
        {
            string parmanentOpening = @"

insert into VAT6_1_Permanent(SerialNo,ItemNo,StartDateTime,InvoiceDateTime,StartingQuantity,StartingAmount,
VendorID,Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,RunningTotal,RunningValue,RunningOpeningQuantity,RunningOpeningValue)

SELECT distinct 'A'SerialNo, itemNo ,'1900-01-01' StartDateTime,'1900-01-01' InvoiceDateTime, 0 StartingQuantity, 0 StartingAmount,
0 VendorID, 
sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost,0TransID,  'Opening'TransType,0 VATRate
,0 SD,'-'BENumber,'Opening'Remarks,sum(isnull(StockQuantity,0)), sum(isnull(p.StockValue,0)) UnitCost,sum(isnull(StockQuantity,0)) Quantity
, sum(isnull(p.StockValue,0)) UnitCost
FROM ProductStocks p  WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_1','VAT6_1_And_6_2'))
and p.ItemNo not in (select ItemNo from VAT6_1_Permanent where transType = 'Opening')
@itemCondition

group by ItemNo

";

            string permanent_BranchOpening = @"

insert into VAT6_1_Permanent_Branch(SerialNo,ItemNo,StartDateTime,InvoiceDateTime,StartingQuantity,StartingAmount,
VendorID,Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,RunningTotal,RunningValue,RunningOpeningQuantity,RunningOpeningValue,BranchId)

SELECT distinct 'A'SerialNo, itemNo ,'1900-01-01' StartDateTime,'1900-01-01' InvoiceDateTime, 0 StartingQuantity, 0 StartingAmount,
0 VendorID, 
sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost,0TransID,  'Opening'TransType,0 VATRate
,0 SD,'-'BENumber,'Opening'Remarks,sum(isnull(StockQuantity,0)), sum(isnull(p.StockValue,0)) UnitCost,sum(isnull(StockQuantity,0)) Quantity
, sum(isnull(p.StockValue,0)) UnitCost, BranchId
FROM ProductStocks p  WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_1','VAT6_1_And_6_2'))
and p.ItemNo not in (select ItemNo from VAT6_1_Permanent_Branch where transType = 'Opening')
@itemCondition

group by ItemNo,branchId
";

            if (flag == ProcessConfig.Permanent_Branch)
            {
                return permanent_BranchOpening;
            }
            else if (flag == ProcessConfig.Permanent)
            {
                return parmanentOpening;
            }

            return "";
        }

        public string GetInsertQuery(string flag)
        {
            string parmanentOpening = @"

insert into VAT6_1_Permanent(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[RunningTotal]
	  ,PeriodId
	  )

SELECT 
      [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,0 [AvgRate]
      ,[RunningTotal]
	  ,@PeriodId
  FROM VAT6_1
  where UserId = @UserId @itemCondition1
  order by ItemNo,StartDateTime, SerialNo,TransID
";

            string permanent_BranchOpening = @"

insert into VAT6_1_Permanent_Branch(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[RunningTotal]
	  ,PeriodId
	  )

SELECT 
      [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,0 [AvgRate]
      ,[RunningTotal]
	  ,FORMAT(StartDateTime,'MMyyyy') PeriodId
  FROM VAT6_1
  where UserId = @UserId @itemCondition1
  order by ItemNo,StartDateTime, SerialNo
";

            if (flag == ProcessConfig.Permanent_Branch)
            {
                return permanent_BranchOpening;
            }
            else if (flag == ProcessConfig.Permanent)
            {
                return parmanentOpening;
            }

            return "";
        }

        public string Get6_1PartitionQuery(string flag)
        {
            #region Partition Query

            string partitionQuery = @"


create table #TempProductAvgPrice(SL int identity(1,1),ItemNo varchar(50),AgvPriceDate datetime,FromDate datetime,PurchaseNo varchar(50)
,AvgPrice decimal(25,9))


insert into #TempProductAvgPrice(ItemNo,AgvPriceDate,FromDate,PurchaseNo,AvgPrice)
select  ItemNo
, AgvPriceDate
, FromDate
,PurchaseNo
,LAST_VALUE(avgprice) over (partition by ItemNo order by ItemNo,AgvPriceDate, inserttime)AvgPrice
from ProductAvgPrice
order by ItemNo,AgvPriceDate, inserttime


update @table set AvgRate = 0 where 1=1 @itemCondition2

--update @table set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
--from ProductAvgPrice
--where ProductAvgPrice.ItemNo=@table.ItemNo
--and @table.TransID=ProductAvgPrice.PurchaseNo
--@itemCondition2
--
--update @table set  AvgRate=ProductAvgPrice.AvgPrice
--from ProductAvgPrice
--where ProductAvgPrice.ItemNo=@table.ItemNo
--and @table.StartDateTime >= isnull(ProductAvgPrice.FromDate, ProductAvgPrice.AgvPriceDate)
--and @table.StartDateTime< DATEADD(d,1, ProductAvgPrice.AgvPriceDate) 
--and @table.TransType in('Opening','Issue')
--@itemCondition2

--update @table set AvgRate=a.AvgPrice
--from(
--select ProductAvgPrice.ItemNo,AvgPrice from ProductAvgPrice
--right outer join (
--select distinct ItemNo,max(sl)SL from ProductAvgPrice where ItemNo in(select ItemNo from @table)
--group by ItemNo)as a
--on a.ItemNo=ProductAvgPrice.ItemNo and a.Sl=ProductAvgPrice.sl) as a
--where a.ItemNo=@table.ItemNo
----and @table.ItemNo in(select ItemNo from @table where (AvgRate is null or AvgRate = 0))
--and @table.AvgRate = 0 or @table.AvgRate is null
--@itemCondition2


update @table set  AvgRate=#TempProductAvgPrice.AvgPrice,RunningTotal=0
from #TempProductAvgPrice
where #TempProductAvgPrice.ItemNo=@table.ItemNo
and @table.TransID=#TempProductAvgPrice.PurchaseNo
@itemCondition2


update @table set  AvgRate=#TempProductAvgPrice.AvgPrice
from #TempProductAvgPrice
where #TempProductAvgPrice.ItemNo=@table.ItemNo
and @table.StartDateTime >= isnull(#TempProductAvgPrice.FromDate, #TempProductAvgPrice.AgvPriceDate)
and @table.StartDateTime< DATEADD(d,1, #TempProductAvgPrice.AgvPriceDate) 
and @table.TransType in('Opening','Issue')
@itemCondition2


update @table set AvgRate=a.AvgPrice
from(
select #TempProductAvgPrice.ItemNo,AvgPrice from #TempProductAvgPrice
right outer join (
select distinct ItemNo,max(sl)SL from #TempProductAvgPrice where ItemNo in(select ItemNo from @table)
group by ItemNo)as a
on a.ItemNo=#TempProductAvgPrice.ItemNo and a.Sl=#TempProductAvgPrice.sl) as a
where a.ItemNo=@table.ItemNo
--and @table.ItemNo in(select ItemNo from @table where (AvgRate is null or AvgRate = 0))
and @table.AvgRate = 0 or @table.AvgRate is null
@itemCondition2


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9) @field1) -- ,BranchId int

insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost @field2)
select Id,ItemNo,TransType,Quantity,UnitCost @field2 from @table
where 1=1 @itemCondition2
order by @field3 ItemNo,StartDateTime,SerialNo,TransID

update @table set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType @field2 ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY @field3 ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=@table.Id @itemCondition3 --branch



Update @table set RunningValue = AvgRate*RunningTotal where 1=1  @itemCondition2
update @table set RunningTotal=Quantity, RunningValue=UnitCost where TransType in('opening') @itemCondition2

update @table set  RunningOpeningQuantity=0,RunningOpeningValue=0 where 1=1 @itemCondition2


drop table #Temp";

            #endregion

            if (flag != ProcessConfig.Permanent_Branch)
            {
                partitionQuery = partitionQuery
                    .Replace("@field1", "")
                    .Replace("@field2", "")
                    .Replace("@field3", "")
                    .Replace("@itemCondition3", "@itemCondition2");
            }

            if (flag == ProcessConfig.Permanent)
            {
                partitionQuery = partitionQuery.Replace("@table", "VAT6_1_Permanent");
            }
            else if (flag == ProcessConfig.Permanent_Branch)
            {
                partitionQuery = partitionQuery
                    .Replace("@field1", ",BranchId int")
                    .Replace("@field2", ",BranchId")
                    .Replace("@field3", "BranchId,")
                    .Replace("@itemCondition3", "and RT.BranchId = VAT6_1_Permanent_Branch.BranchId @itemCondition2")
                    .Replace("@table", "VAT6_1_Permanent_Branch");
            }
            else if (flag == ProcessConfig.Temp)
            {
                partitionQuery = partitionQuery.Replace("@table", "VAT6_1");
                partitionQuery = partitionQuery.Replace("@itemCondition2", "and VAT6_1.UserId = @UserId");

            }

            return partitionQuery;
        }

        private bool CheckProductType(VAT6_1ParamVM vm, SqlConnection currConn, SqlTransaction transaction)
        {

            //string[] notAllowedTypes = { "service", "overhead", "noninventory", "wip", "service(nonstock)" };
            string[] notAllowedTypes = { "service", "overhead", "noninventory", "service(nonstock)" };


            string sqlText = @"  

select isnull(pc.IsRaw,'-') from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1   ";

            #region Conditions

            if (!string.IsNullOrWhiteSpace(vm.ProdutType))
            {
                sqlText += @"  and IsRaw=@ProdutType";
            }
            else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
            {
                if (vm.ProdutCategoryLike == true)
                {
                    sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                }
                else
                {
                    sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                    sqlText += @"  and Products.ActiveStatus='Y'";
                }
            }
            else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
            {
                sqlText += @"  and ItemNo=@ItemNo";
            }

            #endregion

            SqlCommand productCmd = new SqlCommand(sqlText, currConn, transaction);

            #region Parameter Add

            if (!string.IsNullOrWhiteSpace(vm.ProdutType))
            {
                productCmd.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
            }

            else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
            {
                if (vm.ProdutCategoryLike == true)
                {
                    productCmd.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                }
                else
                {
                    productCmd.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                }
            }
            else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
            {
                productCmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
            }

            #endregion


            string typeName = productCmd.ExecuteScalar().ToString();

            return notAllowedTypes.Contains(typeName.ToLower());

        }

        public string[] SaveVAT6_1_Permanent(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string openingQuery = "";

            string SPSQLText = "";
            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = openingQuery; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            CommonDAL commonDal = new CommonDAL();
            //////vm.FromSP = true;

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                SqlCommand cmd = new SqlCommand(SPSQLText, currConn, transaction);

                #region Opening

                openingQuery = GetOpeningQuery(ProcessConfig.Permanent);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    openingQuery = openingQuery.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    openingQuery = openingQuery.Replace("@itemCondition", "");
                }
                if (!vm.FromSP)
                {
                    cmd = new SqlCommand(openingQuery, currConn, transaction);
                    cmd.CommandTimeout = 500;
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SPSQLText = openingQuery;
                    SPSQLText = SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", SPSQLText);
                    cmd.ExecuteNonQuery();


                    //SPSQLText = SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'"+ vm.ItemNo+"'" : "");
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("EXEC SPExecuteCustomQuery @Query", SqlDbType.NVarChar, -1).Value = SPSQLText;
                }

                #endregion

                #region Delete Existing Data

                string periodId = vm.StartDate.ToDateString("MMyyyy");
                string deleteText = @" delete from VAT6_1_Permanent where 
  TransType != 'Opening'  and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
 @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", "and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_1' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }


                if (!vm.FromSP)
                {


                    cmd.CommandText = deleteText;
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {

                    SPSQLText = deleteText;

                    SPSQLText = SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    SPSQLText = SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    SPSQLText = SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", SPSQLText);
                    cmd.ExecuteNonQuery();

                }
                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                vm.PermanentProcess = true;

                retResults = VAT6_1_WithConnForSP(vm, currConn, transaction, connVM);

                string updateMasterItem = @"
update VAT6_1
set ItemNo = P.MasterProductItemNo
from Products p inner join VAT6_1 vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

";

                if (!vm.FromSP)
                {

                    cmd.CommandText = updateMasterItem;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.ExecuteNonQuery();
                }
                else
                {

                    vm.SPSQLText = updateMasterItem;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();

                }

                string insertToPermanent = GetInsertQuery(ProcessConfig.Permanent);

                string partitionQuery = Get6_1PartitionQuery(ProcessConfig.Permanent);

                insertToPermanent += "  " + partitionQuery;


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and VAT6_1_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                }
                if (!vm.FromSP)
                {
                    cmd.CommandText = insertToPermanent;

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", periodId);

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = insertToPermanent;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@PeriodId", !string.IsNullOrWhiteSpace(vm.PeriodId.ToString()) ? "'" + vm.PeriodId.ToString() + "'" : "");


                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();



                }

                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1_Permanent set AvgRate = 0,RunningValue= 0, RunningOpeningValue=0
from (
select distinct ItemNo,min(StartDateTime)StartDateTime  from VAT6_1_Permanent 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
@itemCondition2

group by ItemNo
) a
where VAT6_1_Permanent.ItemNo = a.ItemNo and VAT6_1_Permanent.StartDateTime>= a.StartDateTime
@itemCondition2

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", " and VAT6_1_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", "");
                }
                if (!vm.FromSP)
                {


                    cmd.CommandText = updateNegativeValue;

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {


                    vm.SPSQLText = updateNegativeValue;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();

                }

                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                //transaction.Commit();

                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("VATRegistersDAL", "SaveTempIssue", ex.ToString() + "\n" + openingQuery);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion

            #region Results
            return retResults;
            #endregion
        }

        public DataSet VAT6_1_WithConn(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet dataSet = new DataSet("ReportVAT6_1");
            string SPSQLText = "";
            #endregion

            #region Try

            try
            {
                #region Settings

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV", null, null, connVM) == "Y" ? true : false);
                bool IssueFrom6_1 = Convert.ToBoolean(_cDal.settings("Toll6_4", "IssueFrom6_1", null, null, connVM) == "Y" ? true : false);
                bool TotalIncludeSD = Convert.ToBoolean(_cDal.settings("VAT6_1", "TotalIncludeSD", null, null, connVM) == "Y" ? true : false);
                bool IncludeOtherAMT = _cDal.settings("VAT6_1", "IncludeOtherAMT", null, null, connVM) == "Y";
                bool TollReceiveNotWIP = Convert.ToBoolean(_cDal.settings("IssueFromBOM", "TollReceive-NotWIP", null, null, connVM) == "Y" ? true : false);
                bool TollReceiveWithIssue = Convert.ToBoolean(_cDal.settings("TollReceive", "WithIssue", null, null, connVM) == "Y" ? true : false);
                bool Permanent6_1 = true;//_cDal.settings("VAT6_1", "6_1Permanent") == "Y";
                //bool ContractorFGProduction = Convert.ToBoolean(_cDal.settings("ContractorFGProduction", "IssueFrom6_1") == "Y" ? true : false);

                #endregion

                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(SPSQLText, currConn, transaction);

                if (string.IsNullOrEmpty(vm.UserId))
                {
                    throw new Exception("User Id Not Found");
                }

                string[] resultSave61 = { };

                bool checkProductType = CheckProductType(vm, currConn, transaction);


                if (Permanent6_1 && !vm.PermanentProcess && !checkProductType)
                {
                    if (vm.BranchWise)
                    {
                        resultSave61 = Save6_1_FromPermanent_Branch(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);

                    }
                    else
                    {
                        resultSave61 = Save6_1_FromPermanent(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }

                    string getMaxDate = @"select max(startdatetime) from VAT6_1_Permanent where 1=1 ";
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        getMaxDate += " and ItemNo=@ItemNo";
                    }

                    cmd = new SqlCommand(getMaxDate, currConn, transaction);
                    cmd.CommandTimeout = 500;
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    var maxDate = cmd.ExecuteScalar();

                    DateTime permanent_Max = Convert.ToDateTime(maxDate == DBNull.Value ? null : maxDate);

                    if (Convert.ToDateTime(vm.StartDate) > permanent_Max)
                    {
                        vm.SkipOpening = true;
                        resultSave61 = Save6_1(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }
                    else if (Convert.ToDateTime(vm.EndDate) > permanent_Max)
                    {
                        vm.StartDate = permanent_Max.AddDays(1).ToString("yyyy-MM-dd");
                        vm.SkipOpening = true;
                        resultSave61 = Save6_1(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }

                }
                else
                {
                    resultSave61 = Save6_1(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);

                }

                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1 set AvgRate = 0,RunningValue= 0
, RunningOpeningValue=0
from (
select distinct ItemNo,min(StartDateTime)StartDateTime 
from VAT6_1_Permanent 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
and UserId=@UserId

group by ItemNo
) a
where VAT6_1.ItemNo = a.ItemNo and VAT6_1.StartDateTime>= a.StartDateTime
and VAT6_1.UserId=@UserId


";
                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(updateNegativeValue, currConn, transaction);
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SPSQLText = updateNegativeValue;

                    SPSQLText = SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("EXEC SPExecuteCustomQuery @Query", SqlDbType.NVarChar, -1).Value = SPSQLText;
                    cmd.ExecuteNonQuery();
                }
                #endregion

                #region Select Saved Data

                sqlText = Get6_1SelectText();

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataSet);

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ReportDSDAL", "VAT6_1_WithConn", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ReportDSDAL", "VAT6_1_WithConn", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

            }

            #endregion

            return dataSet;
        }

        public string[] VAT6_1_WithConnForSP(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = ""; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string SPSQLText = "";
            #endregion

            #region Try

            try
            {
                #region Settings

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV", null, null, connVM) == "Y" ? true : false);
                bool IssueFrom6_1 = Convert.ToBoolean(_cDal.settings("Toll6_4", "IssueFrom6_1", null, null, connVM) == "Y" ? true : false);
                bool TotalIncludeSD = Convert.ToBoolean(_cDal.settings("VAT6_1", "TotalIncludeSD", null, null, connVM) == "Y" ? true : false);
                bool IncludeOtherAMT = _cDal.settings("VAT6_1", "IncludeOtherAMT", null, null, connVM) == "Y";
                bool TollReceiveNotWIP = Convert.ToBoolean(_cDal.settings("IssueFromBOM", "TollReceive-NotWIP", null, null, connVM) == "Y" ? true : false);
                bool TollReceiveWithIssue = Convert.ToBoolean(_cDal.settings("TollReceive", "WithIssue", null, null, connVM) == "Y" ? true : false);
                bool Permanent6_1 = true;//_cDal.settings("VAT6_1", "6_1Permanent") == "Y";
                //bool ContractorFGProduction = Convert.ToBoolean(_cDal.settings("ContractorFGProduction", "IssueFrom6_1") == "Y" ? true : false);

                #endregion

                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(SPSQLText, currConn, transaction);

                if (string.IsNullOrEmpty(vm.UserId))
                {
                    throw new Exception("User Id Not Found");
                }


                bool checkProductType = CheckProductType(vm, currConn, transaction);


                if (Permanent6_1 && !vm.PermanentProcess && !checkProductType)
                {
                    if (vm.BranchWise)
                    {
                        retResults = Save6_1_FromPermanent_Branch(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);

                    }
                    else
                    {
                        retResults = Save6_1_FromPermanent(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }

                    string getMaxDate = @"select max(startdatetime) from VAT6_1_Permanent where 1=1 ";
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        getMaxDate += " and ItemNo=@ItemNo";
                    }

                    cmd = new SqlCommand(getMaxDate, currConn, transaction);
                    cmd.CommandTimeout = 500;
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    var maxDate = cmd.ExecuteScalar();

                    DateTime permanent_Max = Convert.ToDateTime(maxDate == DBNull.Value ? null : maxDate);

                    if (Convert.ToDateTime(vm.StartDate) > permanent_Max)
                    {
                        vm.SkipOpening = true;
                        retResults = Save6_1(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }
                    else if (Convert.ToDateTime(vm.EndDate) > permanent_Max)
                    {
                        vm.StartDate = permanent_Max.AddDays(1).ToString("yyyy-MM-dd");
                        vm.SkipOpening = true;
                        retResults = Save6_1(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }

                }
                else
                {
                    retResults = Save6_1(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);

                }

                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1 set AvgRate = 0,RunningValue= 0
, RunningOpeningValue=0
from (
select distinct ItemNo,min(StartDateTime)StartDateTime 
from VAT6_1_Permanent 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
and UserId=@UserId

group by ItemNo
) a
where VAT6_1.ItemNo = a.ItemNo and VAT6_1.StartDateTime>= a.StartDateTime
and VAT6_1.UserId=@UserId


";
                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(updateNegativeValue, currConn, transaction);
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                    cmd.ExecuteNonQuery();
                }
                else
                {


                    vm.SPSQLText = updateNegativeValue;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();

                }
                #endregion



                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";

            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ReportDSDAL", "VAT6_1_WithConn", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                FileLogger.Log("ReportDSDAL", "VAT6_1_WithConn", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

            }

            #endregion

            return retResults;
        }

        private string[] Save6_1_FromPermanent_Branch(VAT6_1ParamVM vm, bool TotalIncludeSD, bool IncludeOtherAMT, bool IssueFrom6_1,
          bool TollReceiveNotWIP, bool TollReceiveWithIssue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText = "";

                CommonDAL commonDal = new CommonDAL();
                bool Permanent6_1 = commonDal.settings("VAT6_1", "6_1Permanent", currConn, transaction, connVM) == "Y";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region SQL Text

                sqlText = "";


                #region Select Product


                sqlText += @"  

create table #ProductReceive(ItemNo varchar(50))

insert into #ProductReceive(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
--and Products.BranchId=@BranchId
 

";

                #region Conditions
                if (vm.Is6_1Permanent)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_1')";
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    string code = new CommonDAL().settingValue("CompanyCode", "Code");
                    if (code.ToLower() == "cp")
                    {
                        sqlText += @"  and Products.BranchId=@BranchId";
                    }
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";

                        #region Debugging

                        ////if (true)
                        ////{
                        ////    sqlText += @"  and ItemNo='73'";
                        ////}

                        #endregion
                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                    }
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  ) as a";



                sqlText += @"

delete from VAT6_1 where UserId = @UserId


insert into VAT6_1(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[RunningTotal]
,RunningValue
)

SELECT 
      [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,@UserId
      ,[AvgRate]
      ,[RunningTotal]

,RunningValue

from   ( 

select * from VAT6_1_Permanent_Branch where
 TransType in  ('Opening')
and ItemNo in ( select distinct ItemNo from #ProductReceive)
and BranchId = @BranchId

union all

select * from VAT6_1_Permanent_Branch where
 TransType not in  ('Opening')
and ItemNo in (select distinct ItemNo from #ProductReceive)
and  StartDateTime >= @StartDate 
and StartDateTime < DATEADD(d,1,@EndDate) 
and BranchId = @BranchId
 ) as a
order by ItemNo, StartDateTime, SerialNo


update VAT6_1 set  Quantity=a.ClosingQty,UnitCost=a.ClosingValue, RunningTotal=a.ClosingQty,RunningValue=a.ClosingValue, StartDateTime = @StartDate,AvgRate=a.AvgRate
from  (
select VAT6_1_Permanent_Branch.Id,VAT6_1_Permanent_Branch.ItemNo,RunningTotal ClosingQty,RunningValue ClosingValue, AvgRate

from VAT6_1_Permanent_Branch
right outer join (
select distinct ItemNo,BranchId, MAX(Id)Id from VAT6_1_Permanent_Branch
where StartDateTime <@StartDate and ItemNo in (select distinct ItemNo from #ProductReceive) and BranchId = @BranchId
group by ItemNo,BranchId ) as a
on   a.Id=VAT6_1_Permanent_Branch.ID and a.BranchId=VAT6_1_Permanent_Branch.BranchId 
where VAT6_1_Permanent_Branch.BranchId=@BranchId
) as a
where a.ItemNo=VAT6_1.ItemNo and VAT6_1.TransType='Opening' and UserId = @UserId


update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.TransID=ProductAvgPrice.PurchaseNo
 and VAT6_1.UserId = @UserId

update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.StartDateTime >= isnull(ProductAvgPrice.FromDate,ProductAvgPrice.AgvPriceDate) 
and VAT6_1.StartDateTime< DATEADD(d,1, ProductAvgPrice.AgvPriceDate) 
and VAT6_1.TransType in('Opening','Issue')
 and VAT6_1.UserId = @UserId

update VAT6_1 set AvgRate=a.AvgPrice
from(
select ProductAvgPrice.ItemNo,AvgPrice from ProductAvgPrice
right outer join (
select distinct ItemNo,max(sl)SL from ProductAvgPrice 
where 
ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
group by ItemNo)as a
on a.ItemNo=ProductAvgPrice.ItemNo and a.Sl=ProductAvgPrice.sl) as a
where a.ItemNo=VAT6_1.ItemNo
and  VAT6_1.ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
and VAT6_1.UserId = @UserId


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9),BranchId int)
insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost)
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_1
where UserId = @UserId and BranchId=@BranchId
order by ItemNo,StartDateTime,SerialNo


update VAT6_1 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_1.Id 
and VAT6_1.UserId = @UserId


--update VAT6_1 set  RunningValue=RT.RunningValue
--from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
--SUM (case when TransType in('Issue') then -1*UnitCost else UnitCost end ) 
--OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningValue
--FROM #Temp)RT
--where RT.Id=VAT6_1.Id 
--and VAT6_1.UserId = @UserId
--and VAT6_1.TransType not in('Opening')

--Update VAT6_1 set RunningValue = AvgRate*RunningTotal where VAT6_1.UserId = @UserId

--update VAT6_1 set  RunningOpeningQuantity=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningTotal) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId
--
-- update VAT6_1 set  RunningOpeningValue=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningValue) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId


drop table #Temp

update VAT6_1 set RunningOpeningQuantity = Quantity,RunningOpeningValue = UnitCost
where TransType = 'Opening' and RunningOpeningQuantity is null and RunningOpeningValue is null and UserId = @UserId

select * from VAT6_1 where UserId = @UserId
order by ItemNo, StartDateTime, SerialNo

";

                #endregion

                #endregion

                #endregion

                #region SQL Command
                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 500;


                    #region Parameter
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                    {
                        cmd.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                    }

                    else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                    {
                        if (vm.ProdutCategoryLike == true)
                        {
                            cmd.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }

                    if (vm.StartDate == "")
                    {
                        if (!cmd.Parameters.Contains("@StartDate"))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (!cmd.Parameters.Contains("@StartDate"))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = vm.StartDate;
                        }
                    } // Common Filed

                    if (vm.EndDate == "")
                    {
                        if (!cmd.Parameters.Contains("@EndDate"))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters["@EndDate"].Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (!cmd.Parameters.Contains("@EndDate"))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                        }
                        else
                        {
                            cmd.Parameters["@EndDate"].Value = vm.EndDate;
                        }
                    }

                    #endregion Parameter

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@BranchId", vm.BranchId != 0 ? "'" + vm.BranchId + "'" : "0");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutType", !string.IsNullOrWhiteSpace(vm.ProdutType) ? "'" + vm.ProdutType + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutCategoryId", !string.IsNullOrWhiteSpace(vm.ProdutCategoryId) ? "'" + vm.ProdutCategoryId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutGroupName", !string.IsNullOrWhiteSpace(vm.ProdutGroupName) ? "'" + vm.ProdutGroupName + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                string[] result = { "success" };

                return result;

                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string[] Save6_1_FromPermanent(VAT6_1ParamVM vm, bool TotalIncludeSD, bool IncludeOtherAMT, bool IssueFrom6_1,
          bool TollReceiveNotWIP, bool TollReceiveWithIssue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = ""; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText = "";

                CommonDAL commonDal = new CommonDAL();
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                bool Permanent6_1 = commonDal.settings("VAT6_1", "6_1Permanent", currConn, transaction, connVM) == "Y";

                #region SQL Text

                sqlText = "";

                #region Select Product

                sqlText += @"  

create table #ProductReceive(ItemNo varchar(50))

insert into #ProductReceive(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
 
 

";

                #region Conditions
                if (vm.Is6_1Permanent)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_1')";
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    string code = new CommonDAL().settingValue("CompanyCode", "Code");
                    if (code.ToLower() == "cp")
                    {
                        sqlText += @"  and Products.BranchId=@BranchId";
                    }
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";
                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                    }
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  ) as a";

                sqlText += @"

delete from VAT6_1 where UserId = @UserId


insert into VAT6_1(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[RunningTotal]
,RunningValue
)

SELECT 
      [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,@UserId
      ,[AvgRate]
      ,[RunningTotal]

,RunningValue

from   ( 

select * from VAT6_1_Permanent where
 TransType in  ('Opening')
and ItemNo in ( select distinct ItemNo from #ProductReceive)

union all

select * from VAT6_1_Permanent where
 TransType not in  ('Opening')
and ItemNo in (select distinct ItemNo from #ProductReceive)
and  StartDateTime >= @StartDate and 
StartDateTime < DATEADD(d,1,@EndDate) 

 ) as a
order by ItemNo, StartDateTime, SerialNo


update VAT6_1 set  Quantity=a.ClosingQty
,UnitCost=a.ClosingValue
, RunningTotal=a.ClosingQty
,RunningValue=a.ClosingValue
, StartDateTime = @StartDate
, AvgRate = a.AvgRate
from  (
select VAT6_1_Permanent.Id,VAT6_1_Permanent.ItemNo,RunningTotal ClosingQty,RunningValue ClosingValue, AvgRate

from VAT6_1_Permanent
right outer join (
select distinct ItemNo, MAX(Id)Id from VAT6_1_Permanent
where StartDateTime <@StartDate and ItemNo in (select distinct ItemNo from #ProductReceive)
group by ItemNo) as a
on   a.Id=VAT6_1_Permanent.ID) as a
where a.ItemNo=VAT6_1.ItemNo and VAT6_1.TransType='Opening' and UserId = @UserId


update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.TransID=ProductAvgPrice.PurchaseNo
 and VAT6_1.UserId = @UserId

update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.StartDateTime >= isnull(ProductAvgPrice.FromDate,ProductAvgPrice.AgvPriceDate) 
and VAT6_1.StartDateTime< DATEADD(d,1, ProductAvgPrice.AgvPriceDate) 
and VAT6_1.TransType in('Opening','Issue')
 and VAT6_1.UserId = @UserId

update VAT6_1 set AvgRate=a.AvgPrice
from(
select ProductAvgPrice.ItemNo,AvgPrice from ProductAvgPrice
right outer join (
select distinct ItemNo,max(sl)SL from ProductAvgPrice 
where 
ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
group by ItemNo)as a
on a.ItemNo=ProductAvgPrice.ItemNo and a.Sl=ProductAvgPrice.sl) as a
where a.ItemNo=VAT6_1.ItemNo
and  VAT6_1.ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
and VAT6_1.UserId = @UserId


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9),)
insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost)
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_1
where UserId = @UserId 
order by ItemNo,StartDateTime,SerialNo


update VAT6_1 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_1.Id and VAT6_1.UserId = @UserId

--update VAT6_1 set  RunningValue=RT.RunningValue
--from (SELECT id,SL, ItemNo, TransType ,Quantity,
--SUM (case when TransType in('Issue') then -1*UnitCost else UnitCost end ) 
--OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningValue
--FROM #Temp)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId = @UserId
--and VAT6_1.TransType not in('Opening')

--Update VAT6_1 set RunningValue = AvgRate*RunningTotal where VAT6_1.UserId = @UserId

--update VAT6_1 set  RunningOpeningQuantity=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningTotal) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId
--
-- update VAT6_1 set  RunningOpeningValue=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningValue) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId


drop table #Temp

update VAT6_1 set RunningOpeningQuantity = Quantity,RunningOpeningValue = UnitCost
where TransType = 'Opening' and RunningOpeningQuantity is null and RunningOpeningValue is null and UserId = @UserId

select * from VAT6_1 where UserId = @UserId
order by ItemNo, StartDateTime, SerialNo

";

                #endregion

                #endregion


                #endregion
                if (!vm.FromSP)
                {


                    #region SQL Command
                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 500;

                    #region Parameter
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId);

                    if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                    {
                        cmd.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                    }

                    else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                    {
                        if (vm.ProdutCategoryLike == true)
                        {
                            cmd.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }

                    if (vm.StartDate == "")
                    {
                        if (!cmd.Parameters.Contains("@StartDate"))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (!cmd.Parameters.Contains("@StartDate"))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = vm.StartDate;
                        }
                    } // Common Filed

                    if (vm.EndDate == "")
                    {
                        if (!cmd.Parameters.Contains("@EndDate"))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters["@EndDate"].Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (!cmd.Parameters.Contains("@EndDate"))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                        }
                        else
                        {
                            cmd.Parameters["@EndDate"].Value = vm.EndDate;
                        }
                    }

                    #endregion Parameter

                    cmd.ExecuteNonQuery();
                    #endregion

                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutType", !string.IsNullOrWhiteSpace(vm.ProdutType) ? "'" + vm.ProdutType + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutCategoryId", !string.IsNullOrWhiteSpace(vm.ProdutCategoryId) ? "'" + vm.ProdutCategoryId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutGroupName", !string.IsNullOrWhiteSpace(vm.ProdutGroupName) ? "'" + vm.ProdutGroupName + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }
                retResults[0] = "Success";//Success or Fail

                return retResults;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataSet Save6_1_ReportFromPermanentByQuery(VAT6_1ParamVM vm, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string selectQuery = "";
            DataSet dataSet = new DataSet("ReportVAT6_1");

            try
            {
                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                CommonDAL commonDal = new CommonDAL();

                string query = "EXEC SPExecuteSelectQuery @Query";

                #region SQL Text

                selectQuery = "";

                #region Select Product

                selectQuery += @"  
--declare @StartDate date='2022-07-10' 
--declare @EndDate date='2023-07-10'

CREATE TABLE #VAT6_1_Permanent (    Id int IDENTITY(1,1) PRIMARY KEY,    SerialNo varchar(200) NULL,    ItemNo varchar(50) NULL,    StartDateTime datetime NULL,    StartingQuantity decimal(25, 9) NULL,    StartingAmount decimal(25, 9) NULL,    VendorID varchar(50) NULL,    SD decimal(25, 9) NULL,    VATRate decimal(25, 9) NULL,    Quantity decimal(25, 9) NULL,    UnitCost decimal(25, 9) NULL,    TransID varchar(50) NULL,    TransType varchar(50) NULL,    BENumber varchar(300) NULL,    InvoiceDateTime datetime NULL,    Remarks varchar(100) NULL,    TransactionType varchar(50) NULL,    BranchId varchar(50) NULL,    UserId varchar(50) NULL,    AvgRate decimal(25, 9) NULL,    PeriodID varchar(50) NULL,    RunningTotal decimal(25, 9) NULL,    RunningValue decimal(25, 9) NULL);

create table #ProductReceive( Id int IDENTITY(1,1) PRIMARY KEY,ItemNo varchar(50))

insert into #ProductReceive(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
";

                #region Conditions
                if (vm.Is6_1Permanent)
                {
                    selectQuery += @"  and Products.ReportType in('VAT6_1')";
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    string code = new CommonDAL().settingValue("CompanyCode", "Code");
                    if (code.ToLower() == "cp")
                    {
                        selectQuery += @"  and Products.BranchId=@BranchId";
                    }
                    if (vm.Flag == "SCBL")
                    {
                        selectQuery += @"  and IsRaw in('Raw','Pack')";
                    }
                    else
                    {
                        selectQuery += @"  and IsRaw=@ProdutType";
                    }
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        selectQuery += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        selectQuery += @"  and Products.CategoryID=@ProdutCategoryId";
                        selectQuery += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    selectQuery += @"  and ItemNo=@ItemNo";
                }

                selectQuery += @"  ) as a ";
                #endregion

                #endregion  Select Product
                #region Process Report

                selectQuery += @"

declare @LastId int =0;
DECLARE @counter INT = 1; 
DECLARE @RecordExist varchar(100) ; 
declare @LItemNo varchar(100)
DECLARE @Maxcounter INT; 
select @Maxcounter= max(id) from #ProductReceive


WHILE @counter <= @Maxcounter   
BEGIN
select @LItemNo=itemno from #ProductReceive where id=@counter
select top 1  @RecordExist= itemNo  FROM     VAT6_1_Permanent
WHERE  1=1 
and ItemNo = @LItemNo
and invoicedatetime <@StartDate
and transType not in('Opening')
ORDER BY  itemno,  id desc
IF @RecordExist IS NULL OR @RecordExist = ''
BEGIN
INSERT INTO #VAT6_1_Permanent (
SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
VendorID, SD, VATRate, Quantity, UnitCost, TransID, TransType,
BENumber, InvoiceDateTime, Remarks, TransactionType, BranchId, UserId,
AvgRate, RunningTotal, RunningValue
)
SELECT 
SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
VendorID, SD, VATRate, Quantity, UnitCost, TransID, TransType,
BENumber, InvoiceDateTime, Remarks, TransactionType, BranchId, UserId,
AvgRate, RunningTotal, RunningValue
FROM VAT6_1_Permanent
WHERE     ItemNo = @LItemNo
and invoicedatetime < DATEADD(d,1,@EndDate)
ORDER BY     ItemNo, StartDateTime, SerialNo;
END
--End If
else
begin

select top 1 @LastId=Id  FROM     VAT6_1_Permanent
WHERE  1=1 
and ItemNo  in(@LItemNo)
and invoicedatetime >=@StartDate
and transType not in('Opening')

INSERT INTO #VAT6_1_Permanent (
SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
VendorID, SD, VATRate, Quantity, UnitCost, TransID, TransType,
BENumber, InvoiceDateTime, Remarks, TransactionType, BranchId, UserId,
AvgRate, RunningTotal, RunningValue
)
select top 1 'A1'  SerialNo, ItemNo, @StartDate, RunningTotal StartingQuantity, RunningValue StartingAmount,
'0' VendorID, 0 SD, 0 VATRate,0  Quantity,0  UnitCost, '-' TransID,'Opening' TransType,
'-' BENumber,@StartDate InvoiceDateTime,'Opening' Remarks,'Opening' TransactionType, BranchId, UserId,
0  AvgRate, RunningTotal, RunningValue   FROM     VAT6_1_Permanent
where id<@lastId
and ItemNo  =@LItemNo
order by id desc
 
INSERT INTO #VAT6_1_Permanent (
SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
VendorID, SD, VATRate, Quantity, UnitCost, TransID, TransType,
BENumber, InvoiceDateTime, Remarks, TransactionType, BranchId, UserId,
AvgRate, RunningTotal, RunningValue
)
SELECT 
SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
VendorID, SD, VATRate, Quantity, UnitCost, TransID, TransType,
BENumber, InvoiceDateTime, Remarks, TransactionType, BranchId, UserId,
AvgRate, RunningTotal, RunningValue
FROM VAT6_1_Permanent
WHERE     ItemNo = @LItemNo
and invoicedatetime >=@StartDate
and invoicedatetime < DATEADD(d,1,@EndDate)
and transType not in('Opening')
ORDER BY     ItemNo, StartDateTime, SerialNo;
end;
--End else

WITH CTE AS (
SELECT 
ID,
LAG(RunningTotal) OVER (ORDER BY ItemNo,ID) AS LaggedQuantity
, LAG(runningValue) OVER (ORDER BY ItemNo,ID) AS LaggedValue
FROM 
#VAT6_1_Permanent
where itemno=@litemNo
)
UPDATE #VAT6_1_Permanent SET 
StartingQuantity = CTE.LaggedQuantity
,Startingamount = CTE.LaggedValue
FROM #VAT6_1_Permanent
JOIN CTE ON #VAT6_1_Permanent.ID = CTE.ID and #VAT6_1_Permanent.ItemNo=@LItemNo;

update #VAT6_1_Permanent set 
StartDateTime=@StartDate,invoiceDateTime=@StartDate,Quantity=0,UnitCost=0,AvgRate=0
,StartingQuantity=RunningTotal,StartingAmount=RunningValue
,TransactionType='Opening'
,VendorID='0'  ,SD= 0 ,VATRate= 0  ,TransID= '-'  ,
BENumber='-' ,Remarks= 'Opening'
where itemNo=@litemNo 
and TransType in('opening')

set @RecordExist=null
set @litemNo=null
set @LastId=0
SET @counter = @counter + 1;
END;

update #VAT6_1_Permanent set StartingQuantity=0,StartingAmount=0,RunningTotal=0,RunningValue=0
from (
select itemNo, pc.israw from products  p
left outer join ProductCategories pc on p.categoryid=pc.categoryid)nonstock
where #VAT6_1_Permanent.ItemNo=nonstock.ItemNo

select 
 ROW_NUMBER() OVER(PARTITION BY d.ItemNo ORDER BY Id) AS RowNumber,
p.ProductCode,p.ProductName,p.UOM FormUom,p.ProductName ProductNameColumn10,isnull(v.VendorCode,'')VendorCode,isnull(v.VendorName,'')VendorName,format(d.InvoiceDateTime,'dd/MM/yyyy')[Day]
,case when TransType in('Issue') and pc.IsRaw not in('service(nonstock)','overhead','noninventory') then Quantity else 0 end IssueQuantity
,case when TransType in('Issue') and pc.IsRaw not in('service(nonstock)','overhead','noninventory') then UnitCost else 0 end IssueAmount
,case when TransType in('purchase') then Quantity else 0 end purchaseQuantity
,case when TransType in('purchase') then UnitCost else 0 end purchaseAmount
, d.* FROM     #VAT6_1_Permanent d
left outer join Products p on d.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.categoryid=pc.categoryid

left outer join vendors v on d.VendorID=v.VendorID
ORDER BY     d.ItemNo, StartDateTime, SerialNo;

--select * from VAT6_1_Permanent
--where itemno in(select itemno from #ProductReceive)
--ORDER BY     ItemNo, StartDateTime, SerialNo;


drop table #ProductReceive
drop table #VAT6_1_Permanent

";
                #endregion Process Report


                #endregion

                selectQuery = selectQuery.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId.Trim() + "'" : "");
                selectQuery = selectQuery.Replace("@ProdutType", !string.IsNullOrWhiteSpace(vm.ProdutType) ? "'" + vm.ProdutType.Trim() + "'" : "");
                selectQuery = selectQuery.Replace("@ProdutGroupName", !string.IsNullOrWhiteSpace(vm.ProdutGroupName) ? "'" + vm.ProdutGroupName.Trim() + "'" : "");
                selectQuery = selectQuery.Replace("@ProdutCategoryId", !string.IsNullOrWhiteSpace(vm.ProdutCategoryId) ? "'" + vm.ProdutCategoryId.Trim() + "'" : "");
                selectQuery = selectQuery.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo.Trim() + "'" : "");
                selectQuery = selectQuery.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate.Trim() + "'" : "1900-01-01");
                selectQuery = selectQuery.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate.Trim() + "'" : "2900-01-01");

                using (SqlCommand cmd = new SqlCommand(query, currConn, transaction))
                {
                    cmd.Parameters.Add("@Query", SqlDbType.NVarChar, -1).Value = selectQuery;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        dataSet = new DataSet();
                        adapter.Fill(dataSet);
                    }
                }

                #region SQL Command

                return dataSet;

                #endregion


                transaction.Commit();

            }
            #region Catch & Finally

            catch (SqlException sqlex)
            {

                transaction.Rollback();


                FileLogger.Log("ReportDSDAL", "Save6_1_ReportFromPermanentByQuery", sqlex.ToString() + "\n" + selectQuery);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ReportDSDAL", "Save6_1_ReportFromPermanentByQuery", ex.ToString() + "\n" + selectQuery);

                throw ex;
            }
            finally
            {
                currConn.Close();
            }

            #endregion
        }

        public string[] Save6_1(VAT6_1ParamVM vm, bool TotalIncludeSD, bool IncludeOtherAMT, bool IssueFrom6_1,
            bool TollReceiveNotWIP, bool TollReceiveWithIssue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = ""; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            try
            {
                DataSet dataSet = new DataSet();
                string sqlText = "";

                CommonDAL commonDal = new CommonDAL();
                bool Permanent6_1 = false;//commonDal.settings("VAT6_1", "6_1Permanent", currConn, transaction) == "Y";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);


                #region SQL Text
                string deleteExisting = "delete from VAT6_1 where UserId = @UserId";

                sqlText = "";


                #region Select Product
                if (Permanent6_1)
                {

                    sqlText += @"  


create table #ProductReceive1(ItemNo varchar(50))

insert into #ProductReceive1(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
--and Products.BranchId=@BranchId
 

";

                    #region Conditions

                    if (vm.Is6_1Permanent && vm.FilterProcessItems)
                    {
                        sqlText += @"  and Products.ReportType in('VAT6_1','VAT6_1_And_6_2')  and Products.ProcessFlag='Y' ";

                    }
                    else if (vm.Is6_1Permanent)
                    {
                        sqlText += @"  and Products.ReportType in('VAT6_1','VAT6_1_And_6_2')  ";
                        if (!string.IsNullOrEmpty(vm.ItemNo))
                        {
                            sqlText += @"  and Products.ItemNo in ('" + vm.ItemNo + "')  ";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                    {
                        string code = new CommonDAL().settingValue("CompanyCode", "Code");
                        if (code.ToLower() == "cp")
                        {
                            sqlText += @"  and Products.BranchId=@BranchId";
                        }
                        if (vm.Flag == "SCBL")
                        {
                            sqlText += @"  and IsRaw in('Raw','Pack')";


                        }
                        else
                        {
                            sqlText += @"  and IsRaw=@ProdutType";
                        }
                    }

                    else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                    {
                        if (vm.ProdutCategoryLike == true)
                        {
                            sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                        }
                        else
                        {
                            sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                            sqlText += @"  and Products.ActiveStatus='Y'";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                    {
                        sqlText += @"  and ItemNo=@ItemNo";
                    }

                    sqlText += @"  ) as a

delete from VAT6_1Temp where UserId = @UserId

insert into VAT6_1Temp(
[SerialNo],[ItemNo],[StartDateTime],[StartingQuantity],[StartingAmount],[VendorID],[SD]
,[VATRate],[Quantity],[UnitCost],[TransID],[TransType],[BENumber],[InvoiceDateTime]
,[Remarks],[CreateDateTime],[TransactionType],[BranchId],[UserId],[AvgRate],[PeriodID]      ,[RunningTotal]
)
select 
[SerialNo],[ItemNo],[StartDateTime],[StartingQuantity],[StartingAmount],[VendorID],[SD]
,[VATRate],[Quantity],[UnitCost],[TransID],[TransType],[BENumber],[InvoiceDateTime]
,[Remarks],[CreateDateTime],[TransactionType],[BranchId],@UserId,[AvgRate],[PeriodID]      ,[RunningTotal]
from VAT6_1_Permanent

where 1=1
and StartDateTime >= @StartDate and StartDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive1)

select  DATEADD(d,1,max(StartDatetime)) from VAT6_1Temp
where UserId = @UserId

";




                    #endregion

                #endregion

                    if (!vm.FromSP)
                    {


                        cmd = new SqlCommand(sqlText, currConn, transaction);
                        cmd.CommandTimeout = 1000;

                        if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                        {
                            cmd.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                        }

                        else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                        {
                            if (vm.ProdutCategoryLike == true)
                            {
                                cmd.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                        {
                            cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                        }
                        //productCmd.ExecuteNonQuery();
                        cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                        cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                        var maxDate = cmd.ExecuteScalar();

                        vm.StartDate = maxDate == null ? vm.StartDate : maxDate.ToString();
                    }
                    else
                    {

                        vm.SPSQLText = sqlText;

                        vm.SPSQLText = vm.SPSQLText.Replace("@ProdutType", !string.IsNullOrWhiteSpace(vm.ProdutType) ? "'" + vm.ProdutType + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@ProdutCategoryId", !string.IsNullOrWhiteSpace(vm.ProdutCategoryId) ? "'" + vm.ProdutCategoryId + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@ProdutGroupName", !string.IsNullOrWhiteSpace(vm.ProdutGroupName) ? "'" + vm.ProdutGroupName + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                        vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        var maxDate = cmd.ExecuteScalar();
                        vm.StartDate = maxDate == null ? vm.StartDate : maxDate.ToString();



                    }

                }



                #region Beginning

                sqlText = @"
                          
---DECLARE @StartDate DATETIME;
---DECLARE @EndDate DATETIME;
---DECLARE @post1 VARCHAR(200);
---DECLARE @post2 VARCHAR(200);
---DECLARE @ItemNo VARCHAR(200);
---SET @Itemno='46';
---SET @post1='Y';
---SET @post2='N';
---SET @StartDate='2014-04-01';
---SET @EndDate='2020-04-27';
---
---DECLARE @BranchId as int = 1;

DECLARE @maxDate as datetime;


declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;

 

CREATE TABLE #VAT_16(	SerialNo [varchar] (2) NULL,
[ItemNo] [varchar](200) NULL,	[StartDateTime] [datetime] NULL,
[StartingQuantity] [decimal](25, 9) NULL,	[StartingAmount] [decimal](25, 9) NULL,
[VendorID] [varchar](200) NULL,	[SD] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[UnitCost] [decimal](25, 9) NULL,	[TransID] [varchar](200) NULL,
[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,[InvoiceDateTime] [datetime] NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL
,TransactionType [varchar] (200)  
)

CREATE TABLE #VATTemp_16([SerialNo] [varchar] (2) NULL,[Dailydate] [datetime] NULL,[InvoiceDateTime] [datetime] NULL,
[TransID] [varchar](200) NULL,	[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,
[ItemNo] [varchar](200) NULL,	[UnitCost] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,	[SD] [decimal](25, 9) NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL
,TransactionType [varchar] (200)  
) 

";

                #endregion

                #region Select Product


                sqlText += @"  


create table #ProductReceive(ItemNo varchar(50))

insert into #ProductReceive(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
--and Products.BranchId=@BranchId
 

";

                #region Conditions
                if (vm.Is6_1Permanent && vm.FilterProcessItems)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_1','VAT6_1_And_6_2')  and Products.ProcessFlag='Y' ";

                }
                else if (vm.Is6_1Permanent)
                {
                    if (vm.VAT6_2_1)
                    {
                        sqlText += @"  and Products.ReportType in('VAT6_2_1')";
                    }
                    else
                    {
                        sqlText += @"  and Products.ReportType in('VAT6_1','VAT6_1_And_6_2')";
                    }

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        sqlText += @"  and Products.ItemNo in ('" + vm.ItemNo + "')  ";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    string code = new CommonDAL().settingValue("CompanyCode", "Code");
                    if (code.ToLower() == "cp")
                    {
                        sqlText += @"  and Products.BranchId=@BranchId";
                    }
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";


                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                    }
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }
                if (vm.VAT6_2_1)
                {
                    sqlText += @"  and  Products.ReportType in('VAT6_2_1')";
                }
                else
                {
                    sqlText += @"  and  Products.ReportType in('VAT6_1','VAT6_1_And_6_2')";
                }
                sqlText += @"  ) as a";




                #endregion

                #endregion

                #region Transaction Data

                if (vm.Opening == false)
                {
                    #region Purchase Data

                    sqlText += @"
-------------------------------------------------- Start Purchase --------------------------------------------------
--------------------------------------------------------------------------------------------------------------------

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost    
,Quantity,VATRate,SD,Remarks,CreateDateTime)

";

                    #region 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService'

                    sqlText += @"
select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase',pd.BENumber,
pd.ItemNo
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @",isnull(subtotal,0)+isnull(pd.SDAmount,0) ";
                    }
                    else
                    {
                        sqlText += @",isnull(subtotal,0) ";
                    }

                    sqlText += @"
,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'Purchase', CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate
from PurchaseInvoiceDetails PD where pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','PurchaseTollcharge')
AND pd.BranchId=@BranchId

";

                    #endregion

                    #region Code To Code Transfer Purchase

                    sqlText += @"
union all

select 'A1',CONVERT(varchar, pd.TransferDate,23)ReceiveDate,
CONVERT(varchar, pd.TransferDate,23)InvoiceDateTime,
pt.TransferCode,'Purchase',
'-',
pd.ToItemNo
,isnull(pd.ReceivePrice,0),isnull(ToQuantity,0) ,
0,0,'CTCPurchase',
CONVERT(varchar, pd.TransferDate,23)ReceiveDate

from ProductTransfersDetails PD left outer join ProductTransfers pt on pd.ProductTransferId = pt.Id
where pd.TransferDate  >=@StartDate  and pd.TransferDate < DATEADD(d,1, @EndDate) 

and pd.ToItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('RawCTC')
AND pd.BranchId=@BranchId";

                    sqlText += @"
union all

select 'A1',CONVERT(varchar, pd.TransferDate,23)ReceiveDate,
CONVERT(varchar, pd.TransferDate,23)InvoiceDateTime,
pt.TransferCode,'Purchase',
'-',
pd.ToItemNo
,isnull(pd.ReceivePrice,0),isnull(ToQuantity,0) ,
0,0,'CTCPurchase',
CONVERT(varchar, pd.TransferDate,23)ReceiveDate

from ProductTransfersDetails PD left outer join ProductTransfers pt on pd.ProductTransferId = pt.Id
where pd.TransferDate  >=@StartDate  and pd.TransferDate < DATEADD(d,1, @EndDate) 

and pd.ToItemNo in(select distinct ItemNo from #ProductReceive)
and pd.ToItemNo in(
select distinct ItemNo from Products p left outer join ProductCategories pc
on p.CategoryID = pc.CategoryID
where pc.IsRaw = 'Trading'
)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('FinishCTC')
AND pd.BranchId=@BranchId



";

                    #endregion

                    if (vm.Is6_1Permanent && vm.BranchId != 0)
                    {
                        #region  Transfer Receive

                        sqlText += @"
union all

select 'A1',CONVERT(varchar, pd.TransactionDateTime,23)ReceiveDate,
CONVERT(varchar, pd.TransactionDateTime,23)InvoiceDateTime,
pd.TransferReceiveNo,'Purchase',
'-',
pd.ItemNo
,isnull(pd.SubTotal,0),isnull(Quantity,0) ,
0,0,'Raw TransferReceive',
CONVERT(varchar, pd.TransactionDateTime,23)ReceiveDate

from TransferReceiveDetails PD  
where pd.TransactionDateTime  >=@StartDate  and pd.TransactionDateTime < DATEADD(d,1, @EndDate) 

and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('61in')
AND pd.BranchId=@BranchId

";

                        #endregion

                    }

                    #region 'TollReceive-WIP'

                    sqlText += @"

union all
select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase',pd.BENumber,
pd.ItemNo, isnull(subtotal,0) ,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'Toll Receive',pd.ReceiveDate
from PurchaseInvoiceDetails PD where pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('TollReceive-WIP')
AND pd.BranchId=@BranchId

";

                    #endregion

                    #region 'TollClient6_4InsWIP'

                    sqlText += @"

union all
select 'A1',CONVERT(varchar, pd.IssueDateTime,23)ReceiveDate,CONVERT(varchar, pd.IssueDateTime,23)InvoiceDateTime
,pd.IssueNo,'Purchase',pd.IssueNo,
pd.ItemNo, isnull(subtotal,0) ,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'TollClient6_4InsWIP',pd.IssueDateTime
from IssueDetails PD where pd.IssueDateTime  >=@StartDate  and pd.IssueDateTime < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('TollClient6_4InsWIP')
AND pd.BranchId=@BranchId

union all
select 'A1',CONVERT(varchar, pd.IssueDateTime,23)ReceiveDate,CONVERT(varchar, pd.IssueDateTime,23)InvoiceDateTime
,pd.IssueNo,'Purchase',pd.IssueNo,
pd.ItemNo, isnull(subtotal,0) ,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'TollClient6_4Backs',pd.IssueDateTime
from IssueDetails PD where pd.IssueDateTime  >=@StartDate  and pd.IssueDateTime < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('TollClient6_4Backs')
AND pd.BranchId=@BranchId

union all
select 'A1',CONVERT(varchar, pd.IssueDateTime,23)ReceiveDate,CONVERT(varchar, pd.IssueDateTime,23)InvoiceDateTime
,pd.IssueNo,'Purchase',pd.IssueNo,
pd.ItemNo, isnull(subtotal,0) ,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'TollClient6_4BacksWIP',pd.IssueDateTime
from IssueDetails PD where pd.IssueDateTime  >=@StartDate  and pd.IssueDateTime < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('TollClient6_4BacksWIP')
AND pd.BranchId=@BranchId

";

                    #endregion

                    #region 'ClientFGReceiveWOBOM'

                    sqlText += @"

union all
select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase',pd.BENumber,
pd.ItemNo, isnull(subtotal,0) ,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'Toll Receive',pd.ReceiveDate
from PurchaseInvoiceDetails PD where pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('ClientFGReceiveWOBOM')
AND pd.BranchId=@BranchId

";

                    #endregion

                    if (vm.StockMovement == true)
                    {
                        sqlText += @"

union all
select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase',pd.BENumber,
pd.ItemNo, isnull(subtotal,0) ,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'Toll Receive',pd.ReceiveDate
from PurchaseInvoiceDetails PD 
where 1=1 
and pd.SubTotal>0
and pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('TollReceiveRaw')
AND pd.BranchId=@BranchId

";
                    }


                    #region 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport'

                    sqlText += @"

 
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,Quantity,VATRate,SD,Remarks,CreateDateTime)


select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase'
,pd.BENumber,pd.ItemNo
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @"
,(isnull(pd.AssessableValue,0)+ isnull(pd.CDAmount,0)+ isnull(pd.RDAmount,0)+ isnull(pd.TVBAmount,0)+ isnull(pd.TVAAmount,0)+isnull(pd.SDAmount,0)+@othervalue)";
                    }
                    else
                    {
                        sqlText += @"
,(isnull(pd.AssessableValue,0)+ isnull(pd.CDAmount,0)+ isnull(pd.RDAmount,0)+ isnull(pd.TVBAmount,0)+ isnull(pd.TVAAmount,0)+@othervalue)";
                    }

                    if (IncludeOtherAMT)
                    {
                        sqlText = sqlText.Replace("@othervalue", "isnull(pd.OthersAmount,0)");
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@othervalue", "0");
                    }


                    sqlText += @"
,isnull(UOMQty,0),pd.VATAmount,pd.SDAmount,PD.TransactionType,ReceiveDate
from PurchaseInvoiceDetails PD 
where pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (pd.Post =@Post1 or pd.Post= @Post2)
AND PD.TransactionType IN('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
AND pd.BranchId=@BranchId

";

                    #endregion

                    #region 'ClientRawReceive'

                    sqlText += @"

union all

select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase',pd.BENumber,
pd.ItemNo
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @",isnull(subtotal,0)+isnull(pd.SDAmount,0) ";
                    }
                    else
                    {
                        sqlText += @",isnull(subtotal,0) ";
                    }

                    sqlText += @"
,isnull(UOMQty,0) ,
pd.VATAmount,pd.SDAmount,'Client Raw Receive',pd.ReceiveDate
from PurchaseInvoiceDetails PD where pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('ClientRawReceive')
AND pd.BranchId=@BranchId

";

                    #endregion

                    #region 'PurchaseReturn','PurchaseDN'

                    sqlText += @"
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'A1',CONVERT(varchar, pd.ReceiveDate,23)ReceiveDate,CONVERT(varchar, pd.InvoiceDateTime,23)InvoiceDateTime,pd.PurchaseInvoiceNo,'Purchase'
,pd.BENumber,pd.ItemNo
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @"
,-1*isnull(subtotal,0)+isnull(pd.SDAmount,0) ";
                    }
                    else
                    {
                        sqlText += @"
,-1*isnull(subtotal,0) ";
                    }

                    sqlText += @"

,-isnull(UOMQty,0) ,
-pd.VATAmount,-pd.SDAmount,PD.TransactionType,CreatedOn
from PurchaseInvoiceDetails PD 
where pd.ReceiveDate  >=@StartDate  and pd.ReceiveDate < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (pd.Post =@Post1 or pd.Post= @Post2)
AND PD.TransactionType IN('PurchaseReturn','PurchaseDN')
AND pd.BranchId=@BranchId

-------------------------------------------------- End Purchase --------------------------------------------------
------------------------------------------------------------------------------------------------------------------

";

                    #endregion

                    #endregion

                    #region VAT 6.2.1 False / Receive, Issue

                    if (vm.VAT6_2_1 == false)
                    {
                        #region Receive Data

                        sqlText += @"
-------------------------------------------------- ReceiveDetails --------------------------------------------------
--------------------------------------------------------------------------------------------------------------------

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost    
,Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'A1',pd.ReceiveDateTime,pd.ReceiveDateTime,pd.ReceiveNo,'Purchase', '' BENumber,
pd.ItemNo,isnull(subtotal,0)  ,isnull(UOMQty,0) ,
pd.VATAmount,0 SDAmount,'WIP',pd.ReceiveDateTime
from ReceiveDetails PD where pd.ReceiveDateTime  >=@StartDate  and pd.ReceiveDateTime < DATEADD(d,1, @EndDate) 
and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
 AND   (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('WIP')
AND pd.BranchId=@BranchId

";

                        #endregion

                        #region Issue Data

                        sqlText += @"
-------------------------------------------------- Start Issue --------------------------------------------------
-----------------------------------------------------------------------------------------------------------------
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'Issue',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
 AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('Other','Trading','TradingAuto','ExportTrading','TradingTender'
,'ExportTradingTender','InternalIssue','Service','ExportService','InputServiceImport'
,'InputService','Tender','WIP','PackageProduction','PurchaseTollcharge')
AND id.BranchId=@BranchId
";

                        #region TollClient6_4Outs TollClient6_4OutWIP

                        sqlText += @"

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'TollClient6_4Outs',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
 AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('TollClient6_4Outs')
AND id.BranchId=@BranchId

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'TollClient6_4OutWIP',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
 AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('TollClient6_4OutWIP')
AND id.BranchId=@BranchId

";

                        #endregion

                        #region Code to Code Transfer

                        sqlText += @"

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.TransferDate,ID.TransferDate,pt.TransferCode,'Issue','-'
,id.FromItemNo,isnull(id.IssuePrice,0) ,isnull(id.FromQuantity,0) ,0 VATAmount,0 SDAmount,'CTCTransfer',ID.TransferDate
from ProductTransfersDetails ID left outer join ProductTransfers pt on id.ProductTransferId = pt.Id
where ID.TransferDate  >=@StartDate  and ID.TransferDate < DATEADD(d,1, @EndDate)  
and ID.FromItemNo in(select distinct ItemNo from #ProductReceive)
 AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN('RawCTC')
AND id.BranchId=@BranchId
";

                        #endregion

                        if (vm.Is6_1Permanent && vm.BranchId != 0)
                        {
                            #region  Transfer Issue

                            sqlText += @"
union all

select 'A1',CONVERT(varchar, pd.TransactionDateTime,23)ReceiveDate,
CONVERT(varchar, pd.TransactionDateTime,23)InvoiceDateTime,
pd.TransferIssueNo,'Issue',
'-',
pd.ItemNo
,isnull(pd.SubTotal,0),isnull(Quantity,0) ,
0,0,'Raw TransferIssue',
CONVERT(varchar, pd.TransactionDateTime,23)ReceiveDate

from TransferIssueDetails PD  
where pd.TransactionDateTime  >=@StartDate  and pd.TransactionDateTime < DATEADD(d,1, @EndDate) 

and pd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND  (pd.Post =@Post1 or pd.Post= @Post2)
AND pd.TransactionType IN('61out')
AND pd.BranchId=@BranchId

";

                            #endregion

                        }


                        #region Toll Issue Data

                        if (IssueFrom6_1)
                        {
                            #region Issue From 6.1

                            sqlText += @"   
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'Issue',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('TollIssue')
AND id.BranchId=@BranchId
";

                            #endregion
                        }
                        else
                        {
                            if (TollReceiveNotWIP)
                            {
                                #region Toll Receive NotWIP

                                sqlText += @"   
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'Issue',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
  AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('TollReceive-NotWIP')
AND id.BranchId=@BranchId
";

                                #endregion
                            }

                            if (TollReceiveWithIssue)
                            {
                                #region Toll Receive With Issue

                                sqlText += @"
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'Toll Issue',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
  AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('TollReceive')
AND id.BranchId=@BranchId

";

                                #endregion
                            }
                        }

                        #endregion

                        #region Toll Finish Receive

                        sqlText += @" 
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'Issue',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (ID.Post =@Post1 or ID.Post= @Post2)  and ID.SubTotal>0
AND ID.TransactionType IN ('TollFinishReceive') 
AND id.BranchId=@BranchId
";

                        #endregion

                        #region Issue Return, Receive Return

                        sqlText += @" 

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select  'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,-isnull(UOMQty,0)*isnull(uomPrice,0) ,-isnull(UOMQty,0) ,-ID.VATAmount,-ID.SDAmount,id.TransactionType,IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
 AND (UOMQty>0)AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('IssueReturn','ReceiveReturn')
AND ID.BranchId=@BranchId
";

                        #endregion

                        #endregion

                        #region Dispose Raw Data

                        sqlText += @"

insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B',ID.TransactionDateTime IssueDateTime,ID.TransactionDateTime IssueDateTime,ID.DisposeNo,'Issue','-'
,id.ItemNo,isnull(Quantity,0)*isnull(SubTotal,0) ,isnull(Quantity,0) ,ID.VATAmount,ID.SDAmount,'Raw Dispose', TransactionDateTime IssueDateTime
from DisposeRawDetails ID
where ID.TransactionDateTime  >=@StartDate  and ID.TransactionDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
and ISNULL(ID.IsSaleable,'N')='N'
AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('Other')
AND id.BranchId=@BranchId
";

                        #endregion
                    }

                    #endregion

                    #region Stock Movement False/ Raw Sale, Raw Credit, Dispose Raw Sale

                    #region Raw Sale

                    sqlText += @"
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B', InvoiceDateTime,InvoiceDateTime,SalesInvoiceNo,'Issue','-',ItemNo,
isnull(UOMQty,0)*isnull(AVGPrice,0),
 (  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),'0',SDAmount,TransactionType,CreatedOn
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0) AND (Post =@Post1 or Post= @Post2)
AND TransactionType IN('RawSale')  
AND BranchId=@BranchId
";

                    #endregion


                    if (vm.StockMovement == false)
                    //if (true)
                    {
                        #region Raw Credit

                        sqlText += @"
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select  'B',ID.InvoiceDateTime,ID.InvoiceDateTime,ID.SalesInvoiceNo,'Issue','-'
,id.ItemNo,-isnull(UOMQty,0)*isnull(AVGPrice,0) ,-isnull(UOMQty,0) ,-0 VATAmount,-0 SDAmount,id.TransactionType,InvoiceDateTime
from SalesInvoiceDetails ID
where ID.InvoiceDateTime  >=@StartDate  and ID.InvoiceDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
 AND (UOMQty>0)AND (ID.Post =@Post1 or ID.Post= @Post2)
AND ID.TransactionType IN ('RawCredit')
AND ID.BranchId=@BranchId

";

                        #endregion

                        #region Dispose Raw Sale

                        sqlText += @"
insert into #VATTemp_16(SerialNo,Dailydate,InvoiceDateTime,TransID,TransType,BENumber,ItemNo,UnitCost,
Quantity,VATRate,SD,Remarks,CreateDateTime)

select 'B', InvoiceDateTime,InvoiceDateTime,SalesInvoiceNo,'Issue','-'
, ItemNo
, isnull(UOMQty,0)*isnull(UOMPrice,0)
, case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end 
,'0'
, SDAmount,'Dispose Raw Sale',CreatedOn
from SalesInvoiceDetails
where 1=1 
and InvoiceDateTime >= @StartDate and InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0) AND (Post =@Post1 or Post= @Post2)
AND TransactionType IN('DisposeRaw')  
AND BranchId=@BranchId
";

                        #endregion
                    }

                    #endregion
                }

                #endregion

                if (vm.Is6_1Permanent == false || vm.StockProcess == true)
                {
                    #region Openinng Data

                    sqlText += @"
-------------------------------------------------- Openinng Data --------------------------------------------------
-------------------------------------------------------------------------------------------------------------------


set @OpeningDate=@StartDate

insert into #VATTemp_16(SerialNo,Dailydate,TransID,VATRate,SD,Remarks,TransType,ItemNo,Quantity,UnitCost,InvoiceDateTime,BENumber)
 		    
SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo, SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost,@OpeningDate InvoiceDateTime,'-' BENumber
	FROM (
SELECT distinct ItemNo, 0 Quantity, 0 Amount  FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId='1'

";

                    #region Opening From Product False

                    if (vm.OpeningFromProduct == false)
                    {
                        if (vm.BranchId > 1)
                        {
                            sqlText += @"	
UNION ALL 	 
SELECT distinct ItemNo, isnull(sum(p.StockQuantity),0) Quantity, isnull(sum(p.StockValue),0) Amount  
FROM ProductStocks p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND BranchId>1
group by ItemNo
 
";
                        }
                        else
                        {
                            sqlText += @"		
UNION ALL  
SELECT distinct itemNo ItemNo, sum(isnull(OpeningBalance,0)) Quantity, sum(isnull(p.OpeningTotalCost,0)) Amount  
FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
--AND BranchId='1'
group by ItemNo

";
                        }
                    }

                    #endregion

                    #region Purchase Data

                    #region 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService'

                    sqlText += @"		
UNION ALL (
		SELECT  distinct   ItemNo, isnull(sum(UOMQty),0)PurchaseQuantity
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)+isnull(SDAmount,0)),0)SubTotal ";
                    }
                    else
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)),0)SubTotal ";
                    }

                    sqlText += @"
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','PurchaseTollcharge') 
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate      AND ItemNo in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

 )   	 
";

                    #endregion


                    #region Code To Code Purchase

                    sqlText += @"
UNION ALL(
SELECT  distinct   ToItemNo, isnull(sum(ToQuantity),0)PurchaseQuantity
,isnull(sum(isnull(ReceivePrice,0)),0)SubTotal
FROM ProductTransfersDetails WHERE Post='Y' 
and TransactionType in('RawCTC') 
AND TransferDate>= '01-Jul-2019' and TransferDate < @StartDate     
AND ToItemNo in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ToItemNo
)
";

                    #endregion


                    #region 'TollReceive-WIP'

                    sqlText += @"		
UNION ALL (
		SELECT  distinct   ItemNo, isnull(sum(UOMQty),0)PurchaseQuantity
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)+isnull(SDAmount,0)),0)SubTotal ";
                    }
                    else
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)),0)SubTotal ";
                    }

                    sqlText += @"
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('TollReceive-WIP') 
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate      AND ItemNo in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

 )   	 
";

                    #endregion


                    #region 'TollReceive-WIP'

                    sqlText += @"		
UNION ALL (
		SELECT  distinct   ItemNo, isnull(sum(UOMQty),0)PurchaseQuantity
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)+isnull(SDAmount,0)),0)SubTotal ";
                    }
                    else
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)),0)SubTotal ";
                    }

                    sqlText += @"
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('ClientFGReceiveWOBOM') 
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate      AND ItemNo in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

 )   	 
";

                    #endregion


                    if (vm.StockMovement == true)
                    {
                        sqlText += @"		
UNION ALL (
		SELECT  distinct   ItemNo, isnull(sum(UOMQty),0)PurchaseQuantity
";
                        if (TotalIncludeSD)
                        {
                            sqlText += @",isnull(sum(isnull(SubTotal,0)+isnull(SDAmount,0)),0)SubTotal ";
                        }
                        else
                        {
                            sqlText += @",isnull(sum(isnull(SubTotal,0)),0)SubTotal ";
                        }

                        sqlText += @"
FROM PurchaseInvoiceDetails WHERE Post='Y' and SubTotal>0
and TransactionType in('TollReceiveRaw') 
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate      AND ItemNo in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

 )   	 
";
                    }
                    #region 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport'

                    sqlText += @"

UNION ALL (
	SELECT distinct   ItemNo,isnull(sum(UOMQty),0) PurchaseQuantity 
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @"	
	,isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+@othervalue2+isnull(SDAmount,0)),0)),0)SubTotal ";
                    }
                    else
                    {
                        sqlText += @"	
	,isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+@othervalue2),0)),0)SubTotal ";
                    }


                    if (IncludeOtherAMT)
                    {
                        sqlText = sqlText.Replace("@othervalue2", "isnull(OthersAmount,0)");
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@othervalue", "0");
                    }

                    sqlText += @"	
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport') 
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate      AND ItemNo  in(select distinct ItemNo from #ProductReceive)
 
AND BranchId=@BranchId
group by ItemNo
 )  


";

                    #endregion

                    #region 'ClientRawReceive'

                    sqlText += @"		
UNION ALL (
		SELECT  distinct   ItemNo, isnull(sum(UOMQty),0)PurchaseQuantity
";
                    if (TotalIncludeSD)
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)+isnull(SDAmount,0)),0)SubTotal ";
                    }
                    else
                    {
                        sqlText += @",isnull(sum(isnull(SubTotal,0)),0)SubTotal ";
                    }

                    sqlText += @"
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('ClientRawReceive') 
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate      AND ItemNo in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

 )   	 
";

                    #endregion

                    #region 'PurchaseReturn','PurchaseDN'

                    sqlText += @"
UNION ALL 
(	SELECT   distinct   ItemNo,-isnull(sum(UOMQty),0) PurchaseQuantity,
-isnull(sum(isnull(SubTotal,0)+isnull(SDAmount,0)),0)SubTotal     
FROM PurchaseInvoiceDetails 
WHERE Post='Y' 
and TransactionType in('PurchaseReturn','PurchaseDN')  
AND ReceiveDate>= '01-Jul-2019' and ReceiveDate < @StartDate       AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

) 
";

                    #endregion

                    #endregion

                    #region VAT6.2.1 False / Receive, Issue, Dispose Raw Data

                    if (vm.VAT6_2_1 == false)
                    {
                        #region Receive - WIP

                        sqlText += @"		
UNION ALL  
(SELECT distinct   ItemNo,  isnull(sum(UOMQty),0) Quantity,isnull(sum(isnull(SubTotal,0)),0)Amount 
FROM ReceiveDetails WHERE Post='Y' 
and TransactionType in('WIP') 
AND ReceiveDateTime>= '01-July-2019' and ReceiveDateTime < @StartDate    
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by itemNo
 )   
";

                        #endregion

                        #region Issue Data

                        sqlText += @"
UNION ALL 
(
SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)  
FROM IssueDetails WHERE Post='Y'   
AND IssueDateTime>= '01-July-2019' and IssueDateTime< @StartDate     
AND TransactionType IN ('Other','Trading','TradingAuto','ExportTrading','TradingTender'
,'ExportTradingTender','InternalIssue','Service','ExportService','InputServiceImport'
,'InputService','Tender','WIP','PackageProduction','PurchaseTollcharge')

AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 
";

                        #region Code To Code Transfer

                        sqlText += @"

UNION ALL 
(
SELECT  distinct   FromItemNo,-isnull(sum(FromQuantity),0) IssueQuantity,-isnull(sum(isnull(IssuePrice,0)),0)  
FROM ProductTransfersDetails WHERE Post='Y'   
AND TransferDate>= '01-July-2019' and TransferDate< @StartDate     
AND FromItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND TransactionType IN('RawCTC')
group by FromItemNo
) 
";

                        #endregion

                        #region Toll Issue

                        if (IssueFrom6_1)
                        {
                            #region Issue From 6.1

                            sqlText += @"
 UNION ALL 
 (
 SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)  
 FROM IssueDetails WHERE Post='Y'  
 AND IssueDateTime>= '01-July-2019' and IssueDateTime< @StartDate    
 AND TransactionType IN ('TollIssue')
 AND ItemNo  in(select distinct ItemNo from #ProductReceive)
 AND BranchId=@BranchId
group by ItemNo

) 
";

                            #endregion
                        }
                        else
                        {
                            if (TollReceiveNotWIP)
                            {
                                #region Toll Receive Not WIP

                                sqlText += @"
 UNION ALL 
 (
 SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)  
 FROM IssueDetails WHERE Post='Y'   
 AND IssueDateTime>= '01-July-2019' and IssueDateTime< @StartDate   
 AND TransactionType IN ('TollReceive-NotWIP')
 AND ItemNo  in(select distinct ItemNo from #ProductReceive)
 AND BranchId=@BranchId
group by ItemNo

) 
";

                                #endregion
                            }

                            if (TollReceiveWithIssue)
                            {
                                #region Toll Receive With Issue

                                sqlText += @"
 UNION ALL 
 (
 SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)  
 FROM IssueDetails WHERE Post='Y'   
 AND IssueDateTime>= '01-July-2019' and IssueDateTime< @StartDate   
 AND TransactionType IN ('TollReceive')
 AND ItemNo  in(select distinct ItemNo from #ProductReceive)
 AND BranchId=@BranchId
group by ItemNo

) 
";

                                #endregion
                            }
                        }

                        #endregion

                        #region Toll Finish Receive

                        sqlText += @" 
UNION ALL 
(
SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)  
FROM IssueDetails WHERE Post='Y'   and SubTotal>0
AND IssueDateTime>= '01-July-2019' and IssueDateTime< @StartDate    
AND TransactionType IN ('TollFinishReceive')  
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo

) 
";

                        #endregion

                        #region Issue Return, Receive Return

                        sqlText += @" 

UNION ALL 
(
SELECT  distinct   ItemNo,isnull(sum(UOMQty),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)
FROM IssueDetails WHERE Post='Y'  
AND IssueDateTime>= '01-July-2019' and IssueDateTime< @StartDate   
   and TransactionType IN('IssueReturn','ReceiveReturn')  AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)   
";

                        #endregion

                        #endregion

                        #region Dispose Raw Data

                        sqlText += @"
UNION ALL 
(
SELECT  distinct   ItemNo,-isnull(sum(Quantity),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)   
FROM DisposeRawDetails WHERE Post='Y'  
and ISNULL(IsSaleable,'N')='N' 
AND TransactionDateTime>= '01-July-2019' and TransactionDateTime< @StartDate     
AND TransactionType IN ('Other')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 
";

                        #endregion
                    }

                    #endregion

                    #region Stock Movement False / Sales

                    #region Raw Sale

                    sqlText += @"

UNION ALL 
(
SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(isnull(UOMQty,0)*isnull(AVGPrice,0),0)),0)  
FROM SalesInvoiceDetails WHERE Post='Y'   
AND InvoiceDateTime>= '01-July-2019' and InvoiceDateTime< @StartDate     
AND TransactionType IN ('RawSale')

AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 
";

                    #endregion

                    if (vm.StockMovement == false)
                    //if (true)
                    {
                        #region Raw Credit

                        sqlText += @"
UNION ALL 
(
SELECT  distinct   ItemNo,isnull(sum(UOMQty),0) IssueQuantity,isnull(sum(isnull(isnull(UOMQty,0)*isnull(AVGPrice,0),0)),0)  
FROM SalesInvoiceDetails WHERE Post='Y'   
AND InvoiceDateTime>= '01-July-2019' and InvoiceDateTime< @StartDate     
AND TransactionType IN ('RawCredit')

AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)
";

                        #endregion


                        #region Raw Sale

                        sqlText += @"

UNION ALL 
(
SELECT  distinct   ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(isnull(UOMQty,0)*isnull(UOMPrice,0),0)),0)  
FROM SalesInvoiceDetails WHERE Post='Y'    
AND InvoiceDateTime>= '01-July-2019' and InvoiceDateTime< @StartDate     
AND TransactionType IN ('DisposeRaw')

AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) 
";

                        #endregion
                    }

                    #endregion

                    sqlText += @"
) AS a GROUP BY a.ItemNo
";

                    #endregion


                }

                #region Insert, Update and Select Data

                sqlText += @"

insert into #VAT_16(SerialNo,ItemNo,StartDateTime,InvoiceDateTime,StartingQuantity,StartingAmount,
VendorID,Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,CreateDateTime)

select SerialNo,ItemNo,Dailydate,InvoiceDateTime,0,0,0,
Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,CreateDateTime
from #VATTemp_16
order by dailydate,SerialNo



update #VAT_16 set 
VendorID=PurchaseInvoiceHeaders.VendorID
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_16.TransID
and #VAT_16.TransType='Purchase'
AND BranchId=@BranchId



update #VAT_16 set 
StartingQuantity=0,
StartingAmount=0
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_16.TransID 
and PurchaseInvoiceHeaders.TransactionType IN('ServiceNS')
AND (PurchaseInvoiceHeaders.Post =@Post1 or PurchaseInvoiceHeaders.Post= @Post2)
and #VAT_16.TransType='Purchase'
AND BranchId=@BranchId


---- update #VAT_16 set UnitCost= ProductAvgPrice.avgprice*Quantity, StartingAmount= ProductAvgPrice.avgprice*Quantity
---- from (select distinct ProductAvgPrice.itemno, avgprice from ProductAvgPrice, 
---- ( 
---- select distinct itemno, max(AgvPriceDate) AgvPriceDate,max(SL)SL from ProductAvgPrice
---- where AgvPriceDate < @StartDate and TransactionType in('purchase','opening')  group by ItemNo)t
---- where t.SL = ProductAvgPrice.SL
---- and TransactionType in('purchase','opening')
---- )ProductAvgPrice where ProductAvgPrice.ItemNo=#VAT_16.ItemNo and #VAT_16.TransType = 'Opening'



--select #VAT_16.SerialNo,convert (varchar,#VAT_16.StartDateTime,120)StartDateTime,
--#VAT_16.StartingQuantity,#VAT_16.StartingAmount
--,TransID,#VAT_16.TransType,
--isnull(v.VendorName,'-')VendorName,isnull(v.Address1,'-')Address1,
--isnull(v.Address2,'-')Address2,isnull(v.Address3,'-')Address3,
--isnull(v.VATRegistrationNo,'-')VATRegistrationNo,
--p.ProductName,p.ProductCode ProductCodeA,p.UOM,#VAT_16.Quantity,#VAT_16.VATRate,#VAT_16.SD,#VAT_16.UnitCost,p.HSCodeNo,#VAT_16.BENumber
--,convert (varchar,#VAT_16.InvoiceDateTime,120)InvoiceDateTime,#VAT_16.Remarks,#VAT_16.ItemNo,
--#VAT_16.CreateDateTime
--from #VAT_16 left outer join
--Vendors as V on #VAT_16.VendorID=v.VendorID left outer join 
--Products P on #VAT_16.ItemNo=p.ItemNo
--order by CreateDateTime ASC,#VAT_16.SerialNo ASC 
--order by #VAT_16.SerialNo ASC ,CreateDateTime ASC

insert into VAT6_1(
SerialNo
,ItemNo
,StartDateTime
,StartingQuantity
,StartingAmount
,VendorID
,SD
,VATRate
,Quantity
,UnitCost
,TransID
,TransType
,BENumber
,InvoiceDateTime
,Remarks
,CreateDateTime
,TransactionType
,UserId
,BranchId
)

select *,@UserId,@BranchId from #VAT_16 
where 1=1 ";
                if (vm.SkipOpening)
                {
                    sqlText += @" and TransType <> 'Opening'";
                }
                sqlText += @" 

order by ItemNo,StartDatetime,SerialNo,TransID



--DROP TABLE #VAT_16
--DROP TABLE #VATTemp_16
--DROP TABLE #ProductReceive

                ";

                #endregion

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command
                if (!vm.SkipOpening)
                {
                    if (!vm.FromSP)
                    {
                        cmd = new SqlCommand(deleteExisting, currConn, transaction);
                        cmd.CommandTimeout = 3600;
                        cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        vm.SPSQLText = deleteExisting;
                        vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 3600;



                    #region Parameter
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                    {
                        cmd.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                    }

                    else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                    {
                        if (vm.ProdutCategoryLike == true)
                        {
                            cmd.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }

                    if (!cmd.Parameters.Contains("@UserName"))
                    {
                        cmd.Parameters.AddWithValue("@UserName", vm.UserName);
                    }
                    else
                    {
                        cmd.Parameters["@UserName"].Value = vm.UserName;
                    }

                    if (vm.StartDate == "")
                    {
                        if (!cmd.Parameters.Contains("@StartDate"))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (!cmd.Parameters.Contains("@StartDate"))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = vm.StartDate;
                        }
                    } // Common Filed

                    if (vm.EndDate == "")
                    {
                        if (!cmd.Parameters.Contains("@EndDate"))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters["@EndDate"].Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        if (!cmd.Parameters.Contains("@EndDate"))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                        }
                        else
                        {
                            cmd.Parameters["@EndDate"].Value = vm.EndDate;
                        }
                    }

                    if (!cmd.Parameters.Contains("@Post1"))
                    {
                        cmd.Parameters.AddWithValue("@Post1", vm.Post1);
                    }
                    else
                    {
                        cmd.Parameters["@Post1"].Value = vm.Post1;
                    }

                    if (!cmd.Parameters.Contains("@Post2"))
                    {
                        cmd.Parameters.AddWithValue("@Post2", vm.Post2);
                    }
                    else
                    {
                        cmd.Parameters["@Post2"].Value = vm.Post2;
                    }

                    #endregion Parameter
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@BranchId", vm.BranchId == 0 ? "'" + vm.BranchId + "'" : "0");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutType", !string.IsNullOrWhiteSpace(vm.ProdutType) ? "'" + vm.ProdutType + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutCategoryId", !string.IsNullOrWhiteSpace(vm.ProdutCategoryId) ? "'" + vm.ProdutCategoryId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ProdutGroupName", !string.IsNullOrWhiteSpace(vm.ProdutGroupName) ? "'" + vm.ProdutGroupName + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserName", !string.IsNullOrWhiteSpace(vm.UserName) ? "'" + vm.UserName + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    vm.SPSQLText = vm.SPSQLText.Replace("@Post1", !string.IsNullOrWhiteSpace(vm.Post1) ? "'" + vm.Post1 + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@Post2", !string.IsNullOrWhiteSpace(vm.Post2) ? "'" + vm.Post2 + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }


                string avgPriceUpdate = Get6_1PartitionQuery(ProcessConfig.Temp);
                string temp6_1Insert = @"
insert into VAT6_1(
[SerialNo],[ItemNo],[StartDateTime],[StartingQuantity],[StartingAmount],[VendorID],[SD]
,[VATRate],[Quantity],[UnitCost],[TransID],[TransType],[BENumber],[InvoiceDateTime]
,[Remarks],[CreateDateTime],[TransactionType],[BranchId],[UserId],[AvgRate]      ,[RunningTotal]
)
select 
[SerialNo],[ItemNo],[StartDateTime],[StartingQuantity],[StartingAmount],[VendorID],[SD]
,[VATRate],[Quantity],[UnitCost],[TransID],[TransType],[BENumber],[InvoiceDateTime]
,[Remarks],[CreateDateTime],[TransactionType],[BranchId],UserId,[AvgRate]     ,[RunningTotal]
from VAT6_1Temp

where 1=1
and UserId = @UserId

";
                if (!Permanent6_1)
                {
                    temp6_1Insert = "";
                }

                avgPriceUpdate = temp6_1Insert + "  " + avgPriceUpdate;
                if (!vm.FromSP)
                {
                    cmd = new SqlCommand(avgPriceUpdate, currConn, transaction);
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = avgPriceUpdate;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }
                retResults[0] = "Success";
                retResults[1] = "Successfully Process";

                return retResults;

                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Get6_1SelectText()
        {
            return @"

select VAT6_1.SerialNo
,convert (varchar,VAT6_1.StartDateTime,120)StartDateTime
,convert (varchar,VAT6_1.StartDateTime,120)Day,
VAT6_1.StartingQuantity
,VAT6_1.StartingAmount
,TransID
,VAT6_1.TransType,
isnull(v.VendorName,'-')VendorName
,isnull(v.Address1,'-')Address1,
isnull(v.Address2,'-')Address2
,isnull(v.Address3,'-')Address3,
isnull(v.VATRegistrationNo,'-')VATRegistrationNo,
p.ProductName
,p.ProductCode ProductCodeA
,p.UOM,VAT6_1.Quantity
,VAT6_1.VATRate
,VAT6_1.SD
,VAT6_1.UnitCost
,p.HSCodeNo
,VAT6_1.BENumber
,convert (varchar,VAT6_1.InvoiceDateTime,120)InvoiceDateTime
,VAT6_1.Remarks
,VAT6_1.ItemNo
,VAT6_1.CreateDateTime
,VAT6_1.AvgRate
,VAT6_1.RunningTotal
,VAT6_1.RunningValue
,isnull(VAT6_1.RunningOpeningQuantity,0)RunningOpeningQuantity
,isnull(VAT6_1.RunningOpeningValue,0)RunningOpeningValue
from VAT6_1 left outer join
Vendors as V on VAT6_1.VendorID=v.VendorID and VAT6_1.TransType  in ('Purchase') 
left outer join 
Products P on VAT6_1.ItemNo=p.ItemNo
where VAT6_1.UserId = @UserId
order by ItemNo, StartDateTime, SerialNo,TransID";
        }

        public string[] SaveVAT6_1_Permanent_Branch(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Opening

                sqlText = GetOpeningQuery(ProcessConfig.Permanent_Branch);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }
                if (!vm.FromSP)
                {

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 500;

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                #endregion

                #region Delete Existing Data

                string periodId = vm.StartDate.ToDateString("MMyyyy");
                string deleteText = @" delete from VAT6_1_Permanent_Branch where  TransType != 'Opening'  and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
     @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", "and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_1' or ReportType='VAT6_1_And_6_2') )");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }
                if (!vm.FromSP)
                {


                    cmd.CommandText = deleteText;
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }
                #endregion

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();

                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList(null, null, null, currConn, transaction, connVM);

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                vm.PermanentProcess = true;

                #region Branch wise data insert

                string insertToPermanent = GetInsertQuery(ProcessConfig.Permanent_Branch);

                string updateMasterItem = @"
update VAT6_1
set ItemNo = P.MasterProductItemNo
from Products p inner join VAT6_1 vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                }

                cmd.CommandText = insertToPermanent;

                if (!vm.FromSP)
                {


                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", periodId);
                }

                foreach (var branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;
                    retResults = VAT6_1_WithConnForSP(vm, currConn, transaction, connVM);

                    if (!vm.FromSP)
                    {
                        cmd.CommandText = updateMasterItem;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = insertToPermanent;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        vm.SPSQLText = updateMasterItem + " " + insertToPermanent;
                        vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@PeriodId", vm.PeriodId != 0 ? "'" + vm.PeriodId + "'" : "0");
                        vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");

                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();
                    }

                }

                #endregion

                string updateValues = Get6_1PartitionQuery(ProcessConfig.Permanent_Branch);


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateValues = updateValues.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                    updateValues = updateValues.Replace("@itemCondition2", " and VAT6_1_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updateValues = updateValues.Replace("@itemCondition1", "");
                    updateValues = updateValues.Replace("@itemCondition2", "");
                }
                if (!vm.FromSP)
                {


                    cmd.CommandText = updateValues;
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", periodId);
                    cmd.CommandTimeout = 500;

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = updateValues;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@PeriodId", vm.PeriodId != 0 ? "'" + vm.PeriodId + "'" : "0");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1_Permanent_Branch set AvgRate = 0,RunningValue= 0, RunningOpeningValue=0
from (
select distinct ItemNo,BranchId,min(StartDateTime)StartDateTime  from VAT6_1_Permanent_Branch 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
@itemCondition2

group by ItemNo,BranchId
) a
where VAT6_1_Permanent_Branch.ItemNo = a.ItemNo 
and  VAT6_1_Permanent_Branch.ItemNo = a.BranchId 
and VAT6_1_Permanent_Branch.StartDateTime>= a.StartDateTime
@itemCondition2

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", " and VAT6_1_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", "");
                }

                cmd.CommandText = updateNegativeValue;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {



                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("VATRegistersDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public DataSet VAT6_1_Permanent_DayWise(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet dataSet = new DataSet("ReportVAT6_1");
            #endregion

            #region Try

            try
            {
                #region Settings

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV", null, null, connVM) == "Y" ? true : false);
                bool IssueFrom6_1 = Convert.ToBoolean(_cDal.settings("Toll6_4", "IssueFrom6_1", null, null, connVM) == "Y" ? true : false);
                bool TotalIncludeSD = Convert.ToBoolean(_cDal.settings("VAT6_1", "TotalIncludeSD", null, null, connVM) == "Y" ? true : false);
                bool IncludeOtherAMT = _cDal.settings("VAT6_1", "IncludeOtherAMT", null, null, connVM) == "Y";
                bool TollReceiveNotWIP = Convert.ToBoolean(_cDal.settings("IssueFromBOM", "TollReceive-NotWIP", null, null, connVM) == "Y" ? true : false);
                bool TollReceiveWithIssue = Convert.ToBoolean(_cDal.settings("TollReceive", "WithIssue", null, null, connVM) == "Y" ? true : false);
                bool Permanent6_1 = true;//_cDal.settings("VAT6_1", "6_1Permanent") == "Y";
                //bool ContractorFGProduction = Convert.ToBoolean(_cDal.settings("ContractorFGProduction", "IssueFrom6_1") == "Y" ? true : false);

                #endregion

                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                if (string.IsNullOrEmpty(vm.UserId))
                {
                    throw new Exception("User Id Not Found");
                }

                string[] resultSave61 = { };

                bool checkProductType = CheckProductType(vm, currConn, transaction);


                if (Permanent6_1 && !vm.PermanentProcess && !checkProductType)
                {
                    if (vm.BranchWise)
                    {
                        resultSave61 = Save6_1_FromPermanent_DayWise_Branch(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);

                    }
                    else
                    {
                        resultSave61 = Save6_1_FromPermanent_DayWise(vm, TotalIncludeSD, IncludeOtherAMT, IssueFrom6_1, TollReceiveNotWIP, TollReceiveWithIssue, currConn, transaction, connVM);
                    }

                    string getMaxDate = @"select max(startdatetime) from VAT6_1_Permanent_DayWise where 1=1 ";
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        getMaxDate += " and ItemNo='" + vm.ItemNo + "'";
                    }
                    SqlCommand sqlCommand = new SqlCommand(getMaxDate, currConn, transaction);
                    sqlCommand.CommandTimeout = 500;
                    var maxDate = sqlCommand.ExecuteScalar();
                    DateTime permanent_Max = Convert.ToDateTime(maxDate == DBNull.Value ? null : maxDate);

                }

                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1 set AvgRate = 0,RunningValue= 0
, RunningOpeningValue=0
from (
select distinct ItemNo,min(StartDateTime)StartDateTime 
from VAT6_1_Permanent 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
and UserId=@UserId

group by ItemNo
) a
where VAT6_1.ItemNo = a.ItemNo and VAT6_1.StartDateTime>= a.StartDateTime
and VAT6_1.UserId=@UserId


";


                SqlCommand sqlupdateNegativeValue = new SqlCommand(updateNegativeValue, currConn, transaction);
                sqlupdateNegativeValue.CommandTimeout = 500;
                sqlupdateNegativeValue.Parameters.AddWithValue("@UserId", vm.UserId);
                sqlupdateNegativeValue.ExecuteNonQuery();


                #endregion

                #region Select Saved Data

                sqlText = Get6_1SelectText();

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@UserId", vm.UserId);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataSet);



                #endregion


                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ReportDSDAL", "VAT6_1_WithConn", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ReportDSDAL", "VAT6_1_WithConn", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

            }

            #endregion

            return dataSet;
        }

        private string[] Save6_1_FromPermanent_DayWise_Branch(VAT6_1ParamVM vm, bool TotalIncludeSD, bool IncludeOtherAMT, bool IssueFrom6_1,
        bool TollReceiveNotWIP, bool TollReceiveWithIssue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText;

                CommonDAL commonDal = new CommonDAL();
                bool Permanent6_1 = commonDal.settings("VAT6_1", "6_1Permanent", currConn, transaction, connVM) == "Y";

                #region SQL Text

                sqlText = "";


                #region Select Product


                sqlText += @"  

create table #ProductReceive(ItemNo varchar(50))

insert into #ProductReceive(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
--and Products.BranchId=@BranchId
 

";

                #region Conditions
                if (vm.Is6_1Permanent)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_1')";
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    string code = new CommonDAL().settingValue("CompanyCode", "Code");
                    if (code.ToLower() == "cp")
                    {
                        sqlText += @"  and Products.BranchId=@BranchId";
                    }
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";

                        #region Debugging

                        ////if (true)
                        ////{
                        ////    sqlText += @"  and ItemNo='73'";
                        ////}

                        #endregion
                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                    }
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  ) as a";

                sqlText += @"

delete from VAT6_1 where UserId = @UserId


insert into VAT6_1(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[RunningTotal]
,RunningValue
)

SELECT 
      [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,@UserId
      ,[AvgRate]
      ,[RunningTotal]

,RunningValue

from   ( 

select * from VAT6_1_Permanent_DayWise_Branch where
 TransType in  ('Opening','MonthOpening')
and ItemNo in ( select distinct ItemNo from #ProductReceive)
and BranchId = @BranchId

union all

select * from VAT6_1_Permanent_DayWise_Branch where
 TransType not in  ('Opening','MonthOpening')
and ItemNo in (select distinct ItemNo from #ProductReceive)
and  StartDateTime >= @StartDate 
and StartDateTime < DATEADD(d,1,@EndDate) 
and BranchId = @BranchId
 ) as a
order by ItemNo, StartDateTime, SerialNo


update VAT6_1 set  Quantity=a.ClosingQty,UnitCost=a.ClosingValue, RunningTotal=a.ClosingQty,RunningValue=a.ClosingValue, StartDateTime = @StartDate,AvgRate=a.AvgRate
from  (
select VAT6_1_Permanent_DayWise_Branch.Id,VAT6_1_Permanent_DayWise_Branch.ItemNo,RunningTotal ClosingQty,RunningValue ClosingValue, AvgRate

from VAT6_1_Permanent_DayWise_Branch
right outer join (
select distinct ItemNo,BranchId, MAX(Id)Id from VAT6_1_Permanent_DayWise_Branch
where StartDateTime <@StartDate and ItemNo in (select distinct ItemNo from #ProductReceive) and BranchId = @BranchId
group by ItemNo,BranchId ) as a
on   a.Id=VAT6_1_Permanent_DayWise_Branch.ID and a.BranchId=VAT6_1_Permanent_DayWise_Branch.BranchId 
where VAT6_1_Permanent_DayWise_Branch.BranchId=@BranchId
) as a
where a.ItemNo=VAT6_1.ItemNo and VAT6_1.TransType='Opening' and UserId = @UserId


update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.TransID=ProductAvgPrice.PurchaseNo
 and VAT6_1.UserId = @UserId

update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.StartDateTime >= isnull(ProductAvgPrice.FromDate,ProductAvgPrice.AgvPriceDate) 
and VAT6_1.StartDateTime< DATEADD(d,1, ProductAvgPrice.AgvPriceDate) 
and VAT6_1.TransType in('Opening','Issue')
 and VAT6_1.UserId = @UserId

update VAT6_1 set AvgRate=a.AvgPrice
from(
select ProductAvgPrice.ItemNo,AvgPrice from ProductAvgPrice
right outer join (
select distinct ItemNo,max(sl)SL from ProductAvgPrice 
where 
ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
group by ItemNo)as a
on a.ItemNo=ProductAvgPrice.ItemNo and a.Sl=ProductAvgPrice.sl) as a
where a.ItemNo=VAT6_1.ItemNo
and  VAT6_1.ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
and VAT6_1.UserId = @UserId


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9),BranchId int)
insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost)
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_1
where UserId = @UserId and BranchId=@BranchId
order by ItemNo,StartDateTime,SerialNo


update VAT6_1 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_1.Id 
and VAT6_1.UserId = @UserId


--update VAT6_1 set  RunningValue=RT.RunningValue
--from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
--SUM (case when TransType in('Issue') then -1*UnitCost else UnitCost end ) 
--OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningValue
--FROM #Temp)RT
--where RT.Id=VAT6_1.Id 
--and VAT6_1.UserId = @UserId
--and VAT6_1.TransType not in('Opening')

--Update VAT6_1 set RunningValue = AvgRate*RunningTotal where VAT6_1.UserId = @UserId

--update VAT6_1 set  RunningOpeningQuantity=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningTotal) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId
--
-- update VAT6_1 set  RunningOpeningValue=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningValue) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId


drop table #Temp

update VAT6_1 set RunningOpeningQuantity = Quantity,RunningOpeningValue = UnitCost
where TransType = 'Opening' and RunningOpeningQuantity is null and RunningOpeningValue is null and UserId = @UserId

select * from VAT6_1 where UserId = @UserId
order by ItemNo, StartDateTime, SerialNo



";

                #endregion

                #endregion


                #endregion

                #region SQL Command

                SqlCommand objCommVAT16 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT16.CommandTimeout = 500;
                objCommVAT16.Parameters.AddWithValue("@UserId", vm.UserId);
                objCommVAT16.Parameters.AddWithValue("@BranchId", vm.BranchId);

                #region Parameter

                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    objCommVAT16.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        objCommVAT16.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                    }
                    else
                    {
                        objCommVAT16.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    objCommVAT16.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                if (vm.StartDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = vm.StartDate;
                    }
                } // Common Filed

                if (vm.EndDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = vm.EndDate;
                    }
                }

                #endregion Parameter

                objCommVAT16.ExecuteNonQuery();

                string[] result = { "success" };

                return result;

                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string[] Save6_1_FromPermanent_DayWise(VAT6_1ParamVM vm, bool TotalIncludeSD, bool IncludeOtherAMT, bool IssueFrom6_1,
         bool TollReceiveNotWIP, bool TollReceiveWithIssue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                DataSet dataSet = new DataSet();
                string sqlText;


                CommonDAL commonDal = new CommonDAL();
                bool Permanent6_1 = commonDal.settings("VAT6_1", "6_1Permanent", currConn, transaction, connVM) == "Y";

                #region SQL Text

                sqlText = "";


                #region Select Product


                sqlText += @"  

create table #ProductReceive(ItemNo varchar(50))

insert into #ProductReceive(ItemNo)
select ItemNo  from   ( 
select Products.ItemNo from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
where 1=1
--and Products.BranchId=@BranchId
 

";

                #region Conditions
                if (vm.Is6_1Permanent)
                {
                    sqlText += @"  and Products.ReportType in('VAT6_1')";
                }
                else if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    string code = new CommonDAL().settingValue("CompanyCode", "Code");
                    if (code.ToLower() == "cp")
                    {
                        sqlText += @"  and Products.BranchId=@BranchId";
                    }
                    if (vm.Flag == "SCBL")
                    {
                        sqlText += @"  and IsRaw in('Raw','Pack')";

                        #region Debugging

                        #endregion
                    }
                    else
                    {
                        sqlText += @"  and IsRaw=@ProdutType";
                    }
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        sqlText += @"  and Products.ProductName LIKE '%' +  @ProdutGroupName   + '%'";
                    }
                    else
                    {
                        sqlText += @"  and Products.CategoryID=@ProdutCategoryId";
                        sqlText += @"  and Products.ActiveStatus='Y'";
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    sqlText += @"  and ItemNo=@ItemNo";
                }

                sqlText += @"  ) as a";

                sqlText += @"

delete from VAT6_1 where UserId = @UserId


insert into VAT6_1(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[RunningTotal]
,RunningValue
)

SELECT 
      [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,@UserId
      ,[AvgRate]
      ,[RunningTotal]

,RunningValue

from   ( 

select * from VAT6_1_Permanent_DayWise where
 TransType in  ('Opening','MonthOpening')
and ItemNo in ( select distinct ItemNo from #ProductReceive)

union all

select * from VAT6_1_Permanent_DayWise where
 TransType not in ('Opening','MonthOpening')
and ItemNo in (select distinct ItemNo from #ProductReceive)
and  StartDateTime >= @StartDate and 
StartDateTime < DATEADD(d,1,@EndDate) 

 ) as a
order by ItemNo, StartDateTime, SerialNo


update VAT6_1 set  Quantity=a.ClosingQty
,UnitCost=a.ClosingValue
, RunningTotal=a.ClosingQty
,RunningValue=a.ClosingValue
, StartDateTime = @StartDate
, AvgRate = a.AvgRate
from  (
select VAT6_1_Permanent_DayWise.Id,VAT6_1_Permanent_DayWise.ItemNo,RunningTotal ClosingQty,RunningValue ClosingValue, AvgRate

from VAT6_1_Permanent_DayWise
right outer join (
select distinct ItemNo, MAX(Id)Id from VAT6_1_Permanent_DayWise
where StartDateTime <@StartDate and ItemNo in (select distinct ItemNo from #ProductReceive)
group by ItemNo) as a
on   a.Id=VAT6_1_Permanent_DayWise.ID) as a
where a.ItemNo=VAT6_1.ItemNo and VAT6_1.TransType='Opening' and UserId = @UserId


update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.TransID=ProductAvgPrice.PurchaseNo
 and VAT6_1.UserId = @UserId

update VAT6_1 set  AvgRate=ProductAvgPrice.AvgPrice
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT6_1.ItemNo
and VAT6_1.StartDateTime >= isnull(ProductAvgPrice.FromDate,ProductAvgPrice.AgvPriceDate) 
and VAT6_1.StartDateTime< DATEADD(d,1, ProductAvgPrice.AgvPriceDate) 
and VAT6_1.TransType in('Opening','Issue')
 and VAT6_1.UserId = @UserId

update VAT6_1 set AvgRate=a.AvgPrice
from(
select ProductAvgPrice.ItemNo,AvgPrice from ProductAvgPrice
right outer join (
select distinct ItemNo,max(sl)SL from ProductAvgPrice 
where 
ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
group by ItemNo)as a
on a.ItemNo=ProductAvgPrice.ItemNo and a.Sl=ProductAvgPrice.sl) as a
where a.ItemNo=VAT6_1.ItemNo
and  VAT6_1.ItemNo in(select ItemNo from VAT6_1 
where AvgRate is null and UserId = @UserId)
and VAT6_1.UserId = @UserId


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9),)
insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost)
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_1
where UserId = @UserId 
order by ItemNo,StartDateTime,SerialNo


update VAT6_1 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_1.Id and VAT6_1.UserId = @UserId

--update VAT6_1 set  RunningValue=RT.RunningValue
--from (SELECT id,SL, ItemNo, TransType ,Quantity,
--SUM (case when TransType in('Issue') then -1*UnitCost else UnitCost end ) 
--OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningValue
--FROM #Temp)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId = @UserId
--and VAT6_1.TransType not in('Opening')

--Update VAT6_1 set RunningValue = AvgRate*RunningTotal where VAT6_1.UserId = @UserId

--update VAT6_1 set  RunningOpeningQuantity=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningTotal) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId
--
-- update VAT6_1 set  RunningOpeningValue=RT.RunningTotalV
--from ( SELECT  Id,
--LAG(RunningValue) 
--OVER (PARTITION BY ItemNo ORDER BY   itemno,StartDateTime,SerialNo) AS RunningTotalV
--FROM VAT6_1
--where  VAT6_1.UserId=@UserId
--)RT
--where RT.Id=VAT6_1.Id and VAT6_1.UserId=@UserId


drop table #Temp

update VAT6_1 set RunningOpeningQuantity = Quantity,RunningOpeningValue = UnitCost
where TransType = 'Opening' and RunningOpeningQuantity is null and RunningOpeningValue is null and UserId = @UserId

select * from VAT6_1 where UserId = @UserId
order by ItemNo, StartDateTime, SerialNo



";

                #endregion

                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT16 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT16.CommandTimeout = 500;
                objCommVAT16.Parameters.AddWithValue("@UserId", vm.UserId);

                #region Parameter

                if (!string.IsNullOrWhiteSpace(vm.ProdutType))
                {
                    objCommVAT16.Parameters.AddWithValue("@ProdutType", vm.ProdutType);
                }

                else if (!string.IsNullOrWhiteSpace(vm.ProdutCategoryId))
                {
                    if (vm.ProdutCategoryLike == true)
                    {
                        objCommVAT16.Parameters.AddWithValue("@ProdutGroupName", vm.ProdutGroupName);
                    }
                    else
                    {
                        objCommVAT16.Parameters.AddWithValue("@ProdutCategoryId", vm.ProdutCategoryId);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    objCommVAT16.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                if (vm.StartDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = vm.StartDate;
                    }
                } // Common Filed

                if (vm.EndDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@EndDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@EndDate"].Value = vm.EndDate;
                    }
                }

                #endregion Parameter

                objCommVAT16.ExecuteNonQuery();

                string[] result = { "success" };

                return result;

                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #endregion

        public string[] SaveVAT6_2_Permanent_Stored(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                    transaction.Commit();
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                    transaction.Commit();
                }
                #endregion open connection and transaction

                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

--SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
--FROM MASTER..SysProcesses
--WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId
--
----SELECT @SQL 
--EXEC(@SQL)

-- IF EXISTS
         -- DROP DATABASE [VAT_Process]

 IF  EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
 BEGIN
                         DROP DATABASE [VAT_Process]
 END

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(18, 6) NULL,
                    Quantity        DECIMAL(18, 6) NULL,
                    VATRate         DECIMAL(18, 6) NULL,
                    SD              DECIMAL(18, 6) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(18, 6),
                    AdjustmentValue DECIMAL(18, 6),
                    BranchId int null
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](18, 6)    NULL,
                    [StartingAmount]              [decimal](18, 6)    NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](18, 6)     NULL,
                    [VATRate]                     [decimal](18, 6)     NULL,
                    [SD]                          [decimal](18, 6)     NULL,
                    [UnitCost]                    [decimal](18, 6)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](25, 9)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](18, 6)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](18, 6)    NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 6)     NULL,
                    [RunningTotalValue]           [decimal](18, 6)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 6)     NULL,
                    [DeclaredPrice]               [decimal](18, 6)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 6)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 6)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();

                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();

                #region Opening

                sqlText = "spInsertOpening";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();


                string openingUpdate = @"
update VAT6_2_Permanent
set RunningTotal = Quantity,
RunningTotalValue = UnitCost,
RunningOpeningValueFinal = UnitCost,
DeclaredPrice = UnitCost/(Case when Quantity = 0 then 1 else Quantity end)
where VAT6_2_Permanent.TransType = 'Opening'
and isnull(VAT6_2_Permanent.RunningTotal,0) = 0";

                cmd.CommandText = openingUpdate;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                cmd.CommandType = CommandType.StoredProcedure;

                #endregion

                foreach (DateTime dateTime in OrdinaryVATDesktop.EachDay(Convert.ToDateTime(vm.StartDate),
                             Convert.ToDateTime(vm.EndDate)))
                {

                    cmd.Parameters.Clear();

                    #region Delete Existing Data

                    string deleteText = @"spDelete6_2";

                    cmd.CommandText = deleteText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);

                    }
                    else if (vm.FilterProcessItems)
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@FilteredItems", vm.FilterProcessItems ? "Y" : "N");
                    }

                    cmd.ExecuteNonQuery();


                    cmd.CommandType = CommandType.StoredProcedure;

                    #endregion

                    DateTime paramMonth = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));

                    vm.PermanentProcess = true;

                    cmd.Parameters.Clear();

                    cmd.CommandText = "spVAT6_2";
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    cmd.Parameters.AddWithValueAndParamCheck("@Opening", vm.Opening);
                    cmd.Parameters.AddWithValueAndParamCheck("@VAT6_2_1", vm.VAT6_2_1);
                    cmd.Parameters.AddWithValueAndParamCheck("@stockMovement", vm.StockMovement);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    cmd.ExecuteNonQuery();

                    #region Order By Insert

                    sqlText = @"    

--INSERT INTO VAT_17_1(SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
--                         CustomerID, Quantity, UnitCost, TransID, TransType, VATRate, SD, remarks, CreatedDateTime,
--                         UnitRate, AdjustmentValue)

delete from [VAT_Process].[dbo].VAT6_2_Permanent

INSERT INTO [VAT_Process].[dbo].VAT6_2_Permanent(SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
                       CustomerID, Quantity, UnitCost, TransID, TransType, VATRate, SD, remarks, CreatedDateTime,
                       UnitRate, AdjustmentValue, UserId, BranchId)
select * from (
SELECT   distinct 'A' SerialNo,
           ItemNo,
           @StartDate Dailydate,
           0 StartingQuantity,
           0 StartingAmount,
           0 CustomerID,
           0 Quantity,
           0 UnitCost,
           '' TransID,
           'MonthOpening' TransType,
           0 VATRate,
           0 SD,
           'MonthOpening' remarks,
           @StartDate CreatedDateTime,
           0 UnitRate,
           0 AdjustmentValue
            ,@UserId UserId
            ,@BranchId BranchId
from [VAT_Process].[dbo].VATTemp_17_1 

union all


    SELECT SerialNo,
           ItemNo,
           Dailydate,
           0 StartingQuantity,
           0 StartingAmount,
           0 CustomerID,
           Quantity,
           UnitCost,
           TransID,
           TransType,
           VATRate,
           SD,
           remarks,
           CreatedDateTime,
           UnitRate,
           AdjustmentValue,
           @UserId UserId,
           @BranchId BranchId
    FROM [VAT_Process].[dbo].VATTemp_17_1

             --OFFSET CAST(@divisor AS INT) * @iterator ROWS FETCH NEXT CAST(@divisor AS INT) ROWS ONLY

) as VAT6_2
    ORDER BY ItemNo, dailydate, SerialNo
    OPTION (OPTIMIZE FOR UNKNOWN)


      UPDATE [VAT_Process].[dbo].VAT6_2_Permanent SET StartDateTime=@StartDate WHERE SerialNo = 'A'
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET CustomerID=SalesInvoiceHeaders.CustomerID,
        SerialId  = SalesInvoiceHeaders.Id
    FROM SalesInvoiceHeaders
    WHERE SalesInvoiceHeaders.SalesInvoiceNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND [VAT_Process].[dbo].VAT6_2_Permanent.TransType = 'Sale'
      AND (SalesInvoiceHeaders.Post = @post1 OR SalesInvoiceHeaders.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET CustomerID=TransferIssueDetails.TransferTo,
        SerialId  = TransferIssueDetails.Id
    FROM TransferIssueDetails
    WHERE TransferIssueDetails.TransferIssueNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND [VAT_Process].[dbo].VAT6_2_Permanent.Remarks = 'Transfer Issue'
      AND (TransferIssueDetails.Post = @post1 OR TransferIssueDetails.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = ReceiveHeaders.Id
    FROM ReceiveHeaders
    WHERE ReceiveHeaders.ReceiveNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND [VAT_Process].[dbo].VAT6_2_Permanent.TransType = 'Receive'
      AND (ReceiveHeaders.Post = @post1 OR ReceiveHeaders.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = ProductTransfers.Id
    FROM ProductTransfers
    WHERE ProductTransfers.TransferCode = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (ProductTransfers.Post = @post1 OR ProductTransfers.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = TransferReceives.Id
    FROM TransferReceives
    WHERE TransferReceives.TransferReceiveNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (TransferReceives.Post = @post1 OR TransferReceives.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = PurchaseInvoiceHeaders.Id
    FROM PurchaseInvoiceHeaders
    WHERE PurchaseInvoiceHeaders.PurchaseInvoiceNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (PurchaseInvoiceHeaders.Post = @post1 OR PurchaseInvoiceHeaders.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = TransferIssues.Id
    FROM TransferIssues
    WHERE TransferIssues.TransferIssueNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (TransferIssues.Post = @post1 OR TransferIssues.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = DisposeFinishs.Id
    FROM DisposeFinishs
    WHERE DisposeFinishs.DisposeNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (DisposeFinishs.Post = @post1 OR DisposeFinishs.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET [VAT_Process].[dbo].VAT6_2_Permanent.ProductDesc=Products.productName
    FROM Products
    WHERE Products.itemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET [VAT_Process].[dbo].VAT6_2_Permanent.ProductDesc=SalesInvoiceDetails.ProductDescription
    FROM SalesInvoiceDetails
    WHERE SalesInvoiceDetails.itemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
      AND SalesInvoiceDetails.SalesInvoiceNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
";


                    #endregion

                    cmd.Parameters.Clear();

                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@post1", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@post2", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", 0);
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    cmd.ExecuteNonQuery();

                    // Partiton
                    #region Partition Query

                    string insertToPermanent = @"


update [VAT_Process].[dbo].VAT6_2_Permanent
set ItemNo = P.MasterProductItemNo
from Products p inner join [VAT_Process].[dbo].VAT6_2_Permanent vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0


UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET
    Quantity=a.ClosingQty,
    UnitCost=a.RunningTotalValue,
    RunningTotal=a.ClosingQty,
    RunningTotalValue=a.ClosingValue,
    RunningTotalValueFinal =a.ClosingValue,
    DeclaredPrice=a.DeclaredPrice,
    UnitRate=a.UnitRate,
    AdjustmentValue=0

FROM (
SELECT VAT6_2_Permanent.Id,
             VAT6_2_Permanent.ItemNo,
             RunningTotal           ClosingQty,
             RunningTotalValueFinal ClosingValue,
             DeclaredPrice,
             UnitRate,
             RunningTotalValue

      FROM VAT6_2_Permanent
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo, MAX(Id) Id
                                 FROM VAT6_2_Permanent
                                 WHERE StartDateTime < @StartDate
                                 GROUP BY ItemNo) AS a
                                ON a.Id = VAT6_2_Permanent.ID) AS a
WHERE a.ItemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
  AND [VAT_Process].[dbo].VAT6_2_Permanent.TransType = 'MonthOpening'


CREATE TABLE #NBRPrive
(
    id         INT IDENTITY (1,1),
    ItemNo     VARCHAR(100),
    CustomerId VARCHAR(100),
    Rate       DECIMAL(18, 6),
    EffectDate DATETIME,
    ToDate     DATETIME
)
CREATE TABLE #Temp
(
    SL        INT IDENTITY (1,1),
    Id        INT,
    ItemNo    VARCHAR(100),
    TransType VARCHAR(100),
    Quantity  DECIMAL(18, 6),
    TotalCost DECIMAL(18,6 )
,StartDatetime datetime
,SerialNo varchar(10)
)

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET CustomerID=ReceiveHeaders.CustomerID
FROM ReceiveHeaders
WHERE ReceiveHeaders.ReceiveNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID @itemCondition3

INSERT INTO #NBRPrive
SELECT itemNo,
       ''           CustomerId,
       (
           CASE
               WHEN NBRPrice = 0 THEN (CASE WHEN OpeningBalance = 0 THEN 0 ELSE OpeningTotalCost / OpeningBalance END)
               ELSE ISNULL(NBRPrice, 0)
               END
           )        NBRPrice,
       '1900/01/01' EffectDate,
       NULL         ToDate
FROM products
WHERE ItemNo IN (SELECT DISTINCT Itemno FROM [VAT_Process].[dbo].VAT6_2_Permanent WHERE 1 = 1 @itemCondition3)

INSERT INTO #NBRPrive
SELECT FinishItemNo, customerId, ISNULL(NBRPrice, 0) NBRPrice, EffectDate EffectDate, NULL ToDate
FROM BOMs
WHERE FinishItemNo IN (SELECT DISTINCT Itemno FROM [VAT_Process].[dbo].VAT6_2_Permanent WHERE 1 = 1 @itemCondition3)
  AND Post = 'Y'
ORDER BY EffectDate

UPDATE #NBRPrive
SET ToDate=NULL
WHERE 1 = 1 @itemCondition2


UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             Customerid,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE customerid > 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND RT.Customerid = #NBRPrive.Customerid
  AND ToDate IS NULL @itemCondition2

UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE ISNULL(NULLIF(customerid, ''), 0) <= 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND ISNULL(NULLIF(customerid, ''), 0) <= 0
  AND ToDate IS NULL @itemCondition2


UPDATE #NBRPrive
SET ToDate='2199/12/31'
WHERE ToDate IS NULL @itemCondition2

INSERT INTO #Temp(Id, ItemNo, TransType, Quantity, TotalCost,StartDateTime,SerialNo)
SELECT Id, ItemNo, TransType, Quantity, UnitCost,StartDateTime,SerialNo
FROM [VAT_Process].[dbo].VAT6_2_Permanent
WHERE 1 = 1  
@itemCondition3
ORDER BY ItemNo, StartDateTime, SerialNo,Id

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET RunningTotal=RT.RunningTotal
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * Quantity ELSE Quantity END)
                 OVER (PARTITION BY ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
      FROM #Temp) RT
WHERE RT.Id = [VAT_Process].[dbo].VAT6_2_Permanent.Id @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET RunningTotalValue=RT.RunningTotalCost
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * TotalCost ELSE TotalCost END)
                 OVER (PARTITION BY ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
      FROM #Temp) RT
WHERE RT.Id = [VAT_Process].[dbo].VAT6_2_Permanent.Id @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET DeclaredPrice =0

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent.ItemNo
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime >= #NBRPrive.EffectDate
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime < #NBRPrive.ToDate
  AND [VAT_Process].[dbo].VAT6_2_Permanent.CustomerID = #NBRPrive.CustomerId
  AND ISNULL([VAT_Process].[dbo].VAT6_2_Permanent.DeclaredPrice, 0) = 0 @itemCondition3


UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime >= #NBRPrive.EffectDate
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime < #NBRPrive.ToDate
  AND ISNULL([VAT_Process].[dbo].VAT6_2_Permanent.DeclaredPrice, 0) = 0 @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET AdjustmentValue=0
WHERE 1 = 1 @itemCondition3
UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET RunningTotalValueFinal= DeclaredPrice * RunningTotal
WHERE 1 = 1 @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET AdjustmentValue= RunningTotalValue - RunningTotalValueFinal
WHERE 1 = 1 @itemCondition3

DROP TABLE #Temp

DROP TABLE #NBRPrive


";
                    #endregion

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo = @ItemNo");
                    }
                    else
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                    }


                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.CommandText = insertToPermanent;
                    cmd.ExecuteNonQuery();


                    // insert back to main db
                    #region insertBackToMain
                    string insertBackToMain = @"

INSERT INTO VAT6_2_Permanent( [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal])

Select 
 [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
from [VAT_Process].[dbo].VAT6_2_Permanent
where [VAT_Process].[dbo].VAT6_2_Permanent.TransType != 'MonthOpening'
Order by  ItemNo, StartDateTime,SerialNo


";
                    #endregion insertBackToMain

                    cmd.CommandText = insertBackToMain;
                    cmd.ExecuteNonQuery();

                }


                #region Comments

                //                sqlText = @"
                //
                //INSERT INTO VAT6_2_Permanent(SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
                //                       CustomerID, Quantity, UnitCost, TransID, TransType, VATRate, SD, remarks, CreatedDateTime,
                //                       UnitRate, AdjustmentValue, UserId, BranchId)
                //    SELECT SerialNo,
                //           ItemNo,
                //           StartDateTime,
                //           StartingQuantity,
                //           StartingAmount,
                //           CustomerID,
                //           Quantity,
                //           UnitCost,
                //           TransID,
                //           TransType,
                //           VATRate,
                //           SD,
                //           remarks,
                //           CreatedDateTime,
                //           UnitRate,
                //           AdjustmentValue,
                //           @UserId,
                //           @BranchId
                //    FROM VAT_17_1
                //    ORDER BY ItemNo, StartDateTime, SerialId
                //";

                //                cmd.CommandText = sqlText;
                //                cmd.CommandType = CommandType.Text;
                //                cmd.ExecuteNonQuery();


                #endregion

                #region Commit
                //if (Vtransaction == null)
                //{
                //    if (transaction != null)
                //    {
                //        transaction.Commit();
                //    }
                //}
                #endregion Commit

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_2_Permanent_Stored_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                    transaction.Commit();
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                    transaction.Commit();
                }
                #endregion open connection and transaction


                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
--EXEC(@SQL)



 IF  EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                                 DROP DATABASE [VAT_Process]

                    END


            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(18, 6) NULL,
                    Quantity        DECIMAL(18, 6) NULL,
                    VATRate         DECIMAL(18, 6) NULL,
                    SD              DECIMAL(18, 6) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(18, 6),
                    AdjustmentValue DECIMAL(18, 6),
                    BranchId int null
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](18, 6)     NULL,
                    [StartingAmount]              [decimal](18, 6)     NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](18, 6)     NULL,
                    [VATRate]                     [decimal](18, 6)     NULL,
                    [SD]                          [decimal](18, 6)     NULL,
                    [UnitCost]                    [decimal](18, 6)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](18, 6)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](18, 6)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](18, 6)     NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 6)     NULL,
                    [RunningTotalValue]           [decimal](18, 6)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 6)     NULL,
                    [DeclaredPrice]               [decimal](18, 6)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 6)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 6)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();

                #region Opening

                sqlText = "spInsertOpeningBranch";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();


                string openingUpdate = @"
update VAT6_2_Permanent_Branch
set RunningTotal = Quantity,
RunningTotalValue = UnitCost,
RunningOpeningValueFinal = UnitCost,
DeclaredPrice = UnitCost/(Case when Quantity = 0 then 1 else Quantity end)
where VAT6_2_Permanent_Branch.TransType = 'Opening'
and isnull(VAT6_2_Permanent_Branch.RunningTotal,0) = 0
";

                cmd.CommandText = openingUpdate;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                cmd.CommandType = CommandType.StoredProcedure;

                #endregion

                foreach (DateTime dateTime in OrdinaryVATDesktop.EachDay(Convert.ToDateTime(vm.StartDate),
                             Convert.ToDateTime(vm.EndDate)))
                {

                    cmd.Parameters.Clear();

                    #region Delete Existing Data

                    string deleteText = @"spDelete6_2_Branch";

                    cmd.CommandText = deleteText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);

                    }
                    else if (vm.FilterProcessItems)
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@FilteredItems", vm.FilterProcessItems ? "Y" : "N");
                    }

                    cmd.ExecuteNonQuery();

                    cmd.CommandType = CommandType.StoredProcedure;

                    #endregion

                    DateTime paramMonth = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));

                    vm.PermanentProcess = true;

                    cmd.Parameters.Clear();

                    cmd.CommandText = "spVAT6_2";
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    cmd.Parameters.AddWithValueAndParamCheck("@Opening", vm.Opening);
                    cmd.Parameters.AddWithValueAndParamCheck("@VAT6_2_1", vm.VAT6_2_1);
                    cmd.Parameters.AddWithValueAndParamCheck("@stockMovement", vm.StockMovement);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", -1);

                    cmd.ExecuteNonQuery();

                    #region Order By Insert

                    sqlText = @"    

--INSERT INTO VAT_17_1(SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
--                         CustomerID, Quantity, UnitCost, TransID, TransType, VATRate, SD, remarks, CreatedDateTime,
--                         UnitRate, AdjustmentValue)

--update [VAT_Process].dbo.VATTemp_17_1
--set BranchId = SalesInvoiceDetails.BranchId
--from
--SalesInvoiceDetails
--where SalesInvoiceDetails.SalesInvoiceNo = VAT_Process.[dbo].VATTemp_17_1.TransID
--and  isnull(VAT_Process.[dbo].VATTemp_17_1.BranchId,0) = 0
--
--update VAT_Process.dbo.VATTemp_17_1
--set BranchId = ReceiveDetails.BranchId
--from
--ReceiveDetails
--where ReceiveDetails.ReceiveNo = VAT_Process.[dbo].VATTemp_17_1.TransID
--and isnull(VAT_Process.[dbo].VATTemp_17_1.BranchId,0) = 0


delete from [VAT_Process].[dbo].VAT6_2_Permanent

INSERT INTO [VAT_Process].[dbo].VAT6_2_Permanent(SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
                       CustomerID, Quantity, UnitCost, TransID, TransType, VATRate, SD, remarks, CreatedDateTime,
                       UnitRate, AdjustmentValue, UserId, BranchId)
select * from (
SELECT   distinct 'A' SerialNo,
           ItemNo,
           @StartDate Dailydate,
           0 StartingQuantity,
           0 StartingAmount,
           0 CustomerID,
           0 Quantity,
           0 UnitCost,
           '' TransID,
           'MonthOpening' TransType,
           0 VATRate,
           0 SD,
           'MonthOpening' remarks,
           @StartDate CreatedDateTime,
           0 UnitRate,
           0 AdjustmentValue
            ,@UserId UserId
            ,BranchId
from [VAT_Process].[dbo].VATTemp_17_1 

union all


    SELECT SerialNo,
           ItemNo,
           Dailydate,
           0 StartingQuantity,
           0 StartingAmount,
           0 CustomerID,
           Quantity,
           UnitCost,
           TransID,
           TransType,
           VATRate,
           SD,
           remarks,
           CreatedDateTime,
           UnitRate,
           AdjustmentValue,
           @UserId UserId,
           BranchId
    FROM [VAT_Process].[dbo].VATTemp_17_1

             --OFFSET CAST(@divisor AS INT) * @iterator ROWS FETCH NEXT CAST(@divisor AS INT) ROWS ONLY

) as VAT6_2
    ORDER BY ItemNo, dailydate, SerialNo
    OPTION (OPTIMIZE FOR UNKNOWN)


      UPDATE [VAT_Process].[dbo].VAT6_2_Permanent SET StartDateTime=@StartDate WHERE SerialNo = 'A'
    
update [VAT_Process].[dbo].VAT6_2_Permanent
set ItemNo = P.MasterProductItemNo
from Products p inner join [VAT_Process].[dbo].VAT6_2_Permanent vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0

    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET CustomerID=SalesInvoiceHeaders.CustomerID,
        SerialId  = SalesInvoiceHeaders.Id
    FROM SalesInvoiceHeaders
    WHERE SalesInvoiceHeaders.SalesInvoiceNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND [VAT_Process].[dbo].VAT6_2_Permanent.TransType = 'Sale'
      AND (SalesInvoiceHeaders.Post = @post1 OR SalesInvoiceHeaders.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET CustomerID=TransferIssueDetails.TransferTo,
        SerialId  = TransferIssueDetails.Id
    FROM TransferIssueDetails
    WHERE TransferIssueDetails.TransferIssueNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND [VAT_Process].[dbo].VAT6_2_Permanent.Remarks = 'Transfer Issue'
      AND (TransferIssueDetails.Post = @post1 OR TransferIssueDetails.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = ReceiveHeaders.Id
    FROM ReceiveHeaders
    WHERE ReceiveHeaders.ReceiveNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND [VAT_Process].[dbo].VAT6_2_Permanent.TransType = 'Receive'
      AND (ReceiveHeaders.Post = @post1 OR ReceiveHeaders.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = ProductTransfers.Id
    FROM ProductTransfers
    WHERE ProductTransfers.TransferCode = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (ProductTransfers.Post = @post1 OR ProductTransfers.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = TransferReceives.Id
    FROM TransferReceives
    WHERE TransferReceives.TransferReceiveNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (TransferReceives.Post = @post1 OR TransferReceives.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = PurchaseInvoiceHeaders.Id
    FROM PurchaseInvoiceHeaders
    WHERE PurchaseInvoiceHeaders.PurchaseInvoiceNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (PurchaseInvoiceHeaders.Post = @post1 OR PurchaseInvoiceHeaders.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = TransferIssues.Id
    FROM TransferIssues
    WHERE TransferIssues.TransferIssueNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (TransferIssues.Post = @post1 OR TransferIssues.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET SerialId = DisposeFinishs.Id
    FROM DisposeFinishs
    WHERE DisposeFinishs.DisposeNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID
      AND (DisposeFinishs.Post = @post1 OR DisposeFinishs.Post = @post2)
      AND [VAT_Process].[dbo].VAT6_2_Permanent.BranchId > @BranchId
      AND SerialId IS NULL
    
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET [VAT_Process].[dbo].VAT6_2_Permanent.ProductDesc=Products.productName
    FROM Products
    WHERE Products.itemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
    
    UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
    SET [VAT_Process].[dbo].VAT6_2_Permanent.ProductDesc=SalesInvoiceDetails.ProductDescription
    FROM SalesInvoiceDetails
    WHERE SalesInvoiceDetails.itemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
      AND SalesInvoiceDetails.SalesInvoiceNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID



";


                    #endregion

                    cmd.Parameters.Clear();

                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@post1", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@post2", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", 0);
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    cmd.ExecuteNonQuery();

                    // Partiton
                    #region Partition Query

                    string insertToPermanent = @"

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET
    Quantity=a.ClosingQty,
    UnitCost=a.RunningTotalValue,
    RunningTotal=a.ClosingQty,
    RunningTotalValue=a.ClosingValue,
    RunningTotalValueFinal =a.ClosingValue,
    DeclaredPrice=a.DeclaredPrice,
    UnitRate=a.UnitRate,
    AdjustmentValue=0

FROM (
SELECT VAT6_2_Permanent_Branch.Id,
             VAT6_2_Permanent_Branch.ItemNo,
             VAT6_2_Permanent_Branch.BranchId,
             RunningTotal           ClosingQty,
             RunningTotalValueFinal ClosingValue,
             DeclaredPrice,
             UnitRate,
             RunningTotalValue

      FROM VAT6_2_Permanent_Branch
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo,BranchId, MAX(Id) Id
                                 FROM VAT6_2_Permanent_Branch
                                 WHERE StartDateTime < @StartDate
                                 GROUP BY ItemNo,BranchId) AS a
                                ON a.Id = VAT6_2_Permanent_Branch.ID
								
								) AS a
WHERE 
a.ItemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
and a.BranchId = [VAT_Process].[dbo].VAT6_2_Permanent.BranchId
  AND [VAT_Process].[dbo].VAT6_2_Permanent.TransType = 'MonthOpening'


CREATE TABLE #NBRPrive
(
    id         INT IDENTITY (1,1),
    ItemNo     VARCHAR(100),
    CustomerId VARCHAR(100),
    Rate       DECIMAL(18, 6),
    EffectDate DATETIME,
    ToDate     DATETIME
    ,BranchId int
)
CREATE TABLE #Temp
(
    SL        INT IDENTITY (1,1),
    Id        INT,
    ItemNo    VARCHAR(100),
    TransType VARCHAR(100),
    Quantity  DECIMAL(18, 6),
    TotalCost DECIMAL(18, 6)
    ,BranchId int
,StartDatetime datetime
,SerialNo varchar(10)
)

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET CustomerID=ReceiveHeaders.CustomerID
FROM ReceiveHeaders
WHERE ReceiveHeaders.ReceiveNo = [VAT_Process].[dbo].VAT6_2_Permanent.TransID @itemCondition3

INSERT INTO #NBRPrive
SELECT itemNo,
       ''           CustomerId,
       (
           CASE
               WHEN NBRPrice = 0 THEN (CASE WHEN OpeningBalance = 0 THEN 0 ELSE OpeningTotalCost / OpeningBalance END)
               ELSE ISNULL(NBRPrice, 0)
               END
           )        NBRPrice,
       '1900/01/01' EffectDate,
       NULL         ToDate,BranchId 
FROM products
WHERE ItemNo IN (SELECT DISTINCT Itemno FROM [VAT_Process].[dbo]. VAT6_2_Permanent WHERE 1 = 1 @itemCondition3)

INSERT INTO #NBRPrive
SELECT FinishItemNo, customerId, ISNULL(NBRPrice, 0) NBRPrice, EffectDate EffectDate, NULL ToDate,BranchId 
FROM BOMs
WHERE FinishItemNo IN (SELECT DISTINCT Itemno FROM [VAT_Process].[dbo].VAT6_2_Permanent WHERE 1 = 1 @itemCondition3)
  AND Post = 'Y'
ORDER BY EffectDate

UPDATE #NBRPrive
SET ToDate=NULL
WHERE 1 = 1 @itemCondition2


UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             Customerid,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY BranchId, Customerid,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE customerid > 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND RT.Customerid = #NBRPrive.Customerid
  AND ToDate IS NULL @itemCondition2

UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY BranchId ,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE ISNULL(NULLIF(customerid, ''), 0) <= 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND ISNULL(NULLIF(customerid, ''), 0) <= 0
  AND ToDate IS NULL @itemCondition2


UPDATE #NBRPrive
SET ToDate='2199/12/31'
WHERE ToDate IS NULL @itemCondition2

INSERT INTO #Temp(Id, ItemNo, TransType, Quantity, TotalCost,BranchId,StartDateTime,SerialNo )
SELECT Id, ItemNo, TransType, Quantity, UnitCost,BranchId ,StartDateTime,SerialNo
FROM [VAT_Process].[dbo].VAT6_2_Permanent
WHERE 1 = 1  
@itemCondition3
ORDER BY BranchId ,ItemNo, StartDateTime, SerialNo,Id

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET RunningTotal=RT.RunningTotal
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * Quantity ELSE Quantity END)
                 OVER (PARTITION BY BranchId,ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotal
      FROM #Temp) RT
WHERE RT.Id = [VAT_Process].[dbo].VAT6_2_Permanent.Id @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET RunningTotalValue=RT.RunningTotalCost
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * TotalCost ELSE TotalCost END)
                 OVER (PARTITION BY BranchId ,ItemNo ORDER BY ItemNo,StartDateTime,SerialNo,SL) AS RunningTotalCost
      FROM #Temp) RT
WHERE RT.Id = [VAT_Process].[dbo].VAT6_2_Permanent.Id @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET DeclaredPrice =0

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent.ItemNo
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime >= #NBRPrive.EffectDate
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime < #NBRPrive.ToDate
  AND [VAT_Process].[dbo].VAT6_2_Permanent.CustomerID = #NBRPrive.CustomerId
  AND ISNULL([VAT_Process].[dbo].VAT6_2_Permanent.DeclaredPrice, 0) = 0 @itemCondition3


UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime >= #NBRPrive.EffectDate
  AND [VAT_Process].[dbo].VAT6_2_Permanent.StartDateTime < #NBRPrive.ToDate
  AND ISNULL([VAT_Process].[dbo].VAT6_2_Permanent.DeclaredPrice, 0) = 0 @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET AdjustmentValue=0
WHERE 1 = 1 @itemCondition3
UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET RunningTotalValueFinal= DeclaredPrice * RunningTotal
WHERE 1 = 1 @itemCondition3

UPDATE [VAT_Process].[dbo].VAT6_2_Permanent
SET AdjustmentValue= RunningTotalValue - RunningTotalValueFinal
WHERE 1 = 1 @itemCondition3

DROP TABLE #Temp

DROP TABLE #NBRPrive


";
                    #endregion

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo = @ItemNo");
                    }
                    else
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                    }

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.CommandText = insertToPermanent;
                    cmd.ExecuteNonQuery();



                    // insert back to main db

                    string insertBackToMain = @"

INSERT INTO VAT6_2_Permanent_Branch( [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal])

Select 
 [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
from [VAT_Process].[dbo].VAT6_2_Permanent
where [VAT_Process].[dbo].VAT6_2_Permanent.TransType != 'MonthOpening'
Order by  BranchId, ItemNo, StartDateTime,SerialNo

=======
select 'B',ID.IssueDateTime,ID.IssueDateTime,ID.IssueNo,'Issue','-'
,id.ItemNo,isnull(UOMQty,0)*isnull(uomPrice,0) ,isnull(UOMQty,0) ,ID.VATAmount,ID.SDAmount,'Issue',IssueDateTime
from IssueDetails ID
where ID.IssueDateTime  >=@StartDate  and ID.IssueDateTime < DATEADD(d,1, @EndDate)  
and ID.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (ID.Post =@post1 or ID.Post= @post2)  
--and ID.SubTotal>0
AND ID.TransactionType IN ('TollFinishReceive') 
AND id.BranchId=@BranchId
>>>>>>> main
";
                    cmd.CommandText = insertBackToMain;
                    cmd.ExecuteNonQuery();

                    // drop the database

                    //string dropDatabase = @"DROP DATABASE IF EXISTS [VAT_Process]";

                    //cmd.CommandText = dropDatabase;
                    //cmd.ExecuteNonQuery();

                }


                #region Comments

                //                sqlText = @"
                //
                //INSERT INTO VAT6_2_Permanent(SerialNo, ItemNo, StartDateTime, StartingQuantity, StartingAmount,
                //                       CustomerID, Quantity, UnitCost, TransID, TransType, VATRate, SD, remarks, CreatedDateTime,
                //                       UnitRate, AdjustmentValue, UserId, BranchId)
                //    SELECT SerialNo,
                //           ItemNo,
                //           StartDateTime,
                //           StartingQuantity,
                //           StartingAmount,
                //           CustomerID,
                //           Quantity,
                //           UnitCost,
                //           TransID,
                //           TransType,
                //           VATRate,
                //           SD,
                //           remarks,
                //           CreatedDateTime,
                //           UnitRate,
                //           AdjustmentValue,
                //           @UserId,
                //           @BranchId
                //    FROM VAT_17_1
                //    ORDER BY ItemNo, StartDateTime, SerialId
                //";

                //                cmd.CommandText = sqlText;
                //                cmd.CommandType = CommandType.Text;
                //                cmd.ExecuteNonQuery();


                #endregion

                #region Commit
                //if (Vtransaction == null)
                //{
                //    if (transaction != null)
                //    {
                //        transaction.Commit();
                //    }
                //}
                #endregion Commit

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        public string[] SaveVAT6_2_Permanent_DayWise(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                    transaction.Commit();
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                    transaction.Commit();
                }
                #endregion open connection and transaction


                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


        --  DROP DATABASE IF EXISTS [VAT_Process]

 IF EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        DROP DATABASE  [VAT_Process]
                    END




            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(18, 6) NULL,
                    Quantity        DECIMAL(18, 6) NULL,
                    VATRate         DECIMAL(18, 6) NULL,
                    SD              DECIMAL(18, 6) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(18, 6),
                    AdjustmentValue DECIMAL(18, 6)
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](18, 6)     NULL,
                    [StartingAmount]              [decimal](18, 6)     NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](18, 6)     NULL,
                    [VATRate]                     [decimal](18, 6)     NULL,
                    [SD]                          [decimal](18, 6)     NULL,
                    [UnitCost]                    [decimal](18, 6)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](18, 6)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](18, 6)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](18, 6)     NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 6)     NULL,
                    [RunningTotalValue]           [decimal](18, 6)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 6)     NULL,
                    [DeclaredPrice]               [decimal](18, 6)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 6)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 6)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();

                sqlText = @"
delete from VAT6_2_Permanent_DayWise where StartDateTime >= @StartDateTime 
                and StartDateTime <= @EndDateTime
                and TransType!='Opening'

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += " and ItemNo='" + vm.ItemNo + "'";
                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@StartDateTime", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDateTime", vm.EndDate);
                cmd.ExecuteNonQuery();


                #region Opening

                sqlText = "spInsertDayOpening";

                SqlCommand spInsertDayOpening = new SqlCommand(sqlText, currConn, transaction);
                spInsertDayOpening.CommandType = CommandType.StoredProcedure;

                spInsertDayOpening.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                spInsertDayOpening.CommandTimeout = 0;

                spInsertDayOpening.ExecuteNonQuery();


                string openingUpdate = @"
update VAT6_2_Permanent_DayWise
set RunningTotal = Quantity,
RunningTotalValue = UnitCost,
RunningOpeningValueFinal = UnitCost,
DeclaredPrice = UnitCost/(Case when Quantity = 0 then 1 else Quantity end)
where VAT6_2_Permanent_DayWise.TransType = 'Opening'
and isnull(VAT6_2_Permanent_DayWise.RunningTotal,0) = 0";

                cmd.CommandText = openingUpdate;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                cmd.CommandType = CommandType.StoredProcedure;

                #endregion


                sqlText = @"
insert into VAT6_2_Permanent_DayWise
(
[SerialNo]
      ,StartDateTime
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      , SD
      ,[UnitCost]
      ,[Remarks]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      , [CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	)



Select * from 
(
select 
       [SerialNo],
	  CAST(StartDateTime AS DATE)StartDateTime
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,'-'[TransID]
      ,[TransType]
      ,'-'[CustomerName]
      ,'-'[Address1]
      ,'-'[Address2]
      ,'-'[Address3]
      ,'-'[VATRegistrationNo]
      ,'-'[ProductName]
      ,'-'[ProductCode]
      ,'-'[UOM]
      ,'-'[HSCodeNo]
      ,sum([Quantity])[Quantity]
      ,sum([VATRate])[VATRate]
      ,sum([SD]) SD
      ,sum([UnitCost])[UnitCost]
      ,'-'[Remarks]
      ,sum([UnitRate])[UnitRate]
      ,[ItemNo]
      ,0 [AdjustmentValue]
      ,0 [UserId]
      ,0 [BranchId]
      ,'-' [CustomerID]
      ,'-'[ProductDesc]
      ,0 [ClosingRate]
      ,0 [PeriodId]
      ,0 [RunningTotal]
      ,0 [RunningTotalValue]
      ,0 [RunningTotalValueFinal]
      ,0 [DeclaredPrice]
      ,0 [RunningOpeningValueFinal]
      ,0 [RunningOpeningQuantityFinal]
	  from VAT6_2_Permanent
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime
	  group by 
	  ItemNo, CAST(StartDateTime AS DATE)
	  , SerialNo
	  ,TransType
	  ) VAT6_2_Day

	  order by 	ItemNo, StartDateTime
	  , SerialNo
";


                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();



                string insertToPermanent = @"

UPDATE VAT6_2_Permanent_DayWise
SET
    Quantity=a.ClosingQty,
    UnitCost=a.RunningTotalValue,
    RunningTotal=a.ClosingQty,
    RunningTotalValue=a.ClosingValue,
    RunningTotalValueFinal =a.ClosingValue,
    DeclaredPrice=a.DeclaredPrice,
    UnitRate=a.UnitRate,
    AdjustmentValue=0

FROM (
SELECT VAT6_2_Permanent.Id,
             VAT6_2_Permanent.ItemNo,
             RunningTotal           ClosingQty,
             RunningTotalValueFinal ClosingValue,
             DeclaredPrice,
             UnitRate,
             RunningTotalValue

      FROM VAT6_2_Permanent
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo, MAX(Id) Id
                                 FROM VAT6_2_Permanent
                                 WHERE StartDateTime < @StartDateTime
                                 GROUP BY ItemNo) AS a
                                ON a.Id = VAT6_2_Permanent.ID) AS a
WHERE a.ItemNo = VAT6_2_Permanent_DayWise.ItemNo
  AND VAT6_2_Permanent_DayWise.TransType = 'MonthOpening'
  AND VAT6_2_Permanent_DayWise.StartDateTime = @StartDateTime




CREATE TABLE #NBRPrive
(
    id         INT IDENTITY (1,1),
    ItemNo     VARCHAR(100),
    CustomerId VARCHAR(100),
    Rate       DECIMAL(18, 6),
    EffectDate DATETIME,
    ToDate     DATETIME
)
CREATE TABLE #Temp
(
    SL        INT IDENTITY (1,1),
    Id        INT,
    ItemNo    VARCHAR(100),
    TransType VARCHAR(100),
    Quantity  DECIMAL(18, 6),
    TotalCost DECIMAL(18, 6)
)

UPDATE VAT6_2_Permanent_DayWise
SET CustomerID=ReceiveHeaders.CustomerID
FROM ReceiveHeaders
WHERE ReceiveHeaders.ReceiveNo = VAT6_2_Permanent_DayWise.TransID @itemCondition3

INSERT INTO #NBRPrive
SELECT itemNo,
       ''           CustomerId,
       (
           CASE
               WHEN NBRPrice = 0 THEN (CASE WHEN OpeningBalance = 0 THEN 0 ELSE OpeningTotalCost / OpeningBalance END)
               ELSE ISNULL(NBRPrice, 0)
               END
           )        NBRPrice,
       '1900/01/01' EffectDate,
       NULL         ToDate

FROM products
WHERE ItemNo IN (SELECT DISTINCT Itemno FROM VAT6_2_Permanent_DayWise WHERE 1 = 1 @itemCondition3)

INSERT INTO #NBRPrive
SELECT FinishItemNo, customerId, ISNULL(NBRPrice, 0) NBRPrice, EffectDate EffectDate, NULL ToDate
FROM BOMs
WHERE FinishItemNo IN (SELECT DISTINCT Itemno FROM VAT6_2_Permanent_DayWise WHERE 1 = 1 @itemCondition3)
  AND Post = 'Y'
ORDER BY EffectDate

UPDATE #NBRPrive
SET ToDate=NULL
WHERE 1 = 1 @itemCondition2


UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             Customerid,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE customerid > 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND RT.Customerid = #NBRPrive.Customerid
  AND ToDate IS NULL @itemCondition2

UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE ISNULL(NULLIF(customerid, ''), 0) <= 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND ISNULL(NULLIF(customerid, ''), 0) <= 0
  AND ToDate IS NULL @itemCondition2


UPDATE #NBRPrive
SET ToDate='2199/12/31'
WHERE ToDate IS NULL @itemCondition2

INSERT INTO #Temp(Id, ItemNo, TransType, Quantity, TotalCost)
SELECT Id, ItemNo, TransType, Quantity, UnitCost
FROM VAT6_2_Permanent_DayWise
WHERE 1 = 1  
and StartDateTime >= @StartDateTime
and StartDateTime <= @EndDateTime
@itemCondition3
ORDER BY ItemNo, StartDateTime, SerialNo

UPDATE VAT6_2_Permanent_DayWise
SET RunningTotal=RT.RunningTotal
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * Quantity ELSE Quantity END)
                 OVER (PARTITION BY ItemNo ORDER BY ItemNo,SL) AS RunningTotal
      FROM #Temp) RT
WHERE RT.Id = VAT6_2_Permanent_DayWise.Id @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET RunningTotalValue=RT.RunningTotalCost
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * TotalCost ELSE TotalCost END)
                 OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningTotalCost
      FROM #Temp) RT
WHERE RT.Id = VAT6_2_Permanent_DayWise.Id @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET DeclaredPrice =0

UPDATE VAT6_2_Permanent_DayWise
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent_DayWise.ItemNo
  AND VAT6_2_Permanent_DayWise.StartDateTime >= #NBRPrive.EffectDate
  AND VAT6_2_Permanent_DayWise.StartDateTime < #NBRPrive.ToDate
  AND VAT6_2_Permanent_DayWise.CustomerID = #NBRPrive.CustomerId
  AND ISNULL(VAT6_2_Permanent_DayWise.DeclaredPrice, 0) = 0 @itemCondition3


UPDATE VAT6_2_Permanent_DayWise
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent_DayWise.ItemNo
  AND VAT6_2_Permanent_DayWise.StartDateTime >= #NBRPrive.EffectDate
  AND VAT6_2_Permanent_DayWise.StartDateTime < #NBRPrive.ToDate
  AND ISNULL(VAT6_2_Permanent_DayWise.DeclaredPrice, 0) = 0 @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET AdjustmentValue=0
WHERE 1 = 1 @itemCondition3
UPDATE VAT6_2_Permanent_DayWise
SET RunningTotalValueFinal= DeclaredPrice * RunningTotal
WHERE 1 = 1 @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET AdjustmentValue= RunningTotalValue - RunningTotalValueFinal
WHERE 1 = 1 @itemCondition3

DROP TABLE #Temp

DROP TABLE #NBRPrive

";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT6_2_Permanent_DayWise.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);

                }
                cmd.CommandText = insertToPermanent;

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();



                #region Commit
                //if (Vtransaction == null)
                //{
                //    if (transaction != null)
                //    {
                //        transaction.Commit();
                //    }
                //}
                #endregion Commit


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    //transaction.Rollback();
                    transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_2_Permanent_MonthWise(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }


                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


          DROP DATABASE IF EXISTS [VAT_Process]

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(25, 9) NULL,
                    Quantity        DECIMAL(25, 9) NULL,
                    VATRate         DECIMAL(25, 9) NULL,
                    SD              DECIMAL(25, 9) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(25, 9),
                    AdjustmentValue DECIMAL(25, 9)
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](25, 9)     NULL,
                    [StartingAmount]              [decimal](25, 9)     NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](25, 9)     NULL,
                    [VATRate]                     [decimal](25, 9)     NULL,
                    [SD]                          [decimal](25, 9)     NULL,
                    [UnitCost]                    [decimal](25, 9)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](25, 9)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](25, 9)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](25, 9)     NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 8)     NULL,
                    [RunningTotalValue]           [decimal](18, 8)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 8)     NULL,
                    [DeclaredPrice]               [decimal](25, 9)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 8)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 8)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();


                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction



                sqlText = @"
delete from VAT6_2_Permanent_DayWise where StartDateTime >= @StartDateTime 
                and StartDateTime <= @EndDateTime

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += " and ItemNo='" + vm.ItemNo + "'";
                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@StartDateTime", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDateTime", vm.EndDate);
                cmd.ExecuteNonQuery();


                sqlText = @"
insert into VAT6_2_Permanent_DayWise
(
[SerialNo]
      ,StartDateTime
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      , SD
      ,[UnitCost]
      ,[Remarks]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      , [CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	)



Select * from 
(select 
      distinct 'A' [SerialNo],
	  @StartDateTime StartDateTime
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,'-'[TransID]
      ,'MonthOpening'[TransType]
      ,'-'[CustomerName]
      ,'-'[Address1]
      ,'-'[Address2]
      ,'-'[Address3]
      ,'-'[VATRegistrationNo]
      ,'-'[ProductName]
      ,'-'[ProductCode]
      ,'-'[UOM]
      ,'-'[HSCodeNo]
      ,0 [Quantity]
      ,0 [VATRate]
      ,0 SD
      ,0 [UnitCost]
      ,'-'[Remarks]
      ,0 [UnitRate]
      ,[ItemNo]
      ,0 [AdjustmentValue]
      ,0 [UserId]
      ,0 [BranchId]
      ,'-' [CustomerID]
      ,'-'[ProductDesc]
      ,0 [ClosingRate]
      ,0 [PeriodId]
      ,0 [RunningTotal]
      ,0 [RunningTotalValue]
      ,0 [RunningTotalValueFinal]
      ,0 [DeclaredPrice]
      ,0 [RunningOpeningValueFinal]
      ,0 [RunningOpeningQuantityFinal]
	  from VAT6_2_Permanent
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime

	union all
	
select 
       [SerialNo],
	  CAST(StartDateTime AS DATE)StartDateTime
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,'-'[TransID]
      ,[TransType]
      ,'-'[CustomerName]
      ,'-'[Address1]
      ,'-'[Address2]
      ,'-'[Address3]
      ,'-'[VATRegistrationNo]
      ,'-'[ProductName]
      ,'-'[ProductCode]
      ,'-'[UOM]
      ,'-'[HSCodeNo]
      ,sum([Quantity])[Quantity]
      ,sum([VATRate])[VATRate]
      ,sum([SD]) SD
      ,sum([UnitCost])[UnitCost]
      ,'-'[Remarks]
      ,sum([UnitRate])[UnitRate]
      ,[ItemNo]
      ,0 [AdjustmentValue]
      ,0 [UserId]
      ,0 [BranchId]
      ,'-' [CustomerID]
      ,'-'[ProductDesc]
      ,0 [ClosingRate]
      ,0 [PeriodId]
      ,0 [RunningTotal]
      ,0 [RunningTotalValue]
      ,0 [RunningTotalValueFinal]
      ,0 [DeclaredPrice]
      ,0 [RunningOpeningValueFinal]
      ,0 [RunningOpeningQuantityFinal]
	  from VAT6_2_Permanent
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime
	  group by 
	  ItemNo, CAST(StartDateTime AS DATE)
	  , SerialNo
	  ,TransType
	  ) VAT6_2_Day

	  order by 	ItemNo, StartDateTime
	  , SerialNo
";


                cmd.CommandText = sqlText;
                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();



                string insertToPermanent = @"

UPDATE VAT6_2_Permanent_DayWise
SET
    Quantity=a.ClosingQty,
    UnitCost=a.RunningTotalValue,
    RunningTotal=a.ClosingQty,
    RunningTotalValue=a.ClosingValue,
    RunningTotalValueFinal =a.ClosingValue,
    DeclaredPrice=a.DeclaredPrice,
    UnitRate=a.UnitRate,
    AdjustmentValue=0

FROM (
SELECT VAT6_2_Permanent.Id,
             VAT6_2_Permanent.ItemNo,
             RunningTotal           ClosingQty,
             RunningTotalValueFinal ClosingValue,
             DeclaredPrice,
             UnitRate,
             RunningTotalValue

      FROM VAT6_2_Permanent
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo, MAX(Id) Id
                                 FROM VAT6_2_Permanent
                                 WHERE StartDateTime < @StartDateTime
                                 GROUP BY ItemNo) AS a
                                ON a.Id = VAT6_2_Permanent.ID) AS a
WHERE a.ItemNo = VAT6_2_Permanent_DayWise.ItemNo
  AND VAT6_2_Permanent_DayWise.TransType = 'MonthOpening'




CREATE TABLE #NBRPrive
(
    id         INT IDENTITY (1,1),
    ItemNo     VARCHAR(100),
    CustomerId VARCHAR(100),
    Rate       DECIMAL(18, 8),
    EffectDate DATETIME,
    ToDate     DATETIME
)
CREATE TABLE #Temp
(
    SL        INT IDENTITY (1,1),
    Id        INT,
    ItemNo    VARCHAR(100),
    TransType VARCHAR(100),
    Quantity  DECIMAL(18, 8),
    TotalCost DECIMAL(18, 8)
)

UPDATE VAT6_2_Permanent_DayWise
SET CustomerID=ReceiveHeaders.CustomerID
FROM ReceiveHeaders
WHERE ReceiveHeaders.ReceiveNo = VAT6_2_Permanent_DayWise.TransID @itemCondition3

INSERT INTO #NBRPrive
SELECT itemNo,
       ''           CustomerId,
       (
           CASE
               WHEN NBRPrice = 0 THEN (CASE WHEN OpeningBalance = 0 THEN 0 ELSE OpeningTotalCost / OpeningBalance END)
               ELSE ISNULL(NBRPrice, 0)
               END
           )        NBRPrice,
       '1900/01/01' EffectDate,
       NULL         ToDate

FROM products
WHERE ItemNo IN (SELECT DISTINCT Itemno FROM VAT6_2_Permanent_DayWise WHERE 1 = 1 @itemCondition3)

INSERT INTO #NBRPrive
SELECT FinishItemNo, customerId, ISNULL(NBRPrice, 0) NBRPrice, EffectDate EffectDate, NULL ToDate
FROM BOMs
WHERE FinishItemNo IN (SELECT DISTINCT Itemno FROM VAT6_2_Permanent_DayWise WHERE 1 = 1 @itemCondition3)
  AND Post = 'Y'
ORDER BY EffectDate

UPDATE #NBRPrive
SET ToDate=NULL
WHERE 1 = 1 @itemCondition2


UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             Customerid,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE customerid > 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND RT.Customerid = #NBRPrive.Customerid
  AND ToDate IS NULL @itemCondition2

UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE ISNULL(NULLIF(customerid, ''), 0) <= 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND ISNULL(NULLIF(customerid, ''), 0) <= 0
  AND ToDate IS NULL @itemCondition2


UPDATE #NBRPrive
SET ToDate='2199/12/31'
WHERE ToDate IS NULL @itemCondition2

INSERT INTO #Temp(Id, ItemNo, TransType, Quantity, TotalCost)
SELECT Id, ItemNo, TransType, Quantity, UnitCost
FROM VAT6_2_Permanent_DayWise
WHERE 1 = 1  
and StartDateTime >= @StartDateTime
and StartDateTime <= @EndDateTime
@itemCondition3
ORDER BY ItemNo, StartDateTime, SerialNo

UPDATE VAT6_2_Permanent_DayWise
SET RunningTotal=RT.RunningTotal
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * Quantity ELSE Quantity END)
                 OVER (PARTITION BY ItemNo ORDER BY ItemNo,SL) AS RunningTotal
      FROM #Temp) RT
WHERE RT.Id = VAT6_2_Permanent_DayWise.Id @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET RunningTotalValue=RT.RunningTotalCost
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * TotalCost ELSE TotalCost END)
                 OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningTotalCost
      FROM #Temp) RT
WHERE RT.Id = VAT6_2_Permanent_DayWise.Id @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET DeclaredPrice =0

UPDATE VAT6_2_Permanent_DayWise
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent_DayWise.ItemNo
  AND VAT6_2_Permanent_DayWise.StartDateTime >= #NBRPrive.EffectDate
  AND VAT6_2_Permanent_DayWise.StartDateTime < #NBRPrive.ToDate
  AND VAT6_2_Permanent_DayWise.CustomerID = #NBRPrive.CustomerId
  AND ISNULL(VAT6_2_Permanent_DayWise.DeclaredPrice, 0) = 0 @itemCondition3


UPDATE VAT6_2_Permanent_DayWise
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent_DayWise.ItemNo
  AND VAT6_2_Permanent_DayWise.StartDateTime >= #NBRPrive.EffectDate
  AND VAT6_2_Permanent_DayWise.StartDateTime < #NBRPrive.ToDate
  AND ISNULL(VAT6_2_Permanent_DayWise.DeclaredPrice, 0) = 0 @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET AdjustmentValue=0
WHERE 1 = 1 @itemCondition3
UPDATE VAT6_2_Permanent_DayWise
SET RunningTotalValueFinal= DeclaredPrice * RunningTotal
WHERE 1 = 1 @itemCondition3

UPDATE VAT6_2_Permanent_DayWise
SET AdjustmentValue= RunningTotalValue - RunningTotalValueFinal
WHERE 1 = 1 @itemCondition3

DROP TABLE #Temp

DROP TABLE #NBRPrive

";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and [VAT_Process].[dbo].VAT6_2_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }


                cmd.CommandText = insertToPermanent;
                cmd.ExecuteNonQuery();



                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    //transaction.Rollback();
                    transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_2_Permanent_DayWise_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }


                #endregion open connection and transaction
                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


      IF EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        DROP DATABASE  [VAT_Process]
                    END

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(18, 6) NULL,
                    Quantity        DECIMAL(18, 6) NULL,
                    VATRate         DECIMAL(18, 6) NULL,
                    SD              DECIMAL(18, 6) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(18, 6),
                    AdjustmentValue DECIMAL(18, 6)
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](18, 6)     NULL,
                    [StartingAmount]              [decimal](18, 6)     NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](18, 6)     NULL,
                    [VATRate]                     [decimal](18, 6)     NULL,
                    [SD]                          [decimal](18, 6)     NULL,
                    [UnitCost]                    [decimal](18, 6)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](18, 6)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](18, 6)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](18, 6)     NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 6)     NULL,
                    [RunningTotalValue]           [decimal](18, 6)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 6)     NULL,
                    [DeclaredPrice]               [decimal](18, 6)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 6)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 6)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                    // transaction.Commit();
                }


                sqlText = @"
delete from VAT6_2_Permanent_DayWise_Branch where StartDateTime >= @StartDateTime 
                and StartDateTime <= @EndDateTime
                and TransType!='Opening'

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += " and ItemNo='" + vm.ItemNo + "'";
                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@StartDateTime", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDateTime", vm.EndDate);
                cmd.ExecuteNonQuery();


                #region Opening

                sqlText = "spInsertDayOpeningBranch";

                SqlCommand spInsertDayOpeningBranch = new SqlCommand(sqlText, currConn, transaction);
                spInsertDayOpeningBranch.CommandType = CommandType.StoredProcedure;

                spInsertDayOpeningBranch.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                spInsertDayOpeningBranch.CommandTimeout = 0;

                spInsertDayOpeningBranch.ExecuteNonQuery();


                string openingUpdate = @"
update VAT6_2_Permanent_DayWise_Branch
set RunningTotal = Quantity,
RunningTotalValue = UnitCost,
RunningOpeningValueFinal = UnitCost,
DeclaredPrice = UnitCost/(Case when Quantity = 0 then 1 else Quantity end)
where VAT6_2_Permanent_DayWise_Branch.TransType = 'Opening'
";

                cmd.CommandText = openingUpdate;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();

                cmd.CommandType = CommandType.StoredProcedure;

                #endregion

                sqlText = @"
insert into VAT6_2_Permanent_DayWise_Branch
(
[SerialNo]
      ,StartDateTime
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      , SD
      ,[UnitCost]
      ,[Remarks]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      , [CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[PeriodId]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[DeclaredPrice]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	)



Select * from 
(
select 
       [SerialNo],
	  CAST(StartDateTime AS DATE)StartDateTime
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,'-'[TransID]
      ,[TransType]
      ,'-'[CustomerName]
      ,'-'[Address1]
      ,'-'[Address2]
      ,'-'[Address3]
      ,'-'[VATRegistrationNo]
      ,'-'[ProductName]
      ,'-'[ProductCode]
      ,'-'[UOM]
      ,'-'[HSCodeNo]
      ,sum([Quantity])[Quantity]
      ,sum([VATRate])[VATRate]
      ,sum([SD]) SD
      ,sum([UnitCost])[UnitCost]
      ,'-'[Remarks]
      ,sum([UnitRate])[UnitRate]
      ,[ItemNo]
      ,0 [AdjustmentValue]
      ,0 [UserId]
      ,[BranchId]
      ,'-' [CustomerID]
      ,'-'[ProductDesc]
      ,0 [ClosingRate]
      ,0 [PeriodId]
      ,0 [RunningTotal]
      ,0 [RunningTotalValue]
      ,0 [RunningTotalValueFinal]
      ,0 [DeclaredPrice]
      ,0 [RunningOpeningValueFinal]
      ,0 [RunningOpeningQuantityFinal]
	  from VAT6_2_Permanent_Branch
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime
	  group by [BranchId], 
	  ItemNo, CAST(StartDateTime AS DATE)
	  , SerialNo
	  ,TransType
	  ) VAT6_2_Day

	  order by 	ItemNo, StartDateTime
	  , SerialNo
";


                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;

                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();



                string insertToPermanent = @"

UPDATE VAT6_2_Permanent_DayWise_Branch
SET
    Quantity=a.ClosingQty,
    UnitCost=a.RunningTotalValue,
    RunningTotal=a.ClosingQty,
    RunningTotalValue=a.ClosingValue,
    RunningTotalValueFinal =a.ClosingValue,
    DeclaredPrice=a.DeclaredPrice,
    UnitRate=a.UnitRate,
    AdjustmentValue=0

FROM (
SELECT VAT6_2_Permanent_Branch.Id,
             VAT6_2_Permanent_Branch.ItemNo,
             RunningTotal           ClosingQty,
             RunningTotalValueFinal ClosingValue,
             DeclaredPrice,
             UnitRate,
             RunningTotalValue,
			 VAT6_2_Permanent_Branch.BranchId

      FROM VAT6_2_Permanent_Branch
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo,BranchId, MAX(Id) Id
                                 FROM VAT6_2_Permanent_Branch
                                 WHERE StartDateTime < @StartDateTime
                                 GROUP BY BranchId,ItemNo) AS a
                                ON a.Id = VAT6_2_Permanent_Branch.ID) AS a
WHERE a.ItemNo = VAT6_2_Permanent_DayWise_Branch.ItemNo
AND a.BranchId = VAT6_2_Permanent_DayWise_Branch.BranchId
  AND VAT6_2_Permanent_DayWise_Branch.TransType = 'MonthOpening'
  AND VAT6_2_Permanent_DayWise_Branch.StartDateTime = @StartDateTime

CREATE TABLE #NBRPrive
(
    id         INT IDENTITY (1,1),
    ItemNo     VARCHAR(100),
    CustomerId VARCHAR(100),
    Rate       DECIMAL(18, 6),
    EffectDate DATETIME,
    ToDate     DATETIME
    ,BranchId int
)
CREATE TABLE #Temp
(
    SL        INT IDENTITY (1,1),
    Id        INT,
    ItemNo    VARCHAR(100),
    TransType VARCHAR(100),
    Quantity  DECIMAL(18, 6),
    TotalCost DECIMAL(18, 6)
    ,BranchId int
)

UPDATE VAT6_2_Permanent_DayWise_Branch
SET CustomerID=ReceiveHeaders.CustomerID
FROM ReceiveHeaders
WHERE ReceiveHeaders.ReceiveNo = VAT6_2_Permanent_DayWise_Branch.TransID @itemCondition3

INSERT INTO #NBRPrive
SELECT itemNo,
       ''           CustomerId,
       (
           CASE
               WHEN NBRPrice = 0 THEN (CASE WHEN OpeningBalance = 0 THEN 0 ELSE OpeningTotalCost / OpeningBalance END)
               ELSE ISNULL(NBRPrice, 0)
               END
           )        NBRPrice,
       '1900/01/01' EffectDate,
       NULL         ToDate,BranchId 
FROM products
WHERE ItemNo IN (SELECT DISTINCT Itemno FROM VAT6_2_Permanent_DayWise_Branch WHERE 1 = 1 @itemCondition3)

INSERT INTO #NBRPrive
SELECT FinishItemNo, customerId, ISNULL(NBRPrice, 0) NBRPrice, EffectDate EffectDate, NULL ToDate,BranchId 
FROM BOMs
WHERE FinishItemNo IN (SELECT DISTINCT Itemno FROM VAT6_2_Permanent_DayWise_Branch WHERE 1 = 1 @itemCondition3)
  AND Post = 'Y'
ORDER BY EffectDate

UPDATE #NBRPrive
SET ToDate=NULL
WHERE 1 = 1 @itemCondition2


UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             Customerid,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY BranchId, Customerid,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE customerid > 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND RT.Customerid = #NBRPrive.Customerid
  AND ToDate IS NULL @itemCondition2

UPDATE #NBRPrive
SET ToDate=RT.RunningTotal
FROM (SELECT id,
             ItemNo,
             LEAD(EffectDate)
                  OVER (PARTITION BY BranchId ,ItemNo ORDER BY id) AS RunningTotal
      FROM #NBRPrive
      WHERE ISNULL(NULLIF(customerid, ''), 0) <= 0) RT
WHERE RT.Id = #NBRPrive.Id
  AND ISNULL(NULLIF(customerid, ''), 0) <= 0
  AND ToDate IS NULL @itemCondition2


UPDATE #NBRPrive
SET ToDate='2199/12/31'
WHERE ToDate IS NULL @itemCondition2

INSERT INTO #Temp(Id, ItemNo, TransType, Quantity, TotalCost,BranchId )
SELECT Id, ItemNo, TransType, Quantity, UnitCost,BranchId 
FROM VAT6_2_Permanent_DayWise_Branch
WHERE 1 = 1  
@itemCondition3
ORDER BY BranchId ,ItemNo, StartDateTime, SerialNo

UPDATE VAT6_2_Permanent_DayWise_Branch
SET RunningTotal=RT.RunningTotal
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * Quantity ELSE Quantity END)
                 OVER (PARTITION BY BranchId,ItemNo ORDER BY ItemNo,SL) AS RunningTotal
      FROM #Temp) RT
WHERE RT.Id = VAT6_2_Permanent_DayWise_Branch.Id @itemCondition3

UPDATE VAT6_2_Permanent_DayWise_Branch
SET RunningTotalValue=RT.RunningTotalCost
FROM (SELECT id,
             SL,
             ItemNo,
             TransType,
             Quantity,
             SUM(CASE WHEN TransType IN ('Sale') THEN -1 * TotalCost ELSE TotalCost END)
                 OVER (PARTITION BY BranchId ,ItemNo ORDER BY SL) AS RunningTotalCost
      FROM #Temp) RT
WHERE RT.Id = VAT6_2_Permanent_DayWise_Branch.Id @itemCondition3

UPDATE VAT6_2_Permanent_DayWise_Branch
SET DeclaredPrice =0

UPDATE VAT6_2_Permanent_DayWise_Branch
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent_DayWise_Branch.ItemNo
  AND VAT6_2_Permanent_DayWise_Branch.StartDateTime >= #NBRPrive.EffectDate
  AND VAT6_2_Permanent_DayWise_Branch.StartDateTime < #NBRPrive.ToDate
  AND VAT6_2_Permanent_DayWise_Branch.CustomerID = #NBRPrive.CustomerId
  AND ISNULL(VAT6_2_Permanent_DayWise_Branch.DeclaredPrice, 0) = 0 @itemCondition3


UPDATE VAT6_2_Permanent_DayWise_Branch
SET DeclaredPrice =#NBRPrive.Rate
FROM #NBRPrive
WHERE #NBRPrive.ItemNo = VAT6_2_Permanent_DayWise_Branch.ItemNo
  AND VAT6_2_Permanent_DayWise_Branch.StartDateTime >= #NBRPrive.EffectDate
  AND VAT6_2_Permanent_DayWise_Branch.StartDateTime < #NBRPrive.ToDate
  AND ISNULL(VAT6_2_Permanent_DayWise_Branch.DeclaredPrice, 0) = 0 @itemCondition3

UPDATE VAT6_2_Permanent_DayWise_Branch
SET AdjustmentValue=0
WHERE 1 = 1 @itemCondition3
UPDATE VAT6_2_Permanent_DayWise_Branch
SET RunningTotalValueFinal= DeclaredPrice * RunningTotal
WHERE 1 = 1 @itemCondition3

UPDATE VAT6_2_Permanent_DayWise_Branch
SET AdjustmentValue= RunningTotalValue - RunningTotalValueFinal
WHERE 1 = 1 @itemCondition3

DROP TABLE #Temp

DROP TABLE #NBRPrive

";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT6_2_Permanent_DayWise_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);

                }
                cmd.CommandText = insertToPermanent;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();



                FileLogger.Log("SaveVAT6_2_Permanent_DayWise_Branch", "SaveVAT6_2_Permanent_DayWise_Branch", vm.StartDate + " " + vm.EndDate);


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public ResultVM UpdateAvgPrice_New_Refresh(AVGPriceVm vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                sqlText = @"delete from ProductAvgPrice where 1=1 ";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                if (!string.IsNullOrEmpty(vm.AvgDateTime))
                {
                    sqlText += " and AgvPriceDate >= @AgvPriceDate";
                }
                if (!vm.FromSP)
                {

                    cmd = new SqlCommand(sqlText, currConn, transaction);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }

                    if (!string.IsNullOrEmpty(vm.AvgDateTime))
                    {
                        cmd.Parameters.AddWithValue("@AgvPriceDate", vm.AvgDateTime);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@AvgDateTime", !string.IsNullOrWhiteSpace(vm.AvgDateTime) ? "'" + vm.AvgDateTime + "'" : "1900-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                rVM = UpdateAvgPrice_New_Partition(vm, currConn, transaction, connVM);

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }
        public ResultVM UpdateAvgPrice_New_Partition(AVGPriceVm vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(" ", currConn, transaction);

                #region Update

                bool IssueFrom6_1 = _cDAL.settings("Toll6_4", "IssueFrom6_1", currConn, transaction, connVM) == "Y"; // toll Issue
                bool TollReceiveNotWIP = _cDAL.settings("IssueFromBOM", "TollReceive-NotWIP", currConn, transaction, connVM) == "Y"; // TollReceive-NotWIP
                bool TollReceiveWithIssue = _cDAL.settings("TollReceive", "WithIssue", currConn, transaction, connVM) == "Y"; // TollReceive

                string[] transactionTypes = new[]
                {
                    "Other", "Trading", "TradingAuto", "ExportTrading", "TradingTender", "ExportTradingTender",
                    "InternalIssue", "Service", "ExportService", "InputServiceImport", "InputService", "Tender", "WIP",
                    "PackageProduction", "TollReceive-NotWIP", "TollIssue", "TollFinishReceive",
                    "ReceiveReturn", "TollReceive"
                }; // , "IssueReturn"

                if (!IssueFrom6_1)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollIssue").ToArray();
                }
                if (!TollReceiveNotWIP)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive-NotWIP").ToArray();
                }
                if (!TollReceiveWithIssue)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive").ToArray();
                }

                string sqlText = "";
                if (vm != null)
                {
                    if (false)
                    {
                        List<IssueMasterVM> MasterVMsX = new List<IssueMasterVM>();
                        sqlText = "select count(SL) from ProductAvgPrice";
                        cmd = new SqlCommand(sqlText, currConn, transaction);
                        cmd.CommandTimeout = 3600;
                        int rowsX = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    #region Initial AVG Price Query

                    sqlText = @"
--delete from ProductAvgPrice


create table #tempAVGPrice(
ID int identity(1,1),
SerialNo varchar(50),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)

create table #tempAVGPriceOrder(
ID int identity(1,1),
SerialNo varchar(50),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)


insert into #tempAVGPrice(SerialNo,ItemNo, AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A',p.ItemNO, '1900-01-01 00:00:00', 0,0,0,'',GetDate(),'Opening'
from Products p left outer join ProductCategories pc on p.CategoryID = pc.CategoryID
where pc.IsRaw in( 'raw','pack','trading','WIP','Overhead') 
and p.ItemNo not in (select ItemNo from ProductAvgPrice where TransactionType = 'Opening')
--and ItemNo = '37'
@itemCondition

------------------------------------------------------------OPENING-------------------------------------------------------

  declare @count int = (select count(BranchId) from BranchProfiles)

    IF @count = 1
    BEGIN
	    update #tempAVGPrice set PurchaseQty = Products.OpeningBalance, 
	    PurchaseValue=Products.OpeningTotalCost
	    from Products 
	    where #tempAVGPrice.ItemNo = Products.ItemNo and #tempAVGPrice.TransactionType='Opening'

    END

    ELSE

    BEGIN

	    create table #tempStocks(id int identity(1,1), ItemNo varchar(50), TotalQty   decimal(25,9), TotalValue   decimal(25,9))

	    insert into #tempStocks (ItemNo, TotalQty,TotalValue)
	    SELECT distinct ItemNo, isnull(sum(StockQuantity),0) Quantity, isnull(sum(StockValue),0) 
	    from ProductStocks 
        where 1=1
        @itemCondition
	    group by ItemNo

	    update #tempAVGPrice set PurchaseQty = #tempStocks.TotalQty, 
	    PurchaseValue=#tempStocks.TotalValue
	    from #tempStocks 
	    where #tempAVGPrice.ItemNo = #tempStocks.ItemNo  and #tempAVGPrice.TransactionType='Opening'

	    drop table #tempStocks

    END

-----------------------------------------------------Opening-------------------------------------------------------------------
-----------------------------------------------------Purchase------------------------------------------------------------------
-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)

--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','ClientFGReceiveWOBOM')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'Toll Receive' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(isnull(UOMQty,0))
,sum(SubTotal)
,0 AvgPrice
,PurchaseInvoiceNo,GetDate()
,'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('TollReceive-WIP')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('RawCTC')
and pd.Post = 'Y'
--and ItemNo = '37'
@itemCondition2
@itemCondition6
group by ToItemNo, pd.TransferDate,pt.TransferCode


--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
)
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y' 
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

-----------------------------------------------------Purchase------------------------------------------------------------------

----------------------------------------------------- Issue -------------------------------------------------------------------

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       ItemNo,
       CAST(IssueDateTime AS DATE) IssueDateTime,
       SUM(UOMQty)                 UOMQty,
       SUM(Subtotal)               Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM IssueDetails
WHERE 1 = 1
  AND TransactionType IN ('" + string.Join("','", transactionTypes) + @"') --,'TollReceive'
  AND Post = 'Y'
@itemCondition
@itemCondition7
GROUP BY ItemNo, CAST(IssueDateTime AS DATE)


INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       ItemNo,
       CAST(IssueDateTime AS DATE) IssueDateTime,
       -1 * SUM(UOMQty)            UOMQty,
       -1 * SUM(Subtotal)          Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM IssueDetails
WHERE 1 = 1
  AND TransactionType IN ('IssueReturn') --,'TollReceive'
  AND Post = 'Y' 
@itemCondition
@itemCondition7
GROUP BY ItemNo, CAST(IssueDateTime AS DATE)


INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       ItemNo,
       CAST(InvoiceDateTime AS DATE) InvoiceDateTime,
       SUM(UOMQty)                   UOMQty,
       SUM(Subtotal)                 Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM SalesInvoiceDetails
WHERE 1 = 1
  AND TransactionType IN ('RawSale') --,'TollReceive'
  AND Post = 'Y' 
@itemCondition
@itemCondition8
GROUP BY ItemNo, CAST(InvoiceDateTime AS DATE)


INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       FromItemNo,
       CAST(TransferDate AS DATE) InvoiceDateTime,
       SUM(FromQuantity)          UOMQty,
       SUM(IssuePrice)            Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM ProductTransfersDetails pd
WHERE 1 = 1 
@itemCondition3
@itemCondition6
  AND pd.TransactionType IN ('RawCTC') --,'TollReceive'
  AND pd.Post = 'Y'
GROUP BY FromItemNo, CAST(TransferDate AS DATE)


-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------
insert into ProductAvgPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select SerialNo, ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType 
from #tempAVGPrice
order by ItemNo, AgvPriceDate, SerialNo

delete from #tempAVGPrice

-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------

Update ProductAvgPrice set RuntimeQty
= ProductTotals.RunningQty
From (
select SL, ItemNo, sum(case when (TransactionType = 'purchase' or TransactionType = 'Opening') then PurchaseQty else PurchaseQty*-1 end) over (partition by ItemNo order by ItemNo,agvPriceDate,SerialNo,SL) RunningQty
from ProductAvgPrice 
where 1=1 @itemCondition

) as ProductTotals

where ProductAvgPrice.SL = ProductTotals.SL


update ProductAvgPrice set RuntimeTotal = PurchaseValue, AvgPrice = PurchaseValue/RuntimeQty
where RuntimeQty != 0 and TransactionType = 'Opening' @itemCondition

update ProductAvgPrice set RuntimeTotal = PurchaseValue, AvgPrice = 0
where RuntimeQty = 0 and TransactionType = 'Opening' @itemCondition


create table #InitialData ( 
 ItemNo varchar(50)
 , AgvPriceDate datetime
 , PurchaseQty decimal(25,9)
, PurchaseValue decimal(25,9)
, RuntimeQty decimal(25,9)
, RuntimeTotal decimal(25,9)
, row_no int 
, SerialNo varchar(50)
, AvgPrice decimal(25,9)
, TransactionType varchar(50)
, SL int
CONSTRAINT PK_Person PRIMARY KEY (row_no,ItemNo)
)

insert into #InitialData 
SELECT       ItemNo,
                       AgvPriceDate,
                       PurchaseQty,
                       CAST(PurchaseValue AS DECIMAL(25,9)) PurchaseValue,
                       RuntimeQty,
                       CAST(RuntimeTotal AS DECIMAL(25,9)) RuntimeTotal,
              row_no = ROW_NUMBER() OVER (PARTITION BY ItemNo ORDER BY ItemNo, AgvPriceDate, SerialNo, SL),
                       SerialNo,
					   CAST(AvgPrice AS DECIMAL(25,9)) AvgPrice,
					   TransactionType,
					   SL
          FROM ProductAvgPrice where 1=1 
 
 ; with   rcte
         AS
         (SELECT ItemNo,
                 AgvPriceDate,
                 PurchaseQty,
                 CAST(PurchaseValue AS DECIMAL(25,9)) PurchaseValue,
                 RuntimeQty,
                 CAST(RuntimeTotal AS DECIMAL(25,9)) RuntimeTotal,
                 row_no,
                 SerialNo,
				 CAST(AvgPrice AS DECIMAL(25,9)) AvgPrice,
				 TransactionType
				 ,SL
          FROM #InitialData
          WHERE row_no = 1

          UNION ALL

          SELECT c.ItemNo,
                 c.AgvPriceDate,
                 c.PurchaseQty,
                 PurchaseValue = case when c.TransactionType = 'Issue' then CAST((r.AvgPrice * c.PurchaseQty) AS DECIMAL(25,9)) else c.PurchaseValue end,
                 c.RuntimeQty,
                 RuntimeTotal =  (case when c.TransactionType = 'Issue' then case when CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty)) as decimal(25,9)) >0 then CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty)) as decimal(25,9)) else 0 end else
				 CAST((r.RuntimeTotal + c.PurchaseValue) as decimal(25,9)) end) ,
                 c.row_no,
                 c.SerialNo,
--AvgPrice = case when c.RuntimeQty > 0 then  (case when c.TransactionType = 'Issue' then case when CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty))/c.RuntimeQty as decimal(25,9)) > 0 then CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty))/c.RuntimeQty as decimal(25,9)) else 0 end
--else case when CAST((r.RuntimeTotal + c.PurchaseValue)/c.RuntimeQty as decimal(25,9)) > 0 then CAST((r.RuntimeTotal + c.PurchaseValue)/c.RuntimeQty as decimal(25,9)) else 0 end end) else 0 end ,
				
AvgPrice= case when c.RuntimeQty  >0
then cast(cast(( case when c.RuntimeQty >0 then   (case when c.TransactionType = 'Issue' then case when CAST((cast(r.RuntimeTotal AS DECIMAL(25,9)) - (cast(r.AvgPrice AS DECIMAL(25,9)) * cast(c.PurchaseQty AS DECIMAL(25,9)))) as decimal(25,9)) >0 then CAST((cast(r.RuntimeTotal  AS DECIMAL(25,9))- (cast(r.AvgPrice AS DECIMAL(25,9)) * cast(c.PurchaseQty AS DECIMAL(25,9)))) as decimal(25,9)) else 0.0 end else CAST((cast(r.RuntimeTotal  AS DECIMAL(25,9))+ cast(c.PurchaseValue AS DECIMAL(25,9))) as decimal(25,9)) end) else 0 end ) AS DECIMAL(25,9))/cast(c.RuntimeQty AS DECIMAL(25,9))AS DECIMAL(25,9))
when c.RuntimeQty =0 and c.TransactionType = 'Issue'then cast(r.AvgPrice   AS DECIMAL(25,9))
else 0.0 end,	

c.TransactionType
				,c.SL
          FROM #InitialData c
                   INNER JOIN rcte r ON c.ItemNo = r.ItemNo
              AND c.row_no = r.row_no + 1)


select * into #tempResult
from rcte
ORDER BY ItemNo, AgvPriceDate, SerialNo
OPTION (MAXRECURSION 0)

 

update ProductAvgPrice set AvgPrice = isnull(ap.AvgPrice,0)
, PurchaseValue = isnull(ap.PurchaseValue,0)
,RuntimeTotal= isnull(ap.RuntimeTotal,0)
from  (
SELECT *
FROM #tempResult
--ORDER BY ItemNo, AgvPriceDate, SerialNo
) as AP 
inner join ProductAvgPrice pap on AP.SL = pap.SL and AP.ItemNo = pap.ItemNo



update IssueDetails
set CostPrice = UOMc*pap.AvgPrice ,SubTotal = UOMc*pap.AvgPrice *Quantity, UOMPrice = pap.AvgPrice
from IssueDetails id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.IssueDateTime as date) = pap.AgvPriceDate
and pap.TransactionType = 'Issue'
where id.TransactionType in ('" + string.Join("','", transactionTypes) + @"') 
@itemCondition4
@itemCondition7


update SalesInvoiceDetails
set AVGPrice = pap.AvgPrice
--, SubTotal = pap.AvgPrice * Quantity

from SalesInvoiceDetails id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.InvoiceDateTime as date) = pap.AgvPriceDate
and pap.TransactionType = 'Issue'
where id.TransactionType in ('rawsale') 
@itemCondition4
@itemCondition8

 
drop table #tempResult
drop table #InitialData

drop table #tempAVGPrice
drop table #tempAVGPriceOrder

";

                    #endregion

                    if (!string.IsNullOrEmpty(vm.AvgDateTime))
                    {
                        sqlText = sqlText.Replace("@itemCondition5", " and ReceiveDate >= @date");
                        sqlText = sqlText.Replace("@itemCondition6", " and pd.TransferDate >= @date");
                        sqlText = sqlText.Replace("@itemCondition7", " and IssueDateTime >= @date");
                        sqlText = sqlText.Replace("@itemCondition8", " and InvoiceDateTime >= @date");
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@itemCondition5", "");
                        sqlText = sqlText.Replace("@itemCondition6", "");
                        sqlText = sqlText.Replace("@itemCondition7", "");
                        sqlText = sqlText.Replace("@itemCondition8", "");
                    }

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        sqlText = sqlText.Replace("@itemCondition4", " and id.ItemNo = @ItemNoSingle");
                        sqlText = sqlText.Replace("@itemCondition3", " and pd.FromItemNo = @ItemNoSingle");
                        sqlText = sqlText.Replace("@itemCondition2", " and ToItemNo = @ItemNoSingle");
                        sqlText = sqlText.Replace("@itemCondition", " and ItemNo = @ItemNoSingle");

                    }
                    else
                    {
                        sqlText = sqlText.Replace("@itemCondition4", "");
                        sqlText = sqlText.Replace("@itemCondition2", "");
                        sqlText = sqlText.Replace("@itemCondition3", "");
                        sqlText = sqlText.Replace("@itemCondition", "");

                    }
                    if (!vm.FromSP)
                    {
                        cmd.CommandText = sqlText;
                        if (!string.IsNullOrEmpty(vm.ItemNo))
                        {
                            cmd.Parameters.AddWithValue("@ItemNoSingle", vm.ItemNo);
                        }
                        if (!string.IsNullOrEmpty(vm.AvgDateTime))
                        {
                            cmd.Parameters.AddWithValue("@date", vm.AvgDateTime);
                        }

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        vm.SPSQLText = sqlText;
                        vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@AvgDateTime", !string.IsNullOrWhiteSpace(vm.AvgDateTime) ? "'" + vm.AvgDateTime + "'" : "1900-01-01");

                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();
                    }

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }
        public ResultVM ProcessFreshStock(ParameterVM vm, SqlConnection currentConnection = null, SqlTransaction currentTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAL = new CommonDAL();
            ResultVM returnVM = new ResultVM();
            try
            {
                commonDAL.ConnectionTransactionOpen(ref currentConnection, ref currentTransaction, ref connection,
                    ref transaction, connVM);
                SqlCommand cmd = new SqlCommand(" ", connection, transaction);


                returnVM = ProductStockProcess(vm, connection, transaction, connVM);

                string updateProductStock = @"

update ProductStocks set CurrentStock = 0


update ProductStocks set CurrentStock=isnull(VAT6_1.RunningTotal,0)
from(

select BranchId,ItemNo,RunningTotal from VAT6_1_Permanent_Branch
where id in(
select id from(
select distinct BranchId,ItemNo,MAX(id)Id from VAT6_1_Permanent_Branch 
where 1=1 @itemCondition1
group by BranchId,ItemNo) as a)
 ) 
as VAT6_1
 where ProductStocks.ItemNo=VAT6_1.ItemNo and ProductStocks.BranchId=VAT6_1.BranchId 
and ProductStocks.ItemNo in(
select ItemNo from Products
where CategoryID in (select CategoryID from ProductCategories where ReportType in('VAT6_1','VAT6_1_And_6_2'))
)


update ProductStocks set CurrentStock=isnull(VAT6_2.RunningTotal,0)
from(

select BranchId,ItemNo,RunningTotal from VAT6_2_Permanent_Branch
where id in(
select id from(
select distinct BranchId,ItemNo,MAX(id)Id from VAT6_2_Permanent_Branch 
where 1=1 @itemCondition1
group by BranchId,ItemNo) as a)
 ) as VAT6_2
 where ProductStocks.ItemNo=VAT6_2.ItemNo and ProductStocks.BranchId=VAT6_2.BranchId 
and ProductStocks.ItemNo in(
select ItemNo from Products
where CategoryID in (select CategoryID from ProductCategories where ReportType in('VAT6_2','VAT6_1_And_6_2'))
)

update ProductStocks set CurrentStock=isnull(VAT6_2_1.RunningTotal,0)
from(

select BranchId,ItemNo,RunningTotal from VAT6_2_1_Permanent_Branch
where id in(
select id from(
select distinct BranchId,ItemNo,MAX(id)Id from VAT6_2_1_Permanent_Branch 
where 1=1 @itemCondition1
group by BranchId,ItemNo) as a)
 ) as VAT6_2_1
 where ProductStocks.ItemNo=VAT6_2_1.ItemNo and ProductStocks.BranchId=VAT6_2_1.BranchId 
and ProductStocks.ItemNo in(
select ItemNo from Products
where CategoryID in (select CategoryID from ProductCategories where ReportType in('VAT6_2_1'))
)

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateProductStock = updateProductStock.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                }
                else
                {
                    updateProductStock = updateProductStock.Replace("@itemCondition1", "");
                }
                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(updateProductStock, connection, transaction);
                    cmd.CommandTimeout = 500;
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = updateProductStock;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");

                    cmd = new SqlCommand("SPExecuteCustomQuery", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                commonDAL.TransactionCommit(ref currentTransaction, ref transaction, connVM);
                returnVM.Status = "success";
                returnVM.Message = "success";
                return returnVM;
            }
            catch (Exception e)
            {
                commonDAL.TransactionRollBack(ref currentTransaction, ref transaction, connVM);

                throw e;
            }
            finally
            {
                commonDAL.CloseConnection(ref currentConnection, ref connection, connVM);
            }
        }
        private ResultVM ProductStockProcess(ParameterVM vm, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            string sqlText = "";
            try
            {



                SqlCommand cmd = new SqlCommand(" ", currConn, transaction);


                sqlText = @"


declare @maxStockId int = Isnull((select max(Id)+1 from ProductStocks),0)

create table  #tempBranches ( ID int identitY(1,1), BranchId int , BranchCode varchar(20))
create table #tempProductStock(ID int identitY(1,1),stockId int null, ItemNo varchar(50), BranchId int , StockQuantity decimal(25,9), StockValue decimal(25,9))

DBCC CHECKIDENT (#tempProductStock, RESEED, @maxStockId)


insert into #tempBranches (BranchId, BranchCode)
select BranchID, BranchCode from BranchProfiles

declare @minID int  = (Select min(ID) from #tempBranches)
declare @maxID int  = (Select max(ID) from #tempBranches)

declare @branchId int = 0

while @minID <= @maxID
begin

	select @branchId = branchId from #tempBranches
	where ID = @minID
	
	insert into #tempProductStock(ItemNo, BranchId, StockQuantity, StockValue)

	select ItemNo, @branchId, Isnull(OpeningBalance,0)OpeningBalance,Isnull(OpeningTotalCost,0)OpeningTotalCost from Products
	where ItemNo  in 
	(	
		select itemNo from Products

		except
		select itemno from ProductStocks
		where BranchId = @branchId
	)

	set @minID = @minID + 1
end
insert into ProductStocks(Id,ItemNo,BranchId,StockQuantity,StockValue,CurrentStock)
select ID,ItemNo,BranchId,StockQuantity,StockValue,0 from #tempProductStock

drop table #tempBranches
drop table #tempProductStock


";
                if (!vm.FromSP)
                {
                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 800;
                    cmd.CommandText = sqlText;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();

                }

                return new ResultVM() { Status = "success", Message = "success" };

            }
            catch (Exception)
            {

                throw;
            }

        }

        public string[] SaveVAT6_2_1_Permanent(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            ProductDAL productDal = new ProductDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);


                #region Opening
                sqlText = @"
Insert into VAT6_2_1_Permanent(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType] 
,[Quantity]
,[SD]
,[UnitCost]   
,[Remarks] 
,[ItemNo]  
,RunningOpeningQuantityFinal
,RunningOpeningValueFinal
,UnitRate)


SELECT distinct 'A'SerialNo, '1900-01-01' StartDateTime, 0 StartingQuantity, 0 StartingAmount,
0 TransID, 'Opening' TransType,
sum(isnull(p.StockQuantity,0)) Quantity,0 SD, sum(isnull(p.StockValue,0)) UnitCost, 'Opening'Remarks, p.ItemNo
,sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost, sum(isnull(p.StockValue,0))/(case when sum(isnull(StockQuantity,0)) =0 then 1 else sum(isnull(StockQuantity,0)) end) UnitRate

FROM ProductStocks p  left outer join Products pd on pd.ItemNo = p.ItemNo
WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_2_1'))
and p.ItemNo not in (select ItemNo from VAT6_2_1_Permanent where transType = 'Opening')

group by p.ItemNo,pd.ProductName";

                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.CommandTimeout = 500;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }
                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_1_Permanent where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate) @itemCondition";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                if (!vm.FromSP)
                {
                    cmd.CommandText = deleteText;
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = deleteText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                vm.PermanentProcess = true;

                string[] result = productDal.StockMovement6_2_1_PermanentProcess(vm, currConn, transaction, connVM);

                string insertToPermanent = @"

delete from ProductStockMISKas where  ProductStockMISKas.TransType='Opening' @itemCondition

update ProductStockMISKas
set ItemNo = P.MasterProductItemNo
from Products p inner join ProductStockMISKas vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

insert into VAT6_2_1_Permanent(
[SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,[BranchId]
	  )
SELECT 
	   [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,[BranchId]
  FROM ProductStockMISKas where UserId = @UserId @itemCondition
   order by ItemNo, StartDateTime, SerialNo
";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", "");
                }

                if (!vm.FromSP)
                {


                    cmd.CommandText = insertToPermanent;

                    cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = insertToPermanent;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@PeriodId", !string.IsNullOrWhiteSpace(vm.PeriodId) ? "'" + Convert.ToDateTime(vm.StartDate).ToString("MMyyyy") + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }


                string generateClosingValues = @"
create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,8), EffectDate datetime,ToDate datetime)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6))

update VAT6_2_1_Permanent set CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.SalesInvoiceNo=VAT6_2_1_Permanent.TransID 
and VAT6_2_1_Permanent.TransType = 'sale'  @itemCondition3

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null ToDate from products
where ItemNo in(select distinct Itemno from VAT6_2_1_Permanent where 1=1 @itemCondition3) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2_1_Permanent where 1=1 @itemCondition3)
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2


update #NBRPrive set  ToDate=RT.RunningTotal
from ( SELECT id,Customerid, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0 
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null
@itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0 
)RT
where RT.Id=#NBRPrive.Id  
and isnull(nullif(customerid,''),0)<=0
and ToDate is null
@itemCondition2


update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost)
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_2_1_Permanent 
where 1=1 @itemCondition3
order by ItemNo,StartDateTime,SerialNo

update VAT6_2_1_Permanent set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2_1_Permanent.Id
@itemCondition3

update VAT6_2_1_Permanent set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where RT.Id=VAT6_2_1_Permanent.Id
@itemCondition3

update VAT6_2_1_Permanent set DeclaredPrice =0

update VAT6_2_1_Permanent set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent.ItemNo
and VAT6_2_1_Permanent.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent.StartDateTime<#NBRPrive.ToDate
and VAT6_2_1_Permanent.CustomerID=#NBRPrive.CustomerId
and isnull(VAT6_2_1_Permanent.DeclaredPrice,0)=0
@itemCondition3


update VAT6_2_1_Permanent set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent.ItemNo
and VAT6_2_1_Permanent.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent.StartDateTime<#NBRPrive.ToDate
and isnull(VAT6_2_1_Permanent.DeclaredPrice,0)=0
@itemCondition3

update VAT6_2_1_Permanent set AdjustmentValue=0 where 1=1 @itemCondition3
update VAT6_2_1_Permanent set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 @itemCondition3
update VAT6_2_1_Permanent set RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition3

update VAT6_2_1_Permanent set Remarks= 'Sale' where 1=1 and Remarks = 'Other' @itemCondition3

drop table #Temp
drop table #NBRPrive
";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    //generateClosingValues = generateClosingValues.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", " and VAT6_2_1_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    //generateClosingValues = generateClosingValues.Replace("@itemCondition1", "");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", "");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", "");
                }
                if (true)
                {

                    cmd.CommandText = generateClosingValues;
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = generateClosingValues;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message.ToString();
                if (transaction != null && Vtransaction == null) { transaction.Commit(); }

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public string[] SaveVAT6_2_1_Permanent_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            ProductDAL productDal = new ProductDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(" ", currConn, transaction);

                #region Opening
                sqlText = @"
Insert into VAT6_2_1_Permanent_Branch(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType] 
,[Quantity]
,[SD]
,[UnitCost]   
,[Remarks] 
,[ItemNo]  
,RunningOpeningQuantityFinal
,RunningOpeningValueFinal
,UnitRate
,BranchId
)


SELECT distinct 'A'SerialNo, '1900-01-01' StartDateTime, 0 StartingQuantity, 0 StartingAmount,
0 TransID, 'Opening' TransType,
sum(isnull(p.StockQuantity,0)) Quantity,0 SD, sum(isnull(p.StockValue,0)) UnitCost, 'Opening'Remarks, p.ItemNo
,sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost, sum(isnull(p.StockValue,0))/(case when sum(isnull(StockQuantity,0)) =0 then 1 else sum(isnull(StockQuantity,0)) end) UnitRate
,p.BranchId
FROM ProductStocks p  left outer join Products pd on pd.ItemNo = p.ItemNo
WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_2_1'))
and p.ItemNo not in (select ItemNo from VAT6_2_1_Permanent_Branch where transType = 'Opening')

group by p.ItemNo,pd.ProductName,p.BranchId";
                if (!vm.FromSP)
                {


                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.CommandTimeout = 500;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = sqlText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }
                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_1_Permanent_Branch where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate) @itemCondition";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                if (!vm.FromSP)
                {


                    cmd.CommandText = deleteText;
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = deleteText;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                    vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");
                    vm.SPSQLText = vm.SPSQLText.Replace("@EndDate", !string.IsNullOrWhiteSpace(vm.EndDate) ? "'" + vm.EndDate + "'" : "2090-01-01");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                #region Insert Query

                string insertToPermanent = @"

delete from ProductStockMISKas where  ProductStockMISKas.TransType='Opening' @itemCondition

update ProductStockMISKas
set ItemNo = P.MasterProductItemNo
from Products p inner join ProductStockMISKas vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

insert into VAT6_2_1_Permanent_Branch(
[SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,[BranchId]
	  )
SELECT 
	   [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,@BranchId
  FROM ProductStockMISKas where UserId = @UserId @itemCondition
  order by ItemNo, StartDateTime, SerialNo
";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", "");
                }



                #endregion

                vm.PermanentProcess = true;

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();
                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList(null, null, null, currConn, transaction, connVM);

                foreach (BranchProfileVM branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;
                    string[] result = productDal.StockMovement6_2_1_PermanentProcess(vm, currConn, transaction, connVM);

                    if (!vm.FromSP)
                    {
                        cmd.CommandText = insertToPermanent;
                        cmd.Parameters.AddWithValueAndParamCheck("@BranchId", branchProfileVm.BranchID);
                        cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                        cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                        cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                        if (!string.IsNullOrEmpty(vm.ItemNo))
                        {
                            cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                        }
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        vm.SPSQLText = insertToPermanent;
                        vm.SPSQLText = vm.SPSQLText.Replace("@BranchId", vm.BranchId != 0 ? "'" + vm.BranchId + "'" : "0");
                        vm.SPSQLText = vm.SPSQLText.Replace("@UserId", !string.IsNullOrWhiteSpace(vm.UserId) ? "'" + vm.UserId + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@PeriodId", !string.IsNullOrWhiteSpace(vm.PeriodId) ? "'" + Convert.ToDateTime(vm.StartDate).ToString("MMyyyy") + "'" : "");
                        vm.SPSQLText = vm.SPSQLText.Replace("@StartDate", !string.IsNullOrWhiteSpace(vm.StartDate) ? "'" + vm.StartDate + "'" : "1900-01-01");

                        cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                        cmd.ExecuteNonQuery();
                    }

                }

                #region Generate Closings

                string generateClosingValues = @"
create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,8), EffectDate datetime,ToDate datetime, BranchId int)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,8),TotalCost decimal(20,8),BranchId int)

update VAT6_2_1_Permanent_Branch set CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.Salesinvoiceno=VAT6_2_1_Permanent_Branch.TransID and TransType='Sale' @itemCondition3

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null,BranchId ToDate from products
where ItemNo in(select distinct Itemno from VAT6_2_1_Permanent_Branch where 1=1 @itemCondition3) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,BranchId from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2_1_Permanent_Branch where 1=1 @itemCondition3)
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

----######################----------------
update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT Customerid,id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null  @itemCondition2

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0
)RT
where RT.Id=#NBRPrive.Id  
and ToDate is null and isnull(nullif(customerid,''),0)<=0  @itemCondition2
----######################----------------

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT6_2_1_Permanent_Branch 
where 1=1 @itemCondition3
order by BranchId,ItemNo,StartDateTime,SerialNo


update VAT6_2_1_Permanent_Branch set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3

update VAT6_2_1_Permanent_Branch set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3

update VAT6_2_1_Permanent_Branch set DeclaredPrice =0

update VAT6_2_1_Permanent_Branch set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent_Branch.ItemNo
and VAT6_2_1_Permanent_Branch.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent_Branch.StartDateTime<#NBRPrive.ToDate
and VAT6_2_1_Permanent_Branch.CustomerID=#NBRPrive.CustomerId
and isnull(VAT6_2_1_Permanent_Branch.DeclaredPrice,0)=0
@itemCondition3

update VAT6_2_1_Permanent_Branch set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent_Branch.ItemNo
and VAT6_2_1_Permanent_Branch.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent_Branch.StartDateTime<#NBRPrive.ToDate
and isnull(VAT6_2_1_Permanent_Branch.DeclaredPrice,0)=0
@itemCondition3


update VAT6_2_1_Permanent_Branch set   RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition3
update VAT6_2_1_Permanent_Branch set AdjustmentValue=0 where 1=1 @itemCondition3
update VAT6_2_1_Permanent_Branch set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 @itemCondition3
update VAT6_2_1_Permanent_Branch set Remarks= 'Sale' where 1=1 and Remarks = 'Other' @itemCondition3


update VAT6_2_1_Permanent_Branch set  RunningOpeningQuantityFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM VAT6_2_1_Permanent_Branch
)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3

 update VAT6_2_1_Permanent_Branch set  RunningOpeningValueFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotalValueFinal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM VAT6_2_1_Permanent_Branch
)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3


drop table #Temp
drop table #NBRPrive
";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {

                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", " and VAT6_2_1_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {

                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", "");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", "");
                }
                if (!vm.FromSP)
                {


                    cmd.CommandText = generateClosingValues;

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    }

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    vm.SPSQLText = generateClosingValues;
                    vm.SPSQLText = vm.SPSQLText.Replace("@ItemNo", !string.IsNullOrWhiteSpace(vm.ItemNo) ? "'" + vm.ItemNo + "'" : "");

                    cmd = new SqlCommand("SPExecuteCustomQuery", currConn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValueAndParamCheck("@Query", vm.SPSQLText);
                    cmd.ExecuteNonQuery();
                }

                #endregion


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message.ToString();
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                throw;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

    }
}
