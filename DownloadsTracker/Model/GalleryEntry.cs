using System;

namespace DownloadsTracker.Model 
{
    public class GalleryEntry
    {
        /// <summary>
        /// Version of the module
        /// </summary>
        public Version Version { get; set; }
        
        /// <summary>
        /// Amount of downloads of this particular version
        /// </summary>
        public int VersionDownloadCount { get; set; }

        /// <summary>
        /// Regardless of the version of the current entry, this property will always show the historic download count of all the versions combined.
        /// </summary>
        public int TotalModuleDownloadCount { get; set; }
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
