using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using Chameleon.EntityFrameworkCore.Seed.Host;
using Chameleon.EntityFrameworkCore.Seed.Tenants;
using Chameleon.EntityFrameworkCore.Seed.Host.App;
using System.Linq;

namespace Chameleon.EntityFrameworkCore.Seed
{
    public class SeedHelper : SeedHelperBase<ChameleonDbContext>
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<ChameleonDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(ChameleonDbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new InitialHostDbBuilder(context).Create();

            // Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();

            var tenantIds = context.Tenants
                .Select(t => t.Id)
                .ToList();
            
            foreach(var tenantId in tenantIds)
            {
                new TenantRoleAndUserBuilder(context, tenantId).Create();
            }

            // Custom seed
            new InitialAppDbBuilder(context).Create();
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}
