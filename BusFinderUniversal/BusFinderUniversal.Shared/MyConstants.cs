using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal
{
	public class MyConstants
	{
		public static bool USE_LOCATION = true;
		public static double RADIUS = 0.3;
		public const int NUMBER_OF_BUS = 74;
		public const int NUMBER_OF_BUS_HCM = 159;
		public static readonly Geopoint DEFAULT_LOCATION = MyUtil.newGeopoint(105.851196289063, 21.0283069610596); // ho hoan kiem
		public static readonly Geopoint DEFAULT_LOCATION_HCM = MyUtil.newGeopoint(106.7081978, 10.7978321); // ho hoan kiem

		public const string ENCRYPT_KEY = "SIG-WP-HAONV-2014";
		public const string ENCRYPT_SALT = "603deb1015ca71be2b73aef0857d7781";
	}

}
