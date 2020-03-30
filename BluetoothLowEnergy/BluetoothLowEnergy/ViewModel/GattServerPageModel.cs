using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using BluetoothLowEnergy.Util;
using nexus.core;
using nexus.core.logging;
using nexus.core.text;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using Xamarin.Forms;

namespace BluetoothLowEnergy.ViewModel
{
    public class GattServerPageModel : BaseViewModel
    {
        private const Int32 CONNECTION_TIMEOUT_SECONDS = 15;
        private String m_valueAsString;
        private String m_unknownValue;
        private String m_writeValue;
        private String m_reciveData;
        private String m_listingValue;
        private IDisposable notifyHandler;
        private String m_connectionState;
        protected readonly IBluetoothLowEnergyAdapter m_bleAdapter;
        protected readonly IUserDialogs m_dialogManager;
        protected IBleGattServerConnection m_gattServer;
        protected Boolean m_isBusy;
        protected BlePeripheralViewModel m_peripheral;
        public Boolean IsConnectedOrConnecting =>
           m_isBusy || m_connectionState != ConnectionState.Disconnected.ToString();

        public Boolean NotifyEnabled
        {
            get { return notifyHandler != null; }
            set
            {
                if (value != (notifyHandler != null))
                {
                    if (value)
                    {
                        EnableNotifications();
                    }
                    else
                    {
                        DisableNotifications();
                    }
                    RaiseCurrentPropertyChanged();
                }
            }
        }
        

     
        public ICommand WriteBytesCommand { get; }
      

        public String DeviceName => m_peripheral?.DeviceName;

        public ICommand DisconnectFromDeviceCommand { get; }
        public ICommand ListingCommand { get; }

        public String Manufacturer => m_peripheral?.Manufacturer;

        public String Name => m_peripheral?.Name;

        public String PageTitle => "BLE Device GATT Server";


        public Int32? Rssi => m_peripheral?.Rssi;

        public String Address => m_peripheral?.Address;

        public String AddressAndName =>
           m_peripheral?.AddressAndName; //Address + " / " + (DeviceName ?? "<no device name>");

        public String ValueAsString
        {
            get { return m_valueAsString; }
            private set { Set(ref m_valueAsString, value); }
        }
        public String UnknownValue
        {
            get { return m_unknownValue; }
            private set { Set(ref m_unknownValue, value); }
        }
        public String ReciveData
        {
            get { return m_reciveData; }
            private set { Set(ref m_reciveData, value); }
        }

        public String ListingValue
        {
            get { return m_listingValue; }
            private set { Set(ref m_listingValue, value); }
        }
        public String WritesValue
        {
            get { return m_writeValue; }

            set {
                m_writeValue = value;
            }
        }
        public Boolean IsBusy
        {
            get { return m_isBusy; }
            protected set
            {
                if (value != m_isBusy)
                {
                    m_isBusy = value;
                    RaiseCurrentPropertyChanged();
                    RaisePropertyChanged(nameof(IsConnectedOrConnecting));
                }
            }
        }

        public GattServerPageModel(IUserDialogs dialogsManager, IBluetoothLowEnergyAdapter bleAdapter)
        {
            m_bleAdapter = bleAdapter;
            WriteBytesCommand = new Command(async () => { await WriteCurrentBytes(); });
            m_connectionState = ConnectionState.Disconnected.ToString();
            m_dialogManager = dialogsManager;
        }

        //68 20 48 00 69 03 00 55 55 01 03 1F 90 00 99 16

        private async Task WriteCurrentBytes()
        {
            var w = WritesValue.Replace(" ","");
            byte[] temp;  
            if (w.Length > 10)
            {
                byte[] wtemp = new byte[w.Length / 2];
                for (int i = 0; i < wtemp.Length; i++)
                {
                    string bb = w.Substring(i * 2, 2);
                    wtemp[i] = (byte)Convert.ToInt32("0x" + bb, 16);
                }
                temp = wtemp;
            }
            else
            {
                temp = w.GetUtf8Bytes();
            }

            Guid Serviceuuid = new Guid("0000ffe0-0000-1000-8000-00805f9b34fb");
            Guid Charauuid = new Guid("0000ffe1-0000-1000-8000-00805f9b34fb");
            if (!w.IsNullOrEmpty())
            {

                try
                {
                    
                    IsBusy = true;
                    var writeTask = m_gattServer.WriteCharacteristicValue(Serviceuuid, Charauuid, temp);
                    Byte[] bytes = await writeTask;

                    ReciveData = ReciveData + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " send: \r\n" + w;

                }
                catch (GattException ex)
                {
                    Log.Warn(ex.ToString());
                    m_dialogManager.Toast(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }

        }
        public String Connection
        {
            get { return m_connectionState; }
            private set
            {
                if (value != m_connectionState)
                {
                    m_connectionState = value;
                    RaiseCurrentPropertyChanged();
                    RaisePropertyChanged(nameof(IsConnectedOrConnecting));
                }
            }
        }
       

       

       

        public async Task Update(BlePeripheralViewModel peripheral)
        {
            if (m_peripheral != null && !m_peripheral.Model.Equals(peripheral.Model))
            {
                await CloseConnection();
            }

            m_peripheral = peripheral;
        }
        public async Task OpenConnection()
        {
            // if we're busy or have a valid connection, then no-op
            if (IsBusy || m_gattServer != null && m_gattServer.State != ConnectionState.Disconnected)
            {
                //Log.Debug( "OnAppearing. state={0} isbusy={1}", m_gattServer?.State, IsBusy );
                return;
            }

            await CloseConnection();
            IsBusy = true;

            var connection = await m_bleAdapter.ConnectToDevice(
               device: m_peripheral.Model,
               timeout: TimeSpan.FromSeconds(CONNECTION_TIMEOUT_SECONDS),
               progress: progress => { Connection = progress.ToString(); });
            if (connection.IsSuccessful())
            {
                m_gattServer = connection.GattServer;
                Log.Debug("Connected to device. id={0} status={1}", m_peripheral.Id, m_gattServer.State);

                m_gattServer.Subscribe(
                   async c =>
                   {
                       if (c == ConnectionState.Disconnected)
                       {
                           m_dialogManager.Toast("Device disconnected");
                           await CloseConnection();
                       }

                       Connection = c.ToString();
                   });

                Connection = "Reading Services";
                try
                {
                    var services = (await m_gattServer.ListAllServices()).ToList();

                    Connection = m_gattServer.State.ToString();
                }
                catch (GattException ex)
                {
                    Log.Warn(ex);
                    m_dialogManager.Toast(ex.Message, TimeSpan.FromSeconds(3));
                }
            }
            else
            {
                String errorMsg;
                if (connection.ConnectionResult == ConnectionResult.ConnectionAttemptCancelled)
                {
                    errorMsg = "Connection attempt cancelled after {0} seconds (see {1})".F(
                       CONNECTION_TIMEOUT_SECONDS,
                       GetType().Name + ".cs");
                }
                else
                {
                    errorMsg = "Error connecting to device: {0}".F(connection.ConnectionResult);
                }

                Log.Info(errorMsg);
                m_dialogManager.Toast(errorMsg, TimeSpan.FromSeconds(5));
            }

            IsBusy = false;
        }


        private async Task CloseConnection()
        {
            IsBusy = true;
            if (m_gattServer != null)
            {
                Log.Trace("Closing connection to GATT Server. state={0:g}", m_gattServer?.State);
                await m_gattServer.Disconnect();
                m_gattServer = null;
            }

            //Services.Clear();
            IsBusy = false;
        }



        private void DisableNotifications()
        {
            notifyHandler?.Dispose();
            notifyHandler = null;
            RaisePropertyChanged(nameof(NotifyEnabled));
        }

        private void EnableNotifications()
        {
            Guid Serviceuuid = new Guid("0000ffe0-0000-1000-8000-00805f9b34fb");
            Guid Charauuid = new Guid("0000ffe1-0000-1000-8000-00805f9b34fb");
            if (notifyHandler == null)
            {
                try
                {
                    notifyHandler = m_gattServer.NotifyCharacteristicValue(
                       Serviceuuid,
                       Charauuid,
                       bytes => { ReciveData = ReciveData + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Revice : \r\n" + bytes.AsUtf8String(); } );
                }
                catch (GattException ex)
                {
                    m_dialogManager.Toast(ex.Message);
                }
            }
            RaisePropertyChanged(nameof(NotifyEnabled));
        }


    }
}
