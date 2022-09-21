using HeartRateNow.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HeartRateNow.Helpers
{
    public sealed class HelpHeartRateMeasurements
    {
        private static AppProperties myAppViewModel = App.MyApp;
        public static AppProperties MyAppViewModel { get => myAppViewModel; }

        private const int maxMinutes = 60;

        public static int MaxMinutes => maxMinutes;

        private static Task RemoveMeasurements()
        {
            // keep only 60 minutes of measurements in memory
            DateTimeOffset maxDateTimeOffset = new DateTimeOffset(DateTime.Now.Subtract(new TimeSpan(0, 0, MaxMinutes, 1, 0)));

            ObservableCollection<HeartRateMeasurementModel> measurementsToRemove = new ObservableCollection<HeartRateMeasurementModel>();

            IEnumerable<HeartRateMeasurementModel> query = MyAppViewModel.HeartRateMeasurements.Where(x => x.Timestamp < maxDateTimeOffset);

            foreach (HeartRateMeasurementModel measurementToRemove in query)
            {
                measurementsToRemove.Add(measurementToRemove);
            }

            foreach (HeartRateMeasurementModel measurementToRemove in measurementsToRemove)
            {
                MyAppViewModel.HeartRateMeasurements.Remove(measurementToRemove);
            }

            return Task.FromResult(true);
        }

        public async static Task<IReadOnlyList<HeartRateMeasurementModel>> GetMeasurementsOrderByTimestampAsync(TimeSpan queryTimeSpan)
        {
            ObservableCollection<HeartRateMeasurementModel> heartRateMeasurements = null;

            heartRateMeasurements = new ObservableCollection<HeartRateMeasurementModel>(MyAppViewModel.HeartRateMeasurements);

            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.Now.Subtract(queryTimeSpan));

            IReadOnlyList<HeartRateMeasurementModel> heartRateMeasurementsList = heartRateMeasurements.OrderBy(x => x.Timestamp).Where(x => x.Timestamp >= dateTimeOffset).ToList();

            return await Task.FromResult(heartRateMeasurementsList);
        }

        public async static Task<IReadOnlyList<HeartRateMeasurementModel>> GetMeasurementsOrderByTimestampAsync()
        {
            ObservableCollection<HeartRateMeasurementModel> heartRateMeasurements = null;

            heartRateMeasurements = new ObservableCollection<HeartRateMeasurementModel>(MyAppViewModel.HeartRateMeasurements);

            IReadOnlyList<HeartRateMeasurementModel> heartRateMeasurementsList = heartRateMeasurements.OrderBy(x => x.Timestamp).ToList();

            return await Task.FromResult(heartRateMeasurementsList);
        }
    }
}
