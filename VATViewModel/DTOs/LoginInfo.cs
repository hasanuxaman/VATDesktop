namespace VATViewModel.DTOs
{
    public class LoginInfo
    {
        public string BranchCode { get; set; }
        public int BranchId { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentUserId { get; set; }



        public string SysdataSource { get; set; }

        public string SysDatabaseName { get; set; }

        public string SysPassword { get; set; }

        public string SysUserName { get; set; }

        public bool IsWindowsAuthentication { get; set; }
        public string DatabaseName { get; set; }
    }
}