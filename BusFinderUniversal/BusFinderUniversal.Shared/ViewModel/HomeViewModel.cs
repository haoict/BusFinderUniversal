using BusFinderUniversal.Common;
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
using GalaSoft.MvvmLight.Messaging;

namespace BusFinderUniversal.ViewModel
{
	public class HomeViewModel : ViewModelBase
	{
		private int _itemSize;
		public int ItemSize
		{
			get
			{
				return _itemSize;
			}
			set
			{
				_itemSize = value;
				RaisePropertyChanged("ItemSize");
			}
		}
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
		private Visibility _splashViewVisibility;
		public Visibility SplashViewVisibility
		{
			get
			{
				return _splashViewVisibility;
			}
			set
			{
				Set("SplashViewVisibility", ref _splashViewVisibility, value);
			}
		}
		private Visibility _layoutVisibility;
		public Visibility LayoutVisibility
		{
			get
			{
				return _layoutVisibility;
			}
			set
			{
				Set("LayoutVisibility", ref _layoutVisibility, value);
			}
		}
		public RelayCommand HanoiSelectedCommand { get; private set; }
		public RelayCommand HCMSelectedCommand { get; private set; }
		public RelayCommand DanangSelectedCommand { get; private set; }
		public RelayCommand HaiphongSelectedCommand { get; private set; }
#if WINDOWS_PHONE_APP
		Windows.UI.ViewManagement.StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
#endif
		public HomeViewModel()
		{
			// In first run. Province = Ha noi
			Name = "Hà Nội";
			ItemSize = (int)Window.Current.Bounds.Width / 2 - 28;
			HanoiSelectedCommand = new RelayCommand(HanoiSelectedCommandHandler);
			HCMSelectedCommand = new RelayCommand(HCMSelectedCommandHandler);
			DanangSelectedCommand = new RelayCommand(DanangSelectedCommandHandler);
			HaiphongSelectedCommand = new RelayCommand(HaiphongSelectedCommandHandler);
			DataInitialization = DataInitializeAsync();
		}

		private void SaveSelectedProvince()
		{
			// Save data
			var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
			if (settings.ContainsKey("Province"))
			{
				settings["Province"] = DataSerialization.Serialize(Name);
			}
			else
			{
				settings.Add("Province", DataSerialization.Serialize(Name));
			}
		}

		private async void ShowLoadingScreen(bool isShow)
		{
			if (isShow)
			{
				LayoutVisibility = Visibility.Collapsed;
				SplashViewVisibility = Visibility.Visible;
				ProgressBarValue = 0;
#if WINDOWS_PHONE_APP
				await statusBar.HideAsync();
#endif
			}
			else
			{
				LayoutVisibility = Visibility.Visible;
				SplashViewVisibility = Visibility.Collapsed;
				ProgressBarValue = 0;
#if WINDOWS_PHONE_APP
				await statusBar.ShowAsync();
#endif
			}
		}

		private async void HanoiSelectedCommandHandler()
		{
			if (Name != "Hà Nội")
			{
				ShowLoadingScreen(true);
				Name = "Hà Nội";
				SaveSelectedProvince();
				await GetHanoiDataAsync();
			}

			if (!((Frame)Window.Current.Content).Navigate(typeof(ListBusPage)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(Buses);
			ShowLoadingScreen(false);
		}
		private async void HCMSelectedCommandHandler()
		{
			if (Name != "TP Hồ Chí Minh")
			{
				ShowLoadingScreen(true);
				Name = "TP Hồ Chí Minh";
				SaveSelectedProvince();
				await GetHCMDataAsync();
			}

			if (!((Frame)Window.Current.Content).Navigate(typeof(ListBusPage)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(Buses);
			ShowLoadingScreen(false);
		}
		private void DanangSelectedCommandHandler()
		{
			if (Name != "Đà Nẵng")
			{
				ShowLoadingScreen(true);
				Name = "Đà Nẵng";
				SaveSelectedProvince();
				//await GetDanangDataAsync();
			}

			if (!((Frame)Window.Current.Content).Navigate(typeof(ListBusPage)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(Buses);
			ShowLoadingScreen(false);
		}
		private void HaiphongSelectedCommandHandler()
		{
			if (Name != "Hải Phòng")
			{
				ShowLoadingScreen(true);
				Name = "Hải Phòng";
				SaveSelectedProvince();
				//await GetHaiphongDataAsync();
			}


			if (!((Frame)Window.Current.Content).Navigate(typeof(ListBusPage)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(Buses);
			ShowLoadingScreen(false);
		}

		private async Task DataInitializeAsync()
		{
			ShowLoadingScreen(true);

			// Get saved province data
			var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
			if (localSettings.Values.Count > 0)
			{
				try
				{
					foreach (var o in Windows.Storage.ApplicationData.Current.LocalSettings.Values)
					{
						if (o.Key == "Province")
						{
							Name = DataSerialization.Deserialize<string>(o.Value.ToString());
						}
					}
				}
				catch (Exception exc)
				{
					string ssss = exc.Message;
				}
			}

			if (Name == "Hà Nội")
				await GetHanoiDataAsync();
			else if (Name == "TP Hồ Chí Minh")
				await GetHCMDataAsync();
			//else if (Name == "Đà Nẵng")
			//	await GetDanangDataAsync();
			//else if (Name == "Hải Phòng")
			//	await GetHaiphongDataAsync();

			ShowLoadingScreen(false);
		}

		private async Task GetHanoiDataAsync()
		{
			if (Buses != null)
				if (Buses.Name == "Hà Nội")
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

				//byte[] cipher = EncryptionHelper.Encrypt(jsonText, MyConstants.ENCRYPT_KEY, MyConstants.ENCRYPT_SALT);
				//string jsonText_ = EncryptionHelper.Decrypt(cipher, MyConstants.ENCRYPT_KEY, MyConstants.ENCRYPT_SALT);

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
						//bn.busStop = obs.Where(x => x.Code == code).FirstOrDefault();
						
						// 20141218
						obs.Where(x => x.Code == code).FirstOrDefault().arrayNode.Add(bn);
						
						
						//foreach (BusStop bb in obs)
						//{
						//	if (bb.Code == code && bs.Code.Contains("565"))
						//	{
						//		bb.arrayNode.Add(bn);
						//	}
						//}
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
						//bn.busStop = obs.Where(x => x.Code == code).FirstOrDefault();
						
						
						// 20141218
						obs.Where(x => x.Code == code).First().arrayNode.Add(bn);
						
						
						//foreach(BusStop bb in obs)
						//{
						//	if (bb.Code == code && bs.Code.Contains("565"))
						//	{
						//		bb.arrayNode.Add(bn);
						//	}
						//}
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

		private async Task GetHCMDataAsync()
		{
			if (Buses != null)
				if (Buses.Name == "TP Hồ Chí Minh")
					return;

			ObservableCollection<BusItem> obi = new ObservableCollection<BusItem>();
			ObservableCollection<BusStop> obs = new ObservableCollection<BusStop>();

			for (int i = 1; i <= MyConstants.NUMBER_OF_BUS_HCM; i++)
			{
				ProgressBarValue += (double)100 / MyConstants.NUMBER_OF_BUS_HCM;
				List<BusStop> RouteGoStations = new List<BusStop>();
				List<BusStop> RouteReturnStations = new List<BusStop>();
				List<Geopoint> RouteGoGeo = new List<Geopoint>();
				List<Geopoint> RouteReturnGeo = new List<Geopoint>();
				List<BusNode> goNode = new List<BusNode>();
				List<BusNode> returnNode = new List<BusNode>();

				Uri dataUri_go = new Uri("ms-appx:///DataModel/HCM Buses/" + i.ToString() + "_listRouteStations_go.txt");
				StorageFile file_go;
				try
				{
					file_go = await StorageFile.GetFileFromApplicationUriAsync(dataUri_go);
				}
				catch (Exception exc)
				{
					continue;
				}
				string jsonText_go = await FileIO.ReadTextAsync(file_go);
				string[] lines = jsonText_go.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

				//
				string[] tmp1 = lines[0].Split(new string[] { "] " }, StringSplitOptions.None);
				string busName = tmp1[1];
				string busID = tmp1[0].Replace("[", "");
				//

				jsonText_go = jsonText_go.Remove(0, lines[0].Length + 1);
				if (jsonText_go == "null")
					continue;
				JsonArray jsonObject_go = JsonObject.Parse(jsonText_go)["TABLE"].GetArray()[0].GetObject()["ROW"].GetArray();

				foreach (JsonValue value in jsonObject_go)
				{
					try
					{	
						JsonObject o = value.GetObject();
						JsonArray a = o["COL"].GetArray();

						string code = a[1].GetObject()["DATA"].GetString();
						string name = a[7].GetObject()["DATA"].GetString();
						try
						{
							double lat = Double.Parse(a[9].GetObject()["DATA"].GetString());
							double lng = Double.Parse(a[8].GetObject()["DATA"].GetString());
							BusStop bs = new BusStop(code, name, "", new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
							RouteGoStations.Add(bs);

							BusNode bn = new BusNode(busID, null, bs, null);
							goNode.Add(bn);

							if (!obs.Any(_tmp => _tmp.Code == code))
							{
								bs.arrayNode.Add(bn);
								bs.FleetOver += busID;
								obs.Add(bs);
							}
							else
							{
								BusStop _tmpBS = obs.Where(x => x.Code == code).First();
								_tmpBS.arrayNode.Add(bn);
								_tmpBS.FleetOver += ", " + busID;
							}
						}
						catch (Exception exc)
						{
							//continue;
						}
					
						string strGeoToNextStationList = a[3].GetObject()["DATA"].GetString();
						string[] geoNode = strGeoToNextStationList.Split(new string[] { " " }, StringSplitOptions.None);
						foreach (string geoPair in geoNode)
						{
							string[] tmppoint = geoPair.Split(new string[] { "," }, StringSplitOptions.None);
							if (tmppoint.Count() == 2)
							{
								double tmplat = Double.Parse(tmppoint[1]);
								double tmplng = Double.Parse(tmppoint[0]);
								RouteGoGeo.Add(new Geopoint(new BasicGeoposition { Latitude = tmplat, Longitude = tmplng }));
							}
						}
					}
					catch (Exception exc)
					{
						//continue;
					}
				}



				//// return
				Uri dataUri_re = new Uri("ms-appx:///DataModel/HCM Buses/" + i.ToString() + "_listRouteStations_re.txt");
				StorageFile file_re = await StorageFile.GetFileFromApplicationUriAsync(dataUri_re);
				string jsonText_re = await FileIO.ReadTextAsync(file_re);
				string[] relines = jsonText_re.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
				jsonText_re = jsonText_re.Remove(0, lines[0].Length + 1);
				JsonArray jsonObject_re = JsonObject.Parse(jsonText_re)["TABLE"].GetArray()[0].GetObject()["ROW"].GetArray();

				foreach (JsonValue value in jsonObject_re)
				{

					JsonObject o = value.GetObject();
					JsonArray a = o["COL"].GetArray();

					string code = a[1].GetObject()["DATA"].GetString();
					string name = a[7].GetObject()["DATA"].GetString();
					try
					{
						double lat = Double.Parse(a[9].GetObject()["DATA"].GetString());
						double lng = Double.Parse(a[8].GetObject()["DATA"].GetString());
						BusStop bs = new BusStop(code, name, "", new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
						RouteReturnStations.Add(bs);

						BusNode bn = new BusNode(busID, null, bs, null);
						returnNode.Add(bn);

						if (!obs.Any(_tmp => _tmp.Code == code))
						{
							bs.arrayNode.Add(bn);
							bs.FleetOver += busID;
							obs.Add(bs);
						}
						else
						{
							BusStop _tmpBS = obs.Where(x => x.Code == code).First();
							_tmpBS.arrayNode.Add(bn);
							_tmpBS.FleetOver += ", " + busID;
						}
					}
					catch (Exception exc)
					{
						//continue;
					}


					string strGeoToNextStationList = a[3].GetObject()["DATA"].GetString();
					string[] geoNode = strGeoToNextStationList.Split(new string[] { " " }, StringSplitOptions.None);
					foreach (string geoPair in geoNode)
					{
						string[] tmppoint = geoPair.Split(new string[] { "," }, StringSplitOptions.None);
						if (tmppoint.Count() == 2)
						{
							double tmplat = Double.Parse(tmppoint[1]);
							double tmplng = Double.Parse(tmppoint[0]);
							RouteReturnGeo.Add(new Geopoint(new BasicGeoposition { Latitude = tmplat, Longitude = tmplng }));
						}
					}
				}

				BusItem bus = new BusItem(busID,
											busID,
											busName,
											"OperationsTime",
											"Frequency",
											"Cost",
											"goRoute",
											RouteGoGeo,
											RouteGoStations,
											"reRoute",
											RouteReturnGeo,
											RouteReturnStations);
				bus.goNode = goNode;
				bus.returnNode = returnNode;
				obi.Add(bus);
			}

			{
				Uri dataUri = new Uri("ms-appx:///DataModel/HCM Buses/busdata.json");
				StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
				string jsonText = await FileIO.ReadTextAsync(file);
				//JsonObject jsonObject = JsonObject.Parse(jsonText)["data"].GetObject();
				JsonArray jsonInfo = JsonObject.Parse(jsonText)["data"].GetArray();
				foreach (JsonValue value in jsonInfo)
				{
					try
					{
						string busName = value.GetObject()["ten_tuyen"].GetArray()[0].GetString().Replace("Tên tuyến : ", "");
						string busID = value.GetObject()["masotuyen"].GetArray()[0].GetString().Replace("Mã số tuyến : ", "");
						string busGoRoute = value.GetObject()["luot_di"].GetArray()[0].GetString().Replace("Lượt đi : ", "");
						string busReRoute = value.GetObject()["luot_ve"].GetArray()[0].GetString().Replace("Lượt về : ", "");

						string freq = value.GetObject()["tan_suat"].GetArray()[0].GetString();
						string opTime = value.GetObject()["thoigian_hoatdong"].GetArray()[0].GetString();

						foreach (BusItem bs in obi)
						{
							if (bs.Code == busID)
							{
								bs.RouteGo = busGoRoute;
								bs.RouteReturn = busReRoute;
								if (freq.Contains("Giãn cách: "))
								{
									bs.Frequency = freq.Replace("Giãn cách: ", "");
								}
								if (opTime.Contains("Thời gian hoạt động: "))
								{
									bs.OperationsTime = opTime.Replace("Thời gian hoạt động: ", "");
								}
							}
						}
					}
					catch (Exception exc)
					{
						continue;
					}
				}
			}

			// 			}
			// 
			Buses = new BusGroup(Name, obi, obs);
			// 
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

		//private async Task GetDanangDataAsync() { }

		//private async Task GetHaiphongDataAsync() { }
	}
}
