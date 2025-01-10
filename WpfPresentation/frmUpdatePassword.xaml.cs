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
using LogicLayer;
using DataDomain;
using System.Collections;

namespace WpfPresentation
{
    /// <summary>
    /// Interaction logic for frmUpdatePassword.xaml
    /// </summary>
    public partial class frmUpdatePassword : Window
    {
        private Staff _staff; // Instance of the staff object for the current user
        private IStaffManager _staffManager;
        private bool _isNewUser; // Flag to determine if staff is new 

        public frmUpdatePassword(Staff staff,
            StaffManager staffManager, bool isNew = false)
        {
            this._staff = staff; // staff object representing the current user
            this._staffManager = staffManager; 
            this._isNewUser = isNew; // Flag to indicate if the user is new. Default false
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Message for any new user
            if (_isNewUser)
            {
                tbkMessage.Text = _staff.GivenName +
                    " as a new user, you must "
                + tbkMessage.Text;
                // Populates the fields for new users
                txtEmail.Text = _staff.Email; 
                pwdOldPassword.Password = "newuser"; // Default password for new users
                txtEmail.IsEnabled = false; // Email can't be changed
                pwdOldPassword.IsEnabled = false; // Old Password can't be changed
            }
            else
            {
                tbkMessage.Text = "You may use this dialog to "
                    + tbkMessage.Text;
            }
            txtEmail.Focus(); // sets focus to email
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Email length must be at least 7
            if (txtEmail.Text.Length < 7 || txtEmail.Text.Length > 100)
            {
                MessageBox.Show("Invalid Email.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                txtEmail.SelectAll();
                return;
            }
            // Password length must be at least 6
            if (pwdOldPassword.Password.Length < 6)
            {
                MessageBox.Show("Invalid Current Password", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwdOldPassword.Focus();
                pwdOldPassword.SelectAll();
                return;
            }
            // New password length must be at least 6
            if (pwdNewPassword.Password.Length < 6)
            {
                MessageBox.Show("Invalid New Password", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwdNewPassword.Focus();
                pwdNewPassword.SelectAll();
                return;
            }
            // Prevents "password" as a new password
            if (string.Equals(pwdNewPassword.Password, "password", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("The password 'password' is not allowed.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwdNewPassword.Focus();
                pwdNewPassword.SelectAll();
                return;
            }
            // Passwords must match
            if (string.Compare(pwdNewPassword.Password, 
                pwdRetypePassword.Password) != 0)
            {
                MessageBox.Show("New Password and Retyped Password must match.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwdRetypePassword.Password = "";
                pwdNewPassword.Focus();
                pwdNewPassword.SelectAll();
                return;
            }
            // Gets user input
            string oldPassword = pwdOldPassword.Password;
            string newPassword = pwdNewPassword.Password;
            string username = txtEmail.Text;
            try
            {
                // Updates the password 
                if (_staffManager.UpdatePassword(username, oldPassword, newPassword))
                {
                    MessageBox.Show("Password Updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
