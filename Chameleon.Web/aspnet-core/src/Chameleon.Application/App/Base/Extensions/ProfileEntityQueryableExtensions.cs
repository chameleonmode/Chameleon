using Abp.Domain.Entities.Auditing;
using Abp.Runtime.Session;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    public static class ProfileEntityQueryableExtensions
    {
        public static IQueryable<TEntity> FilterByMustHaveProfile<TEntity>(
            this IQueryable<TEntity> self, IMayHaveProfile input)
            where TEntity : IMustHaveProfile
        {
            if (input.ProfileId > 0)
            {
                self = self.Where(entity => entity.ProfileId == input.ProfileId);
            }
            return self;
        }

        public static IQueryable<TEntity> FilterByMayHaveProfile<TEntity>(
            this IQueryable<TEntity> self, IMayHaveProfile input)
            where TEntity : IMayHaveProfile
        {
            if (input.ProfileId > 0)
            {
                self = self.Where(entity => 
                    entity.ProfileId == input.ProfileId || 
                    entity.ProfileId == null
                    );
            }
            return self;
        }

        public static IQueryable<TEntity> FilterByCreatorUserId<TEntity>(
            this IQueryable<TEntity> self, long userId)
            where TEntity : ICreationAudited
        {
            return self.Where(entity => entity.CreatorUserId == userId);
        }

        public static IQueryable<TEntity> FilterByCreatorUserId<TEntity>(
            this IQueryable<TEntity> self, IAbpSession session)
            where TEntity : ICreationAudited
        {
            return self.FilterByCreatorUserId(session.GetUserId());
        }

        public static IQueryable<TEntity> FilterByUserId<TEntity>(
            this IQueryable<TEntity> self, long userId)
            where TEntity : IMustHaveUser
        {
            return self.Where(entity => entity.UserId == userId);
        }

        public static IQueryable<TEntity> FilterByUserId<TEntity>(
            this IQueryable<TEntity> self, IAbpSession session)
            where TEntity : IMustHaveUser
        {
            return self.FilterByUserId(session.GetUserId());
        }
    }
}
