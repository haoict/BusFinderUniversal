using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal.Model
{
	public class BusStop : ObservableObject
	{
		public BusStop(string Code, string Name, string FleetOver, Geopoint geo)
		{
			this.Code = Code;
			this.Name = Name;
			this.FleetOver = FleetOver;
			this.geo = geo;
			arrayNode = new List<BusNode>();
		}

		public BusStop()
		{
			this.Code = "";
			this.Name = "";
			this.FleetOver = "";
			this.geo = null;
			arrayNode = new List<BusNode>();
		}

		public string _code;
		public string _name;
		public string _fleetOver;
		public Geopoint _geo;
		public List<BusNode> _arrayNode;

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
		public string FleetOver
		{
			get
			{
				return _fleetOver;
			}
			set
			{
				Set("FleetOver", ref _fleetOver, value);
			}
		}
		public Geopoint geo
		{
			get
			{
				return _geo;
			}
			set
			{
				Set("geo", ref _geo, value);
			}
		}
		public List<BusNode> arrayNode
		{
			get
			{
				return _arrayNode;
			}
			set
			{
				Set("arrayNode", ref _arrayNode, value);
			}
		}
	}
}
