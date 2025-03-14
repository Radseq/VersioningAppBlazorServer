using EFDataAccessLib.DataAccess;

namespace EFDataAccessLib.Repos;
public class DatabaseOperation : IDatabaseOperation
{
    private readonly ProgDbContext dbContext;


    public DatabaseOperation(ProgDbContext _dbContext)
    {
        dbContext = _dbContext;
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }

    public async Task SaveAsync()
    {
        await dbContext.SaveChangesAsync();
    }

}
