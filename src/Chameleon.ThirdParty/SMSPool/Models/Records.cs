using Chameleon.Interfaces.ThirdParty;

namespace Chameleon.ThirdParty.SMSPool.Models;
public record class Country(string ID, string Name, string Short_name, string Region) : RCountry(Name);
public record Service(string ID, string Name, int Favourite) : RService(Name);
