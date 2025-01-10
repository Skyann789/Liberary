using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IStaffManager
    {
        string HashSHA256(string password);

        public StaffVM LoginStaff(string email, string password);

        bool AuthenticateStaff(string email, string password);
        public List<string>GetRolesForStaff(int StaffId);
        Staff RetrieveStaffByEmail(string email);

        public bool UpdatePassword(string email, string oldPassword, string newPassword);

        public List<StaffVM> SelectAllStaff();

        public bool InsertStaff(string givenName, string familyName, string phoneNumber, string email, string roleID);

        public bool InsertRoleToStaff(int staffID, string roleID);
        public bool RemoveRoleFromStaff(int staffID, string roleID);

        public bool DeActivateStaffAndRemoveRole(int staffID);

        public bool ActivateStaff(int staffID);

    }
}
