using BluetoothLowEnergy.ViewModel;

namespace BluetoothLowEnergy.Pages
{
	public partial class GattServerPage 
	{
        public GattServerPage(GattServerPageModel othersModel)
        {
            BindingContext = othersModel;
            InitializeComponent();
        }
    }
}