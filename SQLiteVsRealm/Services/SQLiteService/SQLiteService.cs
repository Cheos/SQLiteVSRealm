using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteVsRealm.Models;

namespace SQLiteVsRealm.SQLiteService
{
    public class SQLiteService : ISQLiteService
    {
        private readonly SQLiteAsyncConnection _sqliteConnection;

        public SQLiteService(string dbPath)
        {
            _sqliteConnection = new SQLiteAsyncConnection(dbPath);
        }

        public Task<CreateTablesResult> CreateTable()
        {
            return _sqliteConnection.CreateTableAsync<SQLiteItem>();
        }

        public async Task InsertRecords(int count)
        {
            await _sqliteConnection.RunInTransactionAsync((SQLiteConnection obj) =>
            {
                for (int i = 0; i < count; i++)
                {
                    obj.Insert(new SQLiteItem
                    {
                        Number = i + 1,
                        Guid = Guid.NewGuid().ToString(),
                        Info = $"Record #{i}"
                    });
                }
            });
        }

        public Task<List<SQLiteItem>> GetAllRecords()
        {
            return _sqliteConnection.Table<SQLiteItem>().ToListAsync();
        }

        public async Task DeleteRecords(IList<SQLiteItem> items)
        {
            await _sqliteConnection.RunInTransactionAsync((SQLiteConnection obj) =>
            {
                foreach (var item in items)
                {
                    obj.Delete(item);
                }
            });
        }
    }
}

