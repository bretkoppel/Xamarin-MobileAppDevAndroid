using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace POI
{
	[Activity (Label = "PointOfInterest", MainLauncher = true, Icon = "@drawable/icon")]
	public class POIListActivity : Activity
	{
		private ListView _poiListView;
		private POIListViewAdapter _adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.POIList);

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
	    }
	}

}


