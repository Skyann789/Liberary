using DataAccessInterfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDomain;
using DataAccessLayer;


namespace DataAccessLayer
{
    public class MemberAccessor : IMemberAccessor
    {
        public List<Member> SelectFines()
        {
            var fineList = new List<Member>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_fines", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var fine = new Member()
                    {
                        FineID = reader.GetInt32(0),
                        MemberGivenName = reader.GetString(1),
                        MemberFamilyName = reader.GetString(2),
                        MemberEmail = reader.GetString(3),
                        Amount = reader.GetDecimal(4),
                        IssueDate = reader.GetDateTime(5),
                        Paid = reader.GetBoolean(6)
                    };
                    fineList.Add(fine);
                }
            }

            return fineList;
        }
        public bool MarkFineAsPaid(int fineID)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_mark_fine_as_paid", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FineID", fineID);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();


                return rowsAffected == 1;
            }
        }

        public bool CreateFine(int memberID, decimal amount, DateTime issueDate)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_insert_fine", conn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@MemberID", memberID);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@IssueDate", issueDate);

                conn.Open();

                var rowsAffected = cmd.ExecuteNonQuery();


                return rowsAffected == 1;
            }
        }

        public List<Member> SelectAllMembers()
        {
            var memberList = new List<Member>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_all_members", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var member = new Member()
                    {
                        MemberID = reader.GetInt32(0),
                        MemberGivenName = reader.GetString(1),
                        MemberFamilyName = reader.GetString(2),
                        MemberEmail = reader.GetString(3),
                        PhoneNumber = reader.GetString(4)
                    };
                    memberList.Add(member);
                }
            }

            return memberList;
        }


    }
}
