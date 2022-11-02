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
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private UserManager userManager;
        public RegisterWindow(UserManager userManager)
        {
            InitializeComponent();
            this.userManager = userManager;
            cbRegisterCountry.ItemsSource = Enum.GetValues(typeof(Countries));
            tbRegisterUsername.Focus();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            // REFRACTOR THIS MESS OF A CODE VOMIT LATER

            if (!string.IsNullOrEmpty(tbRegisterUsername.Text) && !string.IsNullOrEmpty(pbRegisterPassword.Password) && !string.IsNullOrEmpty(pbRegisterConfirmPassword.Password) && cbRegisterCountry != null)
            {
                bool isValidUsernameLength = userManager.ValidateUsernameLength(tbRegisterUsername.Text);
                bool isValidPasswordLength = userManager.ValidatePasswordLength(pbRegisterPassword.Password);

                
                if (isValidUsernameLength)
                {
                    if (isValidPasswordLength)
                    {
                        if (pbRegisterPassword.Password == pbRegisterConfirmPassword.Password)
                        {
                            User newUser = new(tbRegisterUsername.Text, pbRegisterPassword.Password, (Countries)cbRegisterCountry.SelectedItem);

                            if (userManager.AddUser(newUser))
                            {
                                MessageBox.Show("Registration successful!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("User already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Passwords must match!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password must be at least 5 characters!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Username must be at least 3 characters!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
