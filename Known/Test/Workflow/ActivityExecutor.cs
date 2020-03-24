using System;

namespace Known.Workflow
{
    public abstract class ActivityExecutor
    {
        public ActivityExecutor(ActivityContext context)
        {
            Context = context;
        }

        public ActivityContext Context { get; }

        public static ActivityResult Execute(ActivityContext context)
        {
            ActivityExecutor executor = null;
            switch (context.Action)
            {
                case Action.Pass:
                    executor = new PassActivityExecutor(context);
                    break;
                case Action.Return:
                    executor = new ReturnActivityExecutor(context);
                    break;
                case Action.Revoke:
                    executor = new RevokeActivityExecutor(context);
                    break;
                case Action.Stop:
                    executor = new StopActivityExecutor(context);
                    break;
            }

            if (executor == null)
                throw new Exception($"执行操作不支持！");

            return executor.Execute();
        }

        public abstract ActivityResult Execute();

        protected bool CheckContext(ActivityContext context)
        {
            if (context == null)
                return false;
            if (context.Flow == null)
                return false;
            if (context.Flow.NextActivity == null)
                return false;

            return true;
        }
    }

    class PassActivityExecutor : ActivityExecutor
    {
        public PassActivityExecutor(ActivityContext context) : base(context)
        {
        }

        public override ActivityResult Execute()
        {
            throw new NotImplementedException();
        }
    }

    class ReturnActivityExecutor : ActivityExecutor
    {
        public ReturnActivityExecutor(ActivityContext context) : base(context)
        {
        }

        public override ActivityResult Execute()
        {
            throw new NotImplementedException();
        }
    }

    class RevokeActivityExecutor : ActivityExecutor
    {
        public RevokeActivityExecutor(ActivityContext context) : base(context)
        {
        }

        public override ActivityResult Execute()
        {
            throw new NotImplementedException();
        }
    }

    class StopActivityExecutor : ActivityExecutor
    {
        public StopActivityExecutor(ActivityContext context) : base(context)
        {
        }

        public override ActivityResult Execute()
        {
            throw new NotImplementedException();
        }
    }
}
