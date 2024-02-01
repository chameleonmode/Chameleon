using Chameleon.Interfaces.Entities;

namespace Chameleon.Infrastructure.Dto
{
    public class EntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }

    public class EntityDto : EntityDto<int>
    {
    }
}
