using GalaSoft.MvvmLight.Messaging;
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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BusFinderUniversal.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class HomeView : Page
	{
		public HomeView()
		{
			this.InitializeComponent();
			this.NavigationCacheMode = NavigationCacheMode.Required;   
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			
		}

		private async void SuggestApp_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:navigate?appid=dae3cc18-d9fd-459c-aa41-9a60e35caf77"));
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			if (!Frame.Navigate(typeof(AboutView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			//await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=dae3cc18-d9fd-459c-aa41-9a60e35caf77"));
		}

		private void Setting_Click(object sender, RoutedEventArgs e)
		{
			if (!Frame.Navigate(typeof(SettingPage)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
		}
	}
}
