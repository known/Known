using System.Collections.Generic;
using Known.Data;

namespace Known.Tests.Demo
{
    static class ApplyFormRepository
    {
        public static PagingResult QueryApplyForms(this Database database, PagingCriteria criteria)
        {
            return new PagingResult(138, new List<ApplyForm>());
        }

        public static List<ApplyFormList> GetApplyFormLists(this Database database, string formId)
        {
            return new List<ApplyFormList>();
        }
    }
}
