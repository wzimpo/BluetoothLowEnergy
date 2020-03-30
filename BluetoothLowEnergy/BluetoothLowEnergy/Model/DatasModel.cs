using SQLite;

namespace BluetoothLowEnergy.Model
{
    public class DatasModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string DataTitle { get; set; }
        public string DataName { get; set; }
        public string Data { get; set; }
    }
}
