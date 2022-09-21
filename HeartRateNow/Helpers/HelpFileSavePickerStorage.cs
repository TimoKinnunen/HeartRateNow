using HeartRateNow.Constants;
using HeartRateNow.Models;
using HeartRateNow.Views;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Popups;

namespace HeartRateNow.Helpers
{
    public sealed class HelpFileSavePickerStorage
    {
        private static AppProperties myAppViewModel = App.MyApp;
        public static AppProperties MyAppViewModel { get => myAppViewModel; }

        public static async void SaveDataToChosenFolder(StorageFile storageFile)
        {
            if (MyAppViewModel.HeartRateMeasurements.Count > 0)
            {
                ObservableCollection<HeartRateMeasurementModel> measurementsToSave = new ObservableCollection<HeartRateMeasurementModel>(await HelpHeartRateMeasurements.GetMeasurementsOrderByTimestampAsync());
                await Task.Run(() => SaveMeasurementsToChosenFolder(storageFile, measurementsToSave));

                //save a copy and replace older file
                await Task.Run(() => SaveMeasurementsToLocalFolder(measurementsToSave));

                string message = $"Saved {MyAppViewModel.HeartRateMeasurements.Count} heart rate measurements to {storageFile.Path}.";
                MyAppViewModel.CurrentMainPage.NotifyUser(message, NotifyType.StatusMessage);
            }
            else
            {
                MyAppViewModel.CurrentMainPage.NotifyUser("No measurements to save.", NotifyType.ErrorMessage);
            }
        }

        private static async void SaveMeasurementsToLocalFolder(ObservableCollection<HeartRateMeasurementModel> measurementsToSave)
        {
            try
            {
                await SaveObjectToLocalFolder(MyConstants.Measurementsjson, measurementsToSave);
            }
            catch (UnauthorizedAccessException)
            {
                Task t = new Task(() => SaveMeasurementsToLocalFolder(measurementsToSave));
                t.Start();
            }
        }

        private async static Task<bool> SaveObjectToLocalFolder(string filename, Object o)
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(o));
            StorageFolder HeartRateNowSubFolder = await GetLocalSubFolderHeartRateNow();
            if (HeartRateNowSubFolder != null)
            {
                try
                {
                    StorageFile sampleFile = await HeartRateNowSubFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    if (sampleFile != null)
                    {
                        // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(sampleFile);
                        // write to file
                        await FileIO.WriteTextAsync(sampleFile, jsonData);
                        // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                        // Completing updates may require Windows to ask for user input.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(sampleFile);
                        if (status != FileUpdateStatus.Complete)
                        {
                            // message never shown but ok
                            MessageDialog messageDialog = new MessageDialog("File " + sampleFile.Name + " couldn't be saved.");
                            await messageDialog.ShowAsync();
                        }
                    }
                }
                catch
                {
                }
            }
            return true;
        }

        private static async Task<StorageFolder> GetLocalSubFolderHeartRateNow()
        {
            StorageFolder HeartRateNowSubFolder = await LocalFolder.CreateFolderAsync("HeartRateNow", CreationCollisionOption.OpenIfExists);
            return HeartRateNowSubFolder;
        }

        private static StorageFolder LocalFolder
        {
            get { return ApplicationData.Current.LocalFolder; }
        }

        private static async void SaveMeasurementsToChosenFolder(StorageFile storageFile, ObservableCollection<HeartRateMeasurementModel> measurementsToSave)
        {
            try
            {
                await SaveObjectToChosenFolder(storageFile, measurementsToSave);
            }
            catch (UnauthorizedAccessException)
            {
                Task t = new Task(() => SaveMeasurementsToChosenFolder(storageFile, measurementsToSave));
                t.Start();
            }
        }

        private async static Task<bool> SaveObjectToChosenFolder(StorageFile storageFile, Object o)
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(o));
            try
            {
                if (storageFile != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(storageFile);
                    // write to file
                    await FileIO.WriteTextAsync(storageFile, jsonData);
                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(storageFile);
                    if (status != FileUpdateStatus.Complete)
                    {
                        // message never shown but ok
                        MessageDialog messageDialog = new MessageDialog("File " + storageFile.Name + " couldn't be saved.");
                        await messageDialog.ShowAsync();
                    }
                }
            }
            catch
            {
            }
            return true;
        }
    }
}
