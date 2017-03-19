using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbUp;
using DbUp.Helpers;

namespace Octoposh.Database
{
    class Program
    {
        //This console takes the first argument passed to it, which should be a connection string, and feeds it to Scripts/dropDatabase.SQL to drop the DB referenced in the connection string (if it exists)
        static int Main(string[] args)
        {
            var connectionString = args.FirstOrDefault();

            Console.WriteLine("Using connection string: \n {0} \n",connectionString);

            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseToDelete = builder.InitialCatalog;

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .WithVariable("Database", databaseToDelete)
                    .JournalTo(new NullJournal())
                    .LogToConsole()
                    .Build();

            Console.WriteLine("Deleting database [{0}]",databaseToDelete);

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success! Database [{0}] was deleted",databaseToDelete);
            Console.ResetColor();
            return 0;
        }
    }
}
