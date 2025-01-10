
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
    /// Interaction logic for frmAddFine.xaml
    /// </summary>
    public partial class frmAddFine : Window
    {
        private MemberManager _memberManager;
        
        private Action _refreshFineList; // Used to refresh the list after a new fine is added
        public frmAddFine(Action refreshFineList)
        {
            InitializeComponent();
            _memberManager = new MemberManager();
            _refreshFineList = refreshFineList;
            txtMemberID.IsEnabled = false; // Member number cannot be changed
        }

        public void SetMemberId(int memberId)
        {
            txtMemberID.Text = memberId.ToString(); // Converts and displays Member ID
        }
        private void btnAddFine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int memberID = Convert.ToInt32(txtMemberID.Text);
                string amountText = txtAmount.Text;
                // Convert the amount to a decimal
                decimal amount = Convert.ToDecimal(amountText);
                DateTime issueDate = dpIssueDate.SelectedDate.GetValueOrDefault();

                // Validate the inputs
                // Issue Date cannot be in the past or the future
                if (issueDate < DateTime.Now.Date)
                {
                    MessageBox.Show("The issue date cannot be in the past. Please select a valid date.");
                    return;
                }
                else if (issueDate > DateTime.Now.Date) // Issue date cannot be in the future
                {
                    MessageBox.Show("The issue date cannot be in the future. Please select a valid date.", 
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Ensures the amount does not contain letters or symbols, I had to look this up
                if (!System.Text.RegularExpressions.Regex.IsMatch(amountText, @"^[0-9]*(\.[0-9]{1,2})?$")) 
                {
                    MessageBox.Show("The amount must be a valid number" +
                        " with up to two decimal places and no letters or symbols.",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Amount cannot be less than 1
                if (amount < 1)
                {
                    MessageBox.Show("The amount cannot be less than 1.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Calls the FineManager to create the fine
                bool success = _memberManager.CreateFine(memberID, amount, issueDate);
                if (success)
                {
                    MessageBox.Show("Fine has been created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _refreshFineList?.Invoke();
                    this.Close();                  
                }
                else
                {
                    MessageBox.Show("An error occurred while creating the fine.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
