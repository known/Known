using Coravel.Invocable;

namespace Sample.Core.TaskJobs;

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.Execute();
}