using BusFinderUniversal.Model;
using BusFinderUniversal.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml;


namespace BusFinderUniversal.ViewModel
{
	public class ListBusViewModel : ViewModelBase
	{
		private BusGroup _buses;
		private ObservableCollection<BusItem> _searchBusResult;
		private BusItem _busItemSelectedFromListview;
		private string _name = "";
		private string _searchBusStr = "";
		private int _progressBarOpacity;
		private double _progressBarValue;
		public Task DataInitialization { get; private set; }
		public RelayCommand FindRouteCommand { get; private set; }
		public RelayCommand<ItemClickEventArgs> BusItemSelectedCommand { get; private set; }

		public BusItem BusItemSelectedFromListviewBuses
		{
			get
			{
				return _busItemSelectedFromListview;
			}
			set
			{
				Set("BusItemSelectedFromListviewBuses", ref _busItemSelectedFromListview, value);
			}
		}
		public ObservableCollection<BusItem> SearchBusResult
		{
			get
			{
				return _searchBusResult;
			}
			set
			{
				Set("SearchBusResult", ref _searchBusResult, value);
			}
		}
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
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				Set("Name", ref _name, value);
			}
		}
		public string SearchBusStr
		{
			get
			{
				return _searchBusStr;
			}
			set
			{
				Set("SearchBusStr", ref _searchBusStr, value);
			}
		}
		public int ProgressBarOpacity
		{
			get
			{
				return _progressBarOpacity;
			}
			set
			{
				Set("ProgressBarOpacity", ref _progressBarOpacity, value);
			}
		}
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


		public ListBusViewModel()
		{
			Messenger.Default.Register<BusGroup>(this, GetProvinceInVMMessage);
		}


		private void GetProvinceInVMMessage(BusGroup msg)
		{
			Name = msg.Name;
			Buses = msg;
			ProgressBarOpacity = 100;
			ProgressBarValue = 0;
			SearchBusResult = new ObservableCollection<BusItem>(Buses.Items);
			ProgressBarOpacity = 0;
			PropertyChanged += ListBusViewModel_PropertyChanged;
			BusItemSelectedCommand = new RelayCommand<ItemClickEventArgs>((selectedItem) => BusItemSelectedHandler(selectedItem));
			FindRouteCommand = new RelayCommand(FindRouteCommandHandler);
		}

		private void BusItemSelectedHandler(ItemClickEventArgs msg)
		{
			//BusItemSelectedFromListviewBuses = msg.ClickedItem as BusItem;
			if (!((Frame)Window.Current.Content).Navigate(typeof(BusItemView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(msg.ClickedItem as BusItem);
		}

		private void ViewMapCommandHandler()
		{
			if (!((Frame)Window.Current.Content).Navigate(typeof(BusItemMapView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}

		private void FindRouteCommandHandler()
		{
			if (!((Frame)Window.Current.Content).Navigate(typeof(FindRouteView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(Buses);
		}

		private async Task DataInitializeAsync()
		{
			if (Name == "Hà Nội")
				await GetHanoiDataAsync();

			SearchBusResult = new ObservableCollection<BusItem>(Buses.Items);
			ProgressBarOpacity = 0;
		}
		
		private void ListBusViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SearchBusStr")
			{
				if (SearchBusStr != "")
				{
					SearchBusResult.Clear();
					string search = MyUtil.ConvertVN(SearchBusStr.ToLower().Replace(" ", ""));

					foreach (BusItem bus in Buses.Items)
					{
						string id = MyUtil.ConvertVN(bus.FleetID.ToLower().Replace(" ", ""));
						string name = MyUtil.ConvertVN(bus.Name.ToLower().Replace(" ", "").Replace("-", ""));
						if (id.Contains(search) || name.Contains(search))
						{
							SearchBusResult.Add(bus);
						}
					}
					foreach (BusItem bus in Buses.Items)
					{
						if (SearchBusResult.Contains(bus)) continue;
						string goRoute = MyUtil.ConvertVN(bus.RouteGo.ToLower().Replace(" ", "").Replace("-", ""));
						string reRoute = MyUtil.ConvertVN(bus.RouteReturn.ToLower().Replace(" ", "").Replace("-", ""));

						if (goRoute.Contains(search) || reRoute.Contains(search))
						{
							SearchBusResult.Add(bus);
						}
					}
				}
				else
				{
					SearchBusResult = new ObservableCollection<BusItem>(Buses.Items);
				}
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
				ProgressBarValue += (double)100/MyConstants.NUMBER_OF_BUS;
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

		public BusItem FindBusItemByCode(string busCode)
		{
			var matches = Buses.Items.Where((bus) => bus.Code.Equals(busCode));
			if (matches.Count() >= 1) return matches.First();
			return null;
		}

		public List<BusStop> FindNearbyBusStop(Geopoint busGeoPoint, double radius)
		{
			if (Buses == null) return null;
			if (busGeoPoint == null) return null;
			List<BusStop> listBusNearby = new List<BusStop>();
			foreach (BusStop bs in this.Buses.Stops)
			{
				if (MyUtil.DistanceInKiloMetres(bs.geo, busGeoPoint) < radius)
				{
					listBusNearby.Add(bs);
				}
			}
			if (listBusNearby.Count == 0) return null;
			return listBusNearby;
		}

		public BusStop FindBusStopByName(string name)
		{
			var matches = Buses.Stops.Where((stop) => stop.Name == name);
			if (matches.Count() >= 1) return matches.First();
			return null;
		}

		public List<BusStop> FindBusStopByNameUncertainly(string name)
		{
			List<BusStop> result = new List<BusStop>();
			string nameNormalized = MyUtil.ConvertVN(name.ToLower().Replace(" ", ""));
			foreach (BusStop bs in Buses.Stops)
			{
				string bsNameNormalized = MyUtil.ConvertVN(bs.Name.ToLower().Replace(" ", ""));
				if (bsNameNormalized.Contains(nameNormalized))
					result.Add(bs);
			}
			if (result.Any())
				return result;
			return null;
		}


		internal BusStop FindBusStopByCode(string p)
		{
			var matches = Buses.Stops.Where((bus) => bus.Code.Equals(p));
			if (matches.Count() >= 1) return matches.First();
			return null;
		}
	}
}
