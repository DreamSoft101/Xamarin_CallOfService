﻿using System.Collections.Generic;
using Xamarin.Forms;

namespace CallOfService.Mobile
{
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage()
        {
            InitializeComponent();

            BackgroundColor = Color.FromHex("#44B6AE");
            ListViewMenu.BackgroundColor = Color.FromHex("#F5F5F5");

            ListViewMenu.ItemsSource = new List<MenuItem>
            {
                new MenuItem {Text = "Log Out"},
                new MenuItem {Text = "Jobs"},
                //new MenuItem {Text = "Settings"}
            };

            ListViewMenu.SelectedItem = null;

            ListViewMenu.ItemSelected += (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;

                if (((MenuItem)e.SelectedItem).Text == "Log Out")
                {
                    ((MainMenuViewModel)BindingContext).LogoutCommand.Execute(null);
                }
                else if (((MenuItem)e.SelectedItem).Text == "Jobs")
                {
                    ((MainMenuViewModel)BindingContext).ShowJobsCommand.Execute(null);
                }
                //else if (((MenuItem)e.SelectedItem).Text == "Settings")
                //{
                //    ((MainMenuViewModel)BindingContext).ShowSettingsCommand.Execute(null);
                //}

                ListViewMenu.SelectedItem = null;
            };

        }
    }
}

