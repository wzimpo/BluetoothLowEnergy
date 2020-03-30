using Acr.UserDialogs;
using nexus.protocols.ble;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using nexus.core.logging;

namespace BluetoothLowEnergy.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            //var bluetooth = BluetoothLowEnergyAdapter.ObtainDefaultAdapter(ApplicationContext);
            //LoadApplication(new BluetoothLowEnergy.App(BluetoothLowEnergyAdapter  ble, UserDialogs.Instance));
        }
    }
}
