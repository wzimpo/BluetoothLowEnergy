using System;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using BluetoothLowEnergy.Util;
using nexus.core;
using nexus.core.logging;
using nexus.protocols.ble;
using Xamarin.Forms;

namespace BluetoothLowEnergy.ViewModel
{
    public abstract class AbstractScanViewModel : BaseViewModel
    {
        protected readonly IBluetoothLowEnergyAdapter m_bleAdapter;
        protected readonly IUserDialogs m_dialogs;
        protected CancellationTokenSource m_scanCancel;
        private Boolean m_isScanning;

        protected AbstractScanViewModel(IBluetoothLowEnergyAdapter bleAdapter, IUserDialogs dialogs)
        {
            m_bleAdapter = bleAdapter;
            m_dialogs = dialogs;

            EnableAdapterCommand = new Command(async () => await ToggleAdapter(true));
            DisableAdapterCommand = new Command(async () => await ToggleAdapter(false));

            m_bleAdapter.CurrentState.Subscribe(state => { RaisePropertyChanged(nameof(IsAdapterEnabled)); });
        }

        public ICommand DisableAdapterCommand { get; }

        public ICommand EnableAdapterCommand { get; }

        public Boolean IsAdapterEnabled => m_bleAdapter.CurrentState.Value == EnabledDisabledState.Enabled ||
                                           m_bleAdapter.CurrentState.Value == EnabledDisabledState.Unknown;
        public Boolean IsRemembered
        {
            get;
            set;
        }


        public Boolean IsScanning
        {
            get { return m_isScanning; }
            protected set { Set(ref m_isScanning, value); }
        }

        public override void OnAppearing()
        {
            RaisePropertyChanged(nameof(IsAdapterEnabled));
        }

        public override void OnDisappearing()
        {
            StopScan();
        }

        public void StopScan()
        {
            m_scanCancel?.Cancel();
        }

        private async Task ToggleAdapter(Boolean enable)
        {
            StopScan();
            try
            {
                await (enable ? m_bleAdapter.EnableAdapter() : m_bleAdapter.DisableAdapter());
            }
            catch (SecurityException ex)
            {
                m_dialogs.Toast(ex.Message);
                Log.Debug(ex, nameof(BleDeviceScannerViewModel));
            }
            RaisePropertyChanged(nameof(IsAdapterEnabled));
        }
    }
}
