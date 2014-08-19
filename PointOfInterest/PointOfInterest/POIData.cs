using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Path = System.IO.Path;

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

        public static Bitmap GetImageFile(int poiId)
        {
            var filename = Service.GetImageFilename(poiId);
            if (!File.Exists(filename))
                return null;

            var imageFile = new Java.IO.File(filename);
            return BitmapFactory.DecodeFile(imageFile.Path);
        }
	}
}
