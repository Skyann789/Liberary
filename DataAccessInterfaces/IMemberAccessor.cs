using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IMemberAccessor
    {
        List<Member> SelectFines();

        bool MarkFineAsPaid(int fineID);

        bool CreateFine(int memberID, decimal amount, DateTime issueDate);

        List<Member> SelectAllMembers();

    }
}
