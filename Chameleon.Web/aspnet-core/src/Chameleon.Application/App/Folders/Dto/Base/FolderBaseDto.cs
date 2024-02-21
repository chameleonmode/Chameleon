using System.ComponentModel.DataAnnotations;

namespace Chameleon.App
{
    public class FolderBaseDto
    {
        [Required]
        public string Title { get; set; }
        public bool IsFavorite { get; set; }        
    }
}
