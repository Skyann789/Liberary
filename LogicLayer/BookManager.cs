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
    public class BookManager : IBookManager
    {
        private IBookAccessor _bookAccessor;

        public BookManager()
        {
            _bookAccessor = new BookAccessor();
        }

        public BookManager(IBookAccessor bookAccessor)
        {
            _bookAccessor = bookAccessor;
        }

        // Selecting
        public List<Book> SelectBooksByAvailableStatus() // Retrieve available books based on status 
        {
            try
            {
                return _bookAccessor.SelectBooksByAvailableStatus();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving available books.", ex);
            }
        }

        public List<Book> SelectReservedBooks() // Retrieves the list of reserved books 
        {
            try
            {
                return _bookAccessor.SelectReservedBooks();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving reserved books.", ex);
            }
        }

        public List<Book> SelectCheckInBooks() // Retrieves the list of checkin books
        {
            try
            {
                return _bookAccessor.SelectCheckInBooks();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving checkin books", ex);
            }
        }

        public List<Book> SelectMaintenanceBooks() // Retrieves the lsit of maintenance books
        {
            try
            {
                return _bookAccessor.SelectMaintenanceBooks();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving books for maintenance", ex);
            }
        }
        public List<Book> SelectAllBooks() // Retrieves a list of all books
        {
            try
            {
                return _bookAccessor.SelectAllBooks();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while retrieving books for maintenance", ex);
            }
        }
        // Selecting Authors
        public List<Book> SelectAllAuthors()
        {
            try
            {
                return _bookAccessor.SelectAllAuthors();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve authors.", ex);
            }
        } // Retrieves a list of all authors
          // Selecting Genre
        public List<Book> SelectAllGenres()
        {
            try
            {
                return _bookAccessor.SelectAllGenres();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve genres.", ex);
            }
        } // Retrieves a list of all genres

        // Updating
        public bool MoveReservedBookToCheckIn(int bookID)
        {
            try
            {
                return _bookAccessor.MoveReservedToCheckIn(bookID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to move reserved book to check-in.", ex);
            }
        } // updates status from reserved to checkin
        public bool UpdateCheckInToAvailableBook(int bookID)
        {
            try
            {
                return _bookAccessor.UpdateCheckInToAvailableBook(bookID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to move reserved book to check-in.", ex);
            }
        }  // updates status from checkin to avaiable
        public bool UpdateAvailableToReservedBook(int bookID, int staffID, int memberID, DateTime reservationDate)
        {
            try
            {
                return _bookAccessor.UpdateAvailableToReservedBook(bookID, staffID, memberID, reservationDate);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update book status from available to reserved.", ex);
            }
        } // updates status from available to reserved
        public bool UpdateCheckInToMaintenanceBook(int bookID, int staffID, DateTime maintenanceDate, string description)
        {
            try
            {
                return _bookAccessor.UpdateCheckInToMaintenanceBook(bookID, staffID, maintenanceDate, description);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update book status from checkin to maintenance.", ex);
            }
        } // updates status from checkin to maintenance
        public bool UpdateMaintenanceToAvailableBook(int bookID)
        {
            try
            {
                return _bookAccessor.UpdateMaintenanceToAvailableBook(bookID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update book status from maintenance to available.", ex);
            }
        } // updates status from maintenance to available

        // Inserting
        public bool InsertBook(Book newBook)
        {
            try
            {
                return _bookAccessor.InsertBook(newBook);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to insert a new book.", ex);
            }
        } // Adds a new book to the database


    }
}
