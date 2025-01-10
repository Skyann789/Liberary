using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class MemberManager : IMemberManager
    {

        private IMemberAccessor _memberAccessor;

        public MemberManager()
        {
            _memberAccessor = new MemberAccessor();
        }

        public MemberManager(IMemberAccessor fineAccessor)
        {
            _memberAccessor = fineAccessor;
        }

        public List<Member> SelectFines()
        {
            try
            {
                return _memberAccessor.SelectFines();

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving fines.", ex);
            }
        }

        public bool MarkFineAsPaid(int fineID)
        {
            try
            {
                return _memberAccessor.MarkFineAsPaid(fineID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while marking the fine as paid.", ex);
            }
        }

        public bool CreateFine(int memberID, decimal amount, DateTime issueDate)
        {
            try
            {
                return _memberAccessor.CreateFine(memberID, amount, issueDate);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while creating the fine.", ex);
            }
        }

        public List<Member> SelectAllMembers()
        {
            try
            {
                return _memberAccessor.SelectAllMembers();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving members.", ex);
            }
        }
    }
}
