using Chameleon.Interfaces.ThirdParty;

namespace Chameleon.ThirdParty.Codesverify.Models;
public record AppData(string Name, string Price) : RService(Name);
public record class Country(string Name) : RCountry(Name);
