using NUnit.Framework;
using POI;
using System;
using System.IO;

namespace PointOfInterestTests
{
	[TestFixture]
	public class PointOfInterestTestFixture
	{
		private IPointOfInterestService _poiService;
		
		[SetUp]
		public void Setup ()
		{
			string storagePath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			_poiService = new PointOfInterestService (storagePath);

			// clear any existing json files
			foreach (string filename in Directory.EnumerateFiles(storagePath,"*.json"))
				File.Delete (filename);
		}

		
		[TearDown]
		public void Tear ()
		{
		}

        [Test]
        public void CreatePointOfInterest()
        {
            var newPoi = new PointOfInterest
            {
                Name = "New PointOfInterest",
                Description = "PointOfInterest to test creating a new PointOfInterest",
                Address = "100 Main Street\nAnywhere, TX 75069"
            };

            _poiService.SavePOI(newPoi);

            int testId = newPoi.Id.Value;

            // refresh the cache to be sure the data was 
            // saved appropriately
            _poiService.RefreshCache();

            // verify if the newly create PointOfInterest exists
            var poi = _poiService.GetPOI(testId);
            Assert.NotNull(poi);
            Assert.AreEqual(poi.Name, "New PointOfInterest");
        }

        [Test]
        public void UpdatePOI()
        {
            var testPOI = new PointOfInterest
            {
                Name = "Update PointOfInterest",
                Description = "PointOfInterest being saved so we can test update",
                Address = "100 Main Street\nAnywhere, TX 75069"
            };
            _poiService.SavePOI(testPOI);
            
            var testId = testPOI.Id.Value;

            // refresh the cache to be sure the data and 
            // poi was saved appropriately
            _poiService.RefreshCache();

            var poi = _poiService.GetPOI(testId);
            poi.Description = "Updated Description for Update PointOfInterest";
            _poiService.SavePOI(poi);

            // refresh the cache to be sure the data was 
            // updated appropriately
            _poiService.RefreshCache();

            poi = _poiService.GetPOI(testId);
            Assert.NotNull(poi);
            Assert.AreEqual(poi.Description, "Updated Description for Update PointOfInterest");
        }

        [Test]
        public void DeletePOI()
        {
            PointOfInterest testPOI = new PointOfInterest();
            testPOI.Name = "Delete POI";
            testPOI.Description = "POI being saved so we can test delete";
            testPOI.Address = "100 Main Street\nAnywhere, TX 75069";
            _poiService.SavePOI(testPOI);

            int testId = testPOI.Id.Value;

            // refresh the cache to be sure the data and 
            // poi was saved appropriately
            _poiService.RefreshCache();

            PointOfInterest deletePOI = _poiService.GetPOI(testId);
            Assert.IsNotNull(deletePOI);
            _poiService.DeletePOI(deletePOI);

            // refresh the cache to be sure the data was 
            // deleted appropriately
            _poiService.RefreshCache();

            PointOfInterest poi = _poiService.GetPOI(testId);
            Assert.Null(poi);
        }
	}
}

