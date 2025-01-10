using DataAccessFakes;
using DataAccessInterfaces;
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
    public class BookManagerTests 
    {
        private IBookManager _bookManager;

        [TestInitialize]
        public void TestSetup()
        {
            _bookManager = new BookManager(new BookAccessorFake());
        }


        // Testing Selecting 
        // Testing selecting all authors

        [TestMethod]
        public void TestSelectAllAuthorsReturnsAllAuthors()
        {
            // Act
            var authors = _bookManager.SelectAllAuthors();

            // Assert
            Assert.AreEqual(4, authors.Count); // There are 4 authors in the fake data
            Assert.IsTrue(authors.Any(a => a.AuthorGivenName == "Test1" && a.AuthorFamilyName == "Test1"));
            Assert.IsTrue(authors.Any(a => a.AuthorGivenName == "Test2" && a.AuthorFamilyName == "Test2"));
            Assert.IsTrue(authors.Any(a => a.AuthorGivenName == "Test3" && a.AuthorFamilyName == "Test3"));
            Assert.IsTrue(authors.Any(a => a.AuthorGivenName == "Test4" && a.AuthorFamilyName == "Test4"));
        }


        // Testing selecting all genres
        [TestMethod]
        public void TestSelectAllGenresReturnsAllGenres()
        {
            // Act
            var genres = _bookManager.SelectAllGenres();

            // Assert
            Assert.AreEqual(4, genres.Count); // There are 4 genres in the fake data
            Assert.IsTrue(genres.Any(g => g.GenreName == "Test1"));
            Assert.IsTrue(genres.Any(g => g.GenreName == "Test2"));
            Assert.IsTrue(genres.Any(g => g.GenreName == "Test3"));
            Assert.IsTrue(genres.Any(g => g.GenreName == "Test4"));
        }



        // Testing selecting All Books
        [TestMethod]
        public void TestSelectAllBooksReturnsAllBooks()
        {
            // Act
            var books = _bookManager.SelectAllBooks();

            // Assert
            Assert.AreEqual(4, books.Count); 
        } // returns true 4 books

        // Testing selecting Avaiable books

        [TestMethod]
        public void TestSelectBooksByAvailableStatusReturnsAvailableBooks()
        {
            // Act
            var availableBooks = _bookManager.SelectBooksByAvailableStatus();

            // Assert
            Assert.AreEqual(1, availableBooks.Count); // Only 1 book is available in the fake data
            Assert.AreEqual("Test1", availableBooks.First().Title);
        } // returns 1 book

        // Testing selcting reserved books
        [TestMethod]
        public void TestSelectReservedBooksRetursnReservedBooks()
        {
            // Act
            var reservedBooks = _bookManager.SelectReservedBooks();

            // Assert
            Assert.AreEqual(1, reservedBooks.Count); // Only 1 book is reserved in the fake data
            Assert.AreEqual("Test2", reservedBooks.First().Title);
        } // returns 1 book
        

        // Testing selecting Maintenace Books
        [TestMethod]
        public void TestSelectMaintenanceBooksReturnsMaintenanceBooks()
        {
            // Act
            var maintenanceBooks = _bookManager.SelectMaintenanceBooks();

            // Assert
            Assert.AreEqual(1, maintenanceBooks.Count); // Only 1 book is in maintenance in the fake data
            Assert.AreEqual("Test4", maintenanceBooks.First().Title);
        } // returns 1 book

        // Updating 
        [TestMethod]
        public void TestMoveReservedBookToCheckInUpdatesStatusToCheckedIn()
        {
            // Arrange
            var bookID = "B2"; // Reserved book

            // Act
            var result = _bookManager.MoveReservedBookToCheckIn(bookID); // Calls the method which updates the book's status to CheckIn

            // Assert
            // Retrieves the updated books from the list of all books using the bookid
            var book = _bookManager.SelectAllBooks().First(b => b.BookID == bookID);
            // Validates the result of the method is true
            Assert.IsTrue(result); 
            Assert.AreEqual("CheckedIn", book.StatusID); // Status should be updated to "CheckedIn"
        }

        [TestMethod]
        public void UpdateAvailableToReservedBookUpdatesStatusAndReservationDetails() 
        {
            // Arrange
            var bookID = "B1"; // Available book
            int staffID = 1;
            int memberID = 1;
            DateTime reservationDate = DateTime.Now;

            // Act
            var result = _bookManager.UpdateAvailableToReservedBook(bookID, staffID, memberID, reservationDate);

            // Assert
            var book = _bookManager.SelectAllBooks().First(b => b.BookID == bookID);
            Assert.IsTrue(result); 
            Assert.AreEqual("Reserved", book.StatusID); // Status should be updated to "Reserved"
            Assert.AreEqual(memberID, book.ReservedBy.MemberID); // MemberID should match the one passed
            Assert.AreEqual(staffID, book.ReservedByStaff.StaffID); // StaffID should match the one passed
            Assert.AreEqual(reservationDate.Date, book.ReservationDate.Date); // Reservation date should match
        }

        [TestMethod]
        public void TestUpdateCheckInToAvailableBook_ShouldUpdateStatusToAvailable()
        {
            // Arrange
            var bookID = "B3"; // CheckedIn book

            // Act
            var result = _bookManager.UpdateCheckInToAvailableBook(bookID);

            // Assert
            var book = _bookManager.SelectAllBooks().First(b => b.BookID == bookID);
            Assert.IsTrue(result);
            Assert.AreEqual("Available", book.StatusID); // Status should be updated to "Available"
        }

        [TestMethod]
        public void TestUpdateCheckInToMaintenanceBook_ShouldUpdateStatusToMaintenance()
        {
            // Arrange
            var bookID = "B3"; // CheckedIn book
            int staffID = 2;
            DateTime maintenanceDate = DateTime.Now;
            string description = "Minor repair needed";

            // Act
            var result = _bookManager.UpdateCheckInToMaintenanceBook(bookID, staffID, maintenanceDate, description);

            // Assert
            var book = _bookManager.SelectAllBooks().First(b => b.BookID == bookID);
            Assert.IsTrue(result);
            Assert.AreEqual("Maintenance", book.StatusID); // Status should be updated to "Maintenance"
            Assert.AreEqual(staffID, book.MaintenanceByStaff.StaffID); // StaffID should match the one passed
            Assert.AreEqual(maintenanceDate.Date, book.MaintenanceDate.Date); // Maintenance date should match
            Assert.AreEqual(description, book.Description); // Description should match
        }

        [TestMethod]
        public void TestUpdateMaintenanceToAvailableBook()
        {
            // Arrange
            var bookID = "B4"; // Maintenance book

            // Act
            var result = _bookManager.UpdateMaintenanceToAvailableBook(bookID);

            // Assert
            var book = _bookManager.SelectAllBooks().First(b => b.BookID == bookID);
            Assert.IsTrue(result);
            Assert.AreEqual("Available", book.StatusID); // Status should be updated to "Available"
        }


        // Testing inserting books
        [TestMethod]
        public void TestInsertBookAddsBookToList()
        {
            // Arrange
            var newBook = new Book()
            {
                BookID = "B5",
                StatusID = "Available",
                AuthorGivenName = "NewAuthor",
                AuthorFamilyName = "NewFamily",
                GenreName = "NewGenre",
                Title = "NewTitle",
                PublishedYear = 2022,
                Active = true,
                ReservedBy = null,
                ReservedByStaff = null,
                ReservationDate = DateTime.MinValue,
                MaintenanceDate = DateTime.MinValue,
                MaintenanceByStaff = null,
                Description = "New description"
            };

            // Act
            var result = _bookManager.InsertBook(newBook);

            // Assert
            var insertedBook = _bookManager.SelectAllBooks().FirstOrDefault(b => b.BookID == newBook.BookID);
            Assert.IsTrue(result); 
            Assert.IsNotNull(insertedBook); // The book should be added to the list
            Assert.AreEqual(newBook.Title, insertedBook.Title); // Title should match
            Assert.AreEqual(newBook.AuthorGivenName, insertedBook.AuthorGivenName); // Author given name should match
            Assert.AreEqual(newBook.GenreName, insertedBook.GenreName); // Genre name should match
            Assert.AreEqual(newBook.PublishedYear, insertedBook.PublishedYear); // Published year should match
        } // returns book added to list

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInsertBookBookIDAlreadyExists() 
        {
            // Arrange
            var duplicateBook = new Book()
            {
                BookID = "B1", // BookID already exists in the fake data
                StatusID = "Available",
                AuthorGivenName = "Author",
                AuthorFamilyName = "Family",
                GenreName = "Genre",
                Title = "Title",
                PublishedYear = 2023,
                Active = true
            };
            _bookManager.InsertBook(duplicateBook);
        } // fails due to duplicate bookid throws exception

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInsertBookhBookIsNull()
        {
            // Arrange
            Book nullBook = null;

            // Act
            _bookManager.InsertBook(nullBook);
        } // fails due to null book, throws exception
    }
}
