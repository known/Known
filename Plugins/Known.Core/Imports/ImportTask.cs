namespace Known.Imports;

[Task(ImportHelper.BizType)]
class ImportTask : TaskBase
{
    public override Task<Result> ExecuteAsync(Database db, TaskInfo task)
    {
        return ImportHelper.ExecuteAsync(Context, db, task);
    }
}