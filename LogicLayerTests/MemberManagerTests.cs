using DataAccessFakes;
using DataDomain;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayerTests
{
    [TestClass]
    public class MemberManagerTests
    {
        private IMemberManager _memberManager;

        [TestInitialize]
        public void TestSetup()
        {
            _memberManager = new MemberManager(new MemberAccessorFake());
        }

        // Testing Create Fine

        [TestMethod]
        public void TestCreateFineValidMember()
        {
            // Arranage
            int memberID = 1;
            decimal amount = 1;
            DateTime issueDate = DateTime.Now;

            // Act
            bool result = _memberManager.CreateFine(memberID, amount, issueDate);

            // Assert
            Assert.IsTrue(result);
        } // returns true ,fine can be created on valid member 

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCreateFineInvalidMember()
        {
            // Arrange
            int memberID = 999; // Non-existent member
            decimal amount = 10.00m;
            DateTime issueDate = DateTime.Now;

            // Act
            _memberManager.CreateFine(memberID, amount, issueDate);

            // Assert is handled by ExpectedException
        } // throws exception for invalid member


        // Testing Mark Fine as Paid
        [TestMethod]
        public void TestMarkFineAsPaidValidFine()
        {
            // Arrange
            int fineID = 1; // Fine exists for member with ID 1

            // Act
            bool result = _memberManager.MarkFineAsPaid(fineID);

            // Assert
            Assert.IsTrue(result);
        }  // returns true, fine can be marked as paid for valid fine

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMarkFineAsPaidInvalidFine()
        {
            // Arrange
            int fineID = 999; // Non-existent fine

            // Act
            _memberManager.MarkFineAsPaid(fineID);

            // Assert is handled by ExpectedException
        } // throws exception for invlaid fine

        // Testing Select All Members 
        [TestMethod]
        public void TestSelectAllMembers()
        {
            // Act
            List<Member> members = _memberManager.SelectAllMembers();

            // Assert
            Assert.IsNotNull(members, "Member list should not be null.");
            Assert.AreEqual(3, members.Count, "Member list should contain exactly 3 members.");
        }  // returns all selected members



        // Testing Select all Fines
        [TestMethod]
        public void TestSelectFines_ShouldReturnOnlyUnpaidFines()
        {
            // Act
            List<Member> unpaidFines = _memberManager.SelectFines();

            // Assert
            Assert.IsNotNull(unpaidFines, "Unpaid fines list should not be null.");
            Assert.AreEqual(2, unpaidFines.Count, "There should be 2 unpaid fines.");
        }
    }
}
