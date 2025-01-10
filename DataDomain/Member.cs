using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Member
    {
        public int MemberID { get; set; }
        public string MemberGivenName { get; set; }
        public string MemberFamilyName { get; set; }
        public string MemberEmail { get; set; }
        public string PhoneNumber { get; set; }

        // Member Fine Information
        public int FineID { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Paid { get; set; }

    // Concatenates the given name and the family name to form the full name of the member
    public string MemberFullName => $"{MemberGivenName} {MemberFamilyName}";
    }
}
