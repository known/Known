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

        protected Database Database { get; }

        public T QueryById<T>(string id) where T : EntityBase
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            return Database.QueryById<T>(id);
        }

        public List<T> QueryList<T>() where T : EntityBase
        {
            return Database.QueryList<T>();
        }

        public List<T> QueryListById<T>(string[] ids) where T : EntityBase
        {
            if (ids == null || ids.Length == 0)
                return null;

            return Database.QueryListById<T>(ids);
        }

        public void Save<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            Database.Save(entity);
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            Database.Delete(entity);
        }

        public Result Transaction(Action<IRepository> action, object data = null)
        {
            var db = new Database(Database.Name);
            var rep = new DbRepository(db);

            try
            {
                rep.Database.BeginTrans();
                action(rep);
                rep.Database.Commit();
                return Result.Success("提交成功！", data);
            }
            catch (Exception ex)
            {
                rep.Database.Rollback();
                throw ex;
            }
            finally
            {
                rep.Database.Dispose();
            }
        }
    }
}
