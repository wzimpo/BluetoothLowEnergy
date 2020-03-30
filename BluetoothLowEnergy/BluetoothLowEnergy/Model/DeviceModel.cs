using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BluetoothLowEnergy.Model
{
    public class DeviceModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceID { get; set; }
    }
}
