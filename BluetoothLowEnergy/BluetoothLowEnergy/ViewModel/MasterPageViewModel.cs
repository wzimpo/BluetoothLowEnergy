using BluetoothLowEnergy.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using BluetoothLowEnergy;
using Acr.UserDialogs;
using nexus.protocols.ble;
using System.Reflection;

namespace BluetoothLowEnergy.ViewModel
{
      public  class MasterPageViewModel : ContentPage
    {
        private readonly IUserDialogs m_dialogs;
        private readonly NavigationPage m_rootPage;
        public ListView ListView { get { return listView; } }

        ListView listView;

        public MasterPageViewModel(IBluetoothLowEnergyAdapter adapter, IUserDialogs dialogs)
        {
            var masterPageItems = new List<MasterPageItem>();

            m_dialogs = dialogs;
            
            var gattServerPageModel = new GattServerPageModel(dialogs, adapter);


            var bleScanViewModel = new BleDeviceScannerViewModel(
               bleAdapter: adapter,
               dialogs: dialogs,
               onSelectDevice: async p =>
               {
                   try
                   {
                       await gattServerPageModel.Update(p);
                       await m_rootPage.PushAsync(new GattServerPage(gattServerPageModel));
                       await gattServerPageModel.OpenConnection();
                   }
                   catch (Exception e)
                   {

                       throw;
                   }

               }
               );
            var aa = new BleDeviceScannerPage(bleScanViewModel);
            m_rootPage = new NavigationPage(new BleDeviceScannerPage(bleScanViewModel));


            masterPageItems.Add(new MasterPageItem
            {
                Title = "蓝牙模块",
                IconSource = "contacts.png",

                navigation = m_rootPage
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "其他",
                IconSource = "todo.png",
                navigation = new NavigationPage(new OtherPage())
        });
          

            listView = new ListView
            {
                ItemsSource = masterPageItems,
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid { Padding = new Thickness(5, 10) };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                    var image = new Image();
                    image.SetBinding(Image.SourceProperty, "IconSource");
                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Title");

                    grid.Children.Add(image);
                    grid.Children.Add(label, 1, 0);

                    return new ViewCell { View = grid };
                }),
                SeparatorVisibility = SeparatorVisibility.None
            };
            
            Title = "Personal Organiser";
            Padding = new Thickness(0, 40, 0, 0);
            Content = new StackLayout
            {
                Children = { listView }
            };
        }
      }  
}

