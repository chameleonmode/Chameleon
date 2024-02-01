using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.Api
{
    public interface IApiLayer<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        void Delete(TPrimaryKey id);
        TEntityDto Get(TPrimaryKey id);
        TEntityDto Insert(TCreateInput dto);
        void Update(TUpdateInput dto);
        TEntityDto[] GetAll(object query = null);
    }

    public interface IApiLayer<TEntityDto, TPrimaryKey, TOperableInput>
        : IApiLayer<TEntityDto, TPrimaryKey, TOperableInput, TOperableInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TOperableInput : IEntityDto<TPrimaryKey>
    { }

    public interface IApiLayer<TEntityDto, TPrimaryKey>
        : IApiLayer<TEntityDto, TPrimaryKey, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    { }

    public interface IApiLayer<TEntityDto>
       : IApiLayer<TEntityDto, int>
       where TEntityDto : IEntityDto<int>
    { }
}
