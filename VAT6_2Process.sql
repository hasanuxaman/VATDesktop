USE [AGRIVITA2012_Demo_DB]
GO
/****** Object:  StoredProcedure [dbo].[VAT6_2Process]    Script Date: 02/17/2024 1:30:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[VAT6_2Process]
    @JsonData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE 
		
@ItemNo  NVARCHAR(50)
,@StartDate   NVARCHAR(50)
,@EndDate 	  NVARCHAR(50)
,@Post1 	  NVARCHAR(50)
,@Post2 	  NVARCHAR(50)
,@BranchId int
,@Opening  bit
,@Opening6_2 bit
,@ProdutType   NVARCHAR(100)
,@ProdutCategoryId   NVARCHAR(100) 
,@VAT6_2_1 bit
,@StockMovement bit 
,@Flag   NVARCHAR(100)
,@UOMTo   NVARCHAR(100)
,@rbtnService bit
,@rbtnWIP bit
,@IsBureau bit
,@AutoAdjustment bit 
,@PreviewOnly bit
,@InEnglish   NVARCHAR(100)
,@UOMConversion decimal(18,6)
,@UOM   NVARCHAR(100)
,@IsMonthly bit
,@ProdutGroupName   NVARCHAR(100) 
,@ProdutCategoryLike bit
,@IsTopSheet bit
,@UserId   NVARCHAR(100)
,@ReportType   NVARCHAR(100)
,@Permanent bit
,@PermanentProcess bit 
,@SkipOpening bit
,@MainProcess bit
,@DecimalPlace   NVARCHAR(100)
,@QuentitylPlace   NVARCHAR(100)
,@ValuePlace   NVARCHAR(100)
,@BranchWise bit
,@FromPeriodName  NVARCHAR(100) 
,@ToPeriodName   NVARCHAR(100)
,@ProductName   NVARCHAR(100)
,@IsChecked bit
,@PeriodMonth   NVARCHAR(100)
,@FilterProcessItems bit
,@FontSize   NVARCHAR(100)
,@ExportInBDT   NVARCHAR(100)
,@IsExport   NVARCHAR(100)
,@PDesc   NVARCHAR(100)
,@IsBranch bit;
        DECLARE @SqlQuery NVARCHAR(MAX);

        -- Parse JSON string
        SELECT 
	
@ItemNo=JSON_VALUE(@JsonData,'$.ItemNo'),
@StartDate=JSON_VALUE(@JsonData,'$.StartDate'),
@EndDate=JSON_VALUE(@JsonData,'$.EndDate'),
@Post1=JSON_VALUE(@JsonData,'$.Post1'),
@Post2=JSON_VALUE(@JsonData,'$.Post2'),
@BranchId=JSON_VALUE(@JsonData,'$.BranchId'),
@Opening=JSON_VALUE(@JsonData,'$.Opening'),
@Opening6_2=JSON_VALUE(@JsonData,'$.Opening6_2'),
@ProdutType=JSON_VALUE(@JsonData,'$.ProdutType'),
@ProdutCategoryId=JSON_VALUE(@JsonData,'$.ProdutCategoryId'),
@VAT6_2_1=JSON_VALUE(@JsonData,'$.VAT6_2_1'),
@StockMovement=JSON_VALUE(@JsonData,'$.StockMovement'),
@Flag=JSON_VALUE(@JsonData,'$.Flag'),
@UOMTo=JSON_VALUE(@JsonData,'$.UOMTo'),
@rbtnService=JSON_VALUE(@JsonData,'$.rbtnService'),
@rbtnWIP=JSON_VALUE(@JsonData,'$.rbtnWIP'),
@IsBureau=JSON_VALUE(@JsonData,'$.IsBureau'),
@AutoAdjustment=JSON_VALUE(@JsonData,'$.AutoAdjustment'),
@PreviewOnly=JSON_VALUE(@JsonData,'$.PreviewOnly'),
@InEnglish=JSON_VALUE(@JsonData,'$.InEnglish'),
@UOMConversion=JSON_VALUE(@JsonData,'$.UOMConversion'),
@UOM=JSON_VALUE(@JsonData,'$.UOM'),
@IsMonthly=JSON_VALUE(@JsonData,'$.IsMonthly'),
@ProdutGroupName=JSON_VALUE(@JsonData,'$.ProdutGroupName'),
@ProdutCategoryLike=JSON_VALUE(@JsonData,'$.ProdutCategoryLike'),
@IsTopSheet=JSON_VALUE(@JsonData,'$.IsTopSheet'),
@UserId=JSON_VALUE(@JsonData,'$.UserId'),
@ReportType=JSON_VALUE(@JsonData,'$.ReportType'),
@Permanent=JSON_VALUE(@JsonData,'$.Permanent'),
@PermanentProcess=JSON_VALUE(@JsonData,'$.PermanentProcess'),
@SkipOpening=JSON_VALUE(@JsonData,'$.SkipOpening'),
@MainProcess=JSON_VALUE(@JsonData,'$.MainProcess'),
@DecimalPlace=JSON_VALUE(@JsonData,'$.DecimalPlace'),
@QuentitylPlace=JSON_VALUE(@JsonData,'$.QuentitylPlace'),
@ValuePlace=JSON_VALUE(@JsonData,'$.ValuePlace'),
@BranchWise=JSON_VALUE(@JsonData,'$.BranchWise'),
@FromPeriodName=JSON_VALUE(@JsonData,'$.FromPeriodName'),
@ToPeriodName=JSON_VALUE(@JsonData,'$.ToPeriodName'),
@ProductName=JSON_VALUE(@JsonData,'$.ProductName'),
@IsChecked=JSON_VALUE(@JsonData,'$.IsChecked'),
@PeriodMonth=JSON_VALUE(@JsonData,'$.PeriodMonth'),
@FilterProcessItems=JSON_VALUE(@JsonData,'$.FilterProcessItems'),
@FontSize=JSON_VALUE(@JsonData,'$.FontSize'),
@ExportInBDT=JSON_VALUE(@JsonData,'$.ExportInBDT'),
@IsExport=JSON_VALUE(@JsonData,'$.IsExport'),
@PDesc=JSON_VALUE(@JsonData,'$.PDesc'),
@IsBranch=JSON_VALUE(@JsonData,'$.IsBranch');	

set @IsExport='No'

if @ExportInBDT = 'N'
begin
	SELECT @IsExport = CASE WHEN pc.IsRaw = 'Export' THEN 'Yes' ELSE 'No' END   
	from ProductCategories pc join Products p on pc.CategoryID = p.CategoryID 
	where p.ItemNo = @ItemNo;

		IF @@ROWCOUNT = 0
		 begin
		 set @IsExport='No'
		 end
 end
   SET @SqlQuery = '
 ------DECLARE @StartDate DATETIME;
------DECLARE @EndDate DATETIME;
------DECLARE @post1 VARCHAR(2);
------DECLARE @post2 VARCHAR(2);
------DECLARE @ItemNo VARCHAR(20);
------DECLARE @IsExport VARCHAR(20);
------SET @IsExport =@IsExport;
------SET @Itemno=@Itemno;
------SET @post1=@post1;
------SET @post2=@post2;
------SET @StartDate=@StartDate;
------SET @EndDate= @EndDate;
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
 

 select * into #ProductReceive from   ( 
select Products.ItemNo,0 OpeningRate,0 ClosingRate from Products 
left outer join ProductCategories pc
on pc.CategoryID=Products.CategoryID
 where 1=1
 --and Products.BranchId=@BranchId
 ';

if @PermanentProcess=1 AND @FilterProcessItems =1
begin
              set  @SqlQuery = @SqlQuery + 'and Products.ReportType in(''VAT6_2'',''VAT6_1_And_6_2'')  and Products.ProcessFlag=''Y''';
end
else if @PermanentProcess =1and @MainProcess=0
begin
              set  @SqlQuery = @SqlQuery + ' and Products.ReportType in(''VAT6_2'',''VAT6_1_And_6_2'') ';
              set  @SqlQuery = @SqlQuery + ' ';
				if @ItemNo IS not NULL   
				begin
					set  @SqlQuery = @SqlQuery + '  and ItemNo in (@ItemNo) ';
				end
end
if @ProdutType IS not NULL   
begin
		if @Flag='SCBL' 
		begin
              set  @SqlQuery = @SqlQuery + 'and IsRaw in(''Raw'',''Pack'') and Products.ActiveStatus=''Y''';
		end 
		else if @Flag='SCBL_Finish' 
		begin
              set  @SqlQuery = @SqlQuery + ' and IsRaw in(''Finish'',''Export'')  and Products.ActiveStatus=''Y''';
		end 
		else  
		begin
              set  @SqlQuery = @SqlQuery + ' and IsRaw=@ProdutType  and Products.ActiveStatus=''Y''';
		end 

end 
else if @ProdutCategoryId IS not NULL   
begin
              set  @SqlQuery = @SqlQuery + '   and Products.CategoryID=@ProdutCategoryId  and Products.ActiveStatus=''Y''';
end 
else if @ItemNo IS not NULL   
begin
              set  @SqlQuery = @SqlQuery + '   and ItemNo=@ItemNo';
end 
 if @VAT6_2_1=1 
begin
              set  @SqlQuery = @SqlQuery + '  and products.ReportType in(''VAT6_2_1'')';
end 
else
begin
              set  @SqlQuery = @SqlQuery + ' and products.ReportType in(''VAT6_2'',''VAT6_1_And_6_2'')';
end 
              set  @SqlQuery = @SqlQuery + ' ) as a';


if @Opening=0
begin
	if @VAT6_2_1=0
	begin
			set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
			select ''A1'',rd.ReceiveDateTime,rd.ReceiveNo,''Receive'',rd.ItemNo,
			isnull(rd.SubTotal,0) AS SubTotal, 
			isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType
			,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
			from ReceiveDetails RD 
			where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
			and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
			AND (Quantity>0)
			AND (rd.Post =@post1 or rd.Post= @post2)
			AND rd.TransactionType IN(''Other'',''Tender'',''PackageProduction'' 
			,''Wastage'' ,''Trading'',''TradingAuto'',''ExportTrading'',''TradingTender'',''ExportTradingTender'',''InternalIssue'',''Service'',''ExportService''
			,''TradingImport''
			)
			AND rd.BranchId=@BranchId
			insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
			select ''A2'',rd.TransferDate,h.TransferCode,''Receive'',rd.ToItemNo,
			isnull(rd.ReceivePrice,0) AS SubTotal, 
			isnull(NULLIF(ToQuantity,0),isnull(ToQuantity,0) ),0 VATAmount,0 SDAmount,rd.TransactionType 
			,rd.TransferDate,rd.ReceivePrice,0 AdjustmentValue
			from ProductTransfersDetails RD 
			left outer join ProductTransfers h on h.Id =rd.ProductTransferId
			where 1=1
			and rd.TransferDate >= @StartDate and rd.TransferDate <DATEADD(d,1,@EndDate) 
			and rd.ToItemNo in(select distinct ItemNo from #ProductReceive)
			AND (ToQuantity>0)
			AND (rd.Post =@post1 or rd.Post= @post2)
			AND rd.TransactionType IN(''FinishCTC'')
			AND rd.BranchId=@BranchId
			';
		

  
if @StockMovement=0
begin
              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select 'A3', InvoiceDateTime,SalesInvoiceNo,''Receive'',ItemNo
------,CASE WHEN @IsExport='Yes' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType 
,InvoiceDateTime,NBRPrice
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN( ''RawSale'')
AND BranchId=@BranchId';

end

set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType
,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select ''A1'',rd.ReceiveDateTime,rd.ReceiveNo,''Receive'',rd.ItemNo,
CASE WHEN @IsExport=''Yes'' THEN isnull(rd.DollerValue,0) ELSE isnull(rd.SubTotal,0) END AS SubTotal, 
isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),rd.VATAmount,rd.SDAmount,rd.TransactionType 
,rd.ReceiveDateTime,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD 
left outer join products p on RD.ItemNo=p.ItemNo
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN(''TollReceive'')
and pc.IsRaw in(''finish'')
AND rd.BranchId=@BranchId';


              set  @SqlQuery = @SqlQuery + 'insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select ''A1'', rd.ReceiveDateTime,rd.ReceiveNo,''Receive'',rd.ItemNo,
-CASE WHEN @IsExport=''Yes'' THEN isnull(rd.DollerValue,0) ELSE isnull(rd.SubTotal,0) END AS SubTotal, 
-isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ),-rd.VATAmount
,-rd.SDAmount,RD.TransactionType,rd.CreatedOn,rd.CostPrice,rd.AdjustmentValue
from ReceiveDetails RD
where rd.ReceiveDateTime >= @StartDate and rd.ReceiveDateTime <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND RD.TransactionType IN(''ReceiveReturn'')
AND rd.BranchId=@BranchId ';
              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

select ''A5'', ReceiveDate,PurchaseInvoiceNo,''Receive'',ItemNo, SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) ) Quantity,VATAmount,SDAmount, ''Client FG Receive without BOM''
,ReceiveDate,CostPrice
from PurchaseInvoiceDetails rd 
where rd.ReceiveDate >= @StartDate and rd.ReceiveDate <DATEADD(d,1,@EndDate) 
and rd.ItemNo in(select distinct ItemNo from #ProductReceive)
AND (Quantity>0)
AND (rd.Post =@post1 or rd.Post= @post2)
AND rd.TransactionType IN(''ClientFGReceiveWOBOM'')
AND rd.BranchId=@BranchId';

	end -- @VAT6_2_1=0
if	@StockMovement=0
begin
              set  @SqlQuery = @SqlQuery + ' 
			  insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

select ''B10'', InvoiceDateTime,SalesInvoiceNo,''Sale'',ItemNo
------,CASE WHEN @IsExport=''Yes'' THEN isnull(NULLIF(DollerValue,0),0) ELSE isnull(NULLIF(SubTotal,0),0) END AS SubTotal
, CurrencyValue AS SubTotal
,isnull(NULLIF(UOMQty,0),isnull(Quantity,0) )Quantity
,VATAmount
,SDAmount
,SalesInvoiceDetails.TransactionType--''Sale''
,InvoiceDateTime,NBRPrice,AdjustmentValue
from SalesInvoiceDetails
where InvoiceDateTime >= @StartDate and 
InvoiceDateTime < DATEADD(d,1,@EndDate) and ItemNo in(select distinct ItemNo from #ProductReceive)
AND (UOMQty>0)
AND (Post =@post1 or Post= @post2)
AND TransactionType IN(''RawSale'' )
AND BranchId=@BranchId ';
end --@StockMovement=0
              set  @SqlQuery = @SqlQuery + 'insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

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
AND BranchId=@BranchId ';

              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

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
AND rd.BranchId=@BranchId';

  if @StockMovement =0
  begin
              set  @SqlQuery = @SqlQuery + ' ';
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

  end  --@StockMovement =0
              set  @SqlQuery = @SqlQuery + ' 
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
AND BranchId=@BranchId';

              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

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
AND BranchId=@BranchId ';

	if @PermanentProcess=1 and @BranchId != 0)
	begin
              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)

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
AND rd.BranchId=@BranchId';

              set  @SqlQuery = @SqlQuery + ' 
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
AND rd.BranchId=@BranchId';


	end --@PermanentProcess=1 and @BranchId != 0)
              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate,AdjustmentValue)
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
AND BranchId=@BranchId';
 
              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

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
AND BranchId=@BranchId';

              set  @SqlQuery = @SqlQuery + ' insert into #VATTemp_17(SerialNo,Dailydate,TransID,TransType,ItemNo,UnitCost,Quantity,VATRate,SD,remarks,CreatedDateTime,UnitRate)

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
AND BranchId=@BranchId';

end --@Opening=0
              set  @SqlQuery = @SqlQuery + '
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
AND BranchId='1'';

if @Opening6_2 =0
begin

if @Opening =0
begin

	if @BranchId>1
	begin
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
SELECT distinct  ItemNo, isnull(StockQuantity,0) Quantity, isnull(p.StockValue,0) Amount  
FROM ProductStocks p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
AND BranchId>1
--group by ItemNo';

	end ---@BranchId>1
	else
	begin
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
SELECT distinct  ItemNo, isnull(OpeningBalance,0) Quantity, isnull(p.OpeningTotalCost,0) Amount  
FROM Products p  WHERE p.ItemNo  in(select distinct ItemNo from #ProductReceive)
--AND BranchId='1'
------group by ItemNo';

	end --else @BranchId>1

end ---@Opening =0

end --@Opening6_2 =0
 
 if @VAT6_2_1=0
 begin
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
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
) ';
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
(SELECT distinct  ToItemNo,isnull(sum(isnull(ToQuantity,isnull(ToQuantity,0))),0) Quantity,
    isnull(sum(isnull(ReceivePrice,0)),0)  Amount
 FROM ProductTransfersDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND TransferDate>= '07/01/2019' and TransferDate < @StartDate  
AND TransactionType IN('FinishCTC')AND ToItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ToItemNo
) ';
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
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
) ';
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
(
SELECT  distinct ItemNo,isnull(sum(Quantity),0) Quantity, isnull(sum(SubTotal),0)   AS SubTotal
FROM PurchaseInvoiceDetails   WHERE 1=1 
AND (Post =@post1 or Post= @post2)  
AND ReceiveDate>= '07/01/2019' and ReceiveDate < @StartDate  
AND TransactionType IN('ClientFGReceiveWOBOM')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)';

 end --@VAT6_2_1=0

 set  @SqlQuery = @SqlQuery + ' UNION ALL
(SELECT distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM ReceiveDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2)    AND ReceiveDateTime>= '07/01/2019' and ReceiveDateTime < @StartDate  
 and TransactionType IN('ReceiveReturn') AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
) ';

 set  @SqlQuery = @SqlQuery + ' UNION ALL 
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
)';
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
(SELECT distinct  FromItemNo,-1*isnull(sum(isnull(ToQuantity,isnull(ToQuantity,0))),0) Quantity,
   -1* isnull(sum(isnull(ReceivePrice,0)),0)  Amount
 FROM ProductTransfersDetails WHERE 1=1
 AND (Post =@post1 or Post= @post2) AND TransferDate>= '07/01/2019' and TransferDate < @StartDate  
AND TransactionType IN('FinishCTC')AND FromItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by FromItemNo
) ';

if @StockMovement=0
begin
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
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
)';

end --@StockMovement=0
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
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
)';

 set  @SqlQuery = @SqlQuery + ' UNION ALL 
(SELECT  distinct  ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-isnull(sum(isnull(CurrencyValue,0)),0)   AS SubTotal
FROM SalesInvoiceDetails   WHERE 1=1
 AND (Post =@post1 or Post= @post2)  
 AND InvoiceDateTime>= '07/01/2019' and InvoiceDateTime < @StartDate  
AND TransactionType IN('DisposeFinish')
AND ItemNo  in(select distinct ItemNo from #ProductReceive)
AND BranchId=@BranchId
group by ItemNo
)';
 set  @SqlQuery = @SqlQuery + ' UNION ALL  
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

)';
 set  @SqlQuery = @SqlQuery + ' UNION ALL 
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
)';
 set  @SqlQuery = @SqlQuery + ' ) AS a GROUP BY a.ItemNo';
 set  @SqlQuery = @SqlQuery + ' ';

    END TRY
    BEGIN CATCH
        -- Rethrow the error
        THROW;
    END CATCH;
END;
