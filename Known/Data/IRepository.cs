using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    public interface IRepository
    {
        T QueryById<T>(string id) where T : BaseEntity;
        List<T> QueryList<T>() where T : BaseEntity;
        void Save<T>(T entity) where T : BaseEntity;
        void Delete<T>(T entity) where T : BaseEntity;
        Result Transaction(Action<IRepository> action);
    }
}
