using EFDataAccessLib.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFDataAccessLib.Repos;
public class DatabaseTransactionOperation : IDatabaseTransactionOperation, IAsyncDisposable, IDisposable
{
    private readonly ProgDbContext dbContext;
    private IDbContextTransaction? dbContextTransaction = null;
    private bool IsCommitting = false;

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
                IsCommitting = true;
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
            IsCommitting = false;
            dbContextTransaction = null;
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
                IsCommitting = true;
            }
        }
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }

    public void End()
    {
        if (dbContextTransaction != null && IsCommitting)
        {
            Save();
            dbContextTransaction.Commit();
            IsCommitting = false;
            dbContextTransaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (dbContextTransaction != null)
        {
            if (IsCommitting)
                await dbContextTransaction.RollbackAsync();

            await dbContextTransaction.DisposeAsync();
            dbContextTransaction = null;
        }

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (dbContextTransaction != null)
        {
            if (IsCommitting)
                dbContextTransaction?.Rollback();

            dbContextTransaction?.Dispose();
            dbContextTransaction = null;
        }
        GC.SuppressFinalize(this);
    }

    public async Task RollBackAsync()
    {
        if (dbContextTransaction != null)
        {
            await dbContextTransaction.RollbackAsync();
            IsCommitting = false;
            dbContextTransaction = null;
        }
    }

    public void RollBack()
    {
        if (dbContextTransaction != null)
        {
            dbContextTransaction.Rollback();
            IsCommitting = false;
            dbContextTransaction = null;
        }
    }
}
