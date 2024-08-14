using Chameleon.Interfaces.ThirdParty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.ThirdParty.SMSapi.SMSPVA.Models;
public record class Service(int ID, string Logo, string Name, string Code) : RService(Name);
public record class Country(int ID, string Name, string Code) : RCountry(Name);