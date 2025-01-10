using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessFakes;
using LogicLayer;
using DataDomain;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;

namespace LogicLayerTests
{
    [TestClass]
    public class StaffManagerTests
    {
        private IStaffManager? _staffManager;

        [TestInitialize]            // this method will run before each test 
        public void TestSetup()
        {
            _staffManager = new StaffManager(new StaffAccessorFake()); // needs data fakes
        }


        // Testing InsertRoleToStaff
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // fails no matching staff
        public void TestInsertRoleToStaffNoMatchingStaff()
        {
            // Arrange
            const int staffID = 999;  // This staff does not exist
            const string roleID = "Role1";  // This is a valid role ID

            // Act
            _staffManager.InsertRoleToStaff(staffID, roleID);  // Should throw an exception for no matching staff

            // Assert - Exception is expected, no need for assertion
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))] // fails due to empty roleID
        public void TestInsertRoleToStaffInvalidRoleID()
        {
            // Arrange
            const int staffID = 1;  // Existing staff ID
            const string roleID = "";  // Invalid role ID (empty string)

            // Act
            _staffManager.InsertRoleToStaff(staffID, roleID);  // Should throw ApplicationException due to invalid role ID

            // Assert - Exception is expected, no need for assertion
        }

        [TestMethod]
        public void TestInsertRoleToStaffRoleAlreadyExists()  // returns 0, role already assigned
        {
            // Arrange
            const int staffID = 1;
            const string roleID = "Role1";  // This role already exists for the staff
            const bool expectedResult = false;  // Role already exists, so no insertion
            bool actualResult;

            // Act
            actualResult = _staffManager.InsertRoleToStaff(staffID, roleID);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, "Expected no insertion since the role already exists.");
        }

        [TestMethod]
        public void TestInsertRoleToStaffSuccess()  // returns 1, adds new role
        {
            // Arrange
            const int staffID = 1;
            const string roleID = "Role3";  // This is a new role not yet assigned to the staff
            const bool expectedResult = true;  // Successfully added the new role
            bool actualResult;

            // Act
            actualResult = _staffManager.InsertRoleToStaff(staffID, roleID);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, "Expected the new role to be added successfully.");
        }



        // Testing activateStaff
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // fails no matching staff
        public void TestActivateStaffNoMatchingStaff()
        {
            // Arrange
            const int staffID = 999;  // This staff does not exist

            // Act
            _staffManager.ActivateStaff(staffID);  

            // Assert - Exception is expected
        }


        // Testing DeActivateStaffAndRemoveRole 
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // Fails if staff has no roles after deactivation
        public void TestDeActivateStaffAndRemoveRoleNoRolesLeft()
        {
            // Arrange
            const int staffID = 3;  // Staff with ID 3 exists and is inactive with no roles

            // Act
            bool result = _staffManager.DeActivateStaffAndRemoveRole(staffID);

            // Assert - Exception is expected as staff should have no remaining roles
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // Fails if no staff found after roles removed
        public void TestDeActivateStaffAndRemoveRoleWithNoRoles()
        {
            // Arrange
            const int staffID = 4;  // Staff with ID 4 does not exist

            // Act
            _staffManager.DeActivateStaffAndRemoveRole(staffID);

            // Assert - Exception is expected 
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // Fails if no matching staff
        public void TestDeActivateStaffAndRemoveRoleStaffNotFound()
        {
            // Arrange
            const int staffID = 999;  // Staff with ID 999 does not exist

            // Act
            _staffManager.DeActivateStaffAndRemoveRole(staffID); 

            // Assert - Exception is expected
        }


        // Testing RemoveRoleFromStaff
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // fails no matching role
        public void TestRemoveRoleFromStaffNoMatchingRole()
        {
            // Arrange
            const int staffID = 1;
            const string roleID = "NonExistentRole";  // This role doesn't exist for staff

            // Act
            _staffManager.RemoveRoleFromStaff(staffID, roleID);  

            // Assert - Exception is expected, no need for assertion
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]  // fails no matching staff
        public void TestRemoveRoleFromStaffNoMatchingStaff()
        {
            // Arrange
            const int staffID = 999;  // This staff does not exist
            const string roleID = "Role1";  // This role exists

            // Act
            _staffManager.RemoveRoleFromStaff(staffID, roleID);  // Should throw an exception for no matching staff

            // Assert - Exception is expected, no need for assertion
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))] // fails due to invalid roleid
        public void TestRemoveRoleFromStaffInvalidRoleID() 
        {
            // Arrange
            const int staffID = 1; 
            const string roleID = "";  // Invalid roleID (empty string)

            // Act
            _staffManager.RemoveRoleFromStaff(staffID, roleID); // Should throw ApplicationException

            // Assert - exception is expected, no need for assertion
        }

        [TestMethod]
        public void TestRemoveRoleFromStaffLastRole() // returns true, removes last role 
        {
            // Arrange
            const int staffID = 1; 
            const string roleID = "Role1";  //  this is the last role for the staff
            const bool expectedResult = true;  // Successfully removed
            bool actualResult;

            // Act
            actualResult = _staffManager.RemoveRoleFromStaff(staffID, roleID);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, "Expected the last role to be removed successfully.");
        }

        [TestMethod]
        public void TestRemoveRoleFromStaffSuccess() // returns true, removes role
        {
            // Arrange
            const int staffID = 1;
            const string roleID = "Role1";  //  this role exists for the staff
            const bool expectedResult = true;  // Successfully removed
            bool actualResult;

            // Act
            actualResult = _staffManager.RemoveRoleFromStaff(staffID, roleID);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, "Expected the role to be removed successfully.");
        } 

        // Testing Insert Staff
        [TestMethod]
        public void TestInsertStaff() // returns true, successful staff insertion
        {
            // arrange
            const string givenName = "Test4";
            const string familyName = "Test4";
            const string phone = "1234567891";
            const string email = "test4@test.com";
            const string roleID = "Role2";
            const bool expectedResult = true;
            bool actualResult;

            // act
            actualResult = _staffManager.InsertStaff(givenName, familyName, phone, email, roleID);
            var newStaff = _staffManager.RetrieveStaffByEmail(email);

            // assert
            Assert.AreEqual(expectedResult, actualResult, "Expected the staff insertion to succeed.");
            Assert.IsNotNull(newStaff, "New staff should have been added.");
            Assert.AreEqual(givenName, newStaff.GivenName, "Given name mismatch.");
            Assert.AreEqual(familyName, newStaff.FamilyName, "Family name mismatch.");
            Assert.AreEqual(phone, newStaff.Phone, "Phone mismatch.");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInsertStaffDuplicateEmail() 
        {
            // arrange
            const string givenName = "Test1";
            const string familyName = "Test1";
            const string phone = "1234567890";
            const string email = "test1@test.com"; // Existing email in fake data
            const string roleID = "Role1";

            // act
            _staffManager.InsertStaff(givenName, familyName, phone, email, roleID);

            // assert
            // Exception is expected
        } // fails due to duplicate email


        // Testing RetrieveStaffByEmail
        [TestMethod]
        public void TestRetrieveStaffByEmail() // returns the correct staff
        {
            // arrange
            const int expectedStaffID = 1;
            const string email = "test1@test.com";
            int actualStaffID = 0;

            // act
            var Staff = _staffManager.RetrieveStaffByEmail(email);
            actualStaffID = Staff.StaffID;

            // assert
            Assert.AreEqual(expectedStaffID, actualStaffID);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestRetrieveStaffByBadEmail() // fails due to bad email
        {
            // arrange
            const string email = "bad@email.com";

            // act
            _staffManager.RetrieveStaffByEmail(email);

            // assert
            // exception test, nothing to do
        }

        // Testing AuthenticateStaff
        [TestMethod]
        public void TestAuthenticateStaff() // returns true, good email and password
        {
            // arrange
            const string email = "test1@test.com";
            const string password = "password";
            const bool expectedResult = true;
            bool actualResult = false;

            // act -- capture the result
            actualResult = _staffManager.AuthenticateStaff(email, password);
            
            // assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestAuthenticateStaffBadEmail() // returns false
        {
            // arrange
            const string email = "bad1@test.com";
            const string password = "password";
            const bool expectedResult = false;
            bool actualResult = true;

            // act -- capture the result
            actualResult = _staffManager.AuthenticateStaff(email, password);

            // assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestAuthenticateStaffBadPassword()
        {
            // arrange
            const string email = "test1@test.com";
            const string password = "badpassword";
            const bool expectedResult = false;
            bool actualResult = true;

            // act -- capture the result
            actualResult = _staffManager.AuthenticateStaff(email, password);

            // assert
            Assert.AreEqual(expectedResult, actualResult);
        } // returns false

        [TestMethod]
        public void TestAuthenticateForInactiveStaff() // returns false
        {
            // arrange
            const string email = "test3@test.com";
            const string password = "password";
            const bool expectedResult = false; // suppose to fail
            bool actualResult = true; // opposite of what we are looking for

            // act -- capture the result
            actualResult = _staffManager.AuthenticateStaff(email, password);

            // assert
            Assert.AreEqual(expectedResult, actualResult);
        }


        // Testing HashSHA256
        [TestMethod]
        public void TestHashSHA256() // returns correct result
        {
            // arrange
            const string valueToHash = "newuser";
            const string expectedHash = "9c9064c59f1ffa2e174ee754d2979be80dd30db552ec03e7e327e9b1a4bd594e";
            var actualHash = "";

            // act
            actualHash = _staffManager.HashSHA256(valueToHash);

            // assert
            Assert.AreEqual(expectedHash, actualHash);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetHashSHA256ThrowsForEmptyString() // throws ArugmentException for Empty String
        {
            // arrange
            const string valueToHash = "";

            // act
            _staffManager.HashSHA256(valueToHash);

            // assert 
            // nothing to do: looking for exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetHashSHA256ThrowsForNull() // throws an arugment exeception for null
        {
            // arrange
            const string? valueToHash = null;

            // act
            _staffManager.HashSHA256(valueToHash);

            // assert 
            // nothing to do: looking for exception
        }


        // Testing SelectAllStaff
        [TestMethod]
        public void TestSelectAllStaff() // returns true, that the staff list is not null and has elements
        {
            // arrange
            const int expectedStaffCount = 3; 

            // act
            var allStaff = _staffManager.SelectAllStaff();  // This calls the SelectAllStaff method

            // assert
            Assert.IsNotNull(allStaff, "The staff list should not be null.");
            Assert.AreEqual(expectedStaffCount, allStaff.Count, "The number of staff members is incorrect.");
        }

    }
}
