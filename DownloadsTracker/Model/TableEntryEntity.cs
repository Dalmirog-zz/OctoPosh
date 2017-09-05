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
            this.PartitionKey = "DownloadsCount";
            this.RowKey = DateTime.Now.ToString("yyyyMMdd");
        }

        public int? DayCount { get; set; }
        public int TotalToDate { get; set; }
    }
}
