using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ParksWeb.Controllers
{
    public class FavoratesController : Controller
    {
        [HttpPost]
        public ActionResult UploadFavorates(Favorate[] favorates, string signature)
        {
            SaveSignature(favorates, signature);
            return Json(new { favorates = favorates, signature = signature });
        }

        private void SaveSignature(Favorate[] favorates, string signature)
        {
            var container = GetContainer();

            var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".png");
            var bytes = Convert.FromBase64String(signature.Split(',')[1]);
            //var blockBlob = container.GetBlockBlobReference("aaaa.mp4");
            var stream = new MemoryStream(bytes);
            var ints = favorates.Select(x => x._id);
            blockBlob.Metadata["Favorates"] = JsonConvert.SerializeObject(ints);
            blockBlob.UploadFromStream(stream);
        }

        private static CloudBlobContainer GetContainer()
        {
            var account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("ConnectionString"));

            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference("parksignatures");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
            return container;
        }

        public ActionResult Index()
        {
            var container = GetContainer();
            var blobs = container.ListBlobs();
            var lists = new List<SignatureStruct>();
            foreach (var b in blobs.OfType<CloudBlockBlob>())
            {
                b.FetchAttributes();
                var ss = new SignatureStruct()
                {
                    Favorates = b.Metadata["Favorates"],
                    ImageSrc = b.Uri
                };
                lists.Add(ss);
            }
            return View(lists);
        }
    }

    public class SignatureStruct
    {
        public string Favorates { get; set; }
        public Uri ImageSrc { get; set; }
    }



    public class Favorate
    {
        public string _id { get; set; }
        public string ParkName { get; set; }
        public string AdministrativeArea { get; set; }
        public string Location { get; set; }
        public string ParkType { get; set; }
        public string Introduction { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Area { get; set; }
        public string YearBuilt { get; set; }
        public object OpenTime { get; set; }
        public string Image { get; set; }
        public string ManagementName { get; set; }
        public string ManageTelephone { get; set; }
    }





}