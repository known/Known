using Known.Data;
using Known.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Known.SLite
{
    public class Entity<T> : Entity where T : Entity<T>, new()
    {
        public void Save()
        {
            var id = Id;
            var entityName = typeof(T).Name;
            var entityValue = this.ToJson();
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString().Replace("-", "");
            }
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select count(*) from kweb_Entities where EntityId=?EntityId";
            command.Parameters.Add("EntityId", id);
            var count = command.ToScalar<int>();
            var sql = count > 0
                ? "update kweb_Entities set EntityValue=?EntityValue where EntityId=?EntityId and EntityName=?EntityName"
                : "insert into kweb_Entities(EntityId,EntityName,EntityValue) values(?EntityId,?EntityName,?EntityValue)";
            command.Text = sql;
            command.Parameters.Add("EntityId", id);
            command.Parameters.Add("EntityName", entityName);
            command.Parameters.Add("EntityValue", entityValue);
            command.Execute();
        }
    }

    public class Entity
    {
        public string Id { get; set; }
        public DateTime InsertTime { get; set; }

        public virtual List<string> Validate()
        {
            return new List<string>();
        }

        public static List<T> FindAll<T>() where T : Entity
        {
            var typeName = typeof(T).Name;
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from kweb_Entities where EntityName=?EntityName";
            command.Parameters.Add("EntityName", typeName);
            return command.ToList(r =>
            {
                var entity = r.Get<string>("EntityValue").FromJson<T>();
                entity.Id = r.Get<string>("EntityId");
                entity.InsertTime = r.Get<DateTime>("InsertTime");
                return entity;
            });
        }

        public static T FindById<T>(string id) where T : Entity
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select EntityValue,InsertTime from kweb_Entities where EntityId=?EntityId";
            command.Parameters.Add("EntityId", id);
            return command.ToEntity(r =>
            {
                var entity = r.Get<string>("EntityValue").FromJson<T>();
                entity.Id = id;
                entity.InsertTime = r.Get<DateTime>("InsertTime");
                return entity;
            });
        }

        public static List<T> FindByField<T>(string field) where T : Entity
        {
            var typeName = typeof(T).Name;
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from kweb_Entities where EntityName=?EntityName and charindex(?EntityValue,EntityValue)>0";
            command.Parameters.Add("EntityName", typeName);
            command.Parameters.Add("EntityValue", field);
            return command.ToList(r =>
            {
                var entity = r.Get<string>("EntityValue").FromJson<T>();
                entity.Id = r.Get<string>("EntityId");
                entity.InsertTime = r.Get<DateTime>("InsertTime");
                return entity;
            });
        }

        public static void Delete(string id)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "delete from kweb_Entities where EntityId=?EntityId";
            command.Parameters.Add("EntityId", id);
            command.Execute();
        }
    }
}
