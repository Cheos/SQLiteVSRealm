using System;
using System.Collections.Generic;
using System.Linq;
using Realms;
using SQLiteVsRealm.Models;

namespace SQLiteVsRealm.RealmService
{
    public class RealmService : IRealmService
    {
        private Realm _realm;
        private string _path;

        public RealmService(string dbPath)
        {
            _path = dbPath;
        }


        public IDisposable OpenDB()
        {
            return _realm = Realm.GetInstance(RealmConfiguration.DefaultConfiguration.ConfigWithPath(_path));
        }

        public void DeleteDB()
        {
            Realm.DeleteRealm(RealmConfiguration.DefaultConfiguration.ConfigWithPath(_path));
        }

        public void InsertRecords(int count)
        {
            using (var trans = _realm.BeginWrite())
            {
                for (int i = 0; i < count; i++)
                {
                    var realmItem = _realm.CreateObject<RealmItem>();
                    realmItem.Number = i + 1;
                    realmItem.Guid = Guid.NewGuid().ToString();
                    realmItem.Info = $"Record #{i}";
                }
                trans.Commit();
            }
        }

        public RealmResults<RealmItem> GetAllRecords()
        {
            return _realm.All<RealmItem>();
        }

        public RealmResults<RealmItem> GetRecords(Func<RealmItem, bool> predicate)
        {
            return GetAllRecords().Where(predicate) as RealmResults<RealmItem>;
        }

        public IList<RealmItem> GetAllRecordsList()
        {
            return GetAllRecords().ToList();
        }

        public IList<RealmItem> GetRecordsList(int count)
        {
            return GetAllRecordsList().Take(count).ToList();
        }

        public void DeleteRecords(IList<RealmItem> items)
        {
            using (var trans = _realm.BeginWrite())
            {
                foreach (var item in items)
                {
                    _realm.Remove(item);
                }
                trans.Commit();
            }
        }

        public void DeleteRangeRecords(RealmResults<RealmItem> items)
        {
            using (var trans = _realm.BeginWrite())
            {
                _realm.RemoveRange(items);
                trans.Commit();
            }
        }
    }
}

