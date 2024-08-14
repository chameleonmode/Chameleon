using Chameleon.Interfaces.ThirdParty;

namespace Chameleon.ThirdParty.SMSapi.Codesverify.Models;
public record class Country(string Name) : RCountry(Name);
public record AppData(string Name, string Price) : RService(Name);
