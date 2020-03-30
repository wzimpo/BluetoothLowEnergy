using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BluetoothLowEnergy.Util
{
    public class BasePage : ContentPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as IBaseViewModel)?.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as IBaseViewModel)?.OnDisappearing();
        }
    }
}
