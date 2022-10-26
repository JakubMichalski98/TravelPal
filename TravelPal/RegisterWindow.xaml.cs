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
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(tbRegisterUsername.Text) && !string.IsNullOrEmpty(pbRegisterPassword.Password) && !string.IsNullOrEmpty(pbRegisterConfirmPassword.Password) && cbRegisterCountry != null)
            {
                if (pbRegisterPassword.Password != pbRegisterConfirmPassword.Password)
                {
                    MessageBox.Show("Passwords must match!");
                }
                else
                {
                    User newUser = new(tbRegisterUsername.Text, pbRegisterPassword.Password, (Countries)cbRegisterCountry.SelectedItem);

                    if (!userManager.AddUser(newUser))
                    {
                        MessageBox.Show("User already exists!");
                    }
                    else
                    {
                        MessageBox.Show($"Registration successful! ");
                        Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled!");
            }
        }
    }
}