using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using System.Collections;

namespace LogicLayer
{
    public class StaffManager : IStaffManager
    {

        private IStaffAccessor _staffAccessor;

        public StaffManager()
        {
            _staffAccessor = new StaffAccessor();
        }

        public StaffManager(IStaffAccessor staffAccessor)
        {
            _staffAccessor = staffAccessor;
        }

        public bool AuthenticateStaff(string email, string password)
        {
            bool result = false;

            password = HashSHA256(password);
            result = (1 == _staffAccessor.SelectStaffCountByEmailAndPasswordHash(email, password));


            return result;
        }

        public List<string> GetRolesForStaff(int StaffId)
        {
            List<string> roles = null;

            try
            {
                roles = _staffAccessor.SelectRolesByStaffID(StaffId);

                if (roles.Count == 0)
                {
                    throw new Exception("No roles were retrieved from the database.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("No roles were found.", ex);
            }

            return roles;
        }


        public string HashSHA256(string password)
        {
            if (password == "" || password == null)
            {
                throw new ArgumentException("Missing Input.");
            }

            // begin with what you need to return 
            string hashValue = null;

            // cryptographic algorithms run on bits and bytes, so make a byte array
            byte[] data;

            //correct a hash provider object
            using (SHA256 sha256provider = SHA256.Create())
            {
                // hash the input
                data = sha256provider.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // build a string from the result
            var s = new StringBuilder();

            // loop through the bytes to make a string of hex characters
            for (int i = 0; i < data.Length; i++)
            {
                s.Append(data[i].ToString("x2"));
            }

            // convert the result 
            hashValue = s.ToString().ToLower();

            // return once on the last line of the method
            return hashValue;
        }

        public StaffVM LoginStaff(string email, string password)
        {
            StaffVM staffVM = null;

            try
            {
                if (AuthenticateStaff(email, password))
                {
                    staffVM = (StaffVM)RetrieveStaffByEmail(email);
                    if (staffVM != null)
                    {
                        staffVM.Roles = GetRolesForStaff(staffVM.StaffID);
                    }
                }
            }
            catch (Exception)
            {
                throw; // exception would already be wrapped by logic function
            }

            return staffVM;
        }

        public Staff RetrieveStaffByEmail(string email)
        {

            Staff staff = null;

            try
            {

                staff = _staffAccessor.SelectStaffByEmail(email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Login Failed", ex);
            }
            return staff;
        }


        public bool UpdatePassword(string email, string oldPassword, string newPassword)
        {
            bool result = false;
            oldPassword = HashSHA256(oldPassword);
            newPassword = HashSHA256(newPassword);

            try
            {
                result = (1 == _staffAccessor.UpdatePasswordHashByEmail(email, oldPassword, newPassword));
                if (!result)
                {
                    throw new ApplicationException("Duplicate Employee records found.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update Failed.", ex);
            }

            return result;
        }

        public List<StaffVM> SelectAllStaff()
        {
            return _staffAccessor.SelectAllStaff();
        }

        public bool InsertStaff(string givenName, string familyName, string phoneNumber, string email, string roleID)
        {
            try
            {
                int rowsAffected = _staffAccessor.InsertStaff(givenName, familyName, phoneNumber, email, roleID);
                return rowsAffected == 1;  
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Staff insertion failed.", ex);
            }
        }

        public bool InsertRoleToStaff(int staffID, string roleID)
        {
            try
            {
                int rowsAffected = _staffAccessor.InsertRoleToStaff(staffID, roleID);
                return rowsAffected == 1;  
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Role insertion for staff failed.", ex);
            }
        }

        public bool RemoveRoleFromStaff(int staffID, string roleID)
        {
            try
            {
                int rowsAffected = _staffAccessor.RemoveRoleFromStaff(staffID, roleID);
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Role insertion for staff failed.", ex);
            }
        }


        public bool DeActivateStaffAndRemoveRole(int staffID)
        {
            try
            {
                int rowsAffected = _staffAccessor.DeActivateStaffAndRemoveRole(staffID);
                return rowsAffected > 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Deactivation and role removal failed.", ex);
            }
        }

        public bool ActivateStaff(int staffID)
        {
            try
            {
                int rowsAffected = _staffAccessor.ActivateStaff(staffID);
                return rowsAffected > 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Staff activation failed.", ex);
            }
        }

    }
}


