using System;

namespace CallOfService.Mobile.UI
{
    public interface IViewAwareViewModel : IDisposable
    {
        void OnAppearing();
        void OnDisappearing();
    }
}