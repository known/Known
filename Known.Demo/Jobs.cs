using Coravel.Invocable;
using Known.Helpers;

namespace Known.Demo;

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}