
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Uri = Android.Net.Uri;

namespace POI
{
	[Activity (Label = "POIDetailActivity")]			
	public class POIDetailActivity : Activity, ILocationListener
	{
		private EditText _nameEditText;
		private EditText _descrEditText;
		private EditText _addrEditText;
		private EditText _latEditText;
		private EditText _longEditText;
		private ImageView _poiImageView;
		private PointOfInterest _poi;
	    private LocationManager _locationManager;
	    private ImageButton _locationImageButton;
	    private ImageButton _mapImageButton;
	    private ProgressDialog _progressDialog;

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.POIDetail);
            _locationManager = GetSystemService(Context.LocationService) as LocationManager;
			_nameEditText = FindViewById<EditText> (Resource.Id.nameEditText);
			_descrEditText = FindViewById<EditText> (Resource.Id.descrEditText);
			_addrEditText = FindViewById<EditText> (Resource.Id.addrEditText);
			_latEditText = FindViewById<EditText> (Resource.Id.latEditText);
			_longEditText = FindViewById<EditText> (Resource.Id.longEditText);
			_poiImageView = FindViewById<ImageView> (Resource.Id.poiImageView);
	        _locationImageButton = FindViewById<ImageButton>(Resource.Id.locationImageButton);
	        _mapImageButton = FindViewById<ImageButton>(Resource.Id.mapImageButton);

			if (Intent.HasExtra ("poiId")) {
				int poiId = Intent.GetIntExtra ("poiId", -1); 
				_poi = POIData.Service.GetPOI (poiId);
			}
			else
				_poi = new PointOfInterest ();

            _locationImageButton.Click += (s, e) =>
            {
                _progressDialog = ProgressDialog.Show(this, "", "Obtaining location...");
                _locationManager.RequestSingleUpdate(new Criteria{Accuracy = Accuracy.NoRequirement, PowerRequirement = Power.NoRequirement}, this, null);
            };

	        _mapImageButton.Click += (s, e) =>
	        {
	            // uri options:
	            // geo:latitude,longitude
	            // geo:latitude,longitude?z={zoom} where {zoom} is the zoom level
	            // geo:0,0?q=my+street+address
	            // geo:0,0?q=business+near+city
	            var geocodeUri = Uri.Parse(String.IsNullOrEmpty(_addrEditText.Text)
	                ? String.Format("geo:{0},{1}", _poi.Latitude, _poi.Longitude)
	                : String.Format("geo:0,0?q={0}", _addrEditText.Text));

	            var mapIntent = new Intent(Intent.ActionView, geocodeUri);
	            var activities = PackageManager.QueryIntentActivities(mapIntent, 0);
	            if (activities.Any())
	                StartActivity(mapIntent);
	            else
	            {
	                var alertConfirm = new AlertDialog.Builder(this);
	                alertConfirm.SetCancelable(false);
	                alertConfirm.SetPositiveButton("OK", delegate { });
	                alertConfirm.SetMessage("No map app available.");
	                alertConfirm.Show();
	            }
	        };
		    
			UpdateUI ();
		}

		protected void UpdateUI()
		{
			_nameEditText.Text = _poi.Name;
			_descrEditText.Text = _poi.Description;
			_addrEditText.Text = _poi.Address;
			_latEditText.Text = _poi.Latitude.ToString ();
			_longEditText.Text = _poi.Longitude.ToString ();

		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.POIDetailMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);

			// disable delete for a new POI
			if (!_poi.Id.HasValue) {
				IMenuItem item = menu.FindItem (Resource.Id.actionDelete);
				item.SetEnabled (false);
			}

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.actionSave:
				SavePOI();
				return true;

			case Resource.Id.actionDelete: 
				DeletePOI();
				return true;

			default :
				return base.OnOptionsItemSelected (item);
			}
		}

		protected void SavePOI()
		{
			bool errors = false;

			if (String.IsNullOrEmpty (_nameEditText.Text)) {
				_nameEditText.Error = "Name cannot be empty";
				errors = true;
			}
			else
				_nameEditText.Error = null;

			double? tempLatitude = null;
			if (!String.IsNullOrEmpty(_latEditText.Text)) {
				try {
					tempLatitude = Double.Parse(_latEditText.Text);
					if ((tempLatitude > 90) | (tempLatitude < -90)) {
						_latEditText.Error = "Latitude must be a decimal valuebetween -90 and 90";
						errors = true;
					}
					else
						_latEditText.Error = null;
				}
				catch {
					_latEditText.Error = "Latitude must be valid decimal number";
					errors = true;
				}
			}

			if (errors)
				return;

			_poi.Name = _nameEditText.Text;
			_poi.Description = _descrEditText.Text;
			_poi.Address = _addrEditText.Text;
			_poi.Latitude = Double.Parse(_latEditText.Text);
			_poi.Longitude = Double.Parse(_longEditText.Text);
            
			POIData.Service.SavePOI (_poi);
            var toast = Toast.MakeText(this, String.Format("{0} saved.", _poi.Name), ToastLength.Short);
            toast.Show();
			Finish (); // Finish closes the activity and brings the previous activity to the fore
		}

		protected void DeletePOI()
		{
		    var dialog = new AlertDialog.Builder(this);
		    dialog.SetCancelable(false);
		    dialog.SetPositiveButton("OK", (s, e) =>
		    {
		        POIData.Service.DeletePOI(_poi);
                var toast = Toast.MakeText(this, String.Format("{0} deleted.", _poi.Name), ToastLength.Short);
                toast.Show();
		        Finish(); // Finish closes the activity and brings the previous activity to the fore
		    });
		    dialog.SetNegativeButton("Cancel", delegate {});
		    dialog.SetMessage(string.Format("Are you sure you want to delete {0}?", _poi.Name));
		    dialog.Show();
		}

	    public void OnLocationChanged(Location location)
	    {
            _latEditText.Text = location.Latitude.ToString();
            _longEditText.Text = location.Longitude.ToString();

            var geocdr = new Geocoder(this);
            var addresses = geocdr.GetFromLocation(location.Latitude, location.Longitude, 5);

            if (addresses.Any())
                UpdateAddressFields(addresses.First());

            _progressDialog.Cancel();
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

	    protected void UpdateAddressFields(Address addr)
	    {
	        if (string.IsNullOrEmpty(_nameEditText.Text))
	            _nameEditText.Text = addr.FeatureName;

            if (string.IsNullOrEmpty(_addrEditText.Text))
	        {
	            for (int i = 0; i < addr.MaxAddressLineIndex; i++)
	            {
	                if (!String.IsNullOrEmpty(_addrEditText.Text))
                        _addrEditText.Text += System.Environment.NewLine;

	                _addrEditText.Text += addr.GetAddressLine(i);
	            }
	        }
	    }
	}
}

