using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

namespace ParksWeb.Providers
{
    public class NotificationService
    {

        public string SendNotification(List<string> deviceRegIds, string message)
        {
            try
            {
                string regIds = string.Join("\",\"", deviceRegIds);

                string AppId = WebConfigurationManager.AppSettings["GoogleGCMProjectKey"];
                var SenderId = WebConfigurationManager.AppSettings["GoogleGCMProjectId"];


                WebRequest wRequest;
                wRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                wRequest.Method = "post";
                wRequest.ContentType = " application/json;charset=UTF-8";
                wRequest.Headers.Add(string.Format("Authorization: key={0}", AppId));

                wRequest.Headers.Add(string.Format("Sender: id={0}", SenderId));

                var notificationData = new JObject(
                    new JProperty("registration_ids", new JArray(deviceRegIds)),
                    new JProperty("data", new JObject(
                        new JProperty("title", "公園趣" + DateTime.Today.ToString("yyyyMMdd")),
                        new JProperty("message", message),
                        new JProperty("badge", "21")
                    ))
                );

                //string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"message\" : " + "\"" + message + "\",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\"},\"registration_ids\":[\"" + regIds + "\"], \"notification\" : { \"title\" : \"Hi\", \"message\": \"message body\" }}";
                var postData = notificationData.ToString();

                Byte[] bytes = Encoding.UTF8.GetBytes(postData);
                wRequest.ContentLength = bytes.Length;

                Stream stream = wRequest.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();

                WebResponse wResponse = wRequest.GetResponse();

                stream = wResponse.GetResponseStream();

                StreamReader reader = new StreamReader(stream);

                String response = reader.ReadToEnd();

                HttpWebResponse httpResponse = (HttpWebResponse)wResponse;
                string status = httpResponse.StatusCode.ToString();

                reader.Close();
                stream.Close();
                wResponse.Close();

                if (status == "")
                {
                    return response;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        //https://developers.google.com/cloud-messaging/http-server-ref#send-downstream
        public string SendNotificationByUrlencoded(string deviceId, string message)
        {

            string GoogleAppID = WebConfigurationManager.AppSettings["GoogleGCMProjectKey"]; //Google service ikey
            var SENDER_ID = WebConfigurationManager.AppSettings["GoogleGCMProjectId"]; ; //Google project  ID
            var value = System.Web.HttpUtility.UrlEncode(message); //如果有傳遞中文，需要編碼           
            var tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.title=Hello&data.msgcnt=21&data.message="
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
    }
}