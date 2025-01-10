using DataAccessInterfaces;
using DataDomain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookAccessor : IBookAccessor
    {

        // Selecting
        // Selecting Books 
        public List<Book> SelectBooksByAvailableStatus()
        {
            var bookList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_available_books", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var book = new Book()
                    {
                        BookID = reader.GetInt32(0),
                        StatusID = reader.GetString(1),
                        AuthorGivenName = reader.GetString(2),
                        AuthorFamilyName = reader.GetString(3),
                        GenreName = reader.GetString(4),
                        Title = reader.GetString(5),
                        PublishedYear = reader.GetInt32(6),
                        Active = reader.GetBoolean(7)
                    };
                    bookList.Add(book);
                }
            }
            return bookList;
        }

        public List<Book> SelectReservedBooks()
        {
            var reservedBooksList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_reserved_books", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Populate Member details
                    var member = new Member
                    {
                        MemberID = reader.GetInt32(1),
                        MemberGivenName = reader.GetString(6),
                        MemberFamilyName = reader.GetString(7)
                    };

                    var staff = new Staff
                    {
                        StaffID = reader.GetInt32(3)
                    };
                    // Populate Book details with reservation data
                    var book = new Book
                    {
                        BookID = reader.GetInt32(2),
                        StatusID = reader.GetString(5),
                        Title = reader.GetString(8),
                        ReservedBy = member, 
                        ReservedByStaff = staff,
                        ReservationDate = reader.GetDateTime(4)
                    };


                    reservedBooksList.Add(book);
                }
            }
            return reservedBooksList;
        }

        public List<Book> SelectMaintenanceBooks()
        {
            var maintenanceBooksList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_maintenance_books", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var staff = new Staff
                    {
                        StaffID = reader.GetInt32(2)
                    };
                    // Populate Book details with reservation data
                    var book = new Book
                    {
                        BookID = reader.GetInt32(1),
                        MaintenanceByStaff = staff,
                        MaintenanceDate = reader.GetDateTime(3),
                        StatusID = reader.GetString(4),
                        Description = reader.GetString(5),
                        Title = reader.GetString(6),
                    };


                    maintenanceBooksList.Add(book);
                }
            }
            return maintenanceBooksList;
        }

        public List<Book> SelectCheckInBooks()
        {
            var checkInBooksList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_checkin_books", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var book = new Book()
                    {
                        BookID = reader.GetInt32(0),
                        StatusID = reader.GetString(1),
                        AuthorGivenName = reader.GetString(2),
                        AuthorFamilyName = reader.GetString(3),
                        GenreName = reader.GetString(4),
                        Title = reader.GetString(5),
                        PublishedYear = reader.GetInt32(6),
                        Active = reader.GetBoolean(7)
                    };
                    checkInBooksList.Add(book);
                }
            }
            return checkInBooksList;
        }

        public List<Book> SelectAllBooks()
        {
            var checkInBooksList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_all_books", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var book = new Book()
                    {
                        BookID = reader.GetInt32(0),
                        StatusID = reader.GetString(1),
                        AuthorGivenName = reader.GetString(2),
                        AuthorFamilyName = reader.GetString(3),
                        GenreName = reader.GetString(4),
                        Title = reader.GetString(5),
                        PublishedYear = reader.GetInt32(6),
                        Active = reader.GetBoolean(7)
                    };
                    checkInBooksList.Add(book);
                }
            }
            return checkInBooksList;
        }

        // Selecting Author List
        public List<Book> SelectAllAuthors()
        {
            var authorsList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_all_authors", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var author = new Book
                    {
                        AuthorID = reader.GetString(0),
                        AuthorGivenName = reader.GetString(1),
                        AuthorFamilyName = reader.GetString(2)
                    };
                    authorsList.Add(author);
                }
            }
            return authorsList;
        }


        // Selecting Genre List
        public List<Book> SelectAllGenres()
        {
            var genresList = new List<Book>();

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_select_all_genres", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var genre = new Book
                    {
                        GenreID = reader.GetString(0),
                        GenreName = reader.GetString(1)
                    };
                    genresList.Add(genre);
                }
            }
            return genresList;
        }

        // Updating 
        public bool MoveReservedToCheckIn(int bookID)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_update_reserved_to_checkin", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BookID", bookID);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool UpdateCheckInToAvailableBook(int bookID)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_update_checkin_to_available_book", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BookID", bookID);

                    conn.Open();
                    var rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;

            }
        }
        public bool UpdateAvailableToReservedBook(int bookID, int staffID, int memberID, DateTime reservationDate)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_update_available_to_reserve_book", conn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@BookID", bookID);
                cmd.Parameters.AddWithValue("@StaffID", staffID);
                cmd.Parameters.AddWithValue("@MemberID", memberID);
                cmd.Parameters.AddWithValue("@ReservationDate", reservationDate);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();


                return rowsAffected > 0;
            }
        }
        public bool UpdateCheckInToMaintenanceBook(int bookID, int staffID, DateTime maintenanceDate, string description)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_update_checkin_to_maintenance_book", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BookID", bookID);
                cmd.Parameters.AddWithValue("@StaffID", staffID);
                cmd.Parameters.AddWithValue("@MaintenanceDate", maintenanceDate);
                cmd.Parameters.AddWithValue("@Description", description);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        public bool UpdateMaintenanceToAvailableBook(int bookID)
        {
            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_update_maintenace_to_available", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BookID", bookID);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        // Inserting
        public bool InsertBook(Book book)
        {
            bool result = false;

            using (var conn = DBConnection.GetConnection())
            {
                var cmd = new SqlCommand("sp_insert_book", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@BookID", book.BookID);
                cmd.Parameters.AddWithValue("@StatusID", book.StatusID);
                cmd.Parameters.AddWithValue("@AuthorID", book.AuthorID);
                cmd.Parameters.AddWithValue("@GenreID", book.GenreID);
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@PublishedYear", book.PublishedYear);

                try
                {
                    conn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting book: " + ex.Message);
                }
            }

            return result;
        }

        
    }
}
