using System;

namespace SQLiteVsRealm.Services
{
    public interface IPathService
    {
        string GetDBPath (string dbName);
    }
}

