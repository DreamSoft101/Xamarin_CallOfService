using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CallOfService.Mobile.Core;
using SQLite.Net.Async;

namespace CallOfService.Mobile.Database
{
    public class DbSet<T> : IDbSet<T> where T : class, new()
    {
        private static readonly AsyncLock Mutex = new AsyncLock();
        private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

        public DbSet(ISqLiteNet connection)
        {
            _sqLiteAsyncConnection = connection.GetConnection();
        }

        public async Task CreateTable()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                await _sqLiteAsyncConnection.CreateTableAsync<T>();
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.Table<T>().ToListAsync();
            }
        }

        public async Task<T> GetById(long primaryKey)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.GetAsync<T>(primaryKey);
            }
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> predicate)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.Table<T>().Where(predicate).ToListAsync();
            }
        }

        public async Task<int> Add(T obj)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.InsertAsync(obj);
            }
        }

        public async Task<int> Add(List<T> objects)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.InsertAllAsync(objects);
            }
        }

        public async Task<int> Update(T obj)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.UpdateAsync(obj);
            }
        }

        public async Task<int> Delete(long primaryKey)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.DeleteAsync<T>(primaryKey);
            }
        }

        public async Task DeleteAll()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                await _sqLiteAsyncConnection.DeleteAllAsync<T>();
            }
        }

        public async Task ResetTableData(List<T> objects)
        {
            await DeleteAll();
            await ResetTableIndex();
            await Add(objects);
        }

        public async Task<int> ResetTableIndex()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
               return  await _sqLiteAsyncConnection.ExecuteAsync($"delete from sqlite_sequence where name='{TableName()}'");
            }
        }

        public async Task<int> GetTableIndex()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                return await _sqLiteAsyncConnection.ExecuteScalarAsync<int>($"Select seq from sqlite_sequence where name='{TableName()}'");
            }
        }

        public string TableName()
        {
            return typeof(T).Name;
        }
    }

    public interface ISqLiteNet
    {
        SQLiteAsyncConnection GetConnection();
        void DeleteDatabase();
    }

    public interface IDbSet<T>
    {
        Task CreateTable();
        Task<List<T>> GetAllAsync();
        Task<T> GetById(long primaryKey);
        Task<List<T>> Get(Expression<Func<T, bool>> predicate);
        Task<int> Add(T obj);
        Task<int> Add(List<T> objects);
        Task<int> Update(T obj);
        Task<int> Delete(long primaryKey);
        Task<int> ResetTableIndex();
        Task<int> GetTableIndex();
        Task DeleteAll();
        Task ResetTableData(List<T> objects);
        string TableName();
    }
}
