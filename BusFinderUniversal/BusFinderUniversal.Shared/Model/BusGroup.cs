using GalaSoft.MvvmLight;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusFinderUniversal.Model
{
	[Table("BusGroup")]
	public class BusGroup : ObservableObject
	{
		public BusGroup(string name, ObservableCollection<BusItem> bi, ObservableCollection<BusStop> bs)
		{
			this.Items = bi;
			this.Stops = bs;
			this.Name = name;
		}

		public BusGroup()
		{
			this.Items = new ObservableCollection<BusItem>();
			this.Stops = new ObservableCollection<BusStop>();
			this.Name = "";
		}

		public ObservableCollection<BusItem> _items;
		public string _name;
		public ObservableCollection<BusStop> _stops;

		[PrimaryKey]
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
		public ObservableCollection<BusItem> Items { 
			get
			{
				return _items;
			}
			set
			{
				Set("Item", ref _items, value);
			} 
		}
		public ObservableCollection<BusStop> Stops { 
			get
			{
				return _stops;
			}
			set
			{
				Set("Stop", ref _stops, value);
			} 
		}
	}
}
