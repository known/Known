namespace Known.Jobs
{
    public abstract class JobBase
    {
        protected ExecuteResult Result(bool success, string message)
        {
            return new ExecuteResult { IsSuccess = success, Message = message };
        }

        protected ExecuteResult Result(bool success, string format, params object[] args)
        {
            var message = string.Format(format, args);
            return Result(success, message);
        }
    }
}
