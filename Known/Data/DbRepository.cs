using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    public class DbRepository : IRepository
    {
        public DbRepository()
        {
            Database = new Database();
        }

        protected Database Database { get; }

        public T QueryById<T>(string id) where T : BaseEntity
        {
            return Database.QueryById<T>(id);
        }

        public List<T> QueryList<T>() where T : BaseEntity
        {
            return Database.QueryList<T>();
        }

        public void Save<T>(T entity) where T : BaseEntity
        {
            Database.Save(entity);
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            Database.Delete(entity);
        }

        public Result Transaction(Action<IRepository> action)
        {
            return Result.Success("");
        }
    }
}
