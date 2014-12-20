using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal.Model
{
	public class FindRouteResultDetailModel : ObservableObject
	{
		private int _id;
		private string _resultName;
		private string _resultDescription;
		private string _resultTimeConsume;
		private string _detail;
		private List<BusNode> _resultNodes;
		private Geopoint _fromPoint;
		private Geopoint _toPoint;

		public int ResultID
		{
			get
			{
				return _id;
			}
			set
			{
				Set("ResultID", ref _id, value);
			}
		}
		public string ResultName
		{
			get
			{
				return _resultName;
			}
			set
			{
				Set("ResultName", ref _resultName, value);
			}
		}
		public string ResultDescription
		{
			get
			{
				return _resultDescription;
			}
			set
			{
				Set("ResultDescription", ref _resultDescription, value);
			}
		}
		public string ResultTimeConsume
		{
			get
			{
				return _resultTimeConsume;
			}
			set
			{
				Set("ResultTimeConsume", ref _resultTimeConsume, value);
			}
		}
		public string Detail
		{
			get
			{
				return _detail;
			}
			set
			{
				Set("Detail", ref _detail, value);
			}
		}
		public List<BusNode> ResultNodes
		{
			get
			{
				return _resultNodes;
			}
			set
			{
				Set("ResultNodes", ref _resultNodes, value);
			}
		}
		public Geopoint FromPoint
		{
			get
			{
				return _fromPoint;
			}
			set
			{
				Set("FromPoint", ref _fromPoint, value);
			}
		}
		public Geopoint ToPoint
		{
			get
			{
				return _toPoint;
			}
			set
			{
				Set("ToPoint", ref _toPoint, value);
			}
		}
	}
}
