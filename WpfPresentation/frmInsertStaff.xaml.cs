using DataDomain;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    /// Interaction logic for frmInsertStaff.xaml
    /// </summary>
    public partial class frmInsertStaff : Window
    {
        private StaffManager _staffManager;
        public frmInsertStaff()
        {
            InitializeComponent();
            _staffManager = new StaffManager();
            // Sets password to newuser
            txtPassword.Password = "newuser";
            // Makes the box view only and not able to be changed
            txtPassword.IsEnabled = false;

            // Makes the txtbx view only and not able to be changed
            txtEmail.IsEnabled = false;
        }
        private void btnInsertStaff_Click(object sender, RoutedEventArgs e)
        {
            // Gather input from form fields
            string givenName = txtGivenName.Text;
            string familyName = txtFamilyName.Text;
            string phone = txtPhone.Text;
            string password = txtPassword.Password;

            // Auto-fill the email as lowercase givenName + @center.com
            string email = givenName.ToLower() + "@center.com";


            // If a Staff member with the same givenName and familyName
            // already exists the status message will display a message 
            // to change one in order to insert the staff.
            if (IsNameAlreadyExist(givenName, familyName))
            {
                MessageBox.Show("A staff member with this name already exists. Please change the Given Name or Family Name to insert this user." , "Error",
                     MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            // Validates GivenName and FamilyName lengths
            if (givenName.Length > 50)
            {
                MessageBox.Show("Given Name cannot be longer than 50 characters.", "Error",
                                 MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (familyName.Length > 100)
            {
                MessageBox.Show("Family Name cannot be longer than 100 characters.", "Error",
                                 MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Displays a message if given name, family name, or phone number are left empty
            if (string.IsNullOrEmpty(givenName))
            {
                MessageBox.Show("The Given Name text box can not be left empty. Please enter given name to continue.", "Error",
                     MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty (familyName))
            {
                MessageBox.Show("The Family Name text box can not be left empty. Please enter a family name to continue.", "Error",
                     MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("The Phone text box can not be left empty. Please enter a valid phone number.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // If a person with the same email already exists 
            // the first letter of the familyName will be added behind their giveName
            // before the @center.com is added and it will be added as lowercase letter

            if (IsEmailAlreadyExist(givenName))
            {
                email = givenName.ToLower() + familyName[0].ToString().ToLower() + "@center.com";
            }

            txtEmail.Text = email;


            // Validate fields


            // Given and Famaily Name should only contain letters and the first letter of both must be capitalized
            // and the rest lower
            if (!IsValidName(ref givenName))
            {
                MessageBox.Show("Given Name must only contain letters and must have the first letter" +
                   " capitalized and the rest lower case.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!IsValidName(ref familyName))
            {
                MessageBox.Show("Family Name must only contain letters and must have the first letter" +
                  " capitalized and the rest lower case.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Validate phone
            // Phone must only contain numbers and must be 11 digits long
            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Phone number must be exactly 11 digits." , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Ensures all fields are filled before Staff is added
            // Password not added because it is auto filled
            if (string.IsNullOrEmpty(givenName) || string.IsNullOrEmpty(familyName) || string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Retrieve selected role from the ComboBox
            string selectedRole = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Validate that a role is selected
            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Please select a role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Create new staff object
            Staff newStaff = new Staff
            {
                GivenName = givenName,
                FamilyName = familyName,
                Email = email,
                Phone = phone,
                PasswordHash = password,
                Active = true // Set as active by default
            };

            try
            {
                _staffManager.InsertStaff(givenName, familyName, phone, email, selectedRole);


                // Refreshes the staff list
                // Calls the LoadStaffData on the MainWindow to refresh the listView
                ((MainWindow)Application.Current.MainWindow).LoadStaff();

                // Displays a message box to inform the admin the staff has been added successfully
                // before the frmInsertStaff form is closed
                MessageBox.Show("Staff Added", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                // Closes the Insert Staff window after successful insertion of staff
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        // Method to check if a staff member with the same givenName and familyName already exists in the listView
        private bool IsNameAlreadyExist(string givenName, string familyName)
        {
            var existingStaffList = _staffManager.SelectAllStaff();
            // Check if there is any staff member with the same givenName and familyName
            return existingStaffList.Any(staff => staff.GivenName.Equals(givenName, StringComparison.OrdinalIgnoreCase) &&
                                                   staff.FamilyName.Equals(familyName, StringComparison.OrdinalIgnoreCase));
        }
        // Method to check if a staff member with the same email already exists in the listView already exists
        private bool IsEmailAlreadyExist(string givenName)
        {
            var existingStaffList = _staffManager.SelectAllStaff();
            // Check if there is any staff member with the same givenName
            return existingStaffList.Any(staff => staff.GivenName.Equals(givenName, StringComparison.OrdinalIgnoreCase));
        }

        // Method that ensures givenName and familyName only contain letters
        // ensure that given name and family name have values
        private bool IsValidName(ref string name)
        {
            // Check if the name contains only letters
            if (!name.All(char.IsLetter))
            {
                return false;
            }

            // Check if the name follows the pattern (first letter uppercase, rest lowercase)
            if (name != char.ToUpper(name[0]) + name.Substring(1).ToLower())
            {
                return false;
            }

            return true;
        }

        // Method that ensures phone number is 11 long and only contains numbers
        private bool IsValidPhoneNumber(string phone)
        {
            return phone.Length == 11 && phone.All(char.IsDigit);
        }

    }
}
