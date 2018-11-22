﻿using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    public class DbRepository : IRepository
    {
        public Database Database { get; internal set; }

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
            var rep = new DbRepository { Database = Database };

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
