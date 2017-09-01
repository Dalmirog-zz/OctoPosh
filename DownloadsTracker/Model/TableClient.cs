using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DownloadsTracker.Model
{
    public class TableClient
    {
        internal static CloudTable CloudTable;
        internal static string StorageConnectionString;
        internal static string TableName;

        public TableClient(string storageConnectionString,string tableName)
        {
            StorageConnectionString = storageConnectionString;

            TableName = tableName;

            CloudTable = GetOrCreateTableAsync(StorageConnectionString, TableName).Result;
        }

        public void InsertOrMergeGalleryEntry(GalleryEntry galleryEntry)
        {
            var yesterday = DateTime.Now.AddDays(-1);

            var previousDayEntry = GetEntryByDayAsync(yesterday).Result;

            var entryToInsert = new TableEntryEntity();

            if (previousDayEntry == null)
            {
                Console.WriteLine("Couldn't find entry for day [{0}]",yesterday.Date.ToString());
                entryToInsert.TotalToDate = galleryEntry.Downloads;
                entryToInsert.DayCount = 0;
            }

            else
            {
                Console.WriteLine("Found entry for day [{0}]", yesterday.Date.ToString());

                entryToInsert.TotalToDate = galleryEntry.Downloads;
                entryToInsert.DayCount = galleryEntry.Downloads - previousDayEntry.TotalToDate;
            }

            InsertOrMergeTableEntryAsync(entryToInsert).Wait();
        }

        private async Task<CloudTable> GetOrCreateTableAsync(string storageConnectionString, string tableName)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine($"Checking if table [{tableName}] exists...");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);

            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine($"Table [{tableName}] did not exist and it was created.");
            }
            else
            {
                Console.WriteLine($"Table [{tableName}] already exists");
            }

            return table;
        }

        private async Task<TableEntryEntity> GetEntryByDayAsync(DateTime day)
        {
            var yesterdayString = day.ToString("yyyyMMdd");

            Console.WriteLine($"Looking for table entry for [{yesterdayString}]");

            return (await RetrieveEntityUsingPointQueryAsync("DownloadsCount", yesterdayString));
        }

        private async Task<TableEntryEntity> InsertOrMergeTableEntryAsync(TableEntryEntity tableEntry)
        {
            if (tableEntry == null)
            {
                throw new ArgumentNullException("tableEntry");
            }

            // Create the InsertOrReplace  TableOperation
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(tableEntry);

            // Execute the operation.
            TableResult result = await CloudTable.ExecuteAsync(insertOrMergeOperation);
            TableEntryEntity insertedTableEntry = result.Result as TableEntryEntity;

            Console.WriteLine("Inserted/Updated table entry [TotalTodate = {0}] and [DayCount = {1}]", insertedTableEntry.TotalToDate,insertedTableEntry.DayCount);
            return insertedTableEntry;
        }

        private static async Task<TableEntryEntity> RetrieveEntityUsingPointQueryAsync(string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntryEntity>(partitionKey, rowKey);
            TableResult result = await CloudTable.ExecuteAsync(retrieveOperation);
            TableEntryEntity tableEntry = result.Result as TableEntryEntity;
            if (tableEntry != null)
            {
                Console.WriteLine("Found table entry for [{0}] with [TotalToDate = {1}]", tableEntry.RowKey, tableEntry.TotalToDate);
            }

            return tableEntry;
        }
    }
}