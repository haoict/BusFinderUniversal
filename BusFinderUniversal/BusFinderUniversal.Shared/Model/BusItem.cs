using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal.Model
{
	public class BusItem : ObservableObject
	{
		public BusItem(string FleetID,
			string Code,
			string Name,
			string OperationsTime,
			string Frequency,
			string Cost,
			string RouteGo,
			List<Geopoint> RouteGoGeo,
			List<BusStop> RouteGoStops,
			string RouteReturn,
			List<Geopoint> RouteReturnGeo,
			List<BusStop> RouteReturnStops)
		{
			this.FleetID = FleetID;
			this.Code = Code;
			this.Name = Name;
			this.OperationsTime = OperationsTime;
			this.Frequency = Frequency;
			this.Cost = Cost;
			this.RouteGo = RouteGo;
			this.RouteGoGeo = RouteGoGeo;
			this.RouteGoStops = RouteGoStops;
			this.RouteReturn = RouteReturn;
			this.RouteReturnGeo = RouteReturnGeo;
			this.RouteReturnStops = RouteReturnStops;
		}

		public BusItem()
		{
			this.FleetID = "";
			this.Code = "";
			this.Name = "";
			this.OperationsTime = "";
			this.Frequency = "";
			this.Cost = "";
			this.RouteGo = "";
			this.RouteGoGeo = null;
			this.RouteGoStops = null;
			this.RouteReturn = "";
			this.RouteReturnGeo = null;
			this.RouteReturnStops = null;
		}

		public string _fleetID;
		public string _code;
		public string _name;
		public string _operationsTime;
		public string _frequency;
		public string _cost;
		public string _routeGo;
		public List<Geopoint> _routeGoGeo;
		public List<BusStop> _routeGoStops;
		public string _routeReturn;
		public List<Geopoint> _routeReturnGeo;
		public List<BusStop> _routeReturnStops;
		public List<BusNode> _goNode;
		public List<BusNode> _returnNode;

		public string FleetID
		{
			get
			{
				return _fleetID;
			}
			set
			{
				Set("FleetID", ref _fleetID, value);
			}
		}
		public string Code
		{
			get
			{
				return _code;
			}
			set
			{
				Set("Code", ref _code, value);
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
		public string OperationsTime
		{
			get
			{
				return _operationsTime;
			}
			set
			{
				Set("OperationsTime", ref _operationsTime, value);
			}
		}
		public string Frequency
		{
			get
			{
				return _frequency;
			}
			set
			{
				Set("Frequency", ref _frequency, value);
			}
		}
		public string Cost
		{
			get
			{
				return _cost;
			}
			set
			{
				Set("Cost", ref _cost, value);
			}
		}
		public string RouteGo
		{
			get
			{
				return _routeGo;
			}
			set
			{
				Set("RouteGo", ref _routeGo, value);
			}
		}
		public List<Geopoint> RouteGoGeo
		{
			get
			{
				return _routeGoGeo;
			}
			set
			{
				Set("RouteGoGeo", ref _routeGoGeo, value);
			}
		}
		public List<BusStop> RouteGoStops
		{
			get
			{
				return _routeGoStops;
			}
			set
			{
				Set("RouteGoStops", ref _routeGoStops, value);
			}
		}
		public string RouteReturn
		{
			get
			{
				return _routeReturn;
			}
			set
			{
				Set("RouteReturn", ref _routeReturn, value);
			}
		}
		public List<Geopoint> RouteReturnGeo
		{
			get
			{
				return _routeReturnGeo;
			}
			set
			{
				Set("RouteReturnGeo", ref _routeReturnGeo, value);
			}
		}
		public List<BusStop> RouteReturnStops
		{
			get
			{
				return _routeReturnStops;
			}
			set
			{
				Set("RouteReturnStops", ref _routeReturnStops, value);
			}
		}
		public List<BusNode> goNode
		{
			get
			{
				return _goNode;
			}
			set
			{
				Set("goNode", ref _goNode, value);
			}
		}
		public List<BusNode> returnNode
		{
			get
			{
				return _returnNode;
			}
			set
			{
				Set("returnNode", ref _returnNode, value);
			}
		}


		public override string ToString()
		{
			return "Xe " + this.Code + ": " + this.Name;
		}
	}
}
