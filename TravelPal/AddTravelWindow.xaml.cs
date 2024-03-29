﻿using System;
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
using TravelPal.Enums;
using TravelPal.Interfaces;
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal
{
    /// <summary>
    /// Interaction logic for AddTravelWindow.xaml
    /// </summary>

    public partial class AddTravelWindow : Window
    {
        // TODO Refactor this shit it's such a mess, also test packinglist for bugs but if it works just look if it can be done better because yes messy
        private TravelManager travelManager;
        private User? signedInUser;
        private ListViewItem packingListViewItem = new();
        private List<IPackingListItem> packingList = new();
        public AddTravelWindow(UserManager userManager, TravelManager travelManager)
        {
            InitializeComponent();
            this.travelManager = travelManager;
            this.signedInUser = userManager.SignedInUser as User;

            cbDetailsCountry.ItemsSource = Enum.GetValues(typeof(Countries));
            cbTravelType.ItemsSource = travelManager.TravelTypes;
            cbTripType.ItemsSource = Enum.GetValues(typeof(TripTypes));

            dpStartingDate.DisplayDateStart = DateTime.Now;
            dpEndingDate.DisplayDateStart = DateTime.Now;
        }

        // Clears the packing list fields
        private void ClearPackingListUI()
        {
            tbItemName.Clear();
            tbQuantity.Clear();
            xbDocument.IsChecked = false;
            xbRequired.IsChecked = false;
        }

        // Checks if the required checkbox is checked
        private bool CheckIfRequiredIsChecked()
        {
            if (xbRequired.IsChecked == true)
            {
                return true;
            }
            return false;
        }

        // Determines whether a passport is required or not, based on user location and travel destination
        private bool DetermineIfDocumentRequired()
        {
            string userLocation = signedInUser.Location.ToString();
            string travelLocation = cbDetailsCountry.SelectedItem.ToString();

            if (Enum.TryParse<EuropeanCountries>(userLocation, out EuropeanCountries result1) && !Enum.TryParse<EuropeanCountries>(travelLocation, out EuropeanCountries result2))
            {
                return true;
            }
            else if (Enum.TryParse<EuropeanCountries>(userLocation, out EuropeanCountries result3) && Enum.TryParse<EuropeanCountries>(travelLocation, out EuropeanCountries result4))
            {
                return false;
            }
            return true;
        }

        // Determines the travel type (Trip or Vacation)
        private string DetermineTravelType()
        {
            if (cbTravelType.SelectedItem != null)
            {
                if (cbTravelType.SelectedItem == "Trip")
                {
                    return "Trip";
                }
                else if (cbTravelType.SelectedItem == "Vacation")
                {
                    return "Vacation";
                }
            }
            return null;
        }

        // Modifies the UI based on whether Trip or Vacation has been selected
        private void ModifyTravelTypeUI(string travelType)
        {
            if (travelType == "Trip")
            {
                xbAllInclusive.Visibility = Visibility.Hidden;
                txtTripType.Visibility = Visibility.Visible;
                cbTripType.Visibility = Visibility.Visible;
                cbTripType.ItemsSource = Enum.GetValues(typeof(TripTypes));
            }
            else if (travelType == "Vacation")
            {
                txtTripType.Visibility = Visibility.Hidden;
                cbTripType.Visibility = Visibility.Hidden;
                xbAllInclusive.Visibility = Visibility.Visible;
            }
        }

        // Checks if all fields are filled correctly, displays information to user if an input is incorrect
        private bool ValidateInput()
        {
            if (dpStartingDate.SelectedDate != null && dpEndingDate.SelectedDate != null && !string.IsNullOrEmpty(tbDestination.Text) && !string.IsNullOrEmpty(tbTravelers.Text) && cbTravelType.SelectedItem != null && cbDetailsCountry.SelectedItem != null)
            {
                if (dpEndingDate.SelectedDate >= dpStartingDate.SelectedDate)
                {
                    if (int.TryParse(tbTravelers.Text, out int result))
                    {
                        if (DetermineTravelType() == "Trip")
                        {
                            if (cbTripType.SelectedItem != null)
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Please choose a trip type!", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return false;
                            }
                        }
                        else if (DetermineTravelType() == "Vacation")
                        {
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please input a valid amount of travellers!", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("How you gon' travel back in time? Ending date has to be later than starting date yo", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled!", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return false;
        }

        // If input validation is successful - creates the appropriate travel type and adds it to the general travels list as well as currently signed in user's travels list together with packing list and notifies user adding was successful
        private void btnAddTravel_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                if (DetermineTravelType() == "Trip")
                {
                    Trip newTrip = new(tbDestination.Text, (Countries)cbDetailsCountry.SelectedItem, Convert.ToInt32(tbTravelers.Text), (DateTime)dpStartingDate.SelectedDate, (DateTime)dpEndingDate.SelectedDate, (TripTypes)cbTripType.SelectedItem, signedInUser);
                    travelManager.AddTravel(newTrip);
                    newTrip.PackingList.AddRange(packingList);
                }
                else if (DetermineTravelType() == "Vacation")
                {
                    Vacation newVacation;

                    if (xbAllInclusive.IsChecked == true)
                    {
                        newVacation = new(tbDestination.Text, (Countries)cbDetailsCountry.SelectedItem, Convert.ToInt32(tbTravelers.Text), (DateTime)dpStartingDate.SelectedDate, (DateTime)dpEndingDate.SelectedDate, true, signedInUser);

                    }
                    else
                    {
                        newVacation = new(tbDestination.Text, (Countries)cbDetailsCountry.SelectedItem, Convert.ToInt32(tbTravelers.Text), (DateTime)dpStartingDate.SelectedDate, (DateTime)dpEndingDate.SelectedDate, false, signedInUser);
                    }
                    travelManager.AddTravel(newVacation);
                    newVacation.PackingList.AddRange(packingList);
                }

                MessageBox.Show("Added!");
                this.Close();
            }
        }

        // Modifies the UI based on chosen travel type every time it is changed
        private void cbTravelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModifyTravelTypeUI(DetermineTravelType());
        }

        // Whenever country selection is changed, checks if passport is required and adds/updates passport item in user's packing list and listview
        private void cbDetailsCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DetermineIfDocumentRequired())
            {
                if (lvPackingList.Items.Count > 0)
                {
                    lvPackingList.Items.Remove(packingListViewItem);
                    packingList.Remove(packingListViewItem.Tag as IPackingListItem);
                }

                TravelDocument newTravelDocument = new("Passport", true);
                packingListViewItem.Content = newTravelDocument.GetInfo();
                packingListViewItem.Tag = newTravelDocument;
                lvPackingList.Items.Add(packingListViewItem);
                packingList.Add(newTravelDocument);
            }
            else
            {
                if (lvPackingList.Items.Count > 0)
                {
                    lvPackingList.Items.Remove(packingListViewItem);
                    packingList.Remove(packingListViewItem.Tag as IPackingListItem);
                }

                TravelDocument newTravelDocument = new("Passport", false);
                packingListViewItem.Content = newTravelDocument.GetInfo();
                packingListViewItem.Tag = newTravelDocument;
                lvPackingList.Items.Add(packingListViewItem);
                packingList.Add(newTravelDocument);
            }
        }

        // If there is a selection, removes item from packing list of user and listview, otherwise shows warning message
        private void btnPackingListRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lvPackingList.SelectedItem != null)
            {
                ListViewItem item = lvPackingList.SelectedItem as ListViewItem;

                signedInUser.Travels.Remove(item.Tag as Travel);
                lvPackingList.Items.Remove(item);
            }
            else
            {
                MessageBox.Show("Selection required!", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Shows required checkbox and hides quantity input
        private void xbDocument_Checked(object sender, RoutedEventArgs e)
        {
            xbRequired.Visibility = Visibility.Visible;
            tbQuantity.Visibility = Visibility.Hidden;
            txtQuantity.Visibility = Visibility.Hidden;
        }

        // Unchecks checked checkbox, hides required checkbox and shows quantity input
        private void xbDocument_Unchecked(object sender, RoutedEventArgs e)
        {
            xbRequired.IsChecked = false;
            xbRequired.Visibility = Visibility.Hidden;
            tbQuantity.Visibility = Visibility.Visible;
            txtQuantity.Visibility = Visibility.Visible;
        }

        // Validates if name and quantity input is valid and adds item to user's packing list and listview
        private void btnPackingListAdd_Click(object sender, RoutedEventArgs e)
        {
            bool isRequired = CheckIfRequiredIsChecked();
            ListViewItem newPackingListItem = new();

            if (!String.IsNullOrEmpty(tbItemName.Text))
            {

                if (xbDocument.IsChecked == true)
                {
                    TravelDocument newDocument = new(tbItemName.Text, isRequired);
                    newPackingListItem.Tag = newDocument;
                    newPackingListItem.Content = newDocument.GetInfo();
                    lvPackingList.Items.Add(newPackingListItem);
                    packingList.Add(newDocument);
                }
                else
                {
                    if (!String.IsNullOrEmpty(tbQuantity.Text) && int.TryParse(tbQuantity.Text, out int result))
                    {
                        OtherItem newOtherItem = new(tbItemName.Text, Convert.ToInt32(tbQuantity.Text));
                        newPackingListItem.Tag = newOtherItem;
                        newPackingListItem.Content = newOtherItem.GetInfo();
                        lvPackingList.Items.Add(newPackingListItem);
                        packingList.Add(newOtherItem);

                    }
                    else
                    {
                        MessageBox.Show("Please input a valid quantity!", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled!", "warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            ClearPackingListUI();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
