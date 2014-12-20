using BusFinderUniversal.Model;
using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml;
using BusFinderUniversal.View;

namespace BusFinderUniversal.ViewModel
{
	public class SplashViewModel : ViewModelBase
	{
		private BusGroup _buses;
		public BusGroup Buses
		{
			get
			{
				return _buses;
			}
			set
			{
				Set("Buses", ref _buses, value);
			}
		}
		private double _progressBarValue;
		public double ProgressBarValue
		{
			get
			{
				return _progressBarValue;
			}
			set
			{
				Set("ProgressBarValue", ref _progressBarValue, value);
			}
		}
		public Task DataInitialization { get; private set; }
		public string Name { get; set; }

		public SplashViewModel()
		{
			ProgressBarValue = 0;
			Name = "Hà Nội";
			DataInitialization = DataInitializeAsync();

			

		}

		private async Task DataInitializeAsync()
		{
			if (Name == "Hà Nội")
				await GetHanoiDataAsync();
			else if (Name == "Hồ Chí Minh")
				await GetHochiminhDataAsync();
			else if (Name == "Đà Nẵng")
				await GetDanangDataAsync();
			else if (Name == "Hải Phòng")
				await GetHaiphongDataAsync();

			if (!((Frame)Window.Current.Content).Navigate(typeof(HomeView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}

		private async Task GetHanoiDataAsync()
		{
			if (Buses != null)
				return;

			ObservableCollection<BusItem> obi = new ObservableCollection<BusItem>();
			ObservableCollection<BusStop> obs = new ObservableCollection<BusStop>();

			for (int i = 1; i <= MyConstants.NUMBER_OF_BUS; i++)
			{
				ProgressBarValue += (double)100 / MyConstants.NUMBER_OF_BUS;
				List<Geopoint> RouteGoGeo = new List<Geopoint>();
				List<BusStop> RouteGoStations = new List<BusStop>();
				List<Geopoint> RouteReturnGeo = new List<Geopoint>();
				List<BusStop> RouteReturnStations = new List<BusStop>();
				List<BusNode> goNode = new List<BusNode>();
				List<BusNode> returnNode = new List<BusNode>();

				Uri dataUri = new Uri("ms-appx:///DataModel/Hanoi Buses/" + i.ToString() + ".txt");
				StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
				string jsonText = await FileIO.ReadTextAsync(file);
				JsonObject jsonObject = JsonObject.Parse(jsonText)["dt"].GetObject();

				// Get Go
				JsonObject goObject = jsonObject["Go"].GetObject();
				JsonArray goGeoArray = goObject["Geo"].GetArray();
				foreach (JsonValue value in goGeoArray)
				{
					JsonObject goGeoObject = value.GetObject();

					double lng = goGeoObject["Lng"].GetNumber();
					double lat = goGeoObject["Lat"].GetNumber();
					RouteGoGeo.Add(new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
				}

				JsonArray goStationArray = goObject["Station"].GetArray();
				foreach (JsonValue value in goStationArray)
				{
					JsonObject goStationObject = value.GetObject();

					double lng = goStationObject["Geo"].GetObject()["Lng"].GetNumber();
					double lat = goStationObject["Geo"].GetObject()["Lat"].GetNumber();

					string code = "";
					try
					{
						code = goStationObject["ObjectID"].GetNumber().ToString();
						//code = goStationObject["Code"].GetString();
					}
					catch (Exception exc)
					{
						string errMsg = exc.Message;
					}

					BusStop bs = new BusStop(code,
												goStationObject["Name"].GetString(),
												goStationObject["FleetOver"].GetString(),
												new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
					RouteGoStations.Add(bs);

					BusNode bn = new BusNode(jsonObject["Code"].GetString(), null, bs, null);
					goNode.Add(bn);

					if (!obs.Any(a => a.Code == code))
					{
						bs.arrayNode.Add(bn);
						obs.Add(bs);
					}
					else
					{
						obs.Where(x => x.Code == code).FirstOrDefault().arrayNode.Add(bn);
					}

				}



				// Get Return
				JsonObject reObject = jsonObject["Re"].GetObject();
				JsonArray reGeoArray = reObject["Geo"].GetArray();
				foreach (JsonValue value in reGeoArray)
				{
					JsonObject reGeoObject = value.GetObject();

					double lng = reGeoObject["Lng"].GetNumber();
					double lat = reGeoObject["Lat"].GetNumber();
					RouteReturnGeo.Add(new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
				}

				JsonArray reStationArray = reObject["Station"].GetArray();
				foreach (JsonValue value in reStationArray)
				{
					JsonObject reStationObject = value.GetObject();

					double lng = reStationObject["Geo"].GetObject()["Lng"].GetNumber();
					double lat = reStationObject["Geo"].GetObject()["Lat"].GetNumber();

					string code = "";
					try
					{
						code = reStationObject["ObjectID"].GetNumber().ToString();
					}
					catch (Exception exc)
					{
						string errMsg = exc.Message;
					}

					BusStop bs = new BusStop(code,
												reStationObject["Name"].GetString(),
												reStationObject["FleetOver"].GetString(),
												new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
					RouteReturnStations.Add(bs);

					BusNode bn = new BusNode(jsonObject["Code"].GetString(), null, bs, null);
					returnNode.Add(bn);

					if (!obs.Any(a => a.Code == code))
					{
						bs.arrayNode.Add(bn);
						obs.Add(bs);
					}
					else
					{
						obs.Where(x => x.Code == code).First().arrayNode.Add(bn);
					}
				}

				BusItem bus = new BusItem(jsonObject["FleetID"].GetNumber().ToString(),
											jsonObject["Code"].GetString(),
											jsonObject["Name"].GetString(),
											jsonObject["OperationsTime"].GetString(),
											jsonObject["Frequency"].GetString(),
											jsonObject["Cost"].GetString(),
											goObject["Route"].GetString(),
											RouteGoGeo,
											RouteGoStations,
											reObject["Route"].GetString(),
											RouteReturnGeo,
											RouteReturnStations);
				bus.goNode = goNode;
				bus.returnNode = returnNode;
				obi.Add(bus);
			}

			Buses = new BusGroup(Name, obi, obs);

			foreach (BusItem bi in Buses.Items)
			{
				for (int i = 0; i < bi.goNode.Count; i++)
				{
					bi.goNode[i].busItem = bi;
					if (i < bi.goNode.Count - 1)
					{
						bi.goNode[i].nextNode = bi.goNode[i + 1];
					}

				}
				for (int i = 0; i < bi.returnNode.Count; i++)
				{
					bi.returnNode[i].busItem = bi;
					if (i < bi.returnNode.Count - 1)
					{
						bi.returnNode[i].nextNode = bi.returnNode[i + 1];
					}
				}
			}
		}

		private async Task GetHochiminhDataAsync() { }

		private async Task GetDanangDataAsync() { }

		private async Task GetHaiphongDataAsync() { }

	}
}
