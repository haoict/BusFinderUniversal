using GalaSoft.MvvmLight;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal.Model
{
	[Table("BusItem")]
	public class BusItem : ObservableObject
	{
		public BusItem(string FleetID,
			string Code,
			string Name,
			string OperationsTime,
			string Frequency,
			string Cost,
			string RouteGo,
			string RouteGoGeo,
			string RouteGoStops,
			string RouteReturn,
			string RouteReturnGeo,
			string RouteReturnStops)
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

		private string _fleetID;
		private string _code;
		private string _name;
		private string _operationsTime;
		private string _frequency;
		private string _cost;
		private string _routeGo;
		private string _routeGoGeo;
		private string _routeGoStops;
		private string _routeReturn;
		private string _routeReturnGeo;
		private string _routeReturnStops;
		private List<BusNode> _goNode;
		private List<BusNode> _returnNode;

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
		[PrimaryKey]
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
		public string RouteGoGeo
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
		public string RouteGoStops
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
		public string RouteReturnGeo
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
		public string RouteReturnStops
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
