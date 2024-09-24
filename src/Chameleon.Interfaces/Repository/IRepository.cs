using Chameleon.Interfaces.Entities;
using Chameleon.Interfaces.Ioc;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace Chameleon.Interfaces.Repository
{
    public interface IRepository<TEntity, TPrimaryKey, TGetAllRequest>
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
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
    public class Abstraction<TEntity> : Abstraction<TEntity, int, GetAllRequestDto>, IRepository<TEntity> 
    {
    }

    public class Abstraction<TEntity, TPrimaryKey, TGetAllRequest> : IRepository<TEntity, TPrimaryKey, TGetAllRequest> where TGetAllRequest : GetAllRequestDto
    {
        public TEntity[] GetAll(TGetAllRequest request = default)
        {
            return typeof(TEntity).GetMethod("GetAll").Invoke(this, new object[] { request }) as TEntity[];
        }

        public TEntity[] GetAll(bool ignoreCache, TGetAllRequest request = default)
        {
            return typeof(TEntity).GetMethod("GetAll").Invoke(this, new object[] { ignoreCache, request }) as TEntity[];
        }

        public TEntity Get(TPrimaryKey id)
        {
            return (TEntity)typeof(TEntity).GetMethod("Get").Invoke(this, new object[] { id });
        }

        public TPrimaryKey Insert(TEntity entity)
        {
            return (TPrimaryKey)typeof(TEntity).GetMethod("Insert").Invoke(this, new object[] { entity });
        }

        public void Update(TEntity entity)
        {
            typeof(TEntity).GetMethod("Update").Invoke(this, new object[] { entity });
        }

        public void Upsert(UpsertItems<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }
    }

    //<,,,,,,>
    public class StaticFactory
    {
        public static IRepository<T1,T2,T3> Create<T1, T2, T3>() where T3 : GetAllRequestDto
        {
            return new Abstraction<T1, T2, T3>();
        }
        public static IRepository<T1> CreateOne<T1>()
        {
            return new Abstraction<T1>();
        }
    }
}
