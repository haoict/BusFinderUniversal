using BusFinderUniversal.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BusFinderUniversal.UserControls
{
	public sealed partial class ListBusView : UserControl
	{
		public ListBusView()
		{
			this.InitializeComponent();
		}

		private void listBusView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		//private void BusItemClicked(object sender, ItemClickEventArgs e)
		//{
		//	if (!(((Frame)Window.Current.Content).Navigate(typeof(HomeView))))
		//	{
		//		throw new Exception("NavigationFailedExceptionMessage");
		//	}			
		//}
	}
}
