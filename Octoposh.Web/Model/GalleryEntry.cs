using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Web.Model 
{
    public class GalleryEntry
    {
       public Version Version { get; set; }
       public int Downloads { get; set; }
       public DateTime DatePushed { get; set; }
       public string ReleaseNotesURI {
            get
            {
                var version = Version.ToString().Replace(".", "");


                //Adding Appeng for 0.6.x builds because the anchor link for those had an extra "100-betaX" at the end.
                var append = "";
                if(Version.Minor == 6)
                {
                    append = $"100-beta{Version.Build}";
                }

                var URI = $"{"http://octoposh.readthedocs.io/en/latest/releasenotes/release-notes/#"}{version}{append}";
                return URI;
            }
            set { }
        }
        public string GalleryDownloadURI { get; set; }

    }
}
