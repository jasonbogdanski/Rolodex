using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Rolodex.DataStore;
using Rolodex.Models;

namespace Rolodex.IntegrationTests;

[CollectionDefinition(nameof(TestingFixture))]
public class TestingFixtureCollection : ICollectionFixture<TestingFixture> { }

public class TestingFixture
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WebApplicationFactory<Program> _factory;

    public TestingFixture()
    {
        _factory = new DynamicWorkflowEngineApplicationFactory();

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private class DynamicWorkflowEngineApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ConnectionStrings:DefaultConnection", ConnectionString }
                    });
                });
            });

            builder.ConfigureTestServices(services =>
            {
            });
        }

        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=Rolodex-Test;Trusted_Connection=True;MultipleActiveResultSets=True;";
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RolodexContext>();

        try
        {
            await dbContext.BeginTransactionAsync();

            await action(scope.ServiceProvider);

            await dbContext.CommitTransactionAsync();
        }
        catch (Exception)
        {
            dbContext.RollbackTransaction();
            throw;
        }
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RolodexContext>();

        try
        {
            await dbContext.BeginTransactionAsync();

            var result = await action(scope.ServiceProvider);

            await dbContext.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            dbContext.RollbackTransaction();
            throw;
        }
    }

    public Task ExecuteDbContextAsync(Func<RolodexContext, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<RolodexContext>()!));

    public Task ExecuteDbContextAsync(Func<RolodexContext, ValueTask> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<RolodexContext>()!).AsTask());

    public Task ExecuteDbContextAsync(Func<RolodexContext, IMediator, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<RolodexContext>()!, sp.GetService<IMediator>()!));

    public Task<T> ExecuteDbContextAsync<T>(Func<RolodexContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<RolodexContext>()!));

    public Task<T> ExecuteDbContextAsync<T>(Func<RolodexContext, ValueTask<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<RolodexContext>()!).AsTask());

    public Task<T> ExecuteDbContextAsync<T>(Func<RolodexContext, IMediator, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<RolodexContext>()!, sp.GetService<IMediator>()!));

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }
            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
        where TEntity : class
        where TEntity2 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);

            return db.SaveChangesAsync();
        });
    }

    public Task<T> FindAsync<T>(int id)
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask())!;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }
}