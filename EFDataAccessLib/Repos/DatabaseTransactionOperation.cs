using EFDataAccessLib.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFDataAccessLib.Repos;
public class DatabaseTransactionOperation : IDatabaseTransactionOperation, IAsyncDisposable, IDisposable
{
    private readonly ProgDbContext dbContext;
    private IDbContextTransaction? dbContextTransaction = null;
    private bool IsCommiting = false;

    public DatabaseTransactionOperation(ProgDbContext _dbContext)
    {
        dbContext = _dbContext;
    }

    public async Task<bool> StartAsync()
    {
        if (dbContextTransaction == null)
        {
            // Check if a transaction is already in progress
            if (dbContext.Database.CurrentTransaction != null)
            {
                return false;
            }
            else
            {
                dbContextTransaction = await dbContext.Database.BeginTransactionAsync();
                IsCommiting = true;
                return true;
            }
        }
        return false;
    }

    public async Task SaveAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task EndAsync()
    {
        if (dbContextTransaction != null)
        {
            await SaveAsync();
            await dbContextTransaction.CommitAsync();
            IsCommiting = false;
        }
    }

    public void Start()
    {
        if (dbContextTransaction == null)
        {
            // Check if a transaction is already in progress
            if (dbContext.Database.CurrentTransaction != null)
            {

            }
            else
            {
                dbContextTransaction = dbContext.Database.BeginTransaction();
                IsCommiting = true;
            }
        }
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }

    public void End()
    {
        if (dbContextTransaction != null && IsCommiting)
        {
            Save();
            dbContextTransaction.Commit();
            IsCommiting = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (dbContextTransaction != null)
        {
            if (IsCommiting)
                await dbContextTransaction.RollbackAsync();

            await dbContextTransaction.DisposeAsync();
        }

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (dbContextTransaction != null)
        {
            if (IsCommiting)
                dbContextTransaction?.Rollback();

            dbContextTransaction?.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    public async Task RollBackAsync()
    {
        if (dbContextTransaction != null)
        {
            await dbContextTransaction.RollbackAsync();
            IsCommiting = false;
        }
    }

    public void RollBack()
    {
        if (dbContextTransaction != null)
        {
            dbContextTransaction.Rollback();
            IsCommiting = false;
        }
    }
}
