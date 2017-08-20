using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Web.Model 
{
    public class GalleryEntry : IGalleryEntry
    {
       public Version Version { get; set; }
       public int Downloads { get; set; }
       public DateTime DatePushed { get; set; }
       public Uri ReleaseNotesURI {
            get
            {
                var version = Version.ToString().Replace(".", "");
                var URI = new Uri(String.Format("{0}/{1}", "https://github.com/dalmirog/Octoposh/wiki/Release-Notes#",version));
                return URI;
            }
            set { }
        }
        public Uri GalleryDownloadURI { get; set; }

    }
}
