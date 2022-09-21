using HeartRateNow.Constants;
using HeartRateNow.Helpers;
using HeartRateNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace HeartRateNow.Views
{
    public sealed partial class HeartRatePage : Page
    {
        private readonly AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        private IReadOnlyList<HeartRateMeasurementModel> dataSet;
        public IReadOnlyList<HeartRateMeasurementModel> DataSet { get => dataSet; set => dataSet = value; }

        private RenderingOptions renderingOptions;
        public RenderingOptions RenderingOptions { get => renderingOptions; set => renderingOptions = value; }

        private IList<HeartRateDataPoint> offsetList;
        public IList<HeartRateDataPoint> OffsetList { get => offsetList; set => offsetList = value; }

        private const double chartTextBlockFontSize = 12.0;

        private SolidColorBrush whiteSolidColorBrush = new SolidColorBrush(Colors.White);
        public SolidColorBrush WhiteSolidColorBrush { get => whiteSolidColorBrush; set => whiteSolidColorBrush = value; }

        private SolidColorBrush redSolidColorBrush = new SolidColorBrush(Colors.Red);
        public SolidColorBrush RedSolidColorBrush { get => redSolidColorBrush; set => redSolidColorBrush = value; }

        private SolidColorBrush yellowSolidColorBrush = new SolidColorBrush(Colors.Yellow);
        public SolidColorBrush YellowSolidColorBrush { get => yellowSolidColorBrush; set => yellowSolidColorBrush = value; }

        private SolidColorBrush greenSolidColorBrush = new SolidColorBrush(Colors.Green);
        public SolidColorBrush GreenSolidColorBrush { get => greenSolidColorBrush; set => greenSolidColorBrush = value; }

        private TimeSpan queryTimeSpan;
        public TimeSpan QueryTimeSpan { get => queryTimeSpan; set => queryTimeSpan = value; }

        // Number of data points the chart displays
        private double dataPointCount;
        public double DataPointCount { get => dataPointCount; set => dataPointCount = value; }

        public HeartRatePage()
        {
            InitializeComponent();

            RenderingOptions = new RenderingOptions
            {
                MinRenderingHeartRateValue = 0,
                YellowHeartRateValue = MyAppViewModel.YellowZoneHeartRateValue,
                GreenHeartRateValue = MyAppViewModel.GreenZoneHeartRateValue,
                RedHeartRateValue = MyAppViewModel.RedZoneHeartRateValue,
                MaxRenderingHeartRateValue = 400
            };

            SizeChanged += HeartRatePage_SizeChanged;

            Loaded += HeartRatePage_Loaded;

            MyAppViewModel.CurrentHeartRatePage = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // code here
            // code here
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // code here
            // code here
        }

        private void HeartRatePage_Loaded(object sender, RoutedEventArgs e)
        {

            SetPageContentStackPanelWidth();
        }

        private void HeartRatePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageContentStackPanelWidth();
        }

        private void SetPageContentStackPanelWidth()
        {
            PageContentStackPanel.Width =
                ActualWidth -
                PageContentScrollViewer.Margin.Left -
                PageContentScrollViewer.Padding.Right;
            ChartCanvas.Height =
                Math.Max(MyConstants.DefaultChartCanvasHeight, ActualHeight -
                PageTitleTextBlock.ActualHeight -
                AppBarButtonScrollViewer.ActualHeight -
                HeartRateMeasurementStackPanel.ActualHeight -
                TopHeartRateMeasurementTextBlock.ActualHeight -
                ChartRightGrid.Margin.Top -
                ChartRightGrid.Margin.Bottom -
                timeElapsedTextBlock.ActualHeight -
                PlainDatapointsInMinutesStackPanel.ActualHeight -
                BottomHeartRateMeasurementTextBlock.ActualHeight);
        }

        private void HomeAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHomePage();
        }

        private void DeviceAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToDevicePage();
        }

        private void MuteAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.SpeakAppBarButtonVisibility = Visibility.Visible;
        }

        private void SpeakAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.SpeakAppBarButtonVisibility = Visibility.Collapsed;
        }

        private void ClearDataAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HelpClearData.ClearData();
            PlotChart();
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlotChart();
        }

        public async void PlotChart()
        {
            bool success = await InitializeChart();

            // Render the actual chart in natural Z order
            await CreateRenderingOptions();

            // Preprocess the data for rendering
            success = await FillOffsetList();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                //render in big "chunk" prevents UI flickering
                ChartCanvas.Children.Clear();
                await DrawRedPolyLine();
                await DrawYellowPolyLine();
                await DrawGreenPolyLine();
                await DrawMaxHeartRatePolyLine();
                await DrawMeanPolyLine();
                await DrawMinHeartRatePolyLine();
                await DrawHeartRatePolyLine();
            });
        }

        private Task<bool> InitializeChart()
        {
            QueryTimeSpan = new TimeSpan(0, 0, (int)MyAppViewModel.DatapointsInMinutes, 1, 0);
            DataPointCount = MyAppViewModel.DatapointsInMinutes * 60;
            return Task.FromResult(true);
        }

        private async Task CreateRenderingOptions()
        {
            try
            {
                // First set the data points that we are going to render
                // The functions will use this data to plot the chart
                DataSet = await HelpHeartRateMeasurements.GetMeasurementsOrderByTimestampAsync(QueryTimeSpan);
            }
            catch
            {
            }
            finally
            {
                if (DataSet != null && DataSet.Count > 0)
                {
                    RenderingOptions = new RenderingOptions();
                    RenderingOptions.MinHeartRateValue = DataSet.Min(x => x.HeartRateValue);
                    RenderingOptions.MinRenderingHeartRateValue = Math.Min(DataSet.Min(x => x.HeartRateValue), MyAppViewModel.GreenZoneHeartRateValue);
                    RenderingOptions.MaxRenderingHeartRateValue = Math.Max(DataSet.Max(x => x.HeartRateValue), MyAppViewModel.RedZoneHeartRateValue);
                    RenderingOptions.MaxHeartRateValue = DataSet.Max(x => x.HeartRateValue);

                    RenderingOptions.MeanHeartRateValue = DataSet.Sum(x => (double)x.HeartRateValue) / DataSet.Count;
                    RenderingOptions.RedHeartRateValue = MyAppViewModel.RedZoneHeartRateValue;
                    RenderingOptions.YellowHeartRateValue = MyAppViewModel.YellowZoneHeartRateValue;
                    RenderingOptions.GreenHeartRateValue = MyAppViewModel.GreenZoneHeartRateValue;

                    RenderingOptions.MinTimestamp = DataSet.Min(x => x.Timestamp);
                    RenderingOptions.MaxTimestamp = DataSet.Max(x => x.Timestamp);

                    TimeSpan timeElapsed = RenderingOptions.MaxTimestamp.Subtract(RenderingOptions.MinTimestamp);

                    MyAppViewModel.TopHeartRateMeasurement = (int)RenderingOptions.MaxRenderingHeartRateValue;
                    MyAppViewModel.BottomHeartRateMeasurement = (int)RenderingOptions.MinRenderingHeartRateValue;
                    MyAppViewModel.TimeElapsed = string.Format("{0}:{1}:{2}",
                        timeElapsed.Hours.ToString("00"),
                        timeElapsed.Minutes.ToString("00"),
                        timeElapsed.Seconds.ToString("00"));
                }
            }
        }

        private Task<bool> FillOffsetList()
        {
            OffsetList = new List<HeartRateDataPoint>();

            if ((DataSet != null) && (DataSet.Count > 0))
            {
                double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                // Calculate the number of data points used
                int pointsDisplayed = (DataSet.Count > (int)DataPointCount) ? (int)DataPointCount : DataSet.Count;

                // Add a 5% horizontal buffer to the displayed values, for framing
                double tickOffset = ChartCanvas.ActualWidth / pointsDisplayed;
                double currentOffset = 0.0;

                int offsetId = 0;

                for (int i = DataSet.Count - pointsDisplayed; i < DataSet.Count; i++)
                {
                    offsetId++;

                    double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - DataSet[i].HeartRateValue;

                    OffsetList.Add(new HeartRateDataPoint
                    {
                        Id = offsetId,
                        OffsetX = currentOffset,
                        OffsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight,
                        HeartRateValue = DataSet[i].HeartRateValue
                    });
                    currentOffset += tickOffset;
                }
            }
            return Task.FromResult(true);
        }

        private async Task DrawRedPolyLine()
        {
            if (RenderingOptions != null && RenderingOptions.MaxRenderingHeartRateValue != RenderingOptions.MinRenderingHeartRateValue)
            {
                double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.RedHeartRateValue;

                double offsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight;

                TextBlock textBlock = await CreateTextBlock(string.Format("Red zone @ {0} bpm", RenderingOptions.RedHeartRateValue.ToString("N0")), RedSolidColorBrush);

                textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                double WidthTextBlock = textBlock.DesiredSize.Width;
                double HeightTextBlock = textBlock.DesiredSize.Height;

                ChartCanvas.Children.Add(textBlock);

                Canvas.SetTop(textBlock, offsetY - HeightTextBlock - 4);
                Canvas.SetLeft(textBlock, ChartCanvas.ActualWidth - WidthTextBlock - 4);

                Polyline redPolyline = await CreatePolyline(offsetY, RedSolidColorBrush, 0.5, new PointCollection { new Point(0, offsetY), new Point(ChartCanvas.ActualWidth - 4, offsetY) });

                ChartCanvas.Children.Add(redPolyline);
            }
        }

        private async Task DrawYellowPolyLine()
        {
            if (RenderingOptions != null && RenderingOptions.MaxRenderingHeartRateValue != RenderingOptions.MinRenderingHeartRateValue)
            {
                double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.YellowHeartRateValue;

                double offsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight;

                TextBlock textBlock = await CreateTextBlock(string.Format("Yellow zone @ {0} bpm", RenderingOptions.YellowHeartRateValue.ToString("N0")), YellowSolidColorBrush);

                textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                double WidthTextBlock = textBlock.DesiredSize.Width;
                double HeightTextBlock = textBlock.DesiredSize.Height;

                ChartCanvas.Children.Add(textBlock);

                Canvas.SetTop(textBlock, offsetY - HeightTextBlock - 4);
                Canvas.SetLeft(textBlock, ChartCanvas.ActualWidth - WidthTextBlock - 4);

                Polyline yellowPolyline = await CreatePolyline(offsetY, YellowSolidColorBrush, 0.5, new PointCollection { new Point(0, offsetY), new Point(ChartCanvas.ActualWidth - 4, offsetY) });

                ChartCanvas.Children.Add(yellowPolyline);
            }
        }

        private async Task DrawGreenPolyLine()
        {
            if (RenderingOptions != null && RenderingOptions.MaxRenderingHeartRateValue != RenderingOptions.MinRenderingHeartRateValue)
            {
                double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.GreenHeartRateValue;

                double offsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight;

                TextBlock textBlock = await CreateTextBlock(string.Format("Green zone @ {0} bpm", RenderingOptions.GreenHeartRateValue.ToString("N0")), GreenSolidColorBrush);

                textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                double WidthTextBlock = textBlock.DesiredSize.Width;
                double HeightTextBlock = textBlock.DesiredSize.Height;

                ChartCanvas.Children.Add(textBlock);

                Canvas.SetTop(textBlock, offsetY - HeightTextBlock - 4);
                Canvas.SetLeft(textBlock, ChartCanvas.ActualWidth - WidthTextBlock - 4);

                Polyline greenPolyline = await CreatePolyline(offsetY, GreenSolidColorBrush, 0.5, new PointCollection { new Point(0, offsetY), new Point(ChartCanvas.ActualWidth - 4, offsetY) });

                ChartCanvas.Children.Add(greenPolyline);
            }
        }

        private async Task DrawMaxHeartRatePolyLine()
        {
            if (RenderingOptions != null && RenderingOptions.MaxRenderingHeartRateValue != RenderingOptions.MinRenderingHeartRateValue)
            {
                if (RenderingOptions != null && RenderingOptions.MaxHeartRateValue != RenderingOptions.MinHeartRateValue)
                {
                    double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                    double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MaxHeartRateValue;

                    double offsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight;

                    TextBlock textBlock = await CreateTextBlock(string.Format("maximum value is {0} bpm", RenderingOptions.MaxHeartRateValue.ToString("N0")), WhiteSolidColorBrush);

                    textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                    double WidthTextBlock = textBlock.DesiredSize.Width;
                    double HeightTextBlock = textBlock.DesiredSize.Height;

                    ChartCanvas.Children.Add(textBlock);

                    Canvas.SetTop(textBlock, offsetY - HeightTextBlock);
                    Canvas.SetLeft(textBlock, 0);

                    Polyline maxHeartRatePolyline = await CreatePolyline(offsetY, WhiteSolidColorBrush, 0.5, new PointCollection { new Point(0, offsetY), new Point(ChartCanvas.ActualWidth - 4, offsetY) });

                    ChartCanvas.Children.Add(maxHeartRatePolyline);
                }
            }
        }

        private async Task DrawMeanPolyLine()
        {
            if (RenderingOptions != null && RenderingOptions.MaxRenderingHeartRateValue != RenderingOptions.MinRenderingHeartRateValue)
            {
                double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MeanHeartRateValue;

                double offsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight;

                TextBlock textBlock = await CreateTextBlock(string.Format("mean value is {0} bpm", RenderingOptions.MeanHeartRateValue.ToString("N1")), WhiteSolidColorBrush);

                textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                double WidthTextBlock = textBlock.DesiredSize.Width;
                double HeightTextBlock = textBlock.DesiredSize.Height;

                ChartCanvas.Children.Add(textBlock);

                Canvas.SetTop(textBlock, offsetY - HeightTextBlock);
                Canvas.SetLeft(textBlock, ChartCanvas.ActualWidth / 2 - WidthTextBlock / 2);

                Polyline meanPolyline = await CreatePolyline(offsetY, WhiteSolidColorBrush, 0.3, new PointCollection { new Point(0, offsetY), new Point(ChartCanvas.ActualWidth - 4, offsetY) });

                ChartCanvas.Children.Add(meanPolyline);
            }
        }

        private async Task DrawMinHeartRatePolyLine()
        {
            if (RenderingOptions != null && RenderingOptions.MaxRenderingHeartRateValue != RenderingOptions.MinRenderingHeartRateValue)
            {
                if (RenderingOptions != null && RenderingOptions.MaxHeartRateValue != RenderingOptions.MinHeartRateValue)
                {
                    double valueDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinRenderingHeartRateValue;

                    double currentDiff = RenderingOptions.MaxRenderingHeartRateValue - RenderingOptions.MinHeartRateValue;

                    double offsetY = currentDiff / valueDiff * ChartCanvas.ActualHeight;

                    TextBlock textBlock = await CreateTextBlock(string.Format("minimum value is {0} bpm", RenderingOptions.MinHeartRateValue.ToString("N0")), WhiteSolidColorBrush);

                    textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                    double WidthTextBlock = textBlock.DesiredSize.Width;
                    double HeightTextBlock = textBlock.DesiredSize.Height;

                    ChartCanvas.Children.Add(textBlock);

                    Canvas.SetTop(textBlock, offsetY);
                    Canvas.SetLeft(textBlock, 0);

                    Polyline minHeartRatePolyline = await CreatePolyline(offsetY, WhiteSolidColorBrush, 0.5, new PointCollection { new Point(0, offsetY), new Point(ChartCanvas.ActualWidth - 4, offsetY) });

                    ChartCanvas.Children.Add(minHeartRatePolyline);
                }
            }
        }

        private async Task DrawHeartRatePolyLine()
        {
            if (RenderingOptions != null)
            {
                PointCollection points = await CreatePoints();

                Polyline heartRatePolyline = await CreateHeartRatePolyline(points);

                ChartCanvas.Children.Add(heartRatePolyline);
            }
        }

        private Task<Polyline> CreateHeartRatePolyline(PointCollection points)
        {
            return Task.FromResult(new Polyline()
            {
                Stroke = WhiteSolidColorBrush,
                StrokeThickness = 3,
                Points = points
            });
        }

        private Task<PointCollection> CreatePoints()
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < OffsetList.Count; i++)
            {
                points.Add(new Point(OffsetList[i].OffsetX, OffsetList[i].OffsetY));
            }

            return Task.FromResult(points);
        }

        private Task<TextBlock> CreateTextBlock(string text, Brush brush)
        {
            return Task.FromResult(new TextBlock
            {
                Text = text,
                FontSize = chartTextBlockFontSize,
                Foreground = brush,
                Margin = new Thickness(0, 0, 4, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            });
        }

        private Task<Polyline> CreatePolyline(double offsetY, SolidColorBrush strokeSolidColorBrush, double strokeThickness, PointCollection points)
        {
            return Task.FromResult(new Polyline()
            {
                Stroke = strokeSolidColorBrush,
                StrokeThickness = strokeThickness,
                Points = points
            });
        }

        private async void SaveDataAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //save with FileSavePicker
            FileSavePicker fileSavePicker = new FileSavePicker();

            fileSavePicker.DefaultFileExtension = ".json";

            fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            //our file name
            fileSavePicker.SuggestedFileName = MyAppViewModel.FileNameDateTime;

            fileSavePicker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });

            StorageFile storageFile = await fileSavePicker.PickSaveFileAsync();

            if (storageFile != null)
            {
                HelpFileSavePickerStorage.SaveDataToChosenFolder(storageFile);
                return;
            }

            MyAppViewModel.CurrentMainPage.NotifyUser("You didn't save.", NotifyType.ErrorMessage);
        }
    }
}
