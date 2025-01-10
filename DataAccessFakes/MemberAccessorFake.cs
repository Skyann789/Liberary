using DataAccessInterfaces;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessFakes
{
    public class MemberAccessorFake : IMemberAccessor
    {
        private List<Member> _members; 
        public MemberAccessorFake() 
        { 

            _members = new List<Member>();

            _members.Add(new Member()
            {
                MemberID = 1,
                MemberGivenName = "Test1",
                MemberFamilyName = "Test1",
                MemberEmail = "test1@test.com",
                PhoneNumber = "1234567890",
                FineID = 1,
                Amount = 25.50m,
                IssueDate = new DateTime(2024, 1, 15),
                Paid = false
            });
            _members.Add(new Member()
            {
                MemberID = 2,
                MemberGivenName = "Test2",
                MemberFamilyName = "Test2",
                MemberEmail = "test2@test.com",
                PhoneNumber = "1234567890",
                FineID = 2,
                Amount = 0.00m,
                IssueDate = DateTime.MinValue,
                Paid = true
            });
            _members.Add(new Member()
            {
                MemberID = 3,
                MemberGivenName = "Test3",
                MemberFamilyName = "Test3",
                MemberEmail = "test3@test.com",
                PhoneNumber = "1234567890",
                FineID = 3,
                Amount = 50.00m,
                IssueDate = new DateTime(2024, 3, 15),
                Paid = false
            });
            

        }

        public bool CreateFine(int memberID, decimal amount, DateTime issueDate)
        {
            var member = _members.FirstOrDefault(m => m.MemberID == memberID);
            if (member == null)
            {
                throw new ApplicationException("Member not found.");
            }

            return true;
        }

        public bool MarkFineAsPaid(int fineID)
        {
            var fine = _members.FirstOrDefault(m => m.FineID == fineID);
            if (fine == null)
            {
                throw new ApplicationException("Fine not found.");
            }

            return true;
        }

        public List<Member> SelectAllMembers()
        {
            return _members;
        }

        public List<Member> SelectFines()
        {
            return _members.Where(m => m.Amount > 0 && !m.Paid).ToList();
        }
    }
}
