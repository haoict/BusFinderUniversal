using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BusFinderUniversal.ViewModel
{
	public class ViewModelLocator
	{
		static ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
			SimpleIoc.Default.Register<HomeViewModel>();
			SimpleIoc.Default.Register<ListBusViewModel>();
			SimpleIoc.Default.Register<BusItemViewModel>();
			SimpleIoc.Default.Register<AboutViewModel>();
			SimpleIoc.Default.Register<FindRouteResultViewModel>();
		}

		public static void Cleanup()
		{
			var viewModelLocator = (ViewModelLocator)Application.Current.Resources["Locator"];
			viewModelLocator.HomeViewModel.Cleanup();
			viewModelLocator.ListBusViewModel.Cleanup();
			viewModelLocator.BusItemViewModel.Cleanup();
			viewModelLocator.FindRouteResultViewModel.Cleanup();
			viewModelLocator.AboutViewModel.Cleanup();
			Messenger.Reset();
		}

		public HomeViewModel HomeViewModel
		{
			get
			{
				return ServiceLocator.Current.GetInstance<HomeViewModel>();
			}
		}

		public ListBusViewModel ListBusViewModel
		{
			get
			{
				return ServiceLocator.Current.GetInstance<ListBusViewModel>();
			}
		}

		public BusItemViewModel BusItemViewModel
		{
			get
			{
				return ServiceLocator.Current.GetInstance<BusItemViewModel>();
			}
		}

		public AboutViewModel AboutViewModel
		{
			get
			{
				return ServiceLocator.Current.GetInstance<AboutViewModel>();
			}
		}

		public FindRouteResultViewModel FindRouteResultViewModel
		{
			get
			{
				return ServiceLocator.Current.GetInstance<FindRouteResultViewModel>();
			}
		}

	}
}
