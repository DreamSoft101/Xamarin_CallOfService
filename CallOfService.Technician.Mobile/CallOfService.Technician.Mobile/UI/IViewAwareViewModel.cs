using System;

namespace CallOfService.Technician.Mobile.UI
{
    public interface IViewAwareViewModel : IDisposable
    {
        void OnAppearing();
        void OnDisappearing();
    }
}