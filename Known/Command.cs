using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Known
{
    public class Command
    {
        private DbHelper dbHelper;

        public Command(DbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            Text = string.Empty;
            Parameters = new Dictionary<string, object>();
            Parameters.Clear();
        }

        public string Text { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public List<T> ToList<T>(Func<DataRow, T> func)
        {
            return ToTable().ToList<T>(func);
        }

        public T ToEntity<T>(Func<DataRow, T> func)
        {
            return ToRow().ToEntity<T>(func);
        }

        public PagedResult<T> ToPaged<T>(int pageSize, int pageIndex, Func<DataRow, T> func)
        {
            var result = dbHelper.ExecutePaged(Text, Parameters, pageSize, pageIndex);
            Parameters.Clear();
            var list = new List<T>();
            result.DataSource.ForEach(r => list.Add(r.ToEntity<T>(func)));
            return new PagedResult<T>(result.TotalCount, list);
        }

        public DataTable ToTable()
        {
            var table = dbHelper.ExecuteTable(Text, Parameters);
            Parameters.Clear();
            return table;
        }

        public DataRow ToRow()
        {
            var row = dbHelper.ExecuteRow(Text, Parameters);
            Parameters.Clear();
            return row;
        }

        public T ToScalar<T>()
        {
            var scalar = dbHelper.ExecuteScalar<T>(Text, Parameters);
            Parameters.Clear();
            return scalar;
        }

        public string Execute()
        {
            var message = dbHelper.Execute(Text, Parameters);
            Parameters.Clear();
            return message;
        }

        public void ExecuteOnSubmit()
        {
            dbHelper.ExecuteOnSubmit(Text, Parameters);
            Parameters.Clear();
        }
    }
}
