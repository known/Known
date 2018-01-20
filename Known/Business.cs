namespace Known
{
    public class Business
    {
        public Business(Context context)
        {
            Context = context;
        }

        public Context Context { get; }

        protected T LoadBusiness<T>() where T : Business
        {
            return BusinessFactory.Create<T>(Context);
        }
    }
}
