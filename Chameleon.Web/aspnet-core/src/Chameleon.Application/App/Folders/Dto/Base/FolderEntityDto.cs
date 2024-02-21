using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class FolderEntityDto
        : FolderBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
        public ProfileInfoDto[] Profiles { get; set; }
        public int ProfilesCount => Profiles.Length;
    }
}
