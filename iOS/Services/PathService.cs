using System;
using System.IO;
using SQLiteVsRealm.iOS.Services;
using SQLiteVsRealm.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(PathService))]
namespace SQLiteVsRealm.iOS.Services
{
    public class PathService : IPathService
    {
        public string GetDBPath(string dbName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbName);
        }
    }
}

