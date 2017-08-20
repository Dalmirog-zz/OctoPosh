using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Octoposh.Web.Model
{
    public class Gallery
    {
        public Gallery()
        {

        }
        public List<GalleryEntry> GetEntries()
        {
            string UrlRequest = "https://www.powershellgallery.com/api/v2/FindPackagesById()?id=%27Octoposh%27";
            List<GalleryEntry> Entries = ConvertEntries(GetGalleryXML(UrlRequest));
            return Entries.OrderByDescending(o => o.Version).ToList();
        }

        private XmlDocument GetGalleryXML(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Console.Read();
                return null;
            }
        }

        private List<GalleryEntry> ConvertEntries(XmlDocument xml)
        {
            List<GalleryEntry> Entries = new List<GalleryEntry>();
            //XmlNode root = xml.DocumentElement;
            
            foreach (XmlNode childNode in xml.DocumentElement)
            {
                if(childNode.Name == "entry")
                {
                    var properties = (childNode["m:properties"]);

                    Entries.Add(new GalleryEntry
                    {
                        Version = new Version(properties["d:Version"].InnerText),
                        Downloads = Convert.ToInt32(properties["d:VersionDownloadCount"].InnerText),
                        DatePushed = DateTime.Parse(properties["d:Created"].InnerText),
                        GalleryDownloadURI = new Uri(properties["d:GalleryDetailsUrl"].InnerText)
                    });                    
                }
            }
            return Entries;
        }

    }
}
