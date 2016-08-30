using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteVsRealm.Models;

namespace SQLiteVsRealm.SQLiteService
{
    public interface ISQLiteService
    {
        Task<CreateTablesResult> CreateTable();

        Task InsertRecords(int count);

        Task<List<SQLiteItem>> GetAllRecords();

        Task DeleteRecords(IList<SQLiteItem> items);

    }
}

