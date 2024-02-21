using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Transactions;

namespace Chameleon.EntityFrameworkCore.Seed
{
    public class SeedHelperBase<TAbpDbContext>
        where TAbpDbContext : AbpDbContext
    {
        public static ILogger<TAbpDbContext> Logger { get; private set; }

        public static void EnsureLogger(IIocResolver iocResolver)
        {
            if (Logger == null)
            {
                Logger = iocResolver.Resolve<ILogger<TAbpDbContext>>();
            }
        }

        public static void Migrate(IIocResolver iocResolver)
        {
            Migrate<TAbpDbContext>(iocResolver);
        }

        public static void WithDbContext<TDbContext>(
            IIocResolver iocResolver,
            Action<TDbContext> contextAction,
            TransactionScopeOption scope = TransactionScopeOption.Suppress)
            where TDbContext : DbContext
        {
            EnsureLogger(iocResolver);
            try
            {
                using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
                {
                    using (var uow = uowManager.Object.Begin(scope))
                    {
                        var context = uowManager.Object.Current
                            .GetDbContext<TDbContext>(MultiTenancySides.Host);
                        contextAction(context);
                        uow.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Message);
                throw;
            }
        }

        public static void WithDbContext<TDbContext>(
            IIocResolver iocResolver,
            Action<TDbContext> contextAction,
            UnitOfWorkOptions options)
            where TDbContext : DbContext
        {
            EnsureLogger(iocResolver);
            try
            {
                using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
                {
                    using (var uow = uowManager.Object.Begin(options))
                    {
                        var context = uowManager.Object.Current
                            .GetDbContext<TDbContext>(MultiTenancySides.Host);
                        contextAction(context);
                        uow.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Message);
                throw;
            }
        }

        public static void Migrate<TDbContext>(IIocResolver iocResolver)
            where TDbContext : DbContext
        {
            WithDbContext<TDbContext>(iocResolver,
                context => context.Database.Migrate(),
                new UnitOfWorkOptions
                {
                    IsTransactional = false
                });
        }
    }
}
