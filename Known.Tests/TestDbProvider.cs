using System;
using System.Collections.Generic;
using System.Data;
using Known.Data;
using Known.Jobs;
using Known.Mapping;

namespace Known.Tests
{
    class TestDbProvider : IDbProvider
    {
        public string Name { get; }
        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void BeginTrans()
        {
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public void Execute(Command command)
        {
        }

        public object Scalar(Command command)
        {
            return "";
        }

        public DataTable Query(Command command)
        {
            return new DataTable();
        }

        public void WriteTable(DataTable table)
        {
        }

        public void Dispose()
        {
        }
    }

    class JobRepository : IJobRepository
    {
        public List<JobInfo> GetServerJobs(string server)
        {
            return new List<JobInfo>();
        }

        public void UpdateJob(JobInfo job)
        {
        }

        public void AddRecord(JobRecord record)
        {
        }
    }
}
