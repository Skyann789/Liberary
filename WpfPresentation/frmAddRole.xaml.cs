using DataDomain;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for frmAddRole.xaml
    /// </summary>
    public partial class frmAddRole : Window
    {
        private IStaffManager _staffManager;
        private Action _refreshRoleList; // refreshes the the list after a new role is added
        public frmAddRole(IStaffManager staffManager, Action refreshRoleList)
        {
            _staffManager = staffManager;
            InitializeComponent();
            txtStaffID.IsEnabled = false; // StaffID text box can't be changed
            _refreshRoleList = refreshRoleList;
        }

        public void SetStaffId(int staffID)
        {
            txtStaffID.Text = staffID.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Populates ComboBox with Roles
            List<string> roles = new List<string> { "Admin", "Manager", "Reservation", "CheckIn", "Maintenance" };
            roleComboBox.ItemsSource = roles; // Sets the ComboBox items
        }

        private void AddRoleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int staffID = Convert.ToInt32(txtStaffID.Text);
                string roleID = (string)roleComboBox.SelectedItem; // gets the selected role from combobox

                // Ensures the role combobox isn't empty / null
                if (roleID == null)
                {
                    MessageBox.Show("Please select a role.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Calls the InsertRoleToStaff method to add the role
                bool success = _staffManager.InsertRoleToStaff(staffID, roleID);

                // Displays a success or error message
                if (success)
                {
                    MessageBox.Show("Role added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _refreshRoleList?.Invoke();  // Refresh the list after adding the role
                    this.Close();  // Close the window after successful addition
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Role can not be the same as already assigned role." +
                        " Please chose a different role.", "Invalid Role", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Displays error messages
            }
        }
    }
}
