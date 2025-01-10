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
using LogicLayer;
using DataDomain;

namespace WpfPresentation
{
    public partial class frmRemoveRole : Window
    {
        private IStaffManager _staffManager;
        private Action _refreshRoleList; // refreshs the list after role is removed
        private int _staffID; // stores the ID of the staff member whose role is being changed
        private List<string> _roles; // List of roles for the current staff
        public frmRemoveRole(IStaffManager staffManager, List<string> roles, Action refreshRoleList)
        {
            InitializeComponent();
            _staffManager = staffManager;
            txtStaffID.IsEnabled = false; // StaffID Textbox can't be changed
            _refreshRoleList = refreshRoleList;
            _roles = roles;
            // Populates the combobox with list of roles
            cbRole.ItemsSource = _roles;
            if (_roles != null && _roles.Any()) 
            {
                cbRole.ItemsSource = _roles; // Set ComboBox items to the roles
            }
            else
            {
                MessageBox.Show("No roles for this staff member.", "No Roles", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_roles != null && _roles.Any())
            {
                cbRole.ItemsSource = _roles; // Set ComboBox items to the roles
            }
            else
            {
                MessageBox.Show("No roles available for this staff member.");
            }
        }
        public void SetStaffId(int staffID) // sets the StaffID in staff textbox
        {
            _staffID = staffID;
            txtStaffID.Text = _staffID.ToString();
        }
        public void SetStaffRole(int role) //sets the selected role in the combo box
        { 
            cbRole.SelectedItem = role;
        }

        private void btnRemoveRole_Click(object sender, RoutedEventArgs e)
        {
            // Ensures a role is selected before admin can remove it
            if (cbRole.SelectedItem == null)
            {
                MessageBox.Show("Please select a role to remove.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string roleID = cbRole.SelectedItem.ToString(); // gets the selected role
            try
            {
                // attempts to remove the role
                bool isRemoved = _staffManager.RemoveRoleFromStaff(_staffID, roleID);

                if (isRemoved)
                {
                    MessageBox.Show("Role successfully removed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _refreshRoleList?.Invoke();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Role could not be removed. Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        
    }
}
