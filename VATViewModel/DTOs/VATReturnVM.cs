using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class VATReturnVM
    {
        public string PeriodName { get; set; }
        public int BranchId { get; set; }
        public string Date { get; set; }

        public VATReturnHeaderVM varVATReturnHeaderVM { get; set; }

        public string PeriodStart { get; set; }
        public string Operation { get; set; }

        public bool IsVersion2 { get; set; }

        public string PriodLock { get; set; }

        public string BranchName { get; set; }

        public string post1 { get; set; }
        public string post2 { get; set; }
        public string Post { get; set; }


        public string previousmonth { get; set; }

        public string Description { get; set; }
        public string Section { get; set; }
        public string PartNo { get; set; }
        public decimal Value { get; set; }

        public string TransactionType { get; set; }

        public int PeriodId { get; set; }

        public List<BranchProfileVM> BranchList { get; set; }


    }

    public static class UserFlagExtensions
    {
        /// <summary>
        /// Check if flags contains the specific user flag. This method is more efficient compared to 'HasFlag()'.
        /// </summary>
        /// <param name="haystack">The bunch of flags</param>
        /// <param name="needle">The flag to look for.</param>
        /// <returns>Return true if flag found in flags.</returns>
        public static bool Contains(this UserFlags haystack, UserFlags needle)
        {
            return (haystack & needle) == needle;
        }
    }


    [Flags]
    public enum UserFlags
    {
        Script = 1,                                     // 0x1
        AccountDisabled = 2,                            // 0x2
        HomeDirectoryRequired = 8,                      // 0x8
        AccountLockedOut = 16,                          // 0x10
        PasswordNotRequired = 32,                       // 0x20
        PasswordCannotChange = 64,                      // 0x40
        EncryptedTextPasswordAllowed = 128,             // 0x80
        TempDuplicateAccount = 256,                     // 0x100
        NormalAccount = 512,                            // 0x200
        InterDomainTrustAccount = 2048,                 // 0x800
        WorkstationTrustAccount = 4096,                 // 0x1000
        ServerTrustAccount = 8192,                      // 0x2000
        PasswordDoesNotExpire = 65536,                  // 0x10000 (Also 66048 )
        MnsLogonAccount = 131072,                       // 0x20000
        SmartCardRequired = 262144,                     // 0x40000
        TrustedForDelegation = 524288,                  // 0x80000
        AccountNotDelegated = 1048576,                  // 0x100000
        UseDesKeyOnly = 2097152,                        // 0x200000
        DontRequirePreauth = 4194304,                   // 0x400000
        PasswordExpired = 8388608,                      // 0x800000 (Applicable only in Window 2000 and Window Server 2003)
        TrustedToAuthenticateForDelegation = 16777216,  // 0x1000000
        NoAuthDataRequired = 33554432                   // 0x2000000
    }
}
