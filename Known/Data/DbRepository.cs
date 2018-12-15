using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    public class DbRepository : IRepository
    {
        public DbRepository(Database database)
        {
            Database = database;
        }

        public Database Database { get; }

        public T QueryById<T>(string id) where T : BaseEntity
        {
            if (string.IsNullOrWhiteSpace(id))
                return default(T);

            return Database.QueryById<T>(id);
        }

        public List<T> QueryList<T>() where T : BaseEntity
        {
            return Database.QueryList<T>();
        }

        public List<T> QueryListById<T>(string[] ids) where T : BaseEntity
        {
            if (ids == null || ids.Length == 0)
                return null;

            return Database.QueryListById<T>(ids);
        }

        public void Save<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
                return;

            Database.Save(entity);
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
                return;

            Database.Delete(entity);
        }

        public Result Transaction(Action<IRepository> action)
        {
            var db = new Database(Database.Name);
            var rep = new DbRepository(db);

            try
            {
                rep.Database.Provider.BeginTrans();
                action(rep);
                rep.Database.Provider.Commit();
                return Result.Success("");
            }
            catch (Exception ex)
            {
                rep.Database.Provider.Rollback();
                throw ex;
            }
            finally
            {
                rep.Database.Provider.Dispose();
            }
        }
    }
}
