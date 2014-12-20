using BusFinderUniversal.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BusFinderUniversal.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ListBusPage : Page
	{
		public ListBusPage()
		{
			this.InitializeComponent();
			//this.NavigationCacheMode = NavigationCacheMode.Required;  
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			//string Province = e.Parameter as string;
		}

		private void SecondPivot_Loaded(object sender, RoutedEventArgs e)
		{
			ListBusViewModel currentListInstance = ServiceLocator.Current.GetInstance<ListBusViewModel>();

			if (!((Frame)Window.Current.Content).Navigate(typeof(FindRouteView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}

		private void seachrbusid_GotFocus(object sender, RoutedEventArgs e)
		{
			seachrbusid.SelectAll();
		}

		private void deleteButton_Click(object sender, RoutedEventArgs e)
		{
			seachrbusid.Text = "";
		}

		private void seachrbusid_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

		private void SecondPivotLoaded(object sender, RoutedEventArgs e)
		{
			if (!((Frame)Window.Current.Content).Navigate(typeof(FindRouteView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}
	}
}
