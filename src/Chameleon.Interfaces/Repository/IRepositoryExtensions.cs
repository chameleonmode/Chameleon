using Chameleon.Interfaces.Entities;
using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Repository
{
    public static class IRepositoryExtensions
    {
        public static void InsertOrUpdate<TEntity, TPrimaryKey, TGetAllRequest>(
            this IRepository<TEntity, TPrimaryKey, TGetAllRequest> self, TEntity entity)
            where TGetAllRequest : GetAllRequestDto
            where TEntity : IEntity<TPrimaryKey>
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (Equals(entity.Id, default(TPrimaryKey)))
            {
                self.Insert(entity);
            }
            else
            {
                self.Update(entity);
            }
        }

        public static void InsertOrUpdate<TEntity, TPrimaryKey, TGetAllRequest>(
            this IRepository<TEntity, TPrimaryKey, TGetAllRequest> self, IList<TEntity> entities)
            where TGetAllRequest : GetAllRequestDto
            where TEntity : IEntity<TPrimaryKey>
        {
            if (entities == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var entity in entities)
            {
                self.InsertOrUpdate(entity);
            }
        }

        public static void Insert<TEntity, TPrimaryKey, TGetAllRequest>(
            this IRepository<TEntity, TPrimaryKey, TGetAllRequest> self, IList<TEntity> entities)
            where TGetAllRequest : GetAllRequestDto
            where TEntity : IEntity<TPrimaryKey>
        {
            if (entities == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var entity in entities)
            {
                self.Insert(entity);
            }
        }

        public static void Delete<TEntity, TPrimaryKey, TGetAllRequest>(
            this IRepository<TEntity, TPrimaryKey, TGetAllRequest> self, TEntity entity)
            where TGetAllRequest : GetAllRequestDto
            where TEntity : IEntity<TPrimaryKey>
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            self.Delete(entity.Id);
        }
    }
}
