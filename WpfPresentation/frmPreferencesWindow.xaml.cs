using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfPresentation
{
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            InitializeComponent();
        }

        private void btnClosePreferencesWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
