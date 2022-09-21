using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Media.Imaging;

namespace HeartRateNow.Models
{
    /// <summary>
    ///     Display class used to represent a BluetoothLEDevice in the Device list
    /// </summary>
    public class BluetoothLEDeviceDisplay : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Frees resources held by this class.
        /// </summary>
        public void Dispose()
        {
        }

        public BluetoothLEDeviceDisplay(DeviceInformation deviceInformation)
        {
            DeviceInformation = deviceInformation;
            UpdateGlyphBitmapImage();
        }

        public DeviceInformation DeviceInformation { get; private set; }

        public string Id => DeviceInformation.Id;
        public string Name => DeviceInformation.Name;
        public bool IsConnected => (bool?)DeviceInformation.Properties["System.Devices.Aep.IsConnected"] == true;

        public IReadOnlyDictionary<string, object> Properties => DeviceInformation.Properties;

        public BitmapImage GlyphBitmapImage { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Update(DeviceInformationUpdate deviceInformationUpdate)
        {
            DeviceInformation.Update(deviceInformationUpdate);

            OnPropertyChanged("Id");
            OnPropertyChanged("Name");
            OnPropertyChanged("DeviceInformation");
            OnPropertyChanged("IsConnected");
            OnPropertyChanged("Properties");

            UpdateGlyphBitmapImage();
        }

        private async void UpdateGlyphBitmapImage()
        {
            try
            {
                DeviceThumbnail deviceThumbnail = await DeviceInformation.GetGlyphThumbnailAsync();
                BitmapImage glyphBitmapImage = new BitmapImage();
                await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
                GlyphBitmapImage = glyphBitmapImage;
                OnPropertyChanged("GlyphBitmapImage");
            }
            catch
            {
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static explicit operator BluetoothLEDeviceDisplay(BluetoothLEDevice v)
        {
            throw new NotImplementedException();
        }
    }
}