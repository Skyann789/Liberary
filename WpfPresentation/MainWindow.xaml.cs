using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Azure.Core;
using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using LogicLayer;
using Microsoft.Data.SqlClient;

namespace WpfPresentation
{
    public partial class MainWindow : Window
    {

        StaffVM _accessToken = null;
        private IStaffManager _staffManager;
        private IBookManager _bookManager;
        private IMemberManager _memberManager;

        // Happens at the beginning 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtEmail.Focus();
            btnLoginLogout.IsDefault = true;
            hideAllTabs(); // Hides all tabs at the beginning so they aren't visable until the staff logs in
        }
        public MainWindow()
        {
            InitializeComponent();
            _staffManager = new StaffManager(); // Creates an instance of the StaffManager class
            hideAllTabs(); // Hides all tabs 
            LoadStaff(); // Loads the staff list 
            _bookManager = new BookManager(); // Creates an instance of the BookManager class
            LoadAvailableBooks();
            _memberManager = new MemberManager(); // Creates an instance of the MemberManager class
            LoadFine(); // Loads the fine list
            LoadMembers(); // Loads the member list
            LoadReservedBooks(); // Loads the reserved list
            LoadCheckInBooks(); // Loads the checkin list
            LoadMaintenanceBooks(); // Loads the maintenance list
            LoadInventoryBooks(); // Loads the inventory list
        }

        // Menu Item code
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        } 
        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }
        private void mnuPreferences_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow preferencesWindow = new PreferencesWindow();

            preferencesWindow.Show();
        }


        // Load List Code Section
        public void LoadStaff()
        {
            // Checks if _staffManager is properly initialized
            if (_staffManager == null)
            {
                // Displays error message if _staff manager is not initialized
                MessageBox.Show("Staff Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Retrieves a list of staff using the StaffManager
            List<StaffVM> staffList = _staffManager.SelectAllStaff();

            // Checks if the list is null or empty
            if (staffList == null || staffList.Count == 0)
            {
                // hides list if no staff
                listViewStaff.Visibility = Visibility.Collapsed;
                // shows text message
                txtbknoStaff.Visibility = Visibility.Visible;
                return;
            }
            // shows list if no staff
            listViewStaff.Visibility = Visibility.Visible;
            // hides text message
            txtbknoStaff.Visibility = Visibility.Visible;
            listViewStaff.ItemsSource = staffList;
        } // Loads the Staff list


        // Load Members and Fine Lists
        public void LoadFine() // Loads the fine list
        {
            if (_memberManager == null)
            {
                MessageBox.Show("Member Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Retrieves the list of members using the MemberManager
            List<Member> memberFines = _memberManager.SelectFines();

            if (memberFines == null || memberFines.Count == 0)
            {
                // hides list if no fines
                listViewFines.Visibility = Visibility.Collapsed;
                // shows text message
                txtbknoFines.Visibility = Visibility.Visible; 
                return ;
            }
            // shows list
            listViewFines.Visibility = Visibility.Visible;
            // hides text box
            txtbknoFines.Visibility = Visibility.Collapsed;
            var unpaidFines = memberFines.Where(fine => !fine.Paid).ToList();
            listViewFines.ItemsSource = unpaidFines;
        }
        public void LoadMembers() // Loads the member list
        {
            if (_memberManager == null)
            {
                MessageBox.Show("Member Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<Member> members = _memberManager.SelectAllMembers();

            if (members == null || members.Count == 0)
            {
                // hides list of members if no members exist 
                listViewMembers.Visibility = Visibility.Collapsed;
                // shows text box message
                txtbknoMembers.Visibility = Visibility.Visible;
                return;
            }
            // shows list
            listViewMembers.Visibility = Visibility.Visible;
            // hides text box
            txtbknoMembers.Visibility = Visibility.Collapsed;
            // Binds the list to the ListView
            listViewMembers.ItemsSource = members;
        }

        // Load Book Lists
        public void LoadAvailableBooks() // Loads avaiable book List
        {
            // Checks if _bookManager is properly initialized
            if (_bookManager == null)
            {
                MessageBox.Show("Book Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Retrieves the list of books using the BookManager
            List<Book> availableBooks = _bookManager.SelectBooksByAvailableStatus();

            if (availableBooks == null || availableBooks.Count == 0)
            {
                // hides list if no avaiable books
                listViewAvailableBooks.Visibility = Visibility.Collapsed;
                // shows text message
                txtbknoAvailableBooks.Visibility = Visibility.Visible;
                return;
            }

            // shows list 
            listViewAvailableBooks.Visibility = Visibility.Visible;
            // hides text message
            txtbknoAvailableBooks.Visibility = Visibility.Collapsed;

            listViewAvailableBooks.ItemsSource = availableBooks;
        }
        public void LoadReservedBooks()
        {
            if (_bookManager == null)
            {
                MessageBox.Show("Book Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<Book> reservedBooks = _bookManager.SelectReservedBooks();

            if (_bookManager == null || reservedBooks.Count == 0)
            {
                // Hides the list if no reserved books exist
                listViewReservedBooks.Visibility = Visibility.Collapsed;
                // Shows text box message instead 
                txtbknoReservedBooks.Visibility = Visibility.Visible; 
                return;
            }

            // List is ordered by book id
            reservedBooks = reservedBooks.OrderBy(book => book.BookID).ToList();

            // shows list if there is reserved books
            listViewReservedBooks.Visibility = Visibility.Visible;
            // hides text box message if there is reserved books
            txtbknoReservedBooks.Visibility = Visibility.Collapsed;

            listViewReservedBooks.ItemsSource = reservedBooks;
        } // Loads Reserved Book List
        public void LoadMaintenanceBooks()
        {
            if (_bookManager == null)
            {
                MessageBox.Show("Book Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<Book> maintenanceBooks = _bookManager.SelectMaintenanceBooks();

            if (_bookManager == null || maintenanceBooks.Count == 0)
            {
                // Hides the list if no reserved books exist
                listViewMaintenanceBooks.Visibility = Visibility.Collapsed;
                // Shows text box message instead 
                txtbknoMaintenanceBooks.Visibility = Visibility.Visible;
                return;
            }

            //  list is ordered by bookid
            maintenanceBooks = maintenanceBooks.OrderBy(book => book.BookID).ToList(); 

            // shows list if there is reserved books
            listViewMaintenanceBooks.Visibility = Visibility.Visible;
            // hides text box message if there is reserved books
            txtbknoMaintenanceBooks.Visibility = Visibility.Collapsed;
            listViewMaintenanceBooks.ItemsSource = maintenanceBooks;
        } // Loads Maintenance Book List
        public void LoadCheckInBooks()
        {
            if (_bookManager == null)
            {
                MessageBox.Show("Book Manager is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<Book> checkinBooks = _bookManager.SelectCheckInBooks();

            if (_bookManager == null || checkinBooks.Count == 0)
            {
               // hides the list if there is no books to check-in and inspect
               listViewCheckInBooks.Visibility = Visibility.Collapsed;
               // shows text box message instead
               txtbknoCheckInBooks.Visibility = Visibility.Visible;
               return;
            }

        
            // shows list if there is check-in books
            listViewCheckInBooks.Visibility = Visibility.Visible;
            // hides the text box message if there is books in the list
            txtbknoCheckInBooks.Visibility = Visibility.Collapsed;
            listViewCheckInBooks.ItemsSource = checkinBooks;
        } // Loads CheckIn Book list
        public void LoadInventoryBooks()
        {
            if (_bookManager == null)
            {
                MessageBox.Show("Book Manager is not initialized!");
                return;
            }
            List<Book> inventoryBooks = _bookManager.SelectAllBooks();

            if (_bookManager == null || inventoryBooks.Count == 0)
            {
                // hides the list if there is no books to inventory
                listViewInventoryBooks.Visibility = Visibility.Collapsed;
                // shows text box message instead
                txtbknoInventoryBooks.Visibility = Visibility.Visible;
                return;
            }

            // shows list if there is check-in books
            listViewInventoryBooks.Visibility = Visibility.Visible;
            // hides the text box message if there is books in the list
            txtbknoInventoryBooks.Visibility = Visibility.Collapsed;
            listViewInventoryBooks.ItemsSource = inventoryBooks;
        } // Loads all books in inventory


        // Refresh list, code section
        private void RefreshFineList() // Refreshes the list after a new fine is created
        {
            LoadFine();
        }
        private void RefreshStaffList()
        {
            LoadStaff();
        } // Refreshes the staff list

        private void RefreshAvailableBookList()
        {
            LoadAvailableBooks();
            LoadReservedBooks();
        } // Refreshes the avaiable and reserved lists after book is reserved

        private void RefreshCheckInAndMaintenanceLists()
        {
            LoadCheckInBooks();
            LoadMaintenanceBooks();
        } // Refreshes the checkIn and Maintenance lists after book is sent to maintenace

        private void RefreshInventoryAndAvailable() // Refreshes the Inventory and Avaiable Book lists
        {
            LoadAvailableBooks();
            LoadInventoryBooks();
        }


        // Selection Changed, code section
        private void listViewMembers_SelectionChanged(object sender, SelectionChangedEventArgs e) // Selects the member in list
        {
            // Casts the selected item in the ListView to a Member object
            Member selectedMember = listViewMembers.SelectedItem as Member;
        }
        private void listViewStaff_SelectionChanged(object sender, SelectionChangedEventArgs e) // Selects the staff in list
        {
            // Casts the selected item in the ListView to a Staff object
            StaffVM selectedStaff = listViewStaff.SelectedItem as StaffVM;
        }

        private void listViewReservedBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Casts the selected item in the ListView to a Book object
            Book selectedReservedBook = listViewReservedBooks.SelectedItem as Book;
        }

        private void listViewCheckInBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Casts the selected item in the ListView to a Book object
            Book selectedCheckInBook = listViewCheckInBooks.SelectedItem as Book;
        }
        
        private void listViewAvailableBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Casts the selected item in the ListView to a Book object
            Book selectedAvailableBook = listViewAvailableBooks.SelectedItem as Book;
        }

        private void listViewMaintenanceBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Casts the selected item in the ListView to a Book object
            Book selectedMaintenanceBook = listViewMaintenanceBooks.SelectedItem as Book;
        }


        // Fine code section
        private void btnFinePaid_Click(object sender, RoutedEventArgs e)
        {
            // Gets the selected fine from the ListView
            Member selectedFine = listViewFines.SelectedItem as Member;

            // Check if a fine is selected
            if (selectedFine != null)
            {
                try
                {
                    if (selectedFine.Paid)
                    {
                        MessageBox.Show("This fine has already been marked as paid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    // Marks the fine as paid 
                    bool success = _memberManager.MarkFineAsPaid(selectedFine.FineID);

                    if (success)
                    {
                        MessageBox.Show("Fine has been marked as paid.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Refresh the list
                        LoadFine();
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while marking the fine as paid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a fine to mark as paid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnAddFine_Click(object sender, RoutedEventArgs e)
        {
            // Gets the selected member from the member list
            Member selectedMember = listViewMembers.SelectedItem as Member;

            // Checks if the member is selected
            if (selectedMember != null)
            {
                // Creates an instance of frmAddFine and pass the refresh action to it
                frmAddFine addFineWindow = new frmAddFine(() =>
                {
                    RefreshFineList();
                });

                // Calls SetMemberId to pass the selected member's ID to the AddFine window
                addFineWindow.SetMemberId(selectedMember.MemberID);

                // Show the AddFine window
                addFineWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a member from the list.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Tab section code
        private void hideAllTabs() // Hides all the tabs
        {
            // Hides all the tabs
            tabAvailable.Visibility = Visibility.Collapsed; // Avaiable Tab
            tabReserve.Visibility = Visibility.Collapsed; // Reserved Tab
            tabCheckIn.Visibility = Visibility.Collapsed; // CheckIn and inspection tab
            tabFines.Visibility = Visibility.Collapsed; // Fines tab
            tabMaintenance.Visibility = Visibility.Collapsed; // Maintenance tab
            tabInventory.Visibility = Visibility.Collapsed; // Inventory tab
            tabStaff.Visibility = Visibility.Collapsed; // Staff tab
            tabAdmin.Visibility = Visibility.Collapsed; // Admin tab
            tabMembers .Visibility = Visibility.Collapsed;


            // hides the content of the tabs so it doesn't show on before login /after
            gridAvaiable.Visibility = Visibility.Collapsed; // Grid Avaialbe
            gridReserve.Visibility = Visibility.Collapsed; // Grid Reserved
            gridCheckIn.Visibility= Visibility.Collapsed; // Grid Checkin
            gridFines.Visibility = Visibility.Collapsed; // Grid Fines
            gridMaintenance.Visibility = Visibility.Collapsed; // Grid Maintenace
            gridInventory.Visibility = Visibility.Collapsed; // Grid Inventory
            gridStaff.Visibility = Visibility.Collapsed; // Grid Staff
            gridMembers.Visibility = Visibility.Collapsed; // Grid Members
            gridAdmin.Visibility = Visibility.Collapsed; // Grid Admin


        }
        private void showTabsForRoles() // Shows tabs based on Roles assigned
        {
            if (_accessToken != null)
            {
                // Everyone can view the staff tab and contents
                tabStaff.Visibility = Visibility.Visible;
                gridStaff.Visibility = Visibility.Visible;

                if (!(_accessToken == null || !_accessToken.Roles.Contains("Admin")))
                {
                    tabAdmin.Visibility = Visibility.Visible;
                    gridAdmin.Visibility = Visibility.Visible;
                }
                if (_accessToken != null && _accessToken.Roles.Contains("Manager"))
                {
                    // Manager has access to all tabs / grids
                    tabAvailable.Visibility = Visibility.Visible;
                    tabReserve.Visibility = Visibility.Visible;
                    tabCheckIn.Visibility = Visibility.Visible;
                    tabMaintenance.Visibility = Visibility.Visible;
                    tabInventory.Visibility = Visibility.Visible;
                    tabFines.Visibility = Visibility.Visible;
                    tabMembers.Visibility = Visibility.Visible;


                    gridAvaiable.Visibility= Visibility.Visible;
                    gridReserve.Visibility= Visibility.Visible;
                    gridFines.Visibility= Visibility.Visible;
                    gridMembers.Visibility= Visibility.Visible;
                    gridCheckIn.Visibility = Visibility.Visible;
                    gridMaintenance.Visibility= Visibility.Visible;
                    gridInventory.Visibility= Visibility.Visible;
                }
                if (_accessToken != null && _accessToken.Roles.Contains("Reservation"))
                {
                    tabReserve.Visibility = Visibility.Visible;
                    gridReserve.Visibility = Visibility.Visible;
                }
                if (_accessToken != null && _accessToken.Roles.Contains("Maintenance"))
                {
                    // Can only view the maintenance tabs
                    tabMaintenance.Visibility = Visibility.Visible;
                    gridMaintenance.Visibility = Visibility.Visible;
                }
                if (_accessToken != null && _accessToken.Roles.Contains("CheckIn"))
                {
                    // Can only view the Checkin tabs
                    tabCheckIn.Visibility = Visibility.Visible;
                    gridCheckIn.Visibility = Visibility.Visible;
                }
            }
        }


        // Login, Logout, Updated Password code secton
        private void btnLoginLogout_Click(object sender, RoutedEventArgs e)
        {
            var staffManager = new StaffManager();
            string email = txtEmail.Text;
            string password = pwdPassword.Password;

            if (btnLoginLogout.Content.ToString() == "Log Out")
            {
                logoutUser();
                return;
            }
            if (email.Length < 7)
            {
                MessageBox.Show("Invalid Email","Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                txtEmail.SelectAll();
                return;
            }
            if (password.Length < 7)
            {
                MessageBox.Show("Invalid Password", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwdPassword.Focus();
                pwdPassword.SelectAll();
                return;
            }

            // try to login
            try
            {
                _accessToken = staffManager.LoginStaff(email, password);
                //Checks if the login was successful
                if (_accessToken == null)
                {
                    // Handles failed login 
                    MessageBox.Show("Invalid email or password. Please try again. If you believe this is a mistake. Please contact your manager or the admin. ",
                        "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
   
                // Upon Login the Staff Tab is populated with the Staff Member's information
                txtStaffGivenName.Text = _accessToken.GivenName;
                txtStaffFamilyName.Text = _accessToken.FamilyName;
                txtStaffRole.Text = string.Join(", ", _accessToken.Roles); // Role or roles the staff has
                txtStaffEmail.Text = _accessToken.Email;
                txtStaffPhone.Text = _accessToken.Phone;


                string roles = "";


                for (int i = 0; i < _accessToken.Roles.Count; i++)
                {
                    roles += _accessToken.Roles[i];

                    if (i < _accessToken.Roles.Count - 2)
                    {
                        roles += ", ";
                    }
                    else if (i == _accessToken.Roles.Count - 2)
                    {
                        roles += " and ";
                    }
                }
                string message = "Welcome, " + _accessToken.GivenName +
                    ". You are logged in as " + roles + ".";
                lblGreeting.Content = message;


                // Admin's greeting is turned red to indicate important desciences will be made
                if (_accessToken.Roles.Contains("Admin"))
                {
                    lblGreeting.Foreground = new SolidColorBrush(Colors.Red); // Set text color to red
                }
                else
                {
                    lblGreeting.Foreground = new SolidColorBrush(Colors.Black); // All other users will have black lblGretting text
                }

                hideAllTabs();

                // reset the login area and button
                // reset the user interface
                statusMessage.Content = "Logged in. Please log out before you leave.";


                // reset the login
                btnLoginLogout.Content = "Log Out";
                txtEmail.Text = "";
                pwdPassword.Password = "";
                txtEmail.IsEnabled = false;
                pwdPassword.IsEnabled = false;
                txtEmail.Visibility = Visibility.Hidden;
                pwdPassword.Visibility = Visibility.Hidden;
                lblEmail.Visibility = Visibility.Hidden;
                lblPassword.Visibility = Visibility.Hidden;
                btnLoginLogout.IsDefault = false;


                // if the staff enters newuser as their password they will be forced to update their password before being allowed into
                // the application
                if (password == "newuser")
                {
                    var updatePassword = new frmUpdatePassword(_accessToken, staffManager, isNew: true);
                    if (updatePassword.ShowDialog() == false)
                    {
                        logoutUser();
                        MessageBox.Show("You must change your password to continue.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    showTabsForRoles(); // shows the tabs once the password is updated
                }
                else
                {
                    showTabsForRoles(); // if not a new user the tabs are shown 
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "\n\n" + ex.InnerException.Message;
                }
                MessageBox.Show(message, "Login Failed");
            }
            txtEmail.Text = "";
            pwdPassword.Password = "";
            txtEmail.Focus();
        }
        private void logoutUser()
        {
            // remove the access token
            _accessToken = null;

            // reset the user interface
            statusMessage.Content = "You are not logged in. Please log in to continue.";

            // reset the login
            btnLoginLogout.Content = "Log In";
            txtEmail.Text = "";
            pwdPassword.Password = "";
            txtEmail.IsEnabled = true;
            pwdPassword.IsEnabled = true;
            txtEmail.Visibility = Visibility.Visible;
            pwdPassword.Visibility = Visibility.Visible;
            lblEmail.Visibility = Visibility.Visible;
            lblPassword.Visibility = Visibility.Visible;
            lblGreeting.Content = "You are not logged in.";
            lblGreeting.Foreground = new SolidColorBrush(Colors.Black);

            // Resets the Staff Tab
            txtStaffGivenName.Text = "";
            txtStaffFamilyName.Text = "";
            txtStaffRole.Text = "";
            txtStaffEmail.Text = "";
            txtStaffPhone.Text = "";
            // Rehides the tabs on logout
            hideAllTabs();

            txtEmail.Focus();
            btnLoginLogout.IsDefault = true;
            return;
        }
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            // makes it so the user must be logged in to change their password
            if (_accessToken == null)
            {
                MessageBox.Show("You must be logged in to change your password.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Opens the password update form, passes the access token and new StaffManager instance
            var updateWindow = new frmUpdatePassword(_accessToken, new StaffManager());
            // Shows the update window as a dialog and captures the result
            bool? result = updateWindow.ShowDialog();
            
            if (result == true)
            {
                MessageBox.Show("Password Updated", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Password was not updated!", "Error", MessageBoxButton.OK
                    , MessageBoxImage.Error);
            }
        }

        // Admin code section

        // Adding code section
        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            // Opens the Insert Staff form
            frmInsertStaff frmInsertStaff = new frmInsertStaff();
            //Displays the frmInsertStaff form as dialog
            bool? result = frmInsertStaff.ShowDialog();
            // Check the result of the dialog
            if (result == true)
            {
                LoadStaff(); // If staff successfully added, staff list is reloaded
            }

        } // Adds a staff
        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            // Retrieves the selected staff member from the Listview
            StaffVM selectedStaff = listViewStaff.SelectedItem as StaffVM;
            // Checks if a staff member is selected
            if (selectedStaff != null)
            {
                // Opens the add role window
                frmAddRole addRoleWindow = new frmAddRole(_staffManager,() =>
                {
                    RefreshStaffList(); // Refreshes the list after role is added
                });
                // Sets the StaffID of the selected staff member in the Add Role form 
                addRoleWindow.SetStaffId(selectedStaff.StaffID);

                addRoleWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a staff member from the list.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        } // Adds a role to Staff

        private void btnRemoveRole_Click(object sender, RoutedEventArgs e)
        {
            // Gets the selected staff member from the list view
            StaffVM selectedStaff = listViewStaff.SelectedItem as StaffVM;

            if (selectedStaff != null)
            {
                // Fetchs the roles for the selected staff member
                List<string> roles = _staffManager.GetRolesForStaff(selectedStaff.StaffID);

                // Checks if roles are available
                if (roles == null || !roles.Any())
                {
                    MessageBox.Show("No roles available for the selected staff member.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //Ensures at least one role remains
                if (roles.Count == 1)
                {
                    MessageBox.Show("The last role cannot be removed. Each staff member must have at least one role.", "Action Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Creates an instance of the frmRemoveRole window
                frmRemoveRole removeRoleWindow = new frmRemoveRole(_staffManager, roles, () =>
                {
                    RefreshStaffList(); 
                });

                // Passes the Staff ID to the remove role window
                removeRoleWindow.SetStaffId(selectedStaff.StaffID);

                // Shows the dialog window
                removeRoleWindow.ShowDialog();
            }
            else
            {
                // Shows a message if no staff member is selected
                MessageBox.Show("Please select a staff member from the list.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        } // Removes a role from staff
        private void btnDeactivateStaff_Click(object sender, RoutedEventArgs e) 
        {
            StaffVM selectedStaff = listViewStaff.SelectedItem as StaffVM;

            if (selectedStaff != null)
            {
                try
                {
                    if (!selectedStaff.Active) // staff already deactivated
                    {
                        MessageBox.Show("This staff member is already deactivated.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Attempts to deactivate the staff member and remove their role
                    bool success = _staffManager.DeActivateStaffAndRemoveRole(selectedStaff.StaffID);

                    if (success)
                    {
                        MessageBox.Show("Staff member has been deactivated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStaff();
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while deactivating the staff member.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a staff member to deactivate.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }  // deactivates a staff

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            // selected staff
            StaffVM selectedStaff = listViewStaff.SelectedItem as StaffVM;

            if (selectedStaff != null)
            {
                try
                {
                    // Staff is already active
                    if (selectedStaff.Active)
                    {
                        MessageBox.Show("This staff member is already active.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    bool success = _staffManager.ActivateStaff(selectedStaff.StaffID);

                    if (success)
                    {
                        MessageBox.Show("Staff member has been activated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Opens the frmAddRole to assign a role to the now active staff
                        frmAddRole addRoleWindow = new frmAddRole(_staffManager, () =>
                        {
                            RefreshStaffList();
                        });

                        addRoleWindow.SetStaffId(selectedStaff.StaffID);
                        bool? dialogResult = addRoleWindow.ShowDialog();

                        // Handle dialog result properly
                        if (dialogResult.HasValue && dialogResult.Value)
                        {
                            // Role was assigned, proceed as normal
                            LoadStaff(); // Refresh list to reflect the new status and role
                        }   
                        else
                        {
                            // Handle case when no role was assigned, including "X" closure or user cancellation
                            List<string> roles = _staffManager.GetRolesForStaff(selectedStaff.StaffID);
                            if (roles == null || !roles.Any())
                            {
                                MessageBox.Show("Please assign a role to the staff member before activating.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                // Deactivate the staff member if no role was assigned
                                _staffManager.DeActivateStaffAndRemoveRole(selectedStaff.StaffID);
                                MessageBox.Show("Activation canceled due to missing role assignment.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                LoadStaff(); // Refresh list to reflect the status change
                            }
                            else
                            {
                                LoadStaff(); // Refresh list to reflect the new status and role
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while activating the staff member.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Displays a Warning Message Box if the frmAddRole is closed before a role is added
                    // returns to the frmAddRole window after ok is hit on the warning message box
                    MessageBoxResult result = MessageBox.Show("You must assign a role to the staff after activating.",
                        "Activation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        // Reopens the frmAddRole window after the warning message box
                        frmAddRole addRoleWindowReturn = new frmAddRole(_staffManager, () =>
                        {
                            RefreshStaffList(); // Refresh the role list when a role is added
                        });

                        addRoleWindowReturn.SetStaffId(selectedStaff.StaffID);
                        addRoleWindowReturn.ShowDialog();
                    }

                    
                }
            }
            else
            {
                MessageBox.Show("Please select a staff member to activate.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }  // Activates a staff 


        // Moving books button code
        private void btnSendToCheckIn_Click(object sender, RoutedEventArgs e)
        {
            Book selectedReservedBook = listViewReservedBooks.SelectedItem as Book;

            if (selectedReservedBook != null)
            {
                try
                {
                    // Checks if the book is already in check-in
                    if (selectedReservedBook.StatusID == "2")
                    {
                        MessageBox.Show("This book is already in check-in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Moves the book to check-in
                    bool success = _bookManager.MoveReservedBookToCheckIn(selectedReservedBook.BookID);

                    if (success)
                    {

                        MessageBox.Show("Book has been moved to check-in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadReservedBooks();  // Reload reserved books
                        LoadCheckInBooks();   // Reload check-in books
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while moving the book to check-in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a book to move to check-in.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        } // Moves book to Checkin

        private void btnCheckinAvailable_Click(object sender, RoutedEventArgs e)
        {
            Book selectCheckInBook = listViewCheckInBooks.SelectedItem as Book;

            if (selectCheckInBook != null)
            {
                try
                {
                    // Checks if the book is already in avaible
                    if (selectCheckInBook.StatusID == "1")
                    {
                        MessageBox.Show("This book is already in available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Moves the book to available
                    bool success = _bookManager.UpdateCheckInToAvailableBook(selectCheckInBook.BookID);

                    if (success)
                    {

                        MessageBox.Show("Book has been moved to available.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCheckInBooks();   // Reloads check-in books
                        LoadAvailableBooks(); // Reloads the available books
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while moving the book to available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a book to move to available", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        } // Moves book to available from checkin

        private void btnReserveBook_Click(object sender, RoutedEventArgs e)
        {
            Book selectedAvailableBook = listViewAvailableBooks.SelectedItem as Book;

            if (selectedAvailableBook != null)
            {
                // opens the reserved book form
                frmReserveBook reserveBookWindow = new frmReserveBook(_staffManager, _bookManager, () =>
                {
                    RefreshAvailableBookList(); // refreshes the available book list
                }, _memberManager);
                //Sets the selected bookid in the ReserveBook form
                reserveBookWindow.SetAvaiableBookId(selectedAvailableBook.BookID);
                reserveBookWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a available book to reserve.");
            }
        } // Moves book to reserved book

        private void btnMaintenance_Click(object sender, RoutedEventArgs e)
        {
            Book selectedCheckInBook = listViewCheckInBooks.SelectedItem as Book;

            if (selectedCheckInBook != null)
            {
                frmMaintenaceBook maintenaceBookWindow = new frmMaintenaceBook(_staffManager, _bookManager, () =>
                {
                    RefreshCheckInAndMaintenanceLists();
                });
                maintenaceBookWindow.SetBookIdForMaintenace(selectedCheckInBook.BookID);
                maintenaceBookWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a checkin book to send to maintenace.");
            }
        } // Moves book to maintenace

        private void btnMaintenanceAvailable_Click(object sender, RoutedEventArgs e)
        {
            Book selectedMaintenanceBook = listViewMaintenanceBooks.SelectedItem as Book;

            if(selectedMaintenanceBook != null) 
            {
                try
                {

                    // Check if the book is already in available
                    if (selectedMaintenanceBook.StatusID == "1")
                    {
                        MessageBox.Show("This book is already in available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Move the book to available
                    bool success = _bookManager.UpdateMaintenanceToAvailableBook(selectedMaintenanceBook.BookID);

                    if (success)
                    {
                        MessageBox.Show("Book has been moved to available.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMaintenanceBooks();
                        LoadAvailableBooks();
                    }
                    else
                    {
                        MessageBox.Show("An error occurred while moving the book to available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a book to move to available", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        
        } // Moves book to available from maintenance

        private void btnInsertBook_Click(object sender, RoutedEventArgs e)
        {
            // opens the Insert book form
            frmInsertBook frmInsertBook = new frmInsertBook();
            bool? result = frmInsertBook.ShowDialog();

            if (result == true)
            {
                LoadAvailableBooks(); // reloads the available book list
                LoadInventoryBooks(); // reloads the inventory book list
            }
        } // Adds a new book to invenotry
    }
}

