using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    //public class CmpanyListVM
    //{
    //    public string CompanySl { get; set; }
    //    public string CompanyID { get; set; }
    //    public string CompanyName { get; set; }
    //    public string DatabaseName { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string Serial { get; set; }
    //}

    //public class settingVM
    //{
    //    public static DataTable SettingsDT;
    //}

    //public class DatabaseInfoVM
    //{
    //    public static string DatabaseName { get; set; }
    //    public static string dbUserName { get; set; }
    //    public static string dbPassword { get; set; }
    //    public static string dataSource { get; set; }
    //}

    //public class SuperAdminInfoVM
    //{
    //    public static string dbUserName { get; set; }
    //    public static string dbPassword { get; set; }
    //    public static string dataSource { get; set; }
    //}

    //public class SysDBInfoVM
    //{
    //    public static string SysDatabaseName = "SymphonyVATSys";
    //    public static string SysUserName { get; set; }
    //    public static string SysPassword { get; set; }
    //    public static string SysdataSource { get; set; }

    //}

    //public class UserInfoVM
    //{
    //    public static string UserName { get; set; }
    //    public static string Password { get; set; }
    //    public static string UserId { get; set; }
    //}

    //public class BureauInfoVM
    //{
    //    public static bool IsBureau { get; set; }
    //    public static string SessionDate { get; set; }
    //}

    //public class VAT7VM
    //{
    //    public string VAT7No { get; set; }
    //    public string Vat7DateTime { get; set; }
    //    public string FinishItemNo { get; set; }
    //    public string FinishUOM { get; set; }
    //    public string Vat7LineNo { get; set; }


    //    public string ItemNo { get; set; }
    //    public string ItemUOM { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal UOMQty { get; set; }
    //    public decimal UOMc { get; set; }
    //    public string UOMn { get; set; }

    //    public string Post { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }

    //}

    public class CmpanyListVM
    {
        public string CompanySl { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string DatabaseName { get; set; }
        public string ActiveStatus { get; set; }
        public string Serial { get; set; }
    }

    public class settingVM
    {
        public static DataTable SettingsDT;
        public static DataTable SettingsDTUser;
        public static DataTable UserInfoDT;
        public static DataTable BranchInfoDT;
    }

    public class DatabaseInfoVM
    {
        public static string DatabaseName = "master";// { get; set; }
        public static string dbUserName { get; set; }
        public static string dbPassword { get; set; }
        public static string dataSource { get; set; }
    }

    public class SuperAdminInfoVM
    {
        public static string dbUserName { get; set; }
        public static string dbPassword { get; set; }
        public static string dataSource { get; set; }
    }

    public class SysDBInfoVM
    {
        public static string SysDatabaseName = "SymphonyVATSys";
        public static string SysUserName = "sa";// { get; set; }
        public static string SysPassword = "S123456_";// { get; set; }
        public static string SysdataSource = ".";// { get; set; }

        public static bool IsWindowsAuthentication = false;// { get; set; }

    }
    
    //S123456_
    public class SysDBInfoVMTemp
    {
        public  string SysDatabaseName = "SymphonyVATSys";
        public  string SysUserName = "sa";// { get; set; }
        public  string SysPassword = "S123456_";// { get; set; }
        public  string SysdataSource = ".";// { get; set; }

        #region Property

        public string CompanyID { get; set; }
        public string CompanyNameLog { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLegalName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string TINNo { get; set; }
        public string VatRegistrationNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public DateTime FMonthStart { get; set; }
        public DateTime FMonthEnd { get; set; }
        public decimal VATAmount { get; set; }
        public string IsWCF { get; set; }
        public string Section { get; set; }
        public int BranchId = 0;
        public string BranchCode = "";
        public string CurrentUser { get; set; }
        public string CurrentUserID { get; set; }
        public bool IsLoading { get; set; }
        public string R_F { get; set; }
        public string fromOpen { get; set; }
        public string SalesType { get; set; }
        public string Trading { get; set; }
        public string DatabaseName { get; set; }
        public string[] PublicRollLines { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime SessionTime { get; set; }
        public int ChangeTime { get; set; }
        public DateTime ServerDateTime { get; set; }
        public DateTime vMinDate = Convert.ToDateTime("1753/01/02");
        public DateTime vMaxDate = Convert.ToDateTime("9998/12/30");
        public bool successLogin = false;
        public string FontSize = "8";
        public string Access { get; set; }
        public string Post { get; set; }
        public DateTime LicenceDate { get; set; }
        public DateTime serverDate { get; set; }
        public bool IsTrial = false;
        public string Trial = "";
        public string TrialComments = "Unregister Version";
        public string ImportFileName { get; set; }
        public string ItemType = "Other";
        public bool IsAlpha = false;
        public string Alpha = "";
        public string AlphaComments = "Alpha Version";
        public bool IsBeta = false;
        public string Beta = "";
        public string BetaComments = "Beta Version";
        public string AppPathForRootLocation = AppDomain.CurrentDomain.BaseDirectory;
        public bool IsBureau = false;
        public string Add { get; set; }
        public string Edit { get; set; }
        public bool IsDHLCrat { get; set; }



        #endregion
        public static bool IsWindowsAuthentication = false;// { get; set; }

    }
    public class UserInfoVM
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string UserId = "10"; //{ get; set; }
        public static bool IsMainSetting = true; //{ get; set; }

    }

    public class BureauInfoVM
    {
        public static bool IsBureau { get; set; }
        public static string SessionDate { get; set; }
    }
    public class VAT7MasterVM
    {
        public VAT7VM Master { get; set; }
        public List<VAT7VM> Details { get; set; }
        public string Operation { get; set; }
        public string Post { get; set; }
    }
    public class VAT7VM
    {
        public string Id { get; set; }
        public string VAT7No { get; set; }
        public string Vat7DateTime { get; set; }
        public string FinishItemNo { get; set; }
        public string FinishItemCode { get; set; }
        public string FinishItemName { get; set; }
        public string FinishUOM { get; set; }
        public string Vat7LineNo { get; set; }

        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemUOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UOMQty { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }

        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
        public int BranchId { get; set; }
    }


}
