namespace EFDataAccessLib.Repos;
public interface IDatabaseOperation
{
    Task SaveAsync();
    void Save();
}

