using System.ComponentModel.DataAnnotations;

namespace Chameleon.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}