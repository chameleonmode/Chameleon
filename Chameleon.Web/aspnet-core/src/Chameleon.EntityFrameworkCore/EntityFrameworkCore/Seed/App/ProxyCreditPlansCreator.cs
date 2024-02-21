using System.Linq;
using Chameleon.App.Entities;

namespace Chameleon.EntityFrameworkCore.Seed.Host.App
{
    public class ProxyCreditPlansCreator : ApplicationBaseCreator
    {
        private readonly ProxyCreditPlan[] _plans = new []
        {
            new ProxyCreditPlan
            {
                Title = "Basic",
                Amount = 19
            },
            new ProxyCreditPlan
            {
                Title = "Pro",
                Amount = 29
            },
            new ProxyCreditPlan
            {
                Title = "Business",
                Amount = 49
            },
        };

        public ProxyCreditPlansCreator(ChameleonDbContext context)
            : base(context)
        {
        }

        public override void Run()
        {
            if (Context.WebBrowserUserAgents.Any())
            {
                return;
            }

            foreach (var item in _plans)
            {
                Create(item);
            }
            SaveChanges();
        }

        private void Create(ProxyCreditPlan entity)
        {
            var table = Context.ProxyCreditPlans;
            if (table.Any(e => e.Title == entity.Title))
            {
                return;
            }
            
            table.Add(entity);
        }
    }
}
