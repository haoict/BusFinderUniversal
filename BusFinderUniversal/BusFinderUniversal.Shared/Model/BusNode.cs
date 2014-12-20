using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusFinderUniversal.Model
{
	public class BusNode : ObservableObject
	{
		public BusNode()
		{
			busCode = "";
			busItem = null;
			busStop = null;
			nextNode = null;
		}

		public BusNode(string busCode, BusItem busItem, BusStop busStop, BusNode nextNode)
		{
			this.busCode = busCode;
			this.busItem = busItem;
			this.busStop = busStop;
			this.nextNode = nextNode;
		}

		private string _busCode;
		private BusItem _busItem;
		private BusStop _busStop;
		private BusNode _nextNode;

		public string busCode
		{
			get
			{
				return _busCode;
			}
			set
			{
				Set("busCode", ref _busCode, value);
			}
		}
		public BusItem busItem
		{
			get
			{
				return _busItem;
			}
			set
			{
				Set("busItem", ref _busItem, value);
			}
		}
		public BusStop busStop
		{
			get
			{
				return _busStop;
			}
			set
			{
				Set("busStop", ref _busStop, value);
			}
		}
		public BusNode nextNode
		{
			get
			{
				return _nextNode;
			}
			set
			{
				Set("nextNode", ref _nextNode, value);
			}
		}
	}
}
