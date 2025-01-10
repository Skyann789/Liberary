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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        StaffVM _accessToken = null;


        private IStaffManager _staffManager;


        // Happens at the beginning 

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtEmail.Focus();
            btnLoginLogout.IsDefault = true;
            hideAllTabs();
        }
        public MainWindow()
        {
            InitializeComponent();
            _staffManager = new StaffManager();
            hideAllTabs();

            LoadStaffData();

        }


        private void hideAllTabs() // Hides all the tabs
        {
            // Hides all the tabs
            tabAvailable.Visibility = Visibility.Collapsed;
            tabReserve.Visibility = Visibility.Collapsed;
            tabCheckin.Visibility = Visibility.Collapsed;
            tabPrep.Visibility = Visibility.Collapsed;
            tabMaintenance.Visibility = Visibility.Collapsed;
            tabInventory.Visibility = Visibility.Collapsed;
            tabStaff.Visibility = Visibility.Collapsed;
            tabAdmin.Visibility = Visibility.Collapsed;

            // hides the content of the tabs so it doesn't show on before login /after
            dataGridAvailableBooks.Visibility = Visibility.Collapsed;
            dataGridReservedBooks.Visibility = Visibility.Collapsed;
            gridStaff.Visibility = Visibility.Collapsed;


        }
        private void showTabsForRoles() // Shows tabs based on Roles assigned
        {

            if (_accessToken != null)
            {

                tabStaff.Visibility = Visibility.Visible;
                gridStaff.Visibility = Visibility.Visible;

                if (!(_accessToken == null || !_accessToken.Roles.Contains("Admin")))
                {
                    tabAdmin.Visibility = Visibility.Visible;
                }
                if (_accessToken != null && _accessToken.Roles.Contains("Manager"))
                {
                    tabAvailable.Visibility = Visibility.Visible;
                    tabReserve.Visibility = Visibility.Visible;
                    tabCheckin.Visibility = Visibility.Visible;
                    tabPrep.Visibility = Visibility.Visible;
                    tabMaintenance.Visibility = Visibility.Visible;
                    tabInventory.Visibility = Visibility.Visible;

                    dataGridAvailableBooks.Visibility = Visibility.Visible;

                }
                if (_accessToken != null && _accessToken.Roles.Contains("Reservation"))
                {
                    tabReserve.Visibility = Visibility.Visible;
                }
                if (_accessToken != null && _accessToken.Roles.Contains("Maintenance"))
                {
                    tabMaintenance.Visibility = Visibility.Visible;

                }

            }
        }

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
                MessageBox.Show("Invalid Email");
                txtEmail.Focus();
                txtEmail.SelectAll();
                return;
            }
            if (password.Length < 7)
            {
                MessageBox.Show("Invalid Password");
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
                    MessageBox.Show("Invalid email or password. Please try again. " +
                        "If you believe your email and password are correct and are still having issues logging in. Please contact your manager or the admin.",
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



                if (password == "newuser")
                {
                    var updatePassword = new frmUpdatePassword(_accessToken, staffManager, isNew: true);
                    if (updatePassword.ShowDialog() == false)
                    {
                        logoutUser();
                        MessageBox.Show("You must change your password to continue.");
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
                MessageBox.Show("You must be logged in to change your password.");
                return;
            }
            var updateWindow = new frmUpdatePassword(_accessToken, new StaffManager());
            bool? result = updateWindow.ShowDialog();

            if (result == true)
            {
                MessageBox.Show("Password Updated", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Password was not updated!", "Update Failed", MessageBoxButton.OK
                    , MessageBoxImage.Exclamation);
            }
        }

        public void LoadStaffData()
        {
            // Check if _staffManager is properly initialized
            if (_staffManager == null)
            {
                MessageBox.Show("Staff Manager is not initialized!");
                return;
            }

            List<StaffVM> staffList = _staffManager.SelectAllStaff();

            // Check if the list is null or empty
            if (staffList == null || staffList.Count == 0)
            {
                MessageBox.Show("No staff data found.");
                return;
            }

            listViewStaff.ItemsSource = staffList;
        }

        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            frmInsertStaff frmInsertStaff = new frmInsertStaff();

            bool? result = frmInsertStaff.ShowDialog();

            if (result == true)
            {
                LoadStaffData();
            }

        }
            //private void btnAdminUpdatePassword_Click(object sender, RoutedEventArgs e)
            //{

            //    // Get the selected staff member from the ListView
            //    StaffVM? selectedStaff = listViewStaff.SelectedItem as StaffVM;

            //    if (selectedStaff != null)
            //    {
            //        var updateWindow = new frmUpdatePassword(selectedStaff, new StaffManager());
            //        bool? result = updateWindow.ShowDialog();

            //        if (result == true)
            //        {
            //            MessageBox.Show("Password Updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //        }
            //        else
            //        {
            //            MessageBox.Show("Password was not updated!", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please select a staff member first", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    }
            //}
        
    }
    
}