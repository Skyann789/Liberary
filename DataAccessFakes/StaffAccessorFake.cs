using DataAccessInterfaces;
using DataDomain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessFakes
{
    public class StaffAccessorFake : IStaffAccessor
    {

        private List<StaffVM> _staffs;
        private List<StaffRole> _staffRoles;

        public StaffAccessorFake()
        {
            _staffs = new List<StaffVM>();
            _staffRoles = new List<StaffRole>();
            _staffRoles.Add(new StaffRole() { StaffID = 1, RoleID = "Role1" });
            _staffRoles.Add(new StaffRole() { StaffID = 1, RoleID = "Role2" });
            _staffRoles.Add(new StaffRole() { StaffID = 2, RoleID = "Role3" });

            _staffs.Add(new StaffVM()
            {
                StaffID = 1,
                GivenName = "Test1",
                FamilyName = "Test1",
                Email = "test1@test.com",
                Phone = "1234567890",
                PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                Active = true
            });
            _staffs.Add(new StaffVM()
            {
                StaffID = 2,
                GivenName = "Test2",
                FamilyName = "Test2",
                Email = "test2@test.com",
                Phone = "1234567890",
                PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                Active = true
            });
            _staffs.Add(new StaffVM()
            {
                StaffID = 3,
                GivenName = "Test3",
                FamilyName = "Test3",
                Email = "test3@test.com",
                Phone = "1234567890",
                PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                Active = false
            });
        }

        public StaffVM SelectStaffByEmail(string email)
        {
            foreach (StaffVM emp in _staffs)
            {
                if (emp.Email == email)
                {
                    return emp;
                }
            }
            throw new ArgumentException("Staff record not found");
        }

        public int SelectStaffCountByEmailAndPasswordHash(string email, string passwordHash)
        {
            int count = 0;
            foreach (var Staff in _staffs)
            {
                if (Staff.Email == email && Staff.PasswordHash == passwordHash && Staff.Active == true)
                {
                    count++;
                }
            }
            return count;
        }
        public List<string> SelectRolesByStaffID(int StaffID)
        {
            List<string> roles = new List<string>();
            foreach (var StaffRole in _staffRoles)
            {
                if (StaffRole.StaffID == StaffID)
                {
                    roles.Add(StaffRole.RoleID);
                }
            }
            return roles;
        }

        public int UpdatePasswordHashByEmail(string email, string oldPasswordHash, string newPasswordHash)
        {
            int count = 0;

            for (int i = 0; i < _staffs.Count; i++)
            {
                if (_staffs[i].Email == email && _staffs[i].PasswordHash == oldPasswordHash)
                {
                    _staffs[i].PasswordHash = newPasswordHash;
                    count++;
                }
            }
            if (count == 0)
            {
                throw new ArgumentException("Employee record not found.");
            }
            return count;
        }

        public List<StaffVM> SelectAllStaff()
        {
            return _staffs;
        }

        public int InsertStaff(string givenName, string familyName, string phoneNumber, string email, string roleID)
        {
            // Checks if the email already exists
            if (_staffs.Any(staff => staff.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Email already exists.");
            }
            // Generates a new unique StaffID
            int newStaffID = _staffs.Any() ? _staffs.Max(staff => staff.StaffID) + 1 : 1;

            // Creates and adds the new staff member
            var newStaff = new StaffVM
            {
                StaffID = newStaffID,
                GivenName = givenName,
                FamilyName = familyName,
                Phone = phoneNumber,
                Email = email,
                PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                Active = true
            };
            _staffs.Add(newStaff);

            // Assign the role to the staff member
            _staffRoles.Add(new StaffRole { StaffID = newStaffID, RoleID = roleID });

            return 1;
        }

        public int RemoveRoleFromStaff(int staffID, string roleID)
        {
            if (string.IsNullOrEmpty(roleID))
            {
                throw new ApplicationException("Invalid roleID provided."); // Throw exception for invalid roleID
            }

            var staffRoleToRemove = _staffRoles.FirstOrDefault(sr => sr.StaffID == staffID && sr.RoleID == roleID);

            // Check if no matching staff-role found and throw an exception
            if (staffRoleToRemove == null)
            {
                throw new ApplicationException("No matching staff-role found to remove.");
            }

            // If role found, remove it
            _staffRoles.Remove(staffRoleToRemove);
            return 1;  // Successfully removed
        }

        public int DeActivateStaffAndRemoveRole(int staffID)
        {
            var staff = _staffs.FirstOrDefault(s => s.StaffID == staffID);
            if (staff == null)
            {
                throw new ApplicationException("Staff member not found");
            }

            staff.Active = false;
            _staffRoles.RemoveAll(sr => sr.StaffID == staffID); // remove roles associated with staff member

            // Check if the staff member has any remaining roles
            var remainingRoles = _staffRoles.Where(sr => sr.StaffID == staffID).ToList();
            if (!remainingRoles.Any())
            {
                throw new ApplicationException("Staff has no remaining roles.");
            }

            return 1;  // Successfully deactivated and removed roles
        }


        public int ActivateStaff(int staffID)
        {
            var staff = _staffs.FirstOrDefault(s => s.StaffID == staffID);
            if (staff == null)
            {
                throw new ApplicationException("Staff member not found.");
            }

            if (staff.Active)
            {
                throw new InvalidOperationException("Staff member is already active.");
            }

            staff.Active = true;
            return 1;  // Successfully activated

        }

        public int InsertRoleToStaff(int staffID, string roleID)
        {

            if (string.IsNullOrEmpty(roleID))  // Check if the roleID is invalid (empty string)
            {
                throw new ApplicationException("Role ID cannot be empty.");
            }

            var staff = _staffs.FirstOrDefault(s => s.StaffID == staffID);
            if (staff == null)  // If staff member doesn't exist, throw an exception
            {
                throw new ApplicationException("Staff member not found.");
            }

            var existingRole = _staffRoles.FirstOrDefault(sr => sr.StaffID == staffID && sr.RoleID == roleID);
            if (existingRole == null)  // If the role doesn't already exist for the staff
            {
                _staffRoles.Add(new StaffRole { StaffID = staffID, RoleID = roleID });
                return 1;  // Role successfully added
            }

            return 0;  // Role already exists for the staff member
        }
    }

    internal class StaffRole
    {
        public int StaffID { get; set; }
        public string RoleID { get; set; }
    }
}
