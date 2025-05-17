using FlightBooking.Core.Data;
using FlightBooking.Core.Models;
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

namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Window
    {

        private readonly User user;
        public ProfileView(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            LoadBookings();
        }

        private void LoadBookings()
        {
            BookingRepository bookingRepository = new BookingRepository(Config.ConnectionString);
            IEnumerable<Booking> bookings = bookingRepository.GetByUserId(user.UserId);
            UserBookings.ItemsSource = bookings;
        }

        private void Tilbage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
