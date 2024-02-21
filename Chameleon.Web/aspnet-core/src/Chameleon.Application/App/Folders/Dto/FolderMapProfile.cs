using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class FolderMapProfile : AutoMapper.Profile
    {
        public FolderMapProfile()
        {
            CreateMap<Folder, FolderDto>()
                .ForMember(dto => dto.Id, options => options.MapFrom(model => model.Id))
                .ForMember(dto => dto.Title, options => options.MapFrom(model => model.Title))
                .ForMember(dto => dto.CreatorUserId, options => options.MapFrom(model => model.CreatorUserId))
                .ForMember(dto => dto.Profiles, options => options.MapFrom(model => model.Profiles))
                .ForMember(dto => dto.IsFavorite, options => options.MapFrom(model => model.IsFavorite))
                ;

            CreateMap<CreateFolderDto, Folder>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.IsFavorite, options => options.MapFrom(dto => dto.IsFavorite))
                ;

            CreateMap<UpdateFolderDto, Folder>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.IsFavorite, options => options.MapFrom(dto => dto.IsFavorite))
                ;
        }

        private AutoMapper.IMappingExpression<TDto, Folder> CreateBaseDtoMap<TDto>()
            where TDto : FolderBaseDto
        {
            return CreateMap<TDto, Folder>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.IsFavorite, options => options.MapFrom(dto => dto.IsFavorite))
                ;
        }
    }
}
