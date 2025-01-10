using DataAccessInterfaces;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessFakes
{
    public class BookAccessorFake : IBookAccessor
    {
        private List<Book> _books;

        public BookAccessorFake ()
        {
            _books = new List<Book>();

            _books.Add(new Book()
            {
                BookID = 1,
                StatusID = "Available",
                AuthorGivenName = "Test1",
                AuthorFamilyName = "Test1",
                GenreName = "Test1",
                Title = "Test1",
                PublishedYear = 2021,
                Active = true,
                ReservedBy = null,
                ReservedByStaff = null,
                ReservationDate = DateTime.MinValue,
                MaintenanceDate = DateTime.MinValue,
                MaintenanceByStaff = null,
                Description = null
            });
            _books.Add(new Book()
            {
                BookID = 2,
                StatusID = "Reserved",
                AuthorGivenName = "Test2",
                AuthorFamilyName = "Test2",
                GenreName = "Test2",
                Title = "Test2",
                PublishedYear = 2020,
                Active = true,
                ReservedBy = new Member { MemberID = 1, MemberGivenName = "Alice", MemberFamilyName = "Johnson" },
                ReservedByStaff = new Staff { StaffID = 1, GivenName = "Test2", FamilyName = "Test2"},
                ReservationDate = DateTime.Now,
                MaintenanceDate = DateTime.MinValue,
                MaintenanceByStaff = null,
                Description = null
            });
            _books.Add(new Book()
            {
                BookID = 3,
                StatusID = "CheckIn",
                AuthorGivenName = "Test3",
                AuthorFamilyName = "Test3",
                GenreName = "Test3",
                Title = "Test3",
                PublishedYear = 2018,
                Active = true,
                ReservedBy = null,
                ReservedByStaff = null,
                ReservationDate = DateTime.MinValue,
                MaintenanceDate = DateTime.MinValue,
                MaintenanceByStaff = null,
                Description = null
            });
            _books.Add(new Book()
            {
                BookID = 4,
                StatusID = "Maintenance",
                AuthorGivenName = "Test4",
                AuthorFamilyName = "Test4",
                GenreName = "Test4",
                Title = "Test4",
                PublishedYear = 2019,
                Active = true,
                ReservedBy = null,
                ReservedByStaff = null,
                ReservationDate = DateTime.MinValue,
                MaintenanceDate = DateTime.Now,
                MaintenanceByStaff = new Staff { StaffID = 2, GivenName = "Test4", FamilyName = "Test4"},
                Description = "Covering damage."
            });
        }

        // Inserting code section
        public bool InsertBook(Book book)
        {
            // Checks if the book is null
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");
            }
            // Checks if a book with the same ID already exists
            if (_books.Any(b => b.BookID == book.BookID))
            {
                throw new ArgumentException($"A book with ID {book.BookID} already exists.");
            }

            // Adds the new book to the collection
            _books.Add(book);
            return true;
        }

        // Selecting code section

        // Selecting all authors
        public List<Book> SelectAllAuthors()
        {
            // Return a distinct list of authors based on AuthorGivenName and AuthorFamilyName
            var authors = _books
                .Where(b => b.AuthorGivenName != null && b.AuthorFamilyName != null)
                .Select(b => new Book
                {
                    AuthorGivenName = b.AuthorGivenName,
                    AuthorFamilyName = b.AuthorFamilyName
                })
                .Distinct()
                .ToList();

            return authors;
        }

        // Selecting all genres
        public List<Book> SelectAllGenres()
        {
            // Return a distinct list of genres based on GenreName
            var genres = _books
                .Where(b => !string.IsNullOrEmpty(b.GenreName)) // Only consider books with a non-empty genre name
                .Select(b => new Book
                {
                    GenreName = b.GenreName
                })
                .Distinct()
                .ToList();

            return genres;
        }

        // Selecting books code section
        public List<Book> SelectAllBooks()
        {
            return _books;
        }

        public List<Book> SelectBooksByAvailableStatus()
        {
            return _books.Where(b => b.StatusID == "Available").ToList();
        }

        public List<Book> SelectCheckInBooks()
        {
            return _books.Where(b => b.StatusID == "CheckedIn").ToList();
        }


        public List<Book> SelectMaintenanceBooks()
        {
            return _books.Where(b => b.StatusID == "Maintenance").ToList();
        }

        public List<Book> SelectReservedBooks()
        {
            return _books.Where(b => b.StatusID == "Reserved").ToList();
        }

        // Updating code section

        public bool MoveReservedToCheckIn(int bookID)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookID);
            if (book != null && book.StatusID == "Reserved")
            {
                book.StatusID = "CheckedIn";
                return true;
            }
            return false;
        }
        public bool UpdateAvailableToReservedBook(int bookID, int staffID, int memberID, DateTime reservationDate)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookID && b.StatusID == "Available");
            if (book != null)
            {
                book.StatusID = "Reserved";
                book.ReservedBy = new Member { MemberID = memberID };
                book.ReservedByStaff = new Staff { StaffID = staffID };
                book.ReservationDate = reservationDate;
                return true;
            }
            return false;
        }

        public bool UpdateCheckInToAvailableBook(int bookID)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookID && b.StatusID == "CheckIn");
            if (book != null)
            {
                book.StatusID = "Available";
                return true;
            }
            return false;
        }

        public bool UpdateCheckInToMaintenanceBook(int bookID, int staffID, DateTime maintenanceDate, string description)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookID && b.StatusID == "CheckIn");
            if (book != null)
            {
                book.StatusID = "Maintenance";
                book.MaintenanceByStaff = new Staff { StaffID = staffID };
                book.MaintenanceDate = maintenanceDate;
                book.Description = description;
                return true;
            }
            return false;
        }

        public bool UpdateMaintenanceToAvailableBook(int bookID)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookID && b.StatusID == "Maintenance");
            if (book != null)
            {
                book.StatusID = "Available";
                return true;
            }
            return false;
        }
    }
}
