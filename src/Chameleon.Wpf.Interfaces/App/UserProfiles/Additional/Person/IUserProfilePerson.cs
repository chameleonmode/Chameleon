using System;

namespace Chameleon.Interfaces.UserProfiles.Additional
{
    public interface IUserProfilePerson 
        : IUserProfileRequiredEntity
    {
		string Title { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string MiddleName { get; set; }
		string JobTitle { get; set; }
		string PhoneNumber { get; set; }
		string Email { get; set; }
		DateTime BirthDate { get; set; }
		string BirthPlace { get; set; }
		string Notes { get; set; }
        GenderType Gender { get; set; }
	}
}
