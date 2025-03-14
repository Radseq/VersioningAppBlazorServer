namespace EFDataAccessLib.Repos;
public interface IDatabaseTransactionOperation
{
    Task<bool> StartAsync();

    Task SaveAsync();

    Task EndAsync();

    Task RollBackAsync();

    void Start();

    void Save();

    void End();

    void RollBack();
}
