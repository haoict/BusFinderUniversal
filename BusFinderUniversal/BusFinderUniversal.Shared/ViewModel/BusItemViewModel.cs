using BusFinderUniversal.Common;
using BusFinderUniversal.Model;
using BusFinderUniversal.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BusFinderUniversal.ViewModel
{
	public class BusItemViewModel : ViewModelBase
	{
		private BusItem _busItem;
		public BusItem SelectedBusItem
		{
			get
			{
				return _busItem;
			}
			set
			{
				Set("SelectedBusItem", ref _busItem, value);
			}
		}
		public RelayCommand ViewMapCommand { get; private set; }

		public BusItemViewModel()
		{
			Messenger.Default.Register<BusItem>(this, GetBusItemeInVMMessage);
		}

		private void GetBusItemeInVMMessage(BusItem msg)
		{
			SelectedBusItem = msg;
			PropertyChanged += BusItem_PropertyChanged;
			ViewMapCommand = new RelayCommand(ViewMapCommandHandler);
		}

		private void BusItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SelectedBusItem")
			{
			}
		}

		private void ViewMapCommandHandler()
		{
			if (!((Frame)Window.Current.Content).Navigate(typeof(BusItemMapView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}
	}
}
