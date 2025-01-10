using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Staff
    {
        public int StaffID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public bool Active { get; set; }
        public string RoleID { get; set; }
        public string StaffFullName => $"{GivenName} {FamilyName}";
    }

    public class StaffVM : Staff
    {
        public List<string> Roles { get; set; }

        // Adds a read-only property to return Roles as a comma-separated string
        public string RolesDisplay => Roles != null ? string.Join(", ", Roles) : string.Empty;
    }

}
