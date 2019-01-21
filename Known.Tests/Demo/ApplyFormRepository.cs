using System.Collections.Generic;
using Known.Data;

namespace Known.Tests.Demo
{
    public interface IApplyFormRepository : IRepository
    {
        PagingResult QueryApplyForms(PagingCriteria criteria);
        List<ApplyFormList> GetApplyFormLists(string formId);
    }

    class ApplyFormRepository : TestRepository, IApplyFormRepository
    {
        public ApplyFormRepository(Database database) : base(database)
        {
        }

        public PagingResult QueryApplyForms(PagingCriteria criteria)
        {
            return new PagingResult(138, new List<ApplyForm>());
        }

        public List<ApplyFormList> GetApplyFormLists(string formId)
        {
            return new List<ApplyFormList>();
        }
    }
}
