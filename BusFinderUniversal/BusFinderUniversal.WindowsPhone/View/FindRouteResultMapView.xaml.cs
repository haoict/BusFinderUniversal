using BusFinderUniversal.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BusFinderUniversal.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class FindRouteResultMapView : Page
	{
		public FindRouteResultMapView()
		{
			this.InitializeComponent();
			Messenger.Default.Register<FindRouteResultDetailModel>(this, ProcessResut);
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			MyMap.Center = MyConstants.DEFAULT_LOCATION;
			MyMap.ZoomLevel = 13;
		}

		private void ProcessResut(FindRouteResultDetailModel answer)
		{
			MyMap.MapElements.Clear();
			MyMap.Children.Clear();
			List<Geopoint> line = new List<Geopoint>();
			for (int i = 0; i < answer.ResultNodes.Count - 1; i++)
			{
				line.Add(MyUtil.textToGeoList(answer.ResultNodes[i].busStop.geo).First());
				BusNode nxtNode = answer.ResultNodes[i].nextNode;
				if (nxtNode == null)
				{
					return;
				}
				
				if (nxtNode.busStop.Code == answer.ResultNodes.Last().busStop.Code)
				{
					break;
				}
				while (nxtNode.busStop.Code != answer.ResultNodes[i + 1].busStop.Code)
				{
					line.Add(MyUtil.textToGeoList(nxtNode.busStop.geo).First());
					nxtNode = nxtNode.nextNode;
					if (nxtNode == null)
					{
						return;
					}
				}
			}
			line.Add(MyUtil.textToGeoList(answer.ResultNodes.Last().busStop.geo).First());

			//List<BusStop> stations = item.RouteReturnStops;
			Color strokeColor = Color.FromArgb(130, 0, 98, 140);
			var shape = new MapPolyline
			{
				StrokeThickness = 4,
				StrokeColor = strokeColor,
				StrokeDashed = false,
				ZIndex = 1,
				Path = new Geopath(line.Select(p => p.Position))
			};
			MyMap.MapElements.Add(shape);


			// Draw From location label
			if (answer.FromPoint != null)
			{
				Image f1 = new Image();
				f1.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/location.circle.light.png"));
				f1.Height = 20;
				f1.Width = 20;
				f1.Margin = new Thickness(0, 0, 0, 0);
				MyMap.Children.Add(f1);
				MapControl.SetLocation(f1, new Geopoint(answer.FromPoint.Position));
				MapControl.SetNormalizedAnchorPoint(f1, new Point(0.5, 0.5));

				// Draw line from to location to first bus stop
				List<Geopoint> line1 = new List<Geopoint>();
				line1.Add(answer.FromPoint);
				line1.Add(MyUtil.textToGeoList(answer.ResultNodes.First().busStop.geo).First());
				Color sC1 = Color.FromArgb(130, 130, 0, 0);
				var shape1 = new MapPolyline
				{
					StrokeThickness = 4,
					StrokeColor = sC1,
					StrokeDashed = true,
					ZIndex = 1,
					Path = new Geopath(line1.Select(p => p.Position))
				};
				MyMap.MapElements.Add(shape1);
			}
			
			// Draw To location label
			if (answer.ToPoint != null)
			{
				Image t1 = new Image();
				t1.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/location.circle.light.png"));
				t1.Height = 20;
				t1.Width = 20;
				t1.Margin = new Thickness(0, 0, 0, 0);
				MyMap.Children.Add(t1);
				MapControl.SetLocation(t1, new Geopoint(answer.ToPoint.Position));
				MapControl.SetNormalizedAnchorPoint(t1, new Point(0.5, 0.5));

				// Draw line from to location to first bus stop
				List<Geopoint> line2 = new List<Geopoint>();
				line2.Add(MyUtil.textToGeoList(answer.ResultNodes.Last().busStop.geo).First());
				line2.Add(answer.ToPoint);
				Color sC1 = Color.FromArgb(130, 130, 0, 0);
				var shape1 = new MapPolyline
				{
					StrokeThickness = 4,
					StrokeColor = sC1,
					StrokeDashed = true,
					ZIndex = 1,
					Path = new Geopath(line2.Select(p => p.Position))
				};
				MyMap.MapElements.Add(shape1);
			}

			// Draw station
			string busItemsThrough = "Đi tuyến ";
			foreach (BusNode bn in answer.ResultNodes)
			{
				if (!busItemsThrough.Contains(bn.busCode))
				{
					busItemsThrough += bn.busCode;
					busItemsThrough += ", ";

					Image iconStart = new Image();
					iconStart.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/point.png"));
					iconStart.Name = bn.busCode;
					iconStart.Height = 12;
					iconStart.Width = 12;
					MyMap.Children.Add(iconStart);
					MapControl.SetLocation(iconStart, new Geopoint(MyUtil.textToGeoList(bn.busStop.geo).First().Position));
					MapControl.SetNormalizedAnchorPoint(iconStart, new Point(0.5, 0.5));
				}
			}

			// Draw start location label
			Image i1 = new Image();
			i1.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/startlocation.png"));
			i1.Name = answer.ResultNodes.First().busCode;
			i1.Height = 44;
			i1.Width = 96;
			i1.Margin = new Thickness(20, -46, 0, 0);
			MyMap.Children.Add(i1);
			MapControl.SetLocation(i1, new Geopoint(MyUtil.textToGeoList(answer.ResultNodes.First().busStop.geo).First().Position));
			MapControl.SetNormalizedAnchorPoint(i1, new Point(0.5, 0.5));
			
			// Draw stop location label
			Image iconStartLast = new Image();
			iconStartLast.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/stoplocation.png"));
			iconStartLast.Name = answer.ResultNodes.Last().busCode;
			iconStartLast.Height = 44;
			iconStartLast.Width = 96;
			iconStartLast.Margin = new Thickness(20, -46, 0, 0);
			MyMap.Children.Add(iconStartLast);
			MapControl.SetLocation(iconStartLast, new Geopoint(MyUtil.textToGeoList(answer.ResultNodes.Last().busStop.geo).First().Position));
			MapControl.SetNormalizedAnchorPoint(iconStartLast, new Point(0.5, 0.5));

			// Draw stop location point
			Image ic2 = new Image();
			ic2.Source = new BitmapImage(new Uri("ms-appx:///resources/icons/point.png"));
			ic2.Name = answer.ResultNodes.Last().busCode;
			ic2.Height = 12;
			ic2.Width = 12;
			ic2.Margin = new Thickness(0, 0, 0, 0);
			MyMap.Children.Add(ic2);
			MapControl.SetLocation(ic2, new Geopoint(MyUtil.textToGeoList(answer.ResultNodes.Last().busStop.geo).First().Position));
			MapControl.SetNormalizedAnchorPoint(ic2, new Point(0.5, 0.5));

			MessageDialogHelper.Show(answer.Detail);
		}

		
	}
}
