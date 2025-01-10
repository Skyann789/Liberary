using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IBookManager
    {

        // Selecting
        List<Book> SelectBooksByAvailableStatus();
        List<Book> SelectReservedBooks();
        List<Book> SelectCheckInBooks();
        List<Book> SelectMaintenanceBooks();
        List<Book> SelectAllBooks();
        // Selecting Authors
        List<Book> SelectAllAuthors();
        // Selecting Genre
        List<Book> SelectAllGenres();


        // Updating 
        bool MoveReservedBookToCheckIn(int bookID);
        bool UpdateCheckInToAvailableBook(int bookID);
        bool UpdateAvailableToReservedBook(int bookID, int staffID, int memberID, DateTime reservationDate);
        bool UpdateCheckInToMaintenanceBook(int bookID, int staffID, DateTime maintenanceDate, string description);
        bool UpdateMaintenanceToAvailableBook(int bookID);

        // Inserting
        bool InsertBook(Book book);
    }
}
