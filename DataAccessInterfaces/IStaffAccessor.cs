using DataDomain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IStaffAccessor
    {
        int SelectStaffCountByEmailAndPasswordHash(string email, string passwordHash);
        StaffVM SelectStaffByEmail(string email);

        List<string> SelectRolesByStaffID(int StaffID);

        int UpdatePasswordHashByEmail(string email, string oldPasswordHash, string newPasswordHash);

        List<StaffVM> SelectAllStaff();

        int InsertStaff(string givenName, string familyName, string phoneNumber, string email, string roleID);

        int InsertRoleToStaff(int staffID, string roleID);

        int RemoveRoleFromStaff(int staffID, string roleID);

        int DeActivateStaffAndRemoveRole(int staffID);

        int ActivateStaff(int staffID);
    }
}
