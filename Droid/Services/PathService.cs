using System;
using System.IO;
using SQLiteVsRealm.Droid.Services;
using SQLiteVsRealm.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(PathService))]
namespace SQLiteVsRealm.Droid.Services
{
    public class PathService : IPathService
    {
        public string GetDBPath(string dbName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbName);
        }
    }
}

