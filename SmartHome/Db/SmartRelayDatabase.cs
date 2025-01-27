using SmartHome.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Db
{
    public class SmartRelayDatabase
    {
        SQLiteAsyncConnection Database;
        async Task Init()
        {
            
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Db.DatabasePath, Db.Flags);
            var result = await Database.CreateTableAsync<Log>();

            //await DeleteTableAsync("Log");
            //await Database.DeleteAllAsync<Log>();
            //for (int i = 1; i <= 20; i++)
            //{
            //    var timestamp = DateTime.UtcNow;
            //    var newlog = new Log() { Timestamp = timestamp, Text = $"Ringing{i}", IsSeen = false };
            //    await Database.InsertAsync(newlog);
            //    await Task.Delay(2000);
            //}
        }

        public async Task<IEnumerable<Log>> GetLogsAsync()
        {
            await Init();
            return await Database.Table<Log>().ToListAsync();
        }

        /// <summary>
        /// Insert a new log or update the existing log
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<int> SaveLogAsync(Log log)
        {
            await Init();
            if (log.Id != 0)
                return await Database.UpdateAsync(log);
            else
                return await Database.InsertAsync(log);
        }

        public async Task<int> DeleteLogAsync(Log log)
        {
            await Init();
            return await Database.DeleteAsync(log);
        }

        public async Task<int> DeleteAllLogsAsync()
        {
            await Init();
            return await Database.DeleteAllAsync<Log>();
        }
    }
}
