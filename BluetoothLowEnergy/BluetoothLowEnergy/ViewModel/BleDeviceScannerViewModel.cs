using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using BluetoothLowEnergy.Util;
using BluetoothLowEnergy.Model;
using nexus.core.logging;
using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using Xamarin.Forms;
using nexus.core.text;
using nexus.core;

namespace BluetoothLowEnergy.ViewModel
{
    public class BleDeviceScannerViewModel : AbstractScanViewModel
    {
        private readonly Func<BlePeripheralViewModel, Task> m_onSelectDevice;
        private DateTime m_scanStopTime;

        public BleDeviceScannerViewModel(IBluetoothLowEnergyAdapter bleAdapter, IUserDialogs dialogs,
                                          Func<BlePeripheralViewModel, Task> onSelectDevice)
           : base(bleAdapter, dialogs)
        {
            m_onSelectDevice = onSelectDevice;
            FoundDevices = new ObservableCollection<BlePeripheralViewModel>();
            FoundConnectDevices = new ObservableCollection<BlePeripheralViewModel>();
            _alreadyPairedDeviceCount = 0;
            _notPairedDeviceTitle = 0;
            ScanForDevicesCommand =
               new Command(x => { StartScan(x as Double? ?? BleSampleAppUtils.SCAN_SECONDS_DEFAULT); });

        }

        public ObservableCollection<BlePeripheralViewModel> FoundDevices { get; }
        public ObservableCollection<BlePeripheralViewModel> FoundConnectDevices { get; }


        public ICommand ScanForDevicesCommand { get; }

        public int _notPairedDeviceTitle;
        public String notPairedDeviceTitle
        {
            get
            {
                return "未配对设备： " + _notPairedDeviceTitle.ToString();
            }
            private set
            {
                if (value != _notPairedDeviceTitle.ToString())
                {
                    _notPairedDeviceTitle = Convert.ToInt32(value);
                    RaiseCurrentPropertyChanged();
                }
            }

        }
        public int _alreadyPairedDeviceCount;
        public String alreadyPairedDeviceTitle
        {
            get
            {
                return "已配对设备： " + _alreadyPairedDeviceCount.ToString();
            }
            private set
            {
                if (value != _alreadyPairedDeviceCount.ToString())
                {
                    _alreadyPairedDeviceCount = Convert.ToInt32(value);
                    RaiseCurrentPropertyChanged();
                }
            }

        }


        public Int32 ScanTimeRemaining =>
           (Int32)BleSampleAppUtils.ClampSeconds((m_scanStopTime - DateTime.UtcNow).TotalSeconds);

        public async void StartScan(Double seconds)
        {
            FoundDevices.Clear();
            notPairedDeviceTitle = "0";

            if (IsScanning)
            {
                return;
            }

            if (!IsAdapterEnabled)
            {
                m_dialogs.Toast("Cannot start scan, Bluetooth is turned off");
                return;
            }

            StopScan();
            IsScanning = true;
            seconds = BleSampleAppUtils.ClampSeconds(seconds);
            m_scanCancel = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
            m_scanStopTime = DateTime.UtcNow.AddSeconds(seconds);



            Log.Trace("Beginning device scan. timeout={0} seconds", seconds);

            RaisePropertyChanged(nameof(ScanTimeRemaining));
            // RaisePropertyChanged of ScanTimeRemaining while scan is running
            Device.StartTimer(
               TimeSpan.FromSeconds(1),
               () =>
               {
                   RaisePropertyChanged(nameof(ScanTimeRemaining));
                   return IsScanning;
               });

            await m_bleAdapter.ScanForBroadcasts(
                

               peripheral =>
               {

                   Device.BeginInvokeOnMainThread(
                   () =>
                   {

                       var existing = FoundDevices.FirstOrDefault(d => d.Equals(peripheral));
                       var addre = peripheral.Address.Select(b => b.EncodeToBase16String()).Join(":");
                       var deviceModel = App.Database.GetDevice(addre);
                       String deviceName = null;
                       if (App.Database.GetDevice(addre) != null)
                           deviceName = deviceModel.DeviceName;

                       if (existing != null)
                       {
                           existing.Update(peripheral);
                       }
                       else
                       {


                           if (peripheral.Advertisement.DeviceName != null && (deviceName != peripheral.Advertisement.DeviceName) && !IsExist(FoundDevices, addre))
                           {
                               FoundDevices.Add(new BlePeripheralViewModel(peripheral, m_onSelectDevice));
                               notPairedDeviceTitle = FoundDevices.Count.ToString();
                           }

                           
                           if ((peripheral.Advertisement.DeviceName != null) && (deviceName == peripheral.Advertisement.DeviceName) && !IsExist(FoundConnectDevices, addre))

                           {
                               FoundConnectDevices.Add(new BlePeripheralViewModel(peripheral, m_onSelectDevice));


                               alreadyPairedDeviceTitle = FoundConnectDevices.Count.ToString();
                           }
                       }


                   });
               },
               m_scanCancel.Token);

            //DeviceTitle = DeviceTitle + FoundConnectDevices.Count.ToString();

            IsScanning = false;
        }

        public Boolean IsExist(ObservableCollection<BlePeripheralViewModel> bles, string id)
        {
            Boolean sign = false;
            foreach (var aa in bles)
            {
                if (aa.Address == id)
                    sign = true;

            }
            return sign;

        }

    }
}
