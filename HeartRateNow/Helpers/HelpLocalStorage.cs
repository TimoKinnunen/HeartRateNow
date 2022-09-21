using HeartRateNow.Constants;
using HeartRateNow.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Popups;

namespace HeartRateNow.Helpers
{
    public sealed class HelpLocalStorage
    {
        private static AppProperties myAppViewModel = App.MyApp;
        public static AppProperties MyAppViewModel { get => myAppViewModel; }

        public static ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        public static StorageFolder LocalFolder => ApplicationData.Current.LocalFolder;

        public static void DeleteFilePermanentInLocalFolder(string fileName) => Task.Run(() => DeleteFileAsync(LocalFolder, StorageDeleteOption.PermanentDelete, fileName));

        public static async Task DeleteFileAsync(StorageFolder folder, StorageDeleteOption option, string fileName)
        {
            StorageFile file = await folder.GetFileAsync(fileName);
            if (file != null)
            {
                await file.DeleteAsync(option);
            }
        }

        public static async Task<StorageFile> CopyFileToLocalFolder(StorageFile fileName, string newFileName)
        {
            StorageFile fileCopy = await fileName.CopyAsync(LocalFolder, newFileName, NameCollisionOption.GenerateUniqueName);
            return fileCopy;
        }

        public static async Task<bool> DoesFileExistInLocalFolder(string fileName)
        {
            try
            {
                StorageFile file = await LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async void SaveDataToLocalFolder()
        {
            if (MyAppViewModel.HeartRateMeasurements.Count > 0)
            {
                ObservableCollection<HeartRateMeasurementModel> measurementsToSave = new ObservableCollection<HeartRateMeasurementModel>(await HelpHeartRateMeasurements.GetMeasurementsOrderByTimestampAsync());
                await Task.Run(() => SaveMeasurementsToLocalFolder(measurementsToSave));
            }
        }

        public static void LoadSavedDataFromLocalFolder()
        {
            Task.Run(() => LoadMeasurementsFromLocalFolder());
        }

        private static async void LoadMeasurementsFromLocalFolder()
        {
            try
            {
                StorageFile sampleFile = await GetFileFromLocalFolder(MyConstants.Measurementsjson);
                if (sampleFile != null)
                {
                    await Task.Run(() => HelpClearData.ClearDataItems());
                    ObservableCollection<HeartRateMeasurementModel> heartRateMeasurements = await GetObjectFromLocalFolder<ObservableCollection<HeartRateMeasurementModel>>(sampleFile);
                    foreach (HeartRateMeasurementModel heartRateMeasurement in heartRateMeasurements)
                    {
                        // add measurement to collection
                        MyAppViewModel.HeartRateMeasurements.Add(heartRateMeasurement);
                    }
                }
            }
            catch
            {
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

        public static async Task<StorageFile> GetFileFromLocalFolder(string filename)
        {
            StorageFile sampleFile = null;
            try
            {
                StorageFolder HeartRateNowSubFolder = await GetLocalSubFolderHeartRateNow();
                sampleFile = await HeartRateNowSubFolder.GetFileAsync(filename);
            }
            catch
            {
            }
            return sampleFile;
        }

        private async static Task<T> GetObjectFromLocalFolder<T>(StorageFile sampleFile)
        {
            T o = default(T);
            try
            {
                String jsonData = await FileIO.ReadTextAsync(sampleFile);
                o = await Task.Run(() => JsonConvert.DeserializeObject<T>(jsonData));
            }
            catch
            {
            }
            return o;
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

        public static async Task<string> SerializeHeartRateNowObjectAsync(Object o)
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(o));
            return jsonData;
        }

        public static async Task<bool> DeleteFileInLocalFolder(string fileName)
        {
            try
            {
                StorageFolder HeartRateNowSubFolder = await GetLocalSubFolderHeartRateNow();
                StorageFile sampleFile = await HeartRateNowSubFolder.GetFileAsync(fileName);
                if (sampleFile != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(sampleFile);
                    // delete file
                    await sampleFile.DeleteAsync();
                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(sampleFile);
                    if (status != FileUpdateStatus.Complete)
                    {
                        // message never shown but ok
                        MessageDialog messageDialog = new MessageDialog("File " + sampleFile.Name + " couldn't be deleted.");
                        await messageDialog.ShowAsync();
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        public static string ToFileSize(UInt64 source)
        {
            const int byteConversion = 1024;
            double bytes = Convert.ToDouble(source);

            if (bytes >= Math.Pow(byteConversion, 3)) //GB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
            }
            else if (bytes >= Math.Pow(byteConversion, 2)) //MB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
            }
            else if (bytes >= byteConversion) //KB Range
            {
                return string.Concat(Math.Round(bytes / byteConversion, 0), " kB");
            }
            else //Bytes
            {
                return string.Concat(bytes, " Bytes");
            }
        }

        public static async Task<UInt64> GetLocalFolderUsage()
        {
            UInt64 size = 0;

            try
            {
                IReadOnlyList<StorageFile> localFolderFiles = await LocalFolder.GetFilesAsync();
                foreach (StorageFile child in localFolderFiles)
                {
                    Windows.Storage.FileProperties.BasicProperties prop = await child.GetBasicPropertiesAsync();
                    size += (UInt64)prop.Size;
                }

                StorageFolder HeartRateNowSubFolder = await GetLocalSubFolderHeartRateNow();

                IReadOnlyList<StorageFile> HeartRateNowSubFolderFiles = await HeartRateNowSubFolder.GetFilesAsync();
                foreach (StorageFile child in HeartRateNowSubFolderFiles)
                {
                    Windows.Storage.FileProperties.BasicProperties prop = await child.GetBasicPropertiesAsync();
                    size += (UInt64)prop.Size;
                }
            }
            catch
            {
            }

            return size;
        }

        public static async Task<StorageFolder> GetLocalSubFolderHeartRateNow()
        {
            StorageFolder HeartRateNowSubFolder = await LocalFolder.CreateFolderAsync("HeartRateNow", CreationCollisionOption.OpenIfExists);
            return HeartRateNowSubFolder;
        }
    }
}
