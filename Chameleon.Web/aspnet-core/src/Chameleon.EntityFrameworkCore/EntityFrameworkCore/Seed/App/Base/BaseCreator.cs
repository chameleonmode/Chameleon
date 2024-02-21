using System;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Chameleon.EntityFrameworkCore.Seed.Host.App
{
    public abstract class BaseCreator
    {
        protected readonly ILogger Log;
        protected readonly ChameleonDbContext Context;
        protected BaseCreator(ChameleonDbContext context)
        {
            Context = context;
            Log = IocManager.Instance.IocContainer.Resolve<ILogger>();
        }

        public virtual void Create()
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public abstract void Run();

        protected virtual void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
