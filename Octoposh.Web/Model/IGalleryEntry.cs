using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Web.Model
{
    public interface IGalleryEntry
    {
        Version Version { get; set; }
        int Downloads { get; set; }
        DateTime DatePushed { get; set; }
        Uri ReleaseNotesURI { get; set; }
        Uri GalleryDownloadURI { get; set; }
    }
}
