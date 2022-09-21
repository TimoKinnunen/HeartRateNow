using System;
using System.ComponentModel;

namespace HeartRateNow.Models
{
    [Serializable]
    public class HeartRateMeasurementModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Frees resources held by this class.
        /// </summary>
        public void Dispose()
        {
        }

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        private DateTimeOffset timestamp = DateTimeOffset.Now;
        public DateTimeOffset Timestamp
        {
            get => timestamp;

            set
            {
                if (timestamp == value)
                { return; }

                timestamp = value;
                OnPropertyChanged("Timestamp");
            }
        }

        private uint heartRateValue = 0;
        public uint HeartRateValue
        {
            get => heartRateValue;

            set
            {
                if (heartRateValue == value)
                { return; }

                heartRateValue = value;
                OnPropertyChanged("HeartRateValue");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion Properties
    }
}
