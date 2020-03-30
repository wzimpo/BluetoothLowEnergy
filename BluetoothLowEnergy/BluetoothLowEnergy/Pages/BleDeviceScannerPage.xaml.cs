using System;
using BluetoothLowEnergy.ViewModel;
using BluetoothLowEnergy.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using nexus.protocols.ble;
using BluetoothLowEnergy.Util;

namespace BluetoothLowEnergy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BleDeviceScannerPage
    {
        public BleDeviceScannerPage(BleDeviceScannerViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void ListView_OnItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                ((BlePeripheralViewModel)e.SelectedItem).IsExpanded = !((BlePeripheralViewModel)e.SelectedItem).IsExpanded;
                ((ListView)sender).SelectedItem = null;
            }
        }

        private void ListView_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
        }


        private void Switch_OnToggled(Object sender, ToggledEventArgs e)
        {
            var vm = BindingContext as BleDeviceScannerViewModel;
            if (vm == null)
            {
                return;
            }
            if (e.Value)
            {
                if (vm.EnableAdapterCommand.CanExecute(null))
                {
                    vm.EnableAdapterCommand.Execute(null);
                }
            }
            else if (vm.DisableAdapterCommand.CanExecute(null))
            {
                vm.DisableAdapterCommand.Execute(null);
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

            var vm = BindingContext as BleDeviceScannerViewModel;
            if (!vm.IsAdapterEnabled)
            {
                if (vm.EnableAdapterCommand.CanExecute(null))
                {
                    vm.EnableAdapterCommand.Execute(null);
                }

            }
            vm.StartScan(BleSampleAppUtils.SCAN_SECONDS_DEFAULT);

        }


    }
}