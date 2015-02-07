using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using BusFinderUniversal.Model;
using BusFinderUniversal.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace BusFinderUniversal
{
	public class MyUtil
	{
		public static string VNTextNormalize(string text)
		{
			return ConvertVN(text.ToLower().Replace(" ", ""));
		}

		public static string ConvertVN(string chucodau)
		{
			const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
			const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
			int index = -1;
			char[] arrChar = FindText.ToCharArray();
			while ((index = chucodau.IndexOfAny(arrChar)) != -1)
			{
				int index2 = FindText.IndexOf(chucodau[index]);
				chucodau = chucodau.Replace(chucodau[index], ReplText[index2]);
			}
			return chucodau;
		}

		public static Double DistanceInMetres(double lat1, double lon1, double lat2, double lon2)
		{

			if (lat1 == lat2 && lon1 == lon2)
				return 0.0;

			var theta = lon1 - lon2;

			var distance = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
							Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
							Math.Cos(deg2rad(theta));

			distance = Math.Acos(distance);
			if (double.IsNaN(distance))
				return 0.0;

			distance = rad2deg(distance);
			distance = distance * 60.0 * 1.1515 * 1609.344;

			return (distance);
		}

		public static double DistanceInKiloMetres(Geopoint geo1, Geopoint geo2)
		{
			return MyUtil.DistanceInMetres(geo1.Position.Latitude, geo1.Position.Longitude, geo2.Position.Latitude, geo2.Position.Longitude) / 1000;
		}

		private static double deg2rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		private static double rad2deg(double rad)
		{
			return (rad / Math.PI * 180.0);
		}

		public static Geopoint newGeopoint(double lon, double lat)
		{
			return new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lon });
		}

		public static async Task<string> GetPlaceNameByLocation(Geopoint myPoint)
		{
			if (myPoint == null) return "";
			MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(myPoint);
			string addressName = "";
			if (result.Status == MapLocationFinderStatus.Success)
			{
				MapAddress ma = result.Locations[0].Address;
				if (ma.StreetNumber != "") addressName = ma.StreetNumber + " ";
				if (ma.Street != "") addressName += ma.Street + ", ";
				if (ma.District != "") addressName += ma.District + ", ";
				if (ma.Town != "") addressName += ma.Town;
				return addressName;
			}
			return "";
		}

		private async void findShortestRoute(Geopoint wayPoints1, Geopoint wayPoints2)
		{
			var routeResult = await MapRouteFinder.GetDrivingRouteAsync(wayPoints1, wayPoints2);
			if (routeResult.Status == MapRouteFinderStatus.Success)
			{
// 				MyMap.MapElements.Clear();
// 				// Use the route to initialize a MapRouteView.
// 				MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
// 				viewOfRoute.RouteColor = Colors.Yellow;
// 				viewOfRoute.OutlineColor = Colors.Black;
// 
// 				// Add the new MapRouteView to the Routes collection
// 				// of the MapControl.
// 				MyMap.Routes.Add(viewOfRoute);
// 
// 				// Fit the MapControl to the route.
// 				await MyMap.TrySetViewBoundsAsync(
// 					routeResult.Route.BoundingBox,
// 					null,
// 					Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
			}
		}

		public static string GetStringResource(string name)
		{
			return new Windows.ApplicationModel.Resources.ResourceLoader().GetString(name);
		}

		public static List<Geopoint> textToGeoList(string text)
		{

			char[] delimiterChars = { ' ' };
			string[] words = text.Split(delimiterChars);
			List<Geopoint> geoList = new List<Geopoint>();

			foreach (string word in words)
			{
				if (!word.Contains(",")) continue;
				char[] delimiterChars2 = { ',' };
				string[] lnglat = word.Split(delimiterChars2);
				double lng = Double.Parse(lnglat[0]);
				double lat = Double.Parse(lnglat[1]);
				geoList.Add(new Geopoint(new BasicGeoposition { Latitude = lat, Longitude = lng }));
			}

			return geoList;
		}

		public static List<BusStop> textToBusStopList(string text)
		{
			
			char[] delimiterChars = { ' ' };
			string[] words = text.Split(delimiterChars);
			List<BusStop> bsList = new List<BusStop>();
			ListBusViewModel currentListInstance = ServiceLocator.Current.GetInstance<ListBusViewModel>();

			foreach (string word in words)
			{
				var matches = currentListInstance.Buses.Stops.Where((bus) => bus.Code.Equals(word));
				if (matches.Count() >= 1) 
					bsList.Add(matches.First());
			}
			if (bsList.Count == 0) return null;
			return bsList;
		}
	}

	public class MessageDialogHelper
	{
		public static async void Show(string content, string title="")
		{
			try
			{
				MessageDialog messageDialog = new MessageDialog(content, title);
				await messageDialog.ShowAsync();
			}
			catch (Exception exc)
			{
				string excmsg = exc.Message;
			}
		}
	}

	public class DataSerialization
	{
		public static string Serialize(object obj)
		{
			XmlSerializer Xml_Serializer = new XmlSerializer(obj.GetType());
			StringWriter Writer = new StringWriter();
			Xml_Serializer.Serialize(Writer, obj);
			return Writer.ToString();
		}

		public static T Deserialize<T>(string xml)
		{
			var xs = new XmlSerializer(typeof(T));
			return (T)xs.Deserialize(new StringReader(xml));
		}
	}

	public static class EncryptionHelper
	{

#if NETFX_CORE
		public static byte[] Encrypt(string plainText, string pw, string salt)
		{
			IBuffer pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
			IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf16LE);
			IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf16LE);

			// Derive key material for password size 32 bytes for AES256 algorithm
			KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
			// using salt and 1000 iterations
			KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

			// create a key based on original key and derivation parmaters
			CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
			IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
			CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

			// derive buffer to be used for encryption salt from derived password key
			IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

			// display the buffers – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
			string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
			string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
			// create symmetric key from derived password key
			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

			// encrypt data buffer using symmetric key and derived salt material
			IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, plainBuffer, saltMaterial);
			byte[] result;
			CryptographicBuffer.CopyToByteArray(resultBuffer, out result);

			return result;
		}

		public static string Decrypt(byte[] encryptedData, string pw, string salt)
		{
			IBuffer pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
			IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf16LE);
			IBuffer cipherBuffer = CryptographicBuffer.CreateFromByteArray(encryptedData);

			// Derive key material for password size 32 bytes for AES256 algorithm
			KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
			// using salt and 1000 iterations
			KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

			// create a key based on original key and derivation parmaters
			CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
			IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
			CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

			// derive buffer to be used for encryption salt from derived password key
			IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

			// display the keys – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
			string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
			string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
			// create symmetric key from derived password material
			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

			// encrypt data buffer using symmetric key and derived salt material
			IBuffer resultBuffer = CryptographicEngine.Decrypt(symmKey, cipherBuffer, saltMaterial);
			string result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16LE, resultBuffer);
			return result;
		}

#else

		public static byte[] Encrypt(string dataToEncrypt, string password, string salt)
		{
			AesManaged aes = null;
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;

			try
			{
				//Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator
				Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

				//Create AES algorithm with 256 bit key and 128-bit block size
				aes = new AesManaged();
				aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
				rfc2898.Reset(); //needed for WinRT compatibility
				aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

				//Create Memory and Crypto Streams
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

				//Encrypt Data
				byte[] data = Encoding.Unicode.GetBytes(dataToEncrypt);
				cryptoStream.Write(data, 0, data.Length);
				cryptoStream.FlushFinalBlock();

				//Return encrypted data
				return memoryStream.ToArray();
			  
			}
			catch (Exception eEncrypt)
			{
				return null;
			}
			finally
			{
				if (cryptoStream != null)
					cryptoStream.Close();

				if (memoryStream != null)
					memoryStream.Close();

				if (aes != null)
					aes.Clear();

			}
		}

		public static string Decrypt(byte[] dataToDecrypt, string password, string salt)
		{
			AesManaged aes = null;
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			string decryptedText = "";
			try
			{
				//Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator
				Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

				//Create AES algorithm with 256 bit key and 128-bit block size
				aes = new AesManaged();
				aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
				rfc2898.Reset(); //neede to be WinRT compatible
				aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

				//Create Memory and Crypto Streams
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

				//Decrypt Data
				cryptoStream.Write(dataToDecrypt, 0, dataToDecrypt.Length);
				cryptoStream.FlushFinalBlock();

				//Return Decrypted String
				byte[] decryptBytes = memoryStream.ToArray();
				decryptedText = Encoding.Unicode.GetString(decryptBytes, 0, decryptBytes.Length);
			}
			catch (Exception eDecrypt)
			{
   
			}
			finally
			{
				if (cryptoStream != null)
					cryptoStream.Close();

				if (memoryStream != null)
					memoryStream.Close();

				if (aes != null)
					aes.Clear();
			}
			return decryptedText;
		}
#endif

	}
}
