using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ParksWeb.Controllers
{
    public class NotificationsController : Controller
    {
        public static List<string> DeviceIds = new List<string>();

        // GET: Messages
        public ActionResult Index()
        {
            return View(DeviceIds);
        }

        [HttpPost]
        public ActionResult RegisterDevice(string deviceId)
        {
            DeviceIds.Add(deviceId);

            var response = SendNotification(deviceId, "註冊成功!");
            return Json(response);
        }

        private string SendNotification(string deviceId, string message)
        {

            string GoogleAppID = WebConfigurationManager.AppSettings["GoogleGCMProjectKey"]; //Google service ikey
            var SENDER_ID = WebConfigurationManager.AppSettings["GoogleGCMProjectId"]; ; //Google project  ID
            var value = System.Web.HttpUtility.UrlEncode(message); //如果有傳遞中文，需要編碼           
            var tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" 
                + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;
            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tResponse = tRequest.GetResponse();
            dataStream = tResponse.GetResponseStream();
            StreamReader tReader = new StreamReader(dataStream);
            String sResponseFromServer = tReader.ReadToEnd();
            tReader.Close();
            dataStream.Close();
            tResponse.Close();
            return sResponseFromServer;
        }


        [HttpPost]
        public ActionResult SendMessage(string message)
        {
            foreach (var deviceId in DeviceIds)
            {
                SendNotification(deviceId, message);
            }
            return RedirectToAction("Index");
        }
    }
}