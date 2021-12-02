using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using DoubleA.DataAccess.Contract.Repositories;
using DoubleA.DataAccess.Contract.UnitOfWork;
using DoubleA.DataAccess.DataContexts;

namespace DoubleA.DataAccess.UnitOfWork
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected UnitOfWork(IDataContext dataContext, IEnumerable<IRepository> repositories)
        {
            DataContext = dataContext;
            UnResolvedRepositories = repositories.ToHashSet();
        }

        protected IDataContext DataContext { get; }

        protected HashSet<IRepository> UnResolvedRepositories { get; }

        protected Dictionary<Type, IRepository> ResolvedRepositories { get; } = new Dictionary<Type, IRepository>();

        public virtual TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        {
            var repositoryType = typeof(TRepository);

            if (!ResolvedRepositories.TryGetValue(repositoryType, out var repository))
            {
                repository = UnResolvedRepositories.Single(r => r is TRepository);
                ResolvedRepositories.Add(repositoryType, repository);
                UnResolvedRepositories.Remove(repository);
            }

            return (TRepository)repository;
        }

        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            if (DataContext.HasChanges)
            {
                await DataContext.SaveChangesAsync(cancellationToken);
            }
            scope.Complete();
        }
    }
}
