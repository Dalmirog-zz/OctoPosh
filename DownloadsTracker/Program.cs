using Microsoft.Extensions.Configuration;
using System.Linq;
using DownloadsTracker.Model;

namespace DownloadsTracker
{    
    public class Program
    {
        internal const string TableName = "DownloadsCount";

        internal static readonly IConfigurationRoot AppConfig = GetConfig();

        public static void Main(string[] args)
        {
            var storageConnectionString = AppConfig.GetSection("storageConnectionString").Value;

            var tableClient = new TableClient(storageConnectionString,TableName);
            
            var galleryClient = new GalleryClient();

            var latestGalleryEntry = galleryClient.GetEntries().FirstOrDefault();

            tableClient.InsertOrMergeGalleryEntry(latestGalleryEntry);
        }

        private static IConfigurationRoot GetConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        }
    }

}

