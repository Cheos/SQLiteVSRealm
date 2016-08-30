using System;
using SQLite;

namespace SQLiteVsRealm.Models
{
    [Table("SQLiteItems")]
    public class SQLiteItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Number { get; set; }
        public string Guid { get; set; }
        public string Info { get; set; }
    }
}

