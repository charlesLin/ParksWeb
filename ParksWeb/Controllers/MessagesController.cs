using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParksWeb.Providers;

namespace ParksWeb.Controllers
{
    public class NotificationsController : Controller
    {
        public static List<string> DeviceIds = new List<string>()
        {
            "APA91bGbm_lSz3F8rXjwoCFGdtzCEaF5OKvx0BrLAlgWF19_1crscQASgxzHhSZIU5T80DKe3sRUUo3RRMD1aGf0y6o2SaVLoslDHtrX-lxVKUE-I-xMBM3GyNZMVyzKi94R1_cVK-3nwlN8K0kqAGpVHX5-T6kmOQ"
        };

        // GET: Messages
        public ActionResult Index()
        {
            return View(DeviceIds);
        }

        [HttpPost]
        public ActionResult RegisterDevice(string deviceId)
        {
            DeviceIds.Add(deviceId);

            var service = new NotificationService();
            var response = service.SendNotificationByUrlencoded(deviceId, "註冊成功!");
            return Json(response);
        }


       


        [HttpPost]
        public ActionResult SendMessage(string message)
        {
            //foreach (var deviceId in DeviceIds)
            //    SendNotificationByUrlencoded(deviceId, message);
            var service = new NotificationService();
            service.SendNotification(DeviceIds, message);
            return RedirectToAction("Index");
        }
    }
}