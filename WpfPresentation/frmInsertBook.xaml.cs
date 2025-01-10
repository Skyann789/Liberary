using DataDomain;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfPresentation
{
    /// <summary>
    /// Interaction logic for frmInsertBook.xaml
    /// </summary>
    public partial class frmInsertBook : Window
    {
        private IBookManager _bookManager;
        public frmInsertBook()
        {
            InitializeComponent();
            _bookManager = new BookManager();
            LoadGenres();
            LoadAuthors();

            txtStatusID.IsEnabled = false; // disables editing to the StatusID field
            txtStatusID.Text = "1"; // Automatically sets the StatusID to "1" or available
        }
        // Loads genres into ComboBox
        private void LoadGenres()
        {
            try
            {
                List<Book> genres = _bookManager.SelectAllGenres();
                cmbGenre.ItemsSource = genres;
                cmbGenre.DisplayMemberPath = "GenreName"; // Displays the genre name in the combo box
                cmbGenre.SelectedValuePath = "GenreID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading genres: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Loads authors into ComboBox
        private void LoadAuthors()
        {
            try
            {
                List<Book> authors = _bookManager.SelectAllAuthors();
                cmbAuthor.ItemsSource = authors;
                cmbAuthor.DisplayMemberPath = "AuthorFullName";
                cmbAuthor.SelectedValuePath = "AuthorID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading authors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Button click event to insert the book
        private void btnInsertBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensures all fields are filled
                if (string.IsNullOrEmpty(txtBookID.Text) || string.IsNullOrEmpty(txtBookTitle.Text) ||
                    cmbGenre.SelectedValue == null || cmbAuthor.SelectedValue == null ||
                    string.IsNullOrEmpty(txtStatusID.Text) || string.IsNullOrEmpty(txtPublishedYear.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Missing Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                // Validates Published Year, it must be 4 digits long
                if (!int.TryParse(txtPublishedYear.Text, out int publishedYear) || txtPublishedYear.Text.Length != 4)
                {
                    MessageBox.Show("Published Year must be a 4-digit number.", "Invalid Published Year", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Validates Book Title, makes it so it cannot be longer than 100 characters
                if (txtBookTitle.Text.Length > 100)
                {
                    MessageBox.Show("Book Title cannot be longer than 100 characters.", "Invalid Title Length", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Parse BookID as an integer
                if (!int.TryParse(txtBookID.Text, out int bookID))
                {
                    MessageBox.Show("Book ID must be an integer.", "Invalid Book ID", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }



                // Create book object
                var newBook = new Book
                {
                    BookID = bookID,
                    Title = txtBookTitle.Text,
                    GenreID = cmbGenre.SelectedValue.ToString(),
                    AuthorID = cmbAuthor.SelectedValue.ToString(),
                    StatusID = txtStatusID.Text, // This will always be "1"
                    PublishedYear = int.Parse(txtPublishedYear.Text)
                };

                // Inserts the book into the database
                bool isBookInserted = _bookManager.InsertBook(newBook);

                if (isBookInserted)
                {
                    MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refreshes the Inventory Book list
                    // Calls the LoadInventoryBooks on the MainWindow to refresh the listView
                    ((MainWindow)Application.Current.MainWindow).LoadInventoryBooks();
                    // Refreshes the Available Books list
                    // Calls the LoadAvailableBooks on the MainWindow to refresh the listView
                    ((MainWindow)Application.Current.MainWindow).LoadAvailableBooks();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to add the book. Please check the BookID doesn't already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
