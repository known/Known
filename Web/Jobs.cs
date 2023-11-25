using Coravel.Invocable;
using Known.Helpers;

namespace Web;

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}