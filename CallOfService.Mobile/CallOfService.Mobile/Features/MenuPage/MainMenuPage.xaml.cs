using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using PubSub;
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
                    new MenuItem { Text = "Log Out" },
                    new MenuItem { Text = "Jobs" }
                };


            ListViewMenu.SelectedItem = null;

            ListViewMenu.ItemSelected += (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;

                if (((MenuItem)e.SelectedItem).Text == "Log Out")
                {
                    ((MainMenuViewModel)BindingContext).LogoutCommand.Execute(null);

                    //var userDialogs = DependencyResolver.Resolve<IUserDialogs>();
                    //var loginService = DependencyResolver.Resolve<ILoginService>();

                    //userDialogs.ShowLoading("Loging out ...");
                    //var logout = await loginService.Logout();
                    //if (logout)
                    //{
                    //    userDialogs.HideLoading();
                    //    this.Publish(new Logout());
                    //}
                    //else
                    //{
                    //    userDialogs.ShowError("Error while logging out!");
                    //}
                }
                else if (((MenuItem)e.SelectedItem).Text == "Jobs")
                {
                    ((MainMenuViewModel)BindingContext).ShowJobsCommand.Execute(null);

                    //var isJobDetailsLastPage = NavigationService.IsJobDetailsPresent();
                    //if (isJobDetailsLastPage)
                    //    await NavigationService.NavigateBack();
                    //this.Publish(new NewDateSelected(DateTime.Now));
                }

                ListViewMenu.SelectedItem = null;
            };

        }
    }
}

