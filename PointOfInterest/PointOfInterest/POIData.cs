using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace POI
{

	public class POIData
	{
		// external storage is not deleted on app uninstall, letting us hold state between installs/uninstalls
		// external storage requires fewer permissions to access than the secured apps data directory
		// external storage directories can be accessed by other apps
		// permissions for these are granted in the AndroidManifest.xml file
		public static readonly IPointOfInterestService Service = new PointOfInterestService(
			Path.Combine(
				Android.OS.Environment.ExternalStorageDirectory.Path,
				"POIApp")); 
	}
}
