using System;
using System.Reflection;
using Acr.UserDialogs;
using BluetoothLowEnergy.Pages;
using BluetoothLowEnergy.ViewModel;
using nexus.core.logging;
using nexus.protocols.ble;
using Xamarin.Forms;
using System.IO;

namespace BluetoothLowEnergy
{
    public partial class App
    {
        private readonly IUserDialogs m_dialogs;
        private readonly NavigationPage m_rootPage;
        static DeviceDatabase database;

        public static DeviceDatabase Database
        {
            get
            {
                if (database == null)
                {
                    try
                    {
                        database = new DeviceDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Device.db"));
                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                }
                return database;
            }
        }

        public App(IBluetoothLowEnergyAdapter adapter, IUserDialogs dialogs)
        {
            InitializeComponent();

            //m_dialogs = dialogs;

            //var bleAssembly = adapter.GetType().GetTypeInfo().Assembly.GetName();
            //Log.Info(bleAssembly.Name + "@" + bleAssembly.Version);
            

            //var gattServerPageModel = new GattServerPageModel(dialogs, adapter);


            //var bleScanViewModel = new BleDeviceScannerViewModel(
            //   bleAdapter: adapter,
            //   dialogs: dialogs,
            //   onSelectDevice: async p =>
            //   {
            //       try
            //       {
            //           await gattServerPageModel.Update(p);
            //           await m_rootPage.PushAsync(new GattServerPage(gattServerPageModel));
                      
            //           await gattServerPageModel.OpenConnection();


            //       }
            //       catch (Exception e)
            //       {

            //           throw;
            //       }

            //   }
            //   );
            //var aa = new BleDeviceScannerPage(bleScanViewModel);
            //m_rootPage = new NavigationPage(new BleDeviceScannerPage(bleScanViewModel));
            MainPage = new MainPageViewModel(adapter, dialogs);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();
            if (Device.RuntimePlatform == Device.UWP)
            {
                Device.StartTimer(
                   TimeSpan.FromSeconds(3),
                   () =>
                   {
                       m_dialogs.Alert(
                      "The UWP API can listen for advertisements but is not yet able to connect to devices.",
                      "Quick Note",
                      "Aww, ok");
                       return false;
                   });
            }
        }
    }
}
