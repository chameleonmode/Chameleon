using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserDefaultSettingsDto
        : UserDefaultSettingsBaseDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
