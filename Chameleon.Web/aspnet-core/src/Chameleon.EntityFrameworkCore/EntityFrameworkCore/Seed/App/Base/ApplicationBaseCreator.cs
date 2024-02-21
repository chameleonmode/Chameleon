namespace Chameleon.EntityFrameworkCore.Seed.Host.App
{
    public abstract class ApplicationBaseCreator : BaseCreator
    {
        protected ApplicationBaseCreator(ChameleonDbContext context) 
            : base(context)
        {
        }
    }
}
