using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
            return _sqLiteAsyncConnection.CreateTableAsync<T>();
        }

        public Task<List<T>> GetAllAsync()
        {
            return _sqLiteAsyncConnection.Table<T>().ToListAsync();
        }

        public Task<T> GetById(long primaryKey)
        {
            return _sqLiteAsyncConnection.GetAsync<T>(primaryKey);
        }

        public Task<List<T>> Get(Expression<Func<T, bool>> predicate)
        {
            return _sqLiteAsyncConnection.Table<T>().Where(predicate).ToListAsync();
        }

        public Task<int> Add(T obj)
        {
            return _sqLiteAsyncConnection.InsertAsync(obj);
        }

        public Task<int> Add(List<T> objects)
        {
            return _sqLiteAsyncConnection.InsertAllAsync(objects);
        }

        public Task<int> Update(T obj)
        {
            return _sqLiteAsyncConnection.UpdateAsync(obj);
        }

        public Task<int> Delete(long primaryKey)
        {
            return _sqLiteAsyncConnection.DeleteAsync<T>(primaryKey);
        }

        public Task DeleteAll()
        {
            return _sqLiteAsyncConnection.DeleteAllAsync<T>();
        }

        public async Task ResetTableData(List<T> objects)
        {
            await DeleteAll();
            await ResetTableIndex();
            await Add(objects);
        }

        public Task<int> ResetTableIndex()
        {
            return _sqLiteAsyncConnection.ExecuteAsync($"delete from sqlite_sequence where name='{TableName()}'");
        }

        public Task<int> GetTableIndex()
        {
            return _sqLiteAsyncConnection.ExecuteScalarAsync<int>(
                $"Select seq from sqlite_sequence where name='{TableName()}'");
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
