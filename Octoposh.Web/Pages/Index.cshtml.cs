using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DownloadsTracker.Model;

namespace Octoposh.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<GalleryEntry> GalleryEntries { get; set; }

        public void OnGet()
        {
            var gallery = new GalleryClient();
            GalleryEntries = gallery.GetEntries();
        }
    }
}
