using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothLowEnergy.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OtherPage : ContentPage
	{
		public OtherPage ()
		{
			InitializeComponent ();
		}
        protected override  void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = App.Database.GetDatas();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            
            //"/data/user/0/com.companyname.BluetoothLowEnergy/files/.local/share/Device.db"
             
        }
    }
}