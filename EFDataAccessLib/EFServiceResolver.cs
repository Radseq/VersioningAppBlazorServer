using EFDataAccessLib.DataAccess;
using EFDataAccessLib.Repos;
using EFDataAccessLib.Repos.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EFDataAccessLib;

public static class EFServiceResolver
{
    public static IServiceCollection AddDataBase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProgDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddTransient<IDatabaseTransactionOperation, DatabaseTransactionOperation>();
        services.AddTransient<IDatabaseOperation, DatabaseOperation>();

        services.AddTransient<IRepoApplication, RepoApplication>();
        services.AddTransient<IRepoAppVersion, RepoAppVersion>();
        services.AddTransient<IRepoAppCompatibility, RepoAppCompatibility>();


        services.AddTransient<IRawSql, RawSql>();


        return services;
    }
}
