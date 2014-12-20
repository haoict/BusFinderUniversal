using BusFinderUniversal.Common;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusFinderUniversal.ViewModel
{
	public class AboutViewModel : ViewModelBase
	{
		public RelayCommand FeedbackDelegateCommand { get; private set; }
		public RelayCommand RateAppDelegateCommand { get; private set; }
		public RelayCommand FacebookButtonDelegateCommand { get; private set; }
		public RelayCommand AboutSigDelegateCommand { get; private set; }

		private string _descriptionText;
		private string _appNameText;
		private string _versionText;
		private string _copyrightText;
		public string DescriptionText
		{
			get
			{
				return _descriptionText;
			}
			set
			{
				Set("DescriptionText", ref _descriptionText, value);
			}
		}
		public string VersionText
		{
			get
			{
				return _versionText;
			}
			set
			{
				Set("VersionText", ref _versionText, value);
			}
		}
		public string AppNameText
		{
			get
			{
				return _appNameText;
			}
			set
			{
				Set("AppNameText", ref _appNameText, value);
			}
		}
		public string CopyrightText
		{
			get
			{
				return _copyrightText;
			}
			set
			{
				Set("CopyrightText", ref _copyrightText, value);
			}
		}



		public AboutViewModel()
		{
			FeedbackDelegateCommand = new RelayCommand(FeedbackDelegateCommandHandler);
			RateAppDelegateCommand = new RelayCommand(RateAppDelegateCommandHandler);
			FacebookButtonDelegateCommand = new RelayCommand(FacebookButtonDelegateCommandHandler);
			AboutSigDelegateCommand = new RelayCommand(AboutSigDelegateCommandHandler);

			AppNameText = "Bus Finder";
			VersionText = "Version 1.0";
			CopyrightText = "Copyright © Semantic Innovation Group 2014";
			DescriptionText = "Bus Finder - Phần mềm cung cấp dịch vụ thông tin xe bus và hướng dẫn tìm lộ trình bằng xe bus. Bus Finder giúp bạn di chuyển bằng xe bus dễ dàng hơn. Phần mềm cung cấp thông tin về tuyến bus, lộ trình và các dịch vụ tiện ích gắn với vị trí với nội dung phong phú và giao diện thân thiện.";
		}

		public async void FeedbackDelegateCommandHandler()
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:caotuandung@gmail.com"));
		}

		public async void RateAppDelegateCommandHandler()
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=dae3cc18-d9fd-459c-aa41-9a60e35caf77"));
		}

		public async void FacebookButtonDelegateCommandHandler()
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri("http://facebook.com/hao.ict"));
		}

		public void AboutSigDelegateCommandHandler()
		{
			MessageDialogHelper.Show("SIG - Semantic Innovation Group - Là nhóm nghiên cứu công nghệ đến từ Viện Công nghệ thông tin và Truyền thông, Đại học Bách Khoa Hà Nội.\r\nĐại diện liên lạc: Cao Tuấn Dũng", "Nhóm nghiên cứu SIG");
		}
	}
}
