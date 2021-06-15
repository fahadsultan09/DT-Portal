using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UserLoginRegistration.Model;

namespace UserLoginRegistration.HelperClasses
{
    public static class Request
    {
        public static JsonResponse Login(string userName, string password, List<string> macAddres, string appUrl)
        {
            Response jsonResponse = new Response();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            var request = (HttpWebRequest)WebRequest.Create(appUrl + "Login/Index");
            var postData = "UserName=" + userName;
            postData += "&Password=" + password;
            postData += "&MacAddresses=" + string.Join(",", macAddres);
            var data = Encoding.ASCII.GetBytes(postData);
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                jsonResponse = JsonConvert.DeserializeObject<Response>(responseString);
            }
            return jsonResponse.Data;
        }
        public static JsonResponse ChangePassword(string userName, string password, List<string> macAddres, string appUrl)
        {
            Response jsonResponse = new Response();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            var request = (HttpWebRequest)WebRequest.Create(appUrl + "Login/ChangePassword");
            var postData = "UserName=" + userName;
            postData += "&Password=" + password;
            postData += "&MacAddresses=" + string.Join(",", macAddres);
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                jsonResponse = JsonConvert.DeserializeObject<Response>(responseString);
            }
            return jsonResponse.Data;
        }

    }
}
