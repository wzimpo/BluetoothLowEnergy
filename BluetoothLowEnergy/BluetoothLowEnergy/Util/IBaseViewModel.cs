using System.ComponentModel;
namespace BluetoothLowEnergy.Util
{
    public interface IBaseViewModel : INotifyPropertyChanged
    {
        void OnAppearing();

        void OnDisappearing();
    }
}
