using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ParksWeb.Models;

namespace ParksWeb.Controllers
{
    public class ImagesController : Controller
    {
        public ActionResult Post(int parkId, string description)
        {
            var parkImage = new ParkImage() { ParkId = parkId, Description = description };

            var videoFile = Request.Files[0];

            SaveImage(videoFile.InputStream, parkImage);
            return Json(true);
        }

        private void SaveImage(Stream stream, ParkImage image)
        {
            var account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("ConnectionString"));

            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference("parkimgs");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            //var blockBlob = container.GetBlockBlobReference(image.Filename);
            var blockBlob = container.GetBlockBlobReference("aaaa.mp4");
            blockBlob.Metadata["ParkId"] = image.ParkId.ToString();
            blockBlob.Metadata["Description"] = image.Description;
            blockBlob.UploadFromStream(stream);
            image.ImageUri = blockBlob.Uri.ToString();
        }
    }
}
