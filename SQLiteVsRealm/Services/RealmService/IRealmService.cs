using System;
using System.Collections.Generic;
using Realms;
using SQLiteVsRealm.Models;

namespace SQLiteVsRealm.RealmService
{
    public interface IRealmService
    {
        IDisposable OpenDB();

        void DeleteDB();

        void InsertRecords(int count);

        RealmResults<RealmItem> GetAllRecords();

        RealmResults<RealmItem> GetRecords(Func<RealmItem, bool> predicate);

        IList<RealmItem> GetAllRecordsList();

        IList<RealmItem> GetRecordsList(int count);

        void DeleteRecords(IList<RealmItem> items);

        void DeleteRangeRecords(RealmResults<RealmItem> items);
    }
}

