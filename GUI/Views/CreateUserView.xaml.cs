using System;
using System.Windows;



namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for CreateUserView.xaml
    /// </summary>
    public partial class CreateUserView : Window
    {
        public string Email => EmailTextBox.Text.Trim();
        public string UserName => NameTextBox.Text.Trim();
        public string Password => PasswordBox.Password;
        public CreateUserView()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrEmpty(UserName) ||
                string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Udfyld alle felter.", "Manglende oplysninger", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
