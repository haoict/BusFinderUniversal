using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal.Model
{
	public class FindRouteResultModel : ObservableObject
	{
		public List<List<BusNode>> foundAnswerList;

		private string _from;
		private string _to;
		private Geopoint _fromPoint; 
		private Geopoint _toPoint;
		private List<FindRouteResultDetailModel> _resultDetail;
		
		public string From
		{
			get
			{
				return _from;
			}
			set
			{
				Set("From", ref _from, value);
			}
		}
		public string To
		{
			get
			{
				return _to;
			}
			set
			{
				Set("To", ref _to, value);
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
		public List<FindRouteResultDetailModel> ResultDetail
		{
			get
			{
				return _resultDetail;
			}
			set
			{
				Set("ResultDetail", ref _resultDetail, value);
			}
		}

		public FindRouteResultModel()
		{
			From = "";
			To = "";
			_resultDetail = new List<FindRouteResultDetailModel>();

		}
	}
}
