using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Polly;
using SQLite.Net.Async;

namespace CallOfService.Mobile.Database
{
    public class DbSet<T> : IDbSet<T> where T : class, new()
    {
        private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

        public DbSet(ISqLiteNet connection)
        {
            _sqLiteAsyncConnection = connection.GetConnection();
        }

        public Task CreateTable()
        {
            var result = Policy
                  .Handle<Exception>()
                  .WaitAndRetryAsync
                  (
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                    onRetryAsync: (exception, span, retryCount, context) => retryCount == 2 ? _sqLiteAsyncConnection.DropTableAsync<T>() : Task.Delay(1)
                  )
                  .ExecuteAsync(() => _sqLiteAsyncConnection.CreateTableAsync<T>());

            return result;
        }

        public Task<List<T>> GetAllAsync()
        {
            var result = Policy
                 .Handle<Exception>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 3,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
                 )
                 .ExecuteAsync(() => _sqLiteAsyncConnection.Table<T>().ToListAsync());

            return result;
        }

        public Task<T> GetById(long primaryKey)
        {
            var result = Policy
                 .Handle<Exception>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 3,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
                 )
                 .ExecuteAsync(() => _sqLiteAsyncConnection.GetAsync<T>(primaryKey));

            return result;
        }

        public Task<List<T>> Get(Expression<Func<T, bool>> predicate)
        {
            var result = Policy
                 .Handle<Exception>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 3,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
                 )
                 .ExecuteAsync(() => _sqLiteAsyncConnection.Table<T>().Where(predicate).ToListAsync());

            return result;
        }

        public Task<int> Add(T obj)
        {
            var result = Policy
                 .Handle<Exception>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 3,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
                 )
                 .ExecuteAsync(() => _sqLiteAsyncConnection.InsertAsync(obj));

            return result;
        }

        public Task<int> Add(List<T> objects)
        {
            var result = Policy
                 .Handle<Exception>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 3,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
                 )
                 .ExecuteAsync(() => _sqLiteAsyncConnection.InsertAllAsync(objects));

            return result;
        }

        public Task<int> Update(T obj)
        {
            var result = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync
                (
                  retryCount: 3,
                  sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
                )
                .ExecuteAsync(() => _sqLiteAsyncConnection.UpdateAsync(obj));

            return result;
        }

        public Task<int> Delete(long primaryKey)
        {
            var result = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync
               (
                 retryCount: 3,
                 sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
               )
               .ExecuteAsync(() => _sqLiteAsyncConnection.DeleteAsync<T>(primaryKey));

            return result;
        }

        public Task DeleteAll()
        {
            var result = Policy
              .Handle<Exception>()
              .WaitAndRetryAsync
              (
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                onRetryAsync: (exception, span, retryCount, context) => retryCount == 2 ? CreateTable() : Task.Delay(1)
              )
              .ExecuteAsync(() => _sqLiteAsyncConnection.DeleteAllAsync<T>());

            return result;
        }

        public async Task ResetTableData(List<T> objects)
        {
            await DeleteAll();
            await ResetTableIndex();
            await Add(objects);
        }

        public Task<int> ResetTableIndex()
        {
            var result = Policy
              .Handle<Exception>()
              .WaitAndRetryAsync
              (
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
              )
              .ExecuteAsync(() => _sqLiteAsyncConnection.ExecuteAsync($"delete from sqlite_sequence where name='{TableName()}'"));

            return result;
        }

        public Task<int> GetTableIndex()
        {
            var result = Policy
              .Handle<Exception>()
              .WaitAndRetryAsync
              (
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1)
              )
              .ExecuteAsync(() => _sqLiteAsyncConnection.ExecuteScalarAsync<int>($"Select seq from sqlite_sequence where name='{TableName()}'"));

            return result;
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
