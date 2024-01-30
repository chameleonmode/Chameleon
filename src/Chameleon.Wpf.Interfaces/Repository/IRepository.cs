using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Repository
{
    public interface IRepository<TEntity, TPrimaryKey, TGetAllRequest>
        : ISingletonDependency
        where TGetAllRequest: GetAllRequestDto
    {
        TEntity[] GetAll(TGetAllRequest request = null);
        TEntity[] GetAll(bool ignoreCache, TGetAllRequest request = null);
        TEntity Get(TPrimaryKey id);
        TPrimaryKey Insert(TEntity entity);
        void Update(TEntity entity);
        void Upsert(UpsertItems<TEntity> entities);
        void Delete(TPrimaryKey id);
    }

    public interface IRepository<TEntity>
        : IRepository<TEntity, int, GetAllRequestDto>
    {
    }
}
