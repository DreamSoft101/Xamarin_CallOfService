using System;
using CallOfService.Technician.Mobile.Database;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;

namespace CallOfService.Technician.Mobile.Droid
{
	public class SqliteNet : ISqLiteNet
	{
		public SqliteNet()
		{
		}

		private static SQLiteAsyncConnection _connection;
		private static readonly object ConnectionLocker = new object();
		private static readonly object ConnectionWithLockLocker = new object();
		private static SQLiteConnectionWithLock _connectionWithLock;

		public void DeleteDatabase()
		{
			throw new NotImplementedException();
		}

		public global::SQLite.Net.Async.SQLiteAsyncConnection GetConnection()
		{
			throw new NotImplementedException();
		}
	}
}

