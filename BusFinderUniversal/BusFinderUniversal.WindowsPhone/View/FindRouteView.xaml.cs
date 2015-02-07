using BusFinderUniversal.Model;
using BusFinderUniversal.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//TODO: tìm đường còn chưa xử lý hướng của xe bus


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BusFinderUniversal.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class FindRouteView : Page
	{
		private Geopoint currentLocation;
		private Geopoint fromPoint;
		private Geopoint toPoint;
		private Windows.UI.ViewManagement.StatusBar statusBar;
		private ListBusViewModel currentListInstance = ServiceLocator.Current.GetInstance<ListBusViewModel>();
		private ObservableCollection<string> SearchSuggestion { get; set; }
		private bool isProcessing;

		public FindRouteView()
		{
			this.InitializeComponent();
			this.NavigationCacheMode = NavigationCacheMode.Required;
		}

		private async Task CurrentLocationInitializationAsync()
		{
			if (ServiceLocator.Current.GetInstance<HomeViewModel>().Buses.Name == "TP Hồ Chí Minh")
				MyMap.Center = MyConstants.DEFAULT_LOCATION_HCM;
			else
				MyMap.Center = MyConstants.DEFAULT_LOCATION;
			MyMap.ZoomLevel = 13;
			// TODO: Move focus to input To text
			var locator = new Geolocator();
			if (MyConstants.USE_LOCATION && locator.LocationStatus != PositionStatus.Disabled)
			{
				statusBar.ProgressIndicator.Text = MyUtil.GetStringResource("strGettingLocation"); ;
				statusBar.ProgressIndicator.ProgressValue = null;

				inputFROM.Text = "<vị trí hiện tại>";
				locator.DesiredAccuracyInMeters = 500;
				//locator.DesiredAccuracy = Windows.Devices.Geolocation.PositionAccuracy.Default;
				var position = await locator.GetGeopositionAsync();
				currentLocation = position.Coordinate.Point;
				MyMap.Center = currentLocation;
				if (inputFROM.Text == "<vị trí hiện tại>")
					fromPoint = currentLocation;
				//DrawImageToMap("/resources/icons/appbar.location.round.light.png", new Geopoint(MyMap.Center.Position));
				statusBar.ProgressIndicator.Text = MyUtil.GetStringResource("strFindRouteViewReady");
				statusBar.ProgressIndicator.ProgressValue = 0;
			}
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			

			SearchSuggestion = new ObservableCollection<string>();
			this.DataContext = SearchSuggestion;
			// Set the background color of the status bar, and DON'T FORGET to set the opacity!
			statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
			statusBar.BackgroundColor = (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color;
			statusBar.BackgroundOpacity = 0;
			statusBar.ProgressIndicator.Text = MyUtil.GetStringResource("strFindRouteViewReady");
			statusBar.ProgressIndicator.ProgressValue = 0;
			await statusBar.ProgressIndicator.ShowAsync();
			isProcessing = false;
#if _DEBUG
			inputFROM.Text = "So 3 Hang Muoi";
			inputTO.Text = "23 Hang Tre";
			return;
#endif

			

			if (e.NavigationMode == NavigationMode.New)
				await CurrentLocationInitializationAsync();


// 			List<List<BusNode>> foundAnswerList = new List<List<BusNode>>();
// 			BusStop f = currentListInstance.FindBusStopByCode("9067");
// 			BusStop t = currentListInstance.FindBusStopByCode("9035");
// 			AStarAlgo AS1 = new AStarAlgo(t, f);
// 			AS1.algorithm();
// 			if (AS1.answer.Count() != 0)
// 			{
// 				// We found an answer
// 				foundAnswerList.Add(AS1.answer);
// 			}
// 			if (foundAnswerList.Count == 0)
// 			{
// 				MessageDialogHelper.Show("Không tìm thấy kết quả nào");
// 				isProcessing = false;
// 				fromPoint = null;
// 				toPoint = null;
// 				return;
// 			}
// 
// 
// 			FindRouteResultModel result = new FindRouteResultModel();
// 			result.From = inputFROM.Text;
// 			result.To = inputTO.Text;
// 			result.FromPoint = fromPoint;
// 			result.ToPoint = toPoint;
// 			result.foundAnswerList = foundAnswerList;
// 			if (!((Frame)Window.Current.Content).Navigate(typeof(FindRouteResultView)))
// 			{
// 				throw new Exception("NavigationFailedExceptionMessage");
// 			}
// 			Messenger.Default.Send(result);
// 
// 			fromPoint = null;
// 			toPoint = null;
// 			isProcessing = false;
		}

		protected async override void OnNavigatedFrom(NavigationEventArgs e)
		{
			await statusBar.ProgressIndicator.HideAsync();
		}

		private void GetCurrentLocationFROM(object sender, RoutedEventArgs e)
		{
			centerMapLocation.Visibility = Visibility.Visible;
			CommandBar myBar = new CommandBar();
			myBar.IsOpen = true;
			AppBarButton AcceptLocationBtnFROM = new AppBarButton() { Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///resources/icons/appbar.check.dark.png") } };
			AcceptLocationBtnFROM.Label = "đồng ý";
			AcceptLocationBtnFROM.Click += AcceptLocationBtnFROM_Click;
			AcceptLocationBtnFROM.IsEnabled = true;
			myBar.PrimaryCommands.Add(AcceptLocationBtnFROM);
			AppBarButton CancelLocationBtn = new AppBarButton() { Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///resources/icons/cancel.dark.png") } };
			CancelLocationBtn.Label = "hủy";
			CancelLocationBtn.Click += CancelLocationSelect_Click;
			CancelLocationBtn.IsEnabled = true;
			myBar.PrimaryCommands.Add(CancelLocationBtn);
			BottomAppBar = myBar;
		}

		private void GetCurrentLocationTO(object sender, RoutedEventArgs e)
		{
			centerMapLocation.Visibility = Visibility.Visible;
			CommandBar myBar = new CommandBar();
			myBar.IsOpen = true;
			AppBarButton AcceptLocationBtnTO = new AppBarButton() { Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///resources/icons/appbar.check.dark.png") } };
			AcceptLocationBtnTO.Label = "Accept";
			AcceptLocationBtnTO.Click += AcceptLocationBtnTO_Click;
			AcceptLocationBtnTO.IsEnabled = true;
			myBar.PrimaryCommands.Add(AcceptLocationBtnTO);
			AppBarButton CancelLocationBtn = new AppBarButton() { Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///resources/icons/cancel.dark.png") } };
			CancelLocationBtn.Label = "hủy";
			CancelLocationBtn.Click += CancelLocationSelect_Click;
			CancelLocationBtn.IsEnabled = true;
			myBar.PrimaryCommands.Add(CancelLocationBtn);
			BottomAppBar = myBar;
		}

		private void CancelLocationSelect_Click(object sender, RoutedEventArgs e)
		{
			BottomAppBar = null;
			centerMapLocation.Visibility = Visibility.Collapsed;
		}

		private async void AcceptLocationBtnTO_Click(object sender, RoutedEventArgs e)
		{
			BottomAppBar = null;
			centerMapLocation.Visibility = Visibility.Collapsed;
			inputTO.Text = "(" + MyMap.Center.Position.Longitude.ToString() + ", " + MyMap.Center.Position.Latitude.ToString() + ")";
			toPoint = new Geopoint(MyMap.Center.Position);

			MyMap.Children.Clear();
			if (fromPoint != null)
				DrawImageToMap("/resources/icons/appbar.location.round.light.png", fromPoint);
			DrawImageToMap("/resources/icons/appbar.location.round.light.png", toPoint);

// 			if (inputFROM.Text != "" && inputTO.Text != "")
// 				FindRoute();

			string placeName = await MyUtil.GetPlaceNameByLocation(toPoint);
			if (placeName != "")
				inputTO.Text = placeName;
		}

		private async void AcceptLocationBtnFROM_Click(object sender, RoutedEventArgs e)
		{
			BottomAppBar = null;
			centerMapLocation.Visibility = Visibility.Collapsed;
			inputFROM.Text = "(" + MyMap.Center.Position.Longitude.ToString() + ", " + MyMap.Center.Position.Latitude.ToString() + ")";
			fromPoint = new Geopoint(MyMap.Center.Position);

			MyMap.Children.Clear();
			DrawImageToMap("/resources/icons/appbar.location.round.light.png", fromPoint);
			if (toPoint != null)
				DrawImageToMap("/resources/icons/appbar.location.round.light.png", toPoint);

			string placeName = await MyUtil.GetPlaceNameByLocation(fromPoint);
			if (placeName != "")
				inputFROM.Text = placeName;
		}

		private void DrawImageToMap(string imagePath, Geopoint location)
		{
			Image iconStart = new Image();
			BitmapImage bi = new BitmapImage(new Uri("ms-appx://" + imagePath));
			iconStart.Source = bi;
			MyMap.Children.Add(iconStart);
			MapControl.SetLocation(iconStart, location);
			MapControl.SetNormalizedAnchorPoint(iconStart, new Point(0.5, 0.5));
		}

		private void ChangeInputDirection(object sender, RoutedEventArgs e)
		{
// 			string tmp = inputFROM.Text;
// 			inputFROM.Text = inputTO.Text;
// 			inputTO.Text = tmp;
			SearchSuggestion.Clear();
			if (inputFROM.Text != "" && inputTO.Text != "")
				FindRoute();
		}

		private void MyMap_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
		{
			this.Focus(FocusState.Pointer);
		}

		private void FROMBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key.Equals(Windows.System.VirtualKey.Enter))
			{
				if (inputTO.Text != "")
				{
					this.Focus(FocusState.Pointer);
					return;
				}
				FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
			}
		}

		private void TOBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key.Equals(Windows.System.VirtualKey.Enter))
			{
				if (inputFROM.Text == "" || inputTO.Text == "")
				{
					return;
				}
				FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
			}
		}

		private async Task<Geopoint> GetCurrentLocation()
		{
			var locator = new Geolocator();
			if (locator.LocationStatus == PositionStatus.Disabled)
			{
				MessageDialogHelper.Show("Location Service is turned off", "");
				return null;
			}
			locator.DesiredAccuracyInMeters = (uint)(MyConstants.RADIUS * 1000);
			var position = await locator.GetGeopositionAsync();
			return position.Coordinate.Point;
		}

		private async Task<Geopoint> GetPositionByPlaceName(string name)
		{
			try
			{
				Geopoint queryHintPoint = currentLocation;
				var result = await MapLocationFinder.FindLocationsAsync(name, queryHintPoint);
				if (result.Status == MapLocationFinderStatus.Success)
				{
					BasicGeoposition location = new BasicGeoposition();
					location.Latitude = result.Locations[0].Point.Position.Latitude;
					location.Longitude = result.Locations[0].Point.Position.Longitude;
					return new Geopoint(location);
				}
			}
			catch (Exception exc)
			{
				string message = exc.Message;
			}
			return null;
		}

		private void FROMBox_LostFocus(object sender, RoutedEventArgs e)
		{
			SearchSuggestion.Clear();
		}

		private void FROMBox_GotFocus(object sender, RoutedEventArgs e)
		{
			//inputFROM.SelectAll();
			//((TextBox)inputFROM).SelectAll();
		}

		private void TOBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//if (SearchSuggestion.Count != 0)
			//{
// 				SearchSuggestion.Clear();
// 				if (inputFROM.Text != "" && inputTO.Text != "")
// 					FindRoute();
			//}
			
			
		}

		private void TOBox_GotFocus(object sender, RoutedEventArgs e)
		{
			//inputTO.SelectAll();
		}

		private async void FindRoute()
		{
			if (isProcessing)
			{
				return;
			}
			isProcessing = true;

			statusBar.ProgressIndicator.Text = MyUtil.GetStringResource("strFindingRoute"); ;
			statusBar.ProgressIndicator.ProgressValue = null;

			// Prepare list of BusStop as result from user input
			List<BusStop> fromBusStopList;
			List<BusStop> toBusStopList;
			List<List<BusNode>> foundAnswerList = new List<List<BusNode>>();

			try
			{
				if (fromPoint != null)
				{
					fromBusStopList = currentListInstance.FindNearbyBusStop(fromPoint, MyConstants.RADIUS);
				}
				else if (inputFROM.Text == "<vị trí hiện tại>")
				{
					if (currentLocation == null)
						currentLocation = await GetCurrentLocation();
					fromPoint = currentLocation;
					fromBusStopList = currentListInstance.FindNearbyBusStop(fromPoint, MyConstants.RADIUS);
				}
				else
				{
					fromBusStopList = currentListInstance.FindBusStopByNameUncertainly(inputFROM.Text);
					fromPoint = await GetPositionByPlaceName(inputFROM.Text);
				}
				if (fromBusStopList == null && inputFROM.Text != "<vị trí hiện tại>")
				{
					Geopoint tmpPoint = await GetPositionByPlaceName(inputFROM.Text);
					fromBusStopList = currentListInstance.FindNearbyBusStop(tmpPoint, MyConstants.RADIUS);
					fromPoint = tmpPoint;
				}
				if (fromBusStopList == null)
				{
					fromPoint = null;
					toPoint = null;
					isProcessing = false;
					MessageDialogHelper.Show("Không tìm thấy xe bus nào gần điểm bắt đầu");
					return;
				}

				if (toPoint != null)
				{
					toBusStopList = currentListInstance.FindNearbyBusStop(toPoint, MyConstants.RADIUS);
				}
				else if (inputTO.Text == "<vị trí hiện tại>")
				{
					toPoint = currentLocation;
					toBusStopList = currentListInstance.FindNearbyBusStop(toPoint, MyConstants.RADIUS);
				}
				else
				{
					toBusStopList = currentListInstance.FindBusStopByNameUncertainly(inputTO.Text);
					toPoint = await GetPositionByPlaceName(inputTO.Text);
				}
				if (toBusStopList == null && inputTO.Text == "<vị trí hiện tại>")
				{
					Geopoint tmpPoint = await GetPositionByPlaceName(inputTO.Text);
					toBusStopList = currentListInstance.FindNearbyBusStop(tmpPoint, MyConstants.RADIUS);
					toPoint = tmpPoint;
				}
				if (toBusStopList == null)
				{
					fromPoint = null;
					toPoint = null;
					isProcessing = false;
					MessageDialogHelper.Show("Không tìm thấy xe bus nào gần điểm kết thúc");
					return;
				}

				// Let's do it
				foreach (BusStop f in fromBusStopList)
				{
					foreach (BusStop t in toBusStopList)
					{
						if (f.Code == t.Code) continue;
						// Begin Algorithm
						AStarAlgo AS1 = new AStarAlgo(f, t);
						AS1.algorithm();
						if (AS1.answer.Count() != 0)
						{
							// We found an answer
							foundAnswerList.Add(AS1.answer);
						}
					}
				}
			}
			catch (Exception exc)
			{
				string message = exc.Message;
			}

			statusBar.ProgressIndicator.Text = MyUtil.GetStringResource("strFindRouteViewReady"); ;
			statusBar.ProgressIndicator.ProgressValue = 0;

			if (foundAnswerList.Count == 0)
			{
				MessageDialogHelper.Show("Không tìm thấy kết quả nào");
				isProcessing = false;
				fromPoint = null;
				toPoint = null;
				return;
			}


			FindRouteResultModel result = new FindRouteResultModel();
			result.From = inputFROM.Text;
			result.To = inputTO.Text;
			result.FromPoint = fromPoint;
			result.ToPoint = toPoint;
			result.foundAnswerList = foundAnswerList;
			if (!((Frame)Window.Current.Content).Navigate(typeof(FindRouteResultView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(result);

			fromPoint = null;
			toPoint = null;
			inputFROM.Text = "";
			inputTO.Text = "";
			isProcessing = false;
			//string detail = "";
			//foreach (List<BusNode> ans in foundAnswerList)
			//{
			//	// print to map
			//	/*
			//	for (int i = 0; i < foundAnswer.Count(); i++)
			//	{
			//		findShortestRoute(SampleDataSource.FindBusStopByCode(foundAnswer[i].stopCode).geo,
			//			SampleDataSource.FindBusStopByCode(foundAnswer[i + 1].stopCode).geo);
			//	}
			//	*/
			//	// print to string

			//	detail += AStarResultToString(ans);
			//	detail += "*********************\r\n";

			//}
			//MessageDialogHelper.Show(detail);

			

		}
		//
		private List<ResultObject> convertToResultObject(List<BusNode> arrNodes)
		{
			List<ResultObject> result = new List<ResultObject>();
			BusNode startNode;
			startNode = arrNodes[0];
			int foundSize = arrNodes.Count();
			for (int k = 1; k < foundSize; k++)
			{
				BusNode current = arrNodes[k];
				if (current.busCode == startNode.busCode && k < foundSize - 1)
				{
					continue;
				}
				else if (current.busCode == startNode.busCode && k == foundSize - 1)
				{
					ResultObject ro = new ResultObject(startNode.busStop, arrNodes[k].busStop, startNode.busCode);
					result.Add(ro);
				}

				else
				{
					ResultObject ro = new ResultObject(startNode.busStop, arrNodes[k].busStop, startNode.busCode);
					result.Add(ro);
					startNode = current;
				}
			}
			return result;
		}
		private string AStarResultToString(List<BusNode> foundAnswer)
		{
			if (foundAnswer == null)
			{
				return "";
			}

			List<ResultObject> searchResultElement = new List<ResultObject>();
			searchResultElement = convertToResultObject(foundAnswer);
			StringBuilder detail = new StringBuilder();
			String de = "";
			for (int index = 0; index < searchResultElement.Count(); index++)
			{
				ResultObject ro = searchResultElement[index];
				if (index == 0)
				{
					detail.Append("- Di chuyển đến " + ro.getOrigin().Name + ", bắt xe Bus " + ro.getLine()
						+ "(" + currentListInstance.FindBusItemByCode(ro.getLine()).Name + ") tới "
						+ ro.getDestination().Name + "\n");
				}
				else
				{
					detail.Append("- Từ " + ro.getOrigin().Name + ", bắt xe Bus " + ro.getLine()
						+ "(" + currentListInstance.FindBusItemByCode(ro.getLine()).Name + ") tới "
						+ ro.getDestination().Name + "\n");
				}
				de = detail.ToString();
			}

			ResultSearchObject rso = new ResultSearchObject(searchResultElement[0].getOrigin() + " - "
					+ searchResultElement[searchResultElement.Count() - 1].getDestination(), detail.ToString());

			searchResultElement.Clear();
			return de;
		}

		//AutoSuggestBox
		private void suggestions_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (currentListInstance.Buses == null) return;

				if (sender.Name == "inputFROM" && sender.Text == "")
				{
					fromPoint = null;
				}
				if (sender.Name == "inputTO" && sender.Text == "")
				{
					toPoint = null;
				}

				SearchSuggestion.Clear();

				if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
				{
					if (sender.Text == "" || sender.Text == "<vị trí hiện tại>") return;

					foreach (BusStop bs in currentListInstance.Buses.Stops)
					{
						string t1 = MyUtil.VNTextNormalize(sender.Text);
						string t2 = MyUtil.VNTextNormalize(bs.Name);
						if (t2.Contains(t1))
						{
							SearchSuggestion.Add(bs.Name);
						}
					}
				}

			
		}

		private void suggestions_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{

			if (sender.Name == "inputFROM")
			{
				inputFROM.Text = args.SelectedItem as string;
			}
			else
			{
				inputTO.Text = args.SelectedItem as string;
			}

			

			//SearchSuggestion.Clear();
			//FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
		}

		private async void ToggleTracking(object sender, RoutedEventArgs e)
		{
			var locator = new Geolocator();
			if (locator.LocationStatus == PositionStatus.Disabled)
			{
				MessageDialogHelper.Show("Dịch vụ tìm vị trí bị tắt");
				return;
			}

			if (currentLocation == null)
			{
				currentLocation = await GetCurrentLocation();
			}

			await MyMap.TrySetViewAsync(currentLocation, 16D);

			Ellipse myCircle = new Ellipse();
			myCircle.Fill = new SolidColorBrush(Colors.Blue);
			myCircle.Height = 20;
			myCircle.Width = 20;
			myCircle.Opacity = 50;

			MyMap.Children.Add(myCircle);
			MapControl.SetLocation(myCircle, new Geopoint(currentLocation.Position));
			MapControl.SetNormalizedAnchorPoint(myCircle, new Point(0.5, 0.5));
		}
	}
}

// TODO: di chuyeenr trong thoi gian bao lau + hien ra map