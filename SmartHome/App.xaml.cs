﻿using Plugin.LocalNotification;
using SmartHome.Pages;

namespace SmartHome
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Check authentication status
            if (IsAuthenticated())
            {
                MainPage = new AppShell(); // Main application UI
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage()); // Login page
            }

            //MainPage = new AppShell();
        }

        private bool IsAuthenticated()
        {
            // Check for a valid authentication token
            return SecureStorage.Default.GetAsync("AuthToken").Result != null;
        }

        //public async static void ShowLocalNotification(NotificationRequest notification)
        //{
        //    if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        //    {
        //        await LocalNotificationCenter.Current.RequestNotificationPermission();
        //    }

        //    if (notification == null)
        //        return;
            
        //    await LocalNotificationCenter.Current.Show(notification);
        //}
    }
}
