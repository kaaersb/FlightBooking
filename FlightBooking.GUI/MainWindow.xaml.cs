using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using FlightBooking.Core.Models;
using FlightBooking.Core.Data;

namespace FlightBooking.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly HttpClient client = new HttpClient();
        private readonly IFlightRepository _flightRepo = new FlightRepository();

        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var flights = await _flightRepo.SearchAsync(OriginTextbox.Text, 
                                                        DestinationTextbox.Text, 
                                                        DepatureDatepicker.SelectedDate.Value,
                                                        OneWayCheckbox.IsChecked == true ? ReturnDatePicker.SelectedDate : null
                                                        );
            FlightsGrid.ItemsSource = flights;

        }
    }
}