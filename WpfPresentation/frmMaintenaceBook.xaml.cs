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
    /// Interaction logic for frmMaintenaceBook.xaml
    /// </summary>
    public partial class frmMaintenaceBook : Window
    {

        private Action _refreshCheckInandMaintenaceList;
        private IBookManager _bookManager;
        private IStaffManager _staffManager;
        private int _bookID; // Stores the bookID to be moved to maintenace
        public frmMaintenaceBook(IStaffManager staffManager, IBookManager bookManager, Action refreshCheckInandMaintenaceList)
        {
            _staffManager = staffManager;
            _bookManager = bookManager;
            InitializeComponent();
            txtBookID.IsEnabled = false; // text box can't be changed
            _refreshCheckInandMaintenaceList = refreshCheckInandMaintenaceList; // refreshes the list when new book is added to maintenace
        }
        public void SetBookIdForMaintenace(int bookID) // Sets the bookid for maintenace and updates the textbox
        {
            _bookID = bookID;
            txtBookID.Text = _bookID.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load staff into ComboBox
                List<StaffVM> staff = _staffManager.SelectAllStaff();

                if (staff != null && staff.Count > 0)
                {
                    cmbStaff.ItemsSource = staff;  // Bind the ComboBox to the list of staff
                    cmbStaff.DisplayMemberPath = "StaffFullName";  // Display FullName property in ComboBox
                }
                else
                {
                    MessageBox.Show("No staff found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        
        }
        private void btnSendToMaintenace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensures a staff is selected
                var selectedStaff = cmbStaff.SelectedItem as StaffVM;
                if (selectedStaff == null)
                {
                    MessageBox.Show("Please select a staff.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if the selected staff is active if not it will give an message 
                if (selectedStaff.Active == false)
                {
                    MessageBox.Show("The selected staff is inactive and cannot perform this operation.", "Invalid Staff", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gets the selected StaffID
                int staffID = selectedStaff.StaffID;
                // Gets the maintenance date
                DateTime maintenanceDate = dpMaintenanceDate.SelectedDate ?? DateTime.Now;

                // Maintenace Date cannot be in the past
                if (maintenanceDate < DateTime.Now.Date)
                {
                    MessageBox.Show("The maintenace date cannot be in the past. Please select a valid date.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Maintenance date cannot be in the future
                if (maintenanceDate > DateTime.Now.Date)
                {
                    MessageBox.Show("The maintenance date cannot be in the future.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gets the mainteance description
                string description = txtDescription.Text;
                if (string.IsNullOrEmpty(description))
                {
                    MessageBox.Show("Please provide a description for the maintenance.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Checks if description is longer than 250 characters
                if (description.Length > 250)
                {
                    MessageBox.Show("Description cannot be longer than 250 characters.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //Calls the BookManager to send book to maintenace
                var bookManager = new BookManager();
                bool isMaintenanced = _bookManager.UpdateCheckInToMaintenanceBook(_bookID, staffID, maintenanceDate, description);
                if (isMaintenanced)
                {
                    MessageBox.Show("Book successfully moved to maintenance.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _refreshCheckInandMaintenaceList(); // Refresh the book list
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Failed to move the book to maintenance.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        
    }
}
