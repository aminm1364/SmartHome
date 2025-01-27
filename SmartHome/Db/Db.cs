using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Db
{
    public static class Db
    {
        public const string DatabaseFilename = "SmartRelay.db3";

        public const SQLite.SQLiteOpenFlags Flags =
                        // open the database in read/write mode
                        SQLite.SQLiteOpenFlags.ReadWrite |
                        // create the database if it doesn't exist
                        SQLite.SQLiteOpenFlags.Create |
                        // enable multi-threaded database access
                        SQLite.SQLiteOpenFlags.SharedCache;
                        

        public static string DatabasePath =>
                        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static async Task DeleteTableAsync(string tableName)
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, Db.DatabaseFilename);
            var db = new SQLiteAsyncConnection(databasePath);

            // Construct the DROP TABLE query
            var dropTableQuery = $"DROP TABLE IF EXISTS {tableName}";

            // Execute the query
            await db.ExecuteAsync(dropTableQuery);

            Console.WriteLine($"Table {tableName} has been deleted.");
        }
    }
}
