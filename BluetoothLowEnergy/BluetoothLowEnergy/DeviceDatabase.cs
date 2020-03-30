using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using BluetoothLowEnergy.Model;

namespace BluetoothLowEnergy
{
    public class DeviceDatabase
    {
        readonly SQLiteConnection _database;

        public DeviceDatabase(string dbPath)
        {
            try
            {
                _database = new SQLiteConnection(dbPath);
                _database.CreateTable<DeviceModel>();
                _database.CreateTable<DatasModel>();
                SetDatas();
            }
            catch (SQLiteException e)
            {

                throw;
            }

        }

        public  string  CopyDatabase()
        {
            var dbpath = _database.DatabasePath;
            return dbpath;
        }
        private void SetDatas()
        {
            DatasModel datasModel1 = new DatasModel();
            datasModel1.Data = "data1";
            datasModel1.DataName = "dataname1";
            datasModel1.DataTitle = "datatitle1";
            _database.Insert(datasModel1);

            DatasModel datasModel2 = new DatasModel();
            datasModel2.Data = "data2";
            datasModel2.DataName = "dataname2";
            datasModel2.DataTitle = "datatitle2";
            _database.Insert(datasModel2);
        }

        public List<DeviceModel> GetDevice()
        {
            return _database.Table<DeviceModel>().ToList();
        }
        public List<DatasModel> GetDatas()
        {
            return _database.Table<DatasModel>().ToList();
        }
        public DeviceModel GetDevice(string id)
        {
            return _database.Table<DeviceModel>()
                            .Where(i => i.DeviceID == id).FirstOrDefault();
        }

        public int SaveDevice(DeviceModel device)
        {
            if (device.DeviceName != null)
            {
                return _database.Update(device);
            }
            else
            {
                return _database.Insert(device);
            }
        }
        public int AddDevice(DeviceModel device)
        {
            if (device.DeviceName != null)
            {
                return _database.Insert(device);
            }
            return 0;
        }


        public int DeleteDevice(string id)
        {
            var sql = @"delete from DeviceModel where DeviceID = '" + id + @"'";
            return _database.Execute(sql);
        }





    }
}
