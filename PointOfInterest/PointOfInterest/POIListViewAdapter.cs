using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace POI
{

	public class POIListViewAdapter : BaseAdapter<PointOfInterest>
	{
		readonly Activity _context;

		public POIListViewAdapter (Activity context)
		{
			this._context = context;			
		}

		public override int Count
		{
			get { return POIData.Service.POIs.Count; }
		}

	    public Location CurrentLocation { get; set; }

	    public override long GetItemId (int position)
		{
			return POIData.Service.POIs [position].Id.Value;
		}

		public override PointOfInterest this[int index]
		{
			get { return POIData.Service.POIs[index]; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			// when a view is available for reuse, convertView will contain a reference to the view; otherwise, it will be null and a new view should be created. 
			// otherwise we'd need to create a new view for every single row, which would be expensive. better to reuse the rows as old ones scroll out of view.
			var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.POIListItem, null);

		    var poi = this [position];
			view.FindViewById<TextView> (Resource.Id.nameTextView).Text = poi.Name;
			var addressView = view.FindViewById<TextView> (Resource.Id.addrTextView);
			if (string.IsNullOrEmpty (poi.Address))
				addressView.Visibility = ViewStates.Gone;
			else
				addressView.Text = poi.Address;

            if ((CurrentLocation != null) && (poi.Latitude.HasValue) && (poi.Longitude.HasValue))
            {
                var poiLocation = new Location("") {Latitude = poi.Latitude.Value, Longitude = poi.Longitude.Value};
                var distance = CurrentLocation.DistanceTo(poiLocation) * 0.000621371F; // Meters -> Miles
                view.FindViewById<TextView>(Resource.Id.distanceTextView).Text = String.Format("{0:0,0.00} miles", distance);
            }
            else
                view.FindViewById<TextView>(Resource.Id.distanceTextView).Text = "??";

		    using (var poiImage = POIData.GetImageFile(poi.Id.Value))
		        view.FindViewById<ImageView>(Resource.Id.poiImageView).SetImageBitmap(poiImage);

			return view;
		}
	}
}
