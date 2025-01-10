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
using DataDomain;
using System.Printing;

namespace WpfPresentation
{
    /// <summary>
    /// Interaction logic for frmReserveBook.xaml
    /// </summary>
    public partial class frmReserveBook : Window
    {
        private Action _refreshAvailableAndReservedList; // refreshes the list of available books in the mainwindow
        private IBookManager _bookManager;
        private IMemberManager _memberManager;
        private IStaffManager _staffManager;
        private int _bookID; // stores bookID to be reserved
         
        public frmReserveBook(IStaffManager staffManager, IBookManager bookManager, Action refreshAvailableAndReservedList, IMemberManager memberManager)
        {
            _staffManager = staffManager;
            _memberManager = memberManager;
            _bookManager = bookManager;
            InitializeComponent();
            txtBookID.IsEnabled = false; // textbox can't be changed
            _refreshAvailableAndReservedList = refreshAvailableAndReservedList; // refreshes the list when new book is reserved
        }

        public void SetAvaiableBookId(int bookId) // sets the bookid for reservation and updates the textbox
        {
            _bookID = bookId;
            txtBookID.Text = bookId.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<StaffVM> staff = _staffManager.SelectAllStaff(); // loads all the staff

                if (staff != null && staff.Count > 0)
                {
                    cmbStaff.ItemsSource = staff;  // Binds the ComboBox to the list of staff
                    cmbStaff.DisplayMemberPath = "StaffFullName";  // Displays FullName property in ComboBox
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
            try
            {
                List<Member> members = _memberManager.SelectAllMembers(); // loads all the members

                if (members != null && members.Count > 0)
                {
                    cmbMember.ItemsSource = members;  // Bind the ComboBox to the list of members
                    cmbMember.DisplayMemberPath = "MemberFullName";  // Display FullName property in ComboBox
                }
                else
                {
                    MessageBox.Show("No members found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnReserve_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensures a member is selected
                var selectedMember = cmbMember.SelectedItem as Member;
                if (selectedMember == null)
                {
                    MessageBox.Show("Please select a member.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Ensures a staff is selected
                var selectedStaff = cmbStaff.SelectedItem as StaffVM;
                if (selectedStaff == null)
                {
                    MessageBox.Show("Please select a staff.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Check if the selected staff is active
                if (selectedStaff.Active == false)
                {
                    MessageBox.Show("The selected staff is inactive and cannot perform this operation.", "Invalid Staff", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gets the selected StaffID
                int staffID = selectedStaff.StaffID;
                // Gets the selected MemberID
                int memberID = selectedMember.MemberID;  
                // Gets the reservation date
                DateTime reservationDate = dpReservationDate.SelectedDate ?? DateTime.Now;

                // Reservation Date cannot be in the past or the future
                if (reservationDate < DateTime.Now.Date)
                {
                    MessageBox.Show("The reservation date cannot be in the past. Please select a valid date.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Calls the BookManager to reserve the book
                var bookManager = new BookManager(); 
                bool isReserved = _bookManager.UpdateAvailableToReservedBook(_bookID, staffID, memberID, reservationDate);

                if (isReserved)
                {
                    MessageBox.Show("Book reserved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _refreshAvailableAndReservedList();
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Failed to reserve the book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }   
}
