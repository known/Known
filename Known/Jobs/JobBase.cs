namespace Known.Jobs
{
    public abstract class JobBase
    {
        protected JobResult Result(bool success, string message)
        {
            return new JobResult { IsSuccess = success, Message = message };
        }

        protected JobResult Result(bool success, string format, params object[] args)
        {
            var message = string.Format(format, args);
            return new JobResult { IsSuccess = success, Message = message };
        }
    }
}
