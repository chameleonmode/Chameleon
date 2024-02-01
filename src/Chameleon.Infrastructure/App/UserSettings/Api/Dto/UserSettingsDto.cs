using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserSettingsDto
        : UserSettingsBaseDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
