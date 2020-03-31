using Acr.UserDialogs;
using BluetoothLowEnergy.Pages;
using BluetoothLowEnergy.ViewModel;
using nexus.protocols.ble;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BluetoothLowEnergy
{
    public class PagesGet
    {
        private readonly NavigationPage m_rootPage;
        public NavigationPage newpage(IBluetoothLowEnergyAdapter adapter, IUserDialogs dialogs)
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
            var rootpage = new NavigationPage(new BleDeviceScannerPage(bleScanViewModel));
            return rootpage;
        }
    }
}
