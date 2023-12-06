using Coravel.Invocable;
using Known.Helpers;

namespace Known.Shared;

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}