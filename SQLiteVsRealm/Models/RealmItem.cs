using System;
using Realms;

namespace SQLiteVsRealm.Models
{
    public class RealmItem : RealmObject
    {
        public int Number { get; set; }
        public string Guid { get; set; }
        public string Info { get; set; }
    }
}

