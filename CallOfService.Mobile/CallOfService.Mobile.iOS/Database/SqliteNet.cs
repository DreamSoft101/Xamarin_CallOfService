using System;
using System.IO;
using CallOfService.Mobile.Database;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinIOS;

namespace CallOfService.Mobile.iOS.Database
{
    public class SqliteNet :  ISqLiteNet
    {
        private static SQLiteAsyncConnection _connection;
        private static readonly object ConnectionLocker = new object();
        private static readonly object ConnectionWithLockLocker = new object();
        private static SQLiteConnectionWithLock _connectionWithLock;


        public SQLiteAsyncConnection GetConnection()
        {
            lock (ConnectionLocker)
            {
                if (_connection == null)
                {
                    _connection = new SQLiteAsyncConnection(GetSqLiteConnectionWithLock);
                }
                return _connection;
            }
        }

        private SQLiteConnectionWithLock GetSqLiteConnectionWithLock()
        {
            lock (ConnectionWithLockLocker)
            {
                if (_connectionWithLock == null)
                {
                    _connectionWithLock = new SQLiteConnectionWithLock(GetSqLitePlatform(), GetSqliteConnectionString());
                }
                return _connectionWithLock;
            }
        }

        public SQLiteConnectionString GetSqliteConnectionString()
        {
            return new SQLiteConnectionString(GetDbFileLocation(), false);
        }

        public string GetDbFileLocation()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CallOfService.Mobile.db3");
        }

        public void DeleteDatabase()
        {
            if (File.Exists(GetDbFileLocation()))
            {
                File.Delete(GetDbFileLocation());
                _connectionWithLock = null;
            }
        }

        public ISQLitePlatform GetSqLitePlatform()
        {
            return new SQLitePlatformIOS();
        }
    }
}
