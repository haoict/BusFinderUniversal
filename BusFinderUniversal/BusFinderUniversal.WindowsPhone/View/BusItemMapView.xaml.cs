using BusFinderUniversal.Model;
using BusFinderUniversal.UserControls;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using MappingUtilities;
using BusFinderUniversal.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Animation;

namespace MappingUtilities
{
	/// <summary>
	/// Helper class to attach data to map shapes. Well, to any DependencyObject, but that's to make 
	/// it fit into the PCL
	/// </summary>
	public static class MapElementData
	{
		#region Attached Dependency Property ObjectData
		public static readonly DependencyProperty ObjectDataProperty =
			 DependencyProperty.RegisterAttached("ObjectData",
			 typeof(object),
			 typeof(MapElementData),
			 new PropertyMetadata(default(object), null));

		public static object GetObjectData(DependencyObject obj)
		{
			return obj.GetValue(ObjectDataProperty) as object;
		}

		public static void SetObjectData(
		   DependencyObject obj,
		   object value)
		{
			obj.SetValue(ObjectDataProperty, value);
		}
		#endregion

		public static void AddData(this DependencyObject element, object data)
		{
			SetObjectData(element, data);
		}

		public static T ReadData<T>(this DependencyObject element) where T : class
		{
			return GetObjectData(element) as T;
		}
	}
}

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BusFinderUniversal.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class BusItemMapView : Page
	{
		public BusItemMapView()
		{
			this.InitializeComponent();
			this.NavigationCacheMode = NavigationCacheMode.Required;  
		}

		private BusItem item;
		Windows.UI.ViewManagement.StatusBar statusBar;
		BusItemViewModel currentBusItem = ServiceLocator.Current.GetInstance<BusItemViewModel>();
		Popup busStopViewPopup = new Popup();


		private int _busStopViewOpacity;
		public int BusStopViewOpacity
		{
			get
			{
				return _busStopViewOpacity;
			}
			set
			{
				_busStopViewOpacity = value;
				RaisePropertyChanged("BusStopViewOpacity");
			}
		}
		private string _busStopName;
		public string BusStopName
		{
			get
			{
				return _busStopName;
			}
			set
			{
				_busStopName = value;
				RaisePropertyChanged("BusStopName");
			}
		}
		private string _busStopGoThrough;
		public string BusStopGoThrough
		{
			get
			{
				return _busStopGoThrough;
			}
			set
			{
				_busStopGoThrough = value;
				RaisePropertyChanged("BusStopGoThrough");
			}
		}


		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			item = currentBusItem.SelectedBusItem;
			MyMap.Center = MyConstants.DEFAULT_LOCATION;
			MyMap.ZoomLevel = 13;

			try
			{
				var diemdau = item.RouteGoGeo.First().Position;
				var diemcuoi = item.RouteGoGeo.Last().Position;
				Geopoint myPoint = new Geopoint(new BasicGeoposition { Latitude = (diemdau.Latitude + diemcuoi.Latitude) / 2, Longitude = (diemdau.Longitude + diemcuoi.Longitude) / 2 });
				MyMap.Center = myPoint;
				MyMap.ZoomLevel = 13;
			}
			catch (Exception exc)
			{
				string errMsg = exc.Message;
			}

			if (item.RouteGoGeo == null || !item.RouteGoGeo.Any())
			{
				var dialog = new MessageDialog("Chưa có dữ liệu bản đồ cho tuyến này");
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
				Frame.GoBack();
				return;
			}

			ViewGoRoute(null, null);


			// Set the background color of the status bar, and DON'T FORGET to set the opacity!
			statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
			statusBar.BackgroundColor = (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color;
			statusBar.BackgroundOpacity = 0;
			// Set the text on the ProgressIndicator, and show it.
			statusBar.ProgressIndicator.Text = "Bus " + item.Code + ": " + item.Name;
			await statusBar.ProgressIndicator.ShowAsync();
			// If the progress value is null (which is the default value), the progress indicator is in an indeterminate state (dots moving from left to right).
			// Set it to 0 if you don't wish to show the progress bar.
			statusBar.ProgressIndicator.ProgressValue = 0;
			// Set the desired bounds on the application view to use the core window, i.e., the entire screen (including app bar and status bar
			//var applicationView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
			//applicationView.SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
#if WINDOWS_PHONE_APP
			Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif

		}

		protected async override void OnNavigatedFrom(NavigationEventArgs e)
		{
			if (statusBar != null)
				await statusBar.ProgressIndicator.HideAsync();
		}

		private async void ToggleTracking(object sender, RoutedEventArgs e)
		{
			statusBar.ProgressIndicator.Text = "Đang tìm vị trí của bạn...";
			statusBar.ProgressIndicator.ProgressValue = null;
			// Get current location
			Geopoint myPoint;
			var locator = new Geolocator();
			if (locator.LocationStatus == PositionStatus.Disabled)
			{
				// Location is turned off
				var dialog = new MessageDialog("Location Service is turned off");
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
				statusBar.ProgressIndicator.Text = "Bus " + item.Code + ": " + item.Name;
				statusBar.ProgressIndicator.ProgressValue = 0;
				return;
			}
			locator.DesiredAccuracyInMeters = 500;
			var position = await locator.GetGeopositionAsync();
			myPoint = position.Coordinate.Point;
			statusBar.ProgressIndicator.ProgressValue = 1;
			await MyMap.TrySetViewAsync(myPoint, 16D);

			Ellipse myCircle = new Ellipse();
			myCircle.Fill = new SolidColorBrush(Colors.Blue);
			myCircle.Height = 20;
			myCircle.Width = 20;
			myCircle.Opacity = 50;

			MyMap.Children.Add(myCircle);
			MapControl.SetLocation(myCircle, new Geopoint(myPoint.Position));
			MapControl.SetNormalizedAnchorPoint(myCircle, new Point(0.5, 0.5));

			statusBar.ProgressIndicator.Text = "Bus " + item.Code + ": " + item.Name;
			statusBar.ProgressIndicator.ProgressValue = 0;
		}

		private void ViewGoRoute(object sender, RoutedEventArgs e)
		{
			MyMap.MapElements.Clear();
			MyMap.Children.Clear();
			List<Geopoint> line = item.RouteGoGeo;
			List<BusStop> stations = item.RouteReturnStops;
			Color strokeColor = Color.FromArgb(100, 255, 0, 0);
			//Drawing lines, notice the use of Geopath. Consists out of BasicGeopositions
			var shape = new MapPolyline
			{
				StrokeThickness = 4,
				StrokeColor = strokeColor,
				StrokeDashed = false,
				ZIndex = 1,
				Path = new Geopath(line.Select(p => p.Position))
			};
			MyMap.MapElements.Add(shape);

			DrawGoStations(null, null);
		}

		private void ViewReturnRoute(object sender, RoutedEventArgs e)
		{
			MyMap.MapElements.Clear();
			MyMap.Children.Clear();
			List<Geopoint> line = item.RouteReturnGeo;
			//Color strokeColor = Colors.Blue;
			Color strokeColor = Color.FromArgb(100,0,0,255);
			//Drawing lines, notice the use of Geopath. Consists out of BasicGeopositions
			var shape = new MapPolyline
			{
				StrokeThickness = 4,
				StrokeColor = strokeColor,
				StrokeDashed = false,
				ZIndex = 1,
				Path = new Geopath(line.Select(p => p.Position))
			};

			MyMap.MapElements.Add(shape);

			DrawReturnStations(null, null);
		}

		private void DrawGoStations(object sender, RoutedEventArgs e)
		{
			foreach (var station in item.RouteGoStops)
			{
				Image iconStart = new Image();
				iconStart.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/BusStationGo.png"));
				iconStart.Tapped += new TappedEventHandler(BusStationTapped);
				iconStart.Name = station.Code;
				iconStart.Height = 28;
				iconStart.Width = 28;
				iconStart.Margin = new Thickness(28, 0, 0, 28);
				MyMap.Children.Add(iconStart);
				MapControl.SetLocation(iconStart, new Geopoint(station.geo.Position));
				MapControl.SetNormalizedAnchorPoint(iconStart, new Point(0.5, 0.5));

				var shape = new MapIcon
				{
					Title = station.Name,
					Location = station.geo,
					NormalizedAnchorPoint = new Point(0.5, 0.5),
					Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///resources/icons/empty.png")),
					ZIndex = 5
				};
				shape.AddData(station);
				MyMap.MapElements.Add(shape);
			}
		}

		private void DrawReturnStations(object sender, RoutedEventArgs e)
		{
			foreach (var station in item.RouteReturnStops)
			{
				Image iconStart = new Image();
				iconStart.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/BusStationReturn.png"));
				iconStart.Tapped += new TappedEventHandler(BusStationTapped);
				iconStart.Name = station.Code;
				iconStart.Height = 28;
				iconStart.Width = 28;
				iconStart.Margin = new Thickness(28, 0, 0, 28);
				MyMap.Children.Add(iconStart);
				MapControl.SetLocation(iconStart, new Geopoint(station.geo.Position));
				MapControl.SetNormalizedAnchorPoint(iconStart, new Point(0.5, 0.5));

				var shape = new MapIcon
				{
					Title = station.Name,
					Location = station.geo,
					NormalizedAnchorPoint = new Point(0.5, 0.5),
					Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///resources/icons/empty.png")),
					ZIndex = 5
				};
				shape.AddData(station);
				MyMap.MapElements.Add(shape);
			}
			//List<BusStop> stations = item.RouteReturnStations;
			//var anchorPoint = new Point(0.5, 0.5);
			//var image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/BusStationReturn.png"));
			//foreach (var station in stations)
			//{
			//	var shape = new MapIcon
			//	{
			//		Title = station.Name,
			//		Location = station.geo,
			//		NormalizedAnchorPoint = anchorPoint,
			//		Image = image,
			//		ZIndex = 5
			//	};
			//	shape.AddData(station);
			//	MyMap.MapElements.Add(shape);
			//}
		}

		private void BusStationTapped(object sender, TappedRoutedEventArgs e)
		{
			string bsCode = ((Image)sender).Name;
			BusStop bs = FindBusStopByCode(bsCode);
			if (bs == null) return;

			//BusStopName = "Điểm dừng: " + bs.Name;
			//BusStopGoThrough = "Các tuyến buýt đi qua: " + bs.FleetOver;
			//var resultText = new StringBuilder();
			//resultText.AppendLine("Điểm dừng: " + bs.Name);
			//resultText.AppendLine("Các tuyến buýt đi qua: " + bs.FleetOver);

			//var dialog = new MessageDialog(resultText.ToString());
			//await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());


			busStopViewPopup.Child = new BusStopView(bs);
			busStopViewPopup.IsOpen = true;
			busStopViewPopup.VerticalOffset = 0;
			busStopViewPopup.HorizontalOffset = 0;
		}

		private BusStop FindBusStopByCode(string code)
		{
			var matches = item.RouteGoStops.Where((bus) => bus.Code.Equals(code));
			if (matches.Count() >= 1) return matches.First();
			matches = item.RouteReturnStops.Where((bus) => bus.Code.Equals(code));
			if (matches.Count() >= 1) return matches.First();
			return null;
		}

		private void ZoomOut(object sender, RoutedEventArgs e)
		{
			if (MyMap.ZoomLevel > 1)
				MyMap.ZoomLevel--;
		}

		private void ZoomIn(object sender, RoutedEventArgs e)
		{
			if (MyMap.ZoomLevel < 20)
				MyMap.ZoomLevel++;
		}

		private void MapHeadingChanged(MapControl sender, object args)
		{
			// luôn để là hướng bắc
			//MyMap.Heading = 0;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void MyMap_MapTapped(object sender, TappedRoutedEventArgs e)
		{
			/*
			var resultText = new StringBuilder();
			resultText.AppendLine(string.Format("Position={0},{1}", args.Position.X, args.Position.Y));
			resultText.AppendLine(string.Format("Location={0:F9},{1:F9}", args.Location.Position.Latitude, args.Location.Position.Longitude));

			//foreach (var mapObject in sender.FindMapElementsAtOffset(args.Position))
			//{
			//	resultText.AppendLine("Found: " + mapObject.ReadData<PointList>().Name);
			//}
			var dialog = new MessageDialog(resultText.ToString());
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());*/
			//if (busStopViewPopup.IsOpen == true)
			//	busStopViewPopup.IsOpen = false;
		}

		public void ClearMap()
		{
			MyMap.MapElements.Clear();
			MyMap.Children.Clear();
		}

#if WINDOWS_PHONE_APP
		void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
		{
			busStopViewPopup.IsOpen = false;
		}
#endif
	}
}
