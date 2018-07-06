namespace Known
{
    public interface IController
    {
        string UserName { get; }
        bool IsAuthenticated { get; }
    }
}
