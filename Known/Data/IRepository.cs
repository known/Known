using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    public interface IRepository
    {
        T QueryById<T>(string id) where T : EntityBase;
        List<T> QueryList<T>() where T : EntityBase;
        List<T> QueryListById<T>(string[] ids) where T : EntityBase;
        void Save<T>(T entity) where T : EntityBase;
        void Update<T>(T entity) where T : EntityBase;
        void Delete<T>(T entity) where T : EntityBase;
        Result Transaction(Action<IRepository> action, object data = null);
    }
}
