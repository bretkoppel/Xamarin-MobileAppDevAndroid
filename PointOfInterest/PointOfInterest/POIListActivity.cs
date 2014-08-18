using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace POI
{
	[Activity (Label = "PointOfInterest", MainLauncher = true, Icon = "@drawable/icon")]
	public class POIListActivity : Activity, ILocationListener
	{
		private ListView _poiListView;
		private POIListViewAdapter _adapter;
	    private LocationManager _locationManager;

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.POIList);
            _locationManager = GetSystemService(Context.LocationService) as LocationManager;

			_poiListView = FindViewById<ListView> (Resource.Id.poiListView);
			_adapter = new POIListViewAdapter (this);
			_poiListView.Adapter = _adapter;
			_poiListView.ItemClick += POIClicked;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.POIListViewMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.actionNew: 
					StartActivity (typeof(POIDetailActivity));
					return true;

				case Resource.Id.actionRefresh: 
					POIData.Service.RefreshCache ();
					_adapter.NotifyDataSetChanged ();
					return true;

				default :
					return base.OnOptionsItemSelected (item);
			}
		}

		protected void POIClicked(object sender, ListView.ItemClickEventArgs e)
		{
			var poi = POIData.Service.GetPOI ((int)e.Id);
			var poiDetailIntent = new Intent (this, typeof(POIDetailActivity));
			poiDetailIntent.PutExtra ("poiId", poi.Id.Value);
			StartActivity (poiDetailIntent);
		}

        protected override void OnResume()
	    {
            // when the detail activity is called, this activity's OnPause event
            // will fire. when we're finished with a detail activity, it will pass
            // control back here, firing OnResume()
	        base.OnResume();

            // tell the BaseAdapter that the dataset has changed.
            _adapter.NotifyDataSetChanged();

            var locationCriteria = new Criteria
            {
                Accuracy = Accuracy.NoRequirement,
                PowerRequirement = Power.NoRequirement
            };

            var provider = _locationManager.GetBestProvider(locationCriteria, true);
            _locationManager.RequestLocationUpdates(provider, 20000, 100, this);
	    }

	    protected override void OnPause()
	    {
            base.OnPause();

	        _locationManager.RemoveUpdates(this);
	    }

	    public void OnLocationChanged(Location location)
	    {
	        _adapter.CurrentLocation = location;
            _adapter.NotifyDataSetChanged();
	    }

	    public void OnProviderDisabled(string provider)
	    {
	        
	    }

	    public void OnProviderEnabled(string provider)
	    {
	        
	    }

	    public void OnStatusChanged(string provider, Availability status, Bundle extras)
	    {
	        
	    }
	}

}


