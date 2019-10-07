using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace KY.Core
{
    public static class WebHelper
    {
        public static XElement RequestXml(Uri baseUri, params object[] parameter)
        {
            Uri uri = BuildRequestUri(baseUri, parameter);
            WebRequest request = WebRequest.Create(uri);
            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                    return null;

                return XElement.Load(stream);
            }
        }

        public static Task<XElement> RequestXmlAsync(Uri baseUri, params object[] parameter)
        {
            return Task.Factory.StartNew(() => RequestXml(baseUri, parameter));
        }

        public static Uri BuildRequestUri(Uri baseUri, params object[] parameter)
        {
            string uriParameter = string.Empty;
            for (int i = 0; i < parameter.Length; i++)
            {
                if (i == 0)
                {
                    uriParameter = "?";
                }
                else if (i % 2 == 0)
                {
                    uriParameter += "&";
                }
                else
                {
                    uriParameter += "=";
                }
                if (parameter[i] != null)
                {
                    uriParameter += HttpUtility.HtmlEncode(parameter[i].ToString());
                }
            }
            return new Uri(baseUri, uriParameter);
        }

        public static string DownloadString(string url)
        {
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}