using Acr.UserDialogs;
using BluetoothLowEnergy.Pages;
using nexus.protocols.ble;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BluetoothLowEnergy.ViewModel
{
    public  class MainPageViewModel : MasterDetailPage
    {
        MasterPageViewModel masterPage;
        private readonly NavigationPage m_rootPage;

        public MainPageViewModel(IBluetoothLowEnergyAdapter adapter, IUserDialogs dialogs)
        {

            masterPage = new MasterPageViewModel(adapter, dialogs);
            Master = masterPage;
            Detail = new NavigationPage(new OtherPage());
            masterPage.ListView.ItemSelected += OnItemSelected;

            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {

                Detail = item.navigation;
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
        public NavigationPage NewPage(IBluetoothLowEnergyAdapter adapter, IUserDialogs dialogs)
        {
            
            var masterPageItems = new List<MasterPageItem>();
            

            var gattServerPageModel = new GattServerPageModel(dialogs, adapter);


            var bleScanViewModel = new BleDeviceScannerViewModel(
               bleAdapter: adapter,
               dialogs: dialogs,
               onSelectDevice: async p =>
               {
                   try
                   {
                       await gattServerPageModel.Update(p);
                       await m_rootPage.PushAsync(new GattServerPage(gattServerPageModel));
                       await gattServerPageModel.OpenConnection();
                   }
                   catch (Exception e)
                   {

                       throw;
                   }

               }
               );
           var rootpage =    new NavigationPage(new BleDeviceScannerPage(bleScanViewModel));

            return rootpage;
        }

    }

}
