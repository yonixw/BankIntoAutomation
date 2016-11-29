using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using PrivateData = MonyDataMacro.Properties.Settings;

namespace MonyDataMacro
{
    class PushBulletUpdate
    {
        public class Note
        {
            public string type = "note";
            public string title = "Title here";
            public string body = "Insert body here";
        }

        public static void sendDataToPushbullet(string notebody)
        {
            // Sending dat to Pushbullet to "me"
            //_______________________________________________

            var httpWebRequest = WebRequest.Create("https://api.pushbullet.com/v2/pushes");
            httpWebRequest.Headers.Add("Access-Token", PrivateData.Default.Auth);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                Note info = new Note();
                info.title = "עדכון שבועי";
                info.body = notebody;

                string json = (new JavaScriptSerializer()).Serialize(info);
                Console.WriteLine(json);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }
        }
    }
}
