namespace SpeedRegistry.Data
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Build(bool shareConnection = false);
    }
}
