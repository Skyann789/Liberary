using DataAccessInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Data;
using System.Data;
using DataDomain;

namespace DataAccessLayer
{
    public class StaffAccessor : IStaffAccessor
    {
        public int InsertStaff(string givenName, string familyName, string phoneNumber, string email, string roleID)
        {

            int rowsAffected = 0;

            using (var conn = DBConnection.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_insert_staff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the staff details and roleID
                    cmd.Parameters.Add("@GivenName", SqlDbType.NVarChar, 50).Value = givenName;
                    cmd.Parameters.Add("@FamilyName", SqlDbType.NVarChar, 100).Value = familyName;
                    cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 11).Value = phoneNumber;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250).Value = email;
                    cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50).Value = roleID;

                    try
                    {
                        conn.Open();

                        // Execute the stored procedure
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Data access error: Insert Staff failed", ex);
                    }
                }
            }

            return rowsAffected;
        }

        public List<StaffVM> SelectAllStaff()
        {
            var staffList = new List<StaffVM>();
            var staffDictionary = new Dictionary<int, StaffVM>(); // To track unique staff with their roles

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_all_staff", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int staffID = reader.GetInt32(0);

                    // Check if this staff member already exists in the dictionary
                    if (!staffDictionary.TryGetValue(staffID, out var staff))
                    {
                        // Create new StaffVM and add to dictionary if not found
                        staff = new StaffVM()
                        {
                            StaffID = staffID,
                            GivenName = reader.GetString(1),
                            FamilyName = reader.GetString(2),
                            Phone = reader.GetString(3),
                            Email = reader.GetString(4),
                            Active = reader.GetBoolean(5),
                            Roles = new List<string>()
                        };
                        staffDictionary.Add(staffID, staff);
                        staffList.Add(staff); // Add to main list as well
                    }

                    // Add role to the staff member's Roles list
                    if (!reader.IsDBNull(6))
                    {
                        string role = reader.GetString(6);
                        staff.Roles.Add(role);
                    }
                }
            }

            return staffList;
        }


        public StaffVM SelectStaffByEmail(string email)
        {
            StaffVM staff = null;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_select_user_by_email", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);

            cmd.Parameters["@Email"].Value = email;

            try
            {
                conn.Open();

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    staff = new StaffVM()
                    {
                        StaffID = reader.GetInt32(0),
                        GivenName = reader.GetString(1),
                        FamilyName = reader.GetString(2),
                        Phone = reader.GetString(3),
                        Email = reader.GetString(4),
                        Active = reader.GetBoolean(5)
                    };
                }
                else
                {
                    throw new ArgumentException("Staff record not found.");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return staff;
        }

        public int SelectStaffCountByEmailAndPasswordHash(string email, string passwordHash)
        {
            int count = 0;
            // we need to get a count of active Staffs with matching credentails 

            // first, we need to get a connection
            var conn = DBConnection.GetConnection();

            //next, we need a command object
            var cmd = new SqlCommand("sp_authenticate_user", conn);

            // set the command type 
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters to the command object
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100);

            // add the values to the parameters
            cmd.Parameters["@Email"].Value = email;
            cmd.Parameters["@PasswordHash"].Value = passwordHash;

            // now to work with the database. That is unsafe code, so it needs a try-catch 
            try
            {
                // open the connection
                conn.Open();

                // execute the command and capture the result
                var result = cmd.ExecuteScalar();
                count = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }

        public List<string> SelectRolesByStaffID(int staffID)
        {
            List<string> roles = new List<string>();

            // connection
            var conn = DBConnection.GetConnection();
            // command
            var cmd = new SqlCommand("sp_select_roles_by_StaffID", conn);
            // command type
            cmd.CommandType = CommandType.StoredProcedure;
            // parameters
            cmd.Parameters.Add("@StaffID", SqlDbType.Int);
            // values
            cmd.Parameters["@StaffID"].Value = staffID;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    roles.Add(reader.GetString(1));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return roles;
        }

        public int UpdatePasswordHashByEmail(string email, string oldPasswordHash, string newPasswordHash)
        {
            int result = 0;

            // connection
            var conn = DBConnection.GetConnection();
            //command 
            var cmd = new SqlCommand("sp_update_passwordhash_by_email", conn);
            // command type
            cmd.CommandType = CommandType.StoredProcedure;
            // paramenters
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@OldPasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 100);
            // values
            cmd.Parameters["@Email"].Value = email;
            cmd.Parameters["@OldPasswordHash"].Value = oldPasswordHash;
            cmd.Parameters["@NewPasswordHash"].Value = newPasswordHash;

            try
            {
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public int InsertRoleToStaff(int staffID, string roleID)
        {
            int rowsAffected = 0;

            using (var conn = DBConnection.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_insert_role_to_staff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the staffID and roleID
                    cmd.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffID;
                    cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50).Value = roleID;

                    try
                    {
                        conn.Open();

                        // Execute the stored procedure
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Data access error: Insert Role to Staff failed", ex);
                    }
                }
            }

            return rowsAffected;
        }

        public int RemoveRoleFromStaff(int staffID, string roleID)
        {
            int rowsAffected = 0;

            using (var conn = DBConnection.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_remove_role_from_staff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the staffID and roleID
                    cmd.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffID;
                    cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50).Value = roleID;

                    try
                    {
                        conn.Open();

                        // Execute the stored procedure
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Data access error: Remove Role from Staff failed", ex);
                    }
                }
            }

            return rowsAffected;
        }



       

        public int DeActivateStaffAndRemoveRole(int staffID)
        {
            int rowsAffected = 0;

            using (var conn = DBConnection.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_deactivate_staff_and_remove_role", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameter for the staff ID
                    cmd.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffID;

                    try
                    {
                        conn.Open();

                        // Execute the stored procedure and get the rows affected
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Data access error: Deactivate Staff failed", ex);
                    }
                }
            }

            return rowsAffected;
        }
        public int ActivateStaff(int staffID)
        {
            int rowsAffected = 0;

            using (var conn = DBConnection.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_activate_staff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    
                    cmd.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffID;

                    try
                    {
                        conn.Open();

                        
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Data access error: Activate Staff failed", ex);
                    }
                }
            }
            return rowsAffected;
        }
    }
}




