using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octoposh.Web.Model;

namespace Octoposh.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<GalleryEntry> GalleryEntries { get; set; }

        public void OnGet()
        {
            var gallery = new Gallery();
            GalleryEntries = gallery.GetEntries();
        }
    }
}
