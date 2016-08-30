using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Realms;
using SQLite;
using SQLiteVsRealm.Models;
using SQLiteVsRealm.RealmService;
using SQLiteVsRealm.Services;
using SQLiteVsRealm.SQLiteService;
using Xamarin.Forms;

namespace SQLiteVsRealm.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T backingField, T Value, [CallerMemberName] string propertyName = null)
        {
            var changed = !EqualityComparer<T>.Default.Equals(backingField, Value);

            if (changed)
            {
                backingField = Value;
                RaisePropertyChanged(propertyName);
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class SQLiteVsRealmViewModel : BaseViewModel
    {
        private const string SQLiteDBName = "sqlite.db";
        private const string RealmDBName = "realm.db";

        private readonly ISQLiteService _sqliteService;
        private readonly IRealmService _realmService;

        private ICommand _sqliteInsertRecordsCommand;
        private ICommand _sqliteGetRecordsCommand;
        private ICommand _sqliteDeleteRecordsCommand;
        private ICommand _realmInsertRecordsCommand;
        private ICommand _realmGetRecordsCommand;
        private ICommand _realmDeleteRecordsCommand;
        private ICommand _realmDeleteRangeRecordsCommand;
        private ICommand _cleanLogCommand;

        private string _recordsCount;
        private string _outputLog;

        public SQLiteVsRealmViewModel()
        {
            var pathService = DependencyService.Get<IPathService>();
            _sqliteService = new SQLiteService.SQLiteService(pathService.GetDBPath(SQLiteDBName));
            _realmService = new RealmService.RealmService(pathService.GetDBPath(RealmDBName));

            RecordsCount = "100";
        }

        #region Properties

        public string RecordsCount
        {
            get { return _recordsCount; }
            set { SetProperty(ref _recordsCount, value); }
        }

        public string OutputLog
        {
            get { return _outputLog; }
            set { SetProperty(ref _outputLog, value); }
        }

        public ICommand SQLiteInsertRecordsCommand
        {
            get
            {
                return _sqliteInsertRecordsCommand ??
                    (_sqliteInsertRecordsCommand = new Command(async _ => await SQLiteInsertRecordsExecute()));
            }
        }

        public ICommand SQLiteGetRecordsCommand
        {
            get
            {
                return _sqliteGetRecordsCommand ??
                    (_sqliteGetRecordsCommand = new Command(async _ => await SQLiteGetRecordsExecute()));
            }
        }

        public ICommand SQLiteDeleteRecordsCommand
        {
            get
            {
                return _sqliteDeleteRecordsCommand ??
                    (_sqliteDeleteRecordsCommand = new Command(async _ => await SQLiteDeleteRecordsExecute()));
            }
        }

        public ICommand RealmInsertRecordsCommand
        {
            get
            {
                return _realmInsertRecordsCommand ??
                    (_realmInsertRecordsCommand = new Command(RealmInsertRecordsExecute));
            }
        }

        public ICommand RealmGetRecordsCommand
        {
            get
            {
                return _realmGetRecordsCommand ??
                    (_realmGetRecordsCommand = new Command(RealmGetRecordsExecute));
            }
        }

        public ICommand RealmDeleteRecordsCommand
        {
            get
            {
                return _realmDeleteRecordsCommand ??
                    (_realmDeleteRecordsCommand = new Command(RealmDeleteRecordsExecute));
            }
        }

        public ICommand RealmDeleteRangeRecordsCommand
        {
            get
            {
                return _realmDeleteRangeRecordsCommand ??
                    (_realmDeleteRangeRecordsCommand = new Command(RealmDeleteRecordsRangeExecute));
            }
        }
        public ICommand CleanLogCommand
        {
            get
            {
                return _cleanLogCommand ??
                    (_cleanLogCommand = new Command(CleanLogCommandExecute));
            }
        }

        #endregion

        public void Init()
        {
            _sqliteService.CreateTable();
            using (_realmService.OpenDB()) ;
        }

        private int ParseRecordsCount()
        {
            int recordsCount = 0;
            int.TryParse(RecordsCount, out recordsCount);
            return recordsCount;
        }

        private void Log(string info)
        {
            OutputLog += $"{info}\n";
        }

        private async Task SQLiteInsertRecordsExecute()
        {
            int recordsCount = ParseRecordsCount();
            if (recordsCount == 0)
            {
                Log("RecordsCount should be numerical and greater than 0");
                return;
            }
            System.GC.Collect(0, GCCollectionMode.Forced, true);

            var watch = Stopwatch.StartNew();
            await _sqliteService.InsertRecords(recordsCount);
            watch.Stop();

            Log($"SQLite Insert {RecordsCount}: {watch.ElapsedMilliseconds} ms");
        }

        private async Task SQLiteGetRecordsExecute()
        {
            System.GC.Collect(0, GCCollectionMode.Forced, true);

            var watch = Stopwatch.StartNew();
            var records = await _sqliteService.GetAllRecords();
            watch.Stop();

            Log($"SQLite Select {records.Count}: {watch.ElapsedMilliseconds} ms");
        }

        private async Task SQLiteDeleteRecordsExecute()
        {
            System.GC.Collect(0, GCCollectionMode.Forced, true);

            var records = await _sqliteService.GetAllRecords();

            var watch = Stopwatch.StartNew();
            await _sqliteService.DeleteRecords(records);
            watch.Stop();

            Log($"SQLite Delete {records.Count}: {watch.ElapsedMilliseconds} ms");
        }

        private void RealmInsertRecordsExecute()
        {
            int recordsCount = ParseRecordsCount();
            if (recordsCount == 0)
            {
                Log("RecordsCount should be numerical and greater than 0");
                return;
            }

            System.GC.Collect(0, GCCollectionMode.Forced, true);
            using (_realmService.OpenDB())
            {
                var watch = Stopwatch.StartNew();
                _realmService.InsertRecords(recordsCount);
                watch.Stop();
                Log($"Realm Insert {RecordsCount}: {watch.ElapsedMilliseconds} ms");
            }
        }

        private void RealmGetRecordsExecute()
        {
            System.GC.Collect(0, GCCollectionMode.Forced, true);
            using (_realmService.OpenDB())
            {
                var watch = Stopwatch.StartNew();
                var records = _realmService.GetAllRecordsList();
                watch.Stop();

                Log($"Realm Select {records.Count()}: {watch.ElapsedMilliseconds} ms");
            }
        }

        private void RealmDeleteRecordsExecute()
        {
            int recordsCount = ParseRecordsCount();
            if (recordsCount == 0)
            {
                Log("RecordsCount should be numerical and greater than 0");
                return;
            }

            System.GC.Collect(0, GCCollectionMode.Forced, true);
            using (_realmService.OpenDB())
            {
                var records = _realmService.GetAllRecordsList();
                recordsCount = records.Count;

                var watch = Stopwatch.StartNew();
                _realmService.DeleteRecords(records);
                watch.Stop();

                Log($"Realm Delete {recordsCount}: {watch.ElapsedMilliseconds} ms");
            }
        }

        private void RealmDeleteRecordsRangeExecute()
        {
            System.GC.Collect(0, GCCollectionMode.Forced, true);
            using (_realmService.OpenDB())
            {
                var records = _realmService.GetAllRecords();
                var recordsCount = records.Count();

                var watch = Stopwatch.StartNew();
                _realmService.DeleteRangeRecords(records);
                watch.Stop();

                Log($"Realm Delete {recordsCount}: {watch.ElapsedMilliseconds} ms");
            }
        }

        private void CleanLogCommandExecute()
        {
            OutputLog = string.Empty;
        }
    }
}

