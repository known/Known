using Coravel.Invocable;
using Known.Helpers;

namespace Known.Web;

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}