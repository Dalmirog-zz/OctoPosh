using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace DownloadsTracker.Model
{
    public class TableEntryEntity : TableEntity
    {
        // Your entity type must expose a parameter-less constructor
        public TableEntryEntity()
        {
        }
        public TableEntryEntity(string moduleVersion)
        {
            this.PartitionKey = moduleVersion;
            this.RowKey = DateTime.Now.ToString("yyyyMMdd");
        }

        public int? VersionDayCount { get; set; }
        public int VersionTotalToDate { get; set; }
        public int ModuleTotalToDate { get; set; }
    }
}
