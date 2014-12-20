using BusFinderUniversal.Model;
using BusFinderUniversal.View;
using BusFinderUniversal.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BusFinderUniversal.UserControls
{
	public sealed partial class BusStopView : UserControl
	{
		private BusStop busStop;
		ListBusViewModel currentListInstance = ServiceLocator.Current.GetInstance<ListBusViewModel>();
		BusItemViewModel currentBusItem = ServiceLocator.Current.GetInstance<BusItemViewModel>();

		public BusStopView(BusStop busStop)
		{
			this.InitializeComponent();
			this.busStop = busStop;
			
			var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
			//this.Width = Window.Current.Bounds.Width * scaleFactor;

			this.Width = Window.Current.Bounds.Width;
			this.busStopName.Text = "Điểm dừng: " + busStop.Name;
			this.busStopThrough.Text = "Các tuyến buýt đi qua: ";// + busStop.FleetOver;

			char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

			string text = busStop.FleetOver;
			string[] words = text.Split(delimiterChars);

			int count = 0; 
			foreach (string s in words.Distinct())
			{
				if (s == "") { continue; }
				Button hb = new Button();
				hb.Content = s;
				hb.MinWidth = 45;
				hb.Width = 45;
				hb.BorderThickness = new Thickness(1, 1, 1, 1);
				hb.Padding = new Thickness(1, 1, 1, 1);
				hb.Margin = new Thickness(5, 0, 0, 0);
				hb.Name = s;
				hb.Click += BusStopThroughClick;

				count++;
				if (count <= 7)	
					busThroughPanel1.Children.Add(hb);
				else if (count <= 14)
					busThroughPanel2.Children.Add(hb);
				else
					busThroughPanel3.Children.Add(hb);
			}

			/*int count = 0;
			if (words.Length <= 7)
			{
				foreach (string s in words)
				{
					count++;
					Button hb = new Button();
					hb.Content = s;
					hb.MinWidth = 45;
					hb.Width = 45;
					hb.Margin = new Thickness(2, 0, 0, 0);
					hb.Name = s;
					hb.Click += BusStopThroughClick;
					busThroughPanel1.Children.Add(hb);
				}
			}*/
			
		}

		private void BusStopThroughClick(object sender, RoutedEventArgs e)
		{
			string code = ((Button)sender).Content as string;
			currentBusItem.SelectedBusItem = currentListInstance.FindBusItemByCode(code);

			Frame rootFrame = Window.Current.Content as Frame;
			if (rootFrame != null && rootFrame.CanGoBack)
			{
				rootFrame.GoBack();
			}
			//if (!((Frame)Window.Current.Content).Navigate(typeof(BusItemView)))
			//{
			//	throw new Exception("NavigationFailedExceptionMessage");
			//}
			closeBtn_Click(null, null);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			//this.Focus(FocusState.Pointer);
		}

		private void UserControl_LostFocus(object sender, RoutedEventArgs e)
		{
			Popup mensaje = this.Parent as Popup;
			mensaje.IsOpen = false;
		}

		private void closeBtn_Click(object sender, RoutedEventArgs e)
		{
			Popup mensaje = this.Parent as Popup;
			mensaje.IsOpen = false;
		}
	}
}
