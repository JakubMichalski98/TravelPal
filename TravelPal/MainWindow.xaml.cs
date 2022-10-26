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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TravelPal.Managers;
using TravelPal.Models;

namespace TravelPal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserManager userManager = new();
        public MainWindow()
        {
            InitializeComponent();
            tbUsername.Focus();
        }

        public MainWindow(UserManager userManager) : this()
        {
 
            this.userManager = userManager;
        }

        private void ResetLoginUI()
        {
            tbUsername.Clear();
            pbPassword.Clear();
            tbUsername.Focus();
        }
            private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            bool isUserFound = userManager.SignInUser(tbUsername.Text, pbPassword.Password);
            bool isUserAdmin = userManager.CheckIfAdmin();

            if (isUserFound)
            {
                if (isUserAdmin)
                {
                    MessageBox.Show("Admin!");
                }
                else
                {
                    ResetLoginUI();
                    TravelsWindow travelsWindow = new(userManager);
                    travelsWindow.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid Username or Password");
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new(userManager);
            registerWindow.ShowDialog();
        }
    }
}
