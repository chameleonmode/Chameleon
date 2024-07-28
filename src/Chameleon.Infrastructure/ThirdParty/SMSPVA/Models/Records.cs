using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.ThirdParty.SMSPVA.Models;
public record class Service(int ID, string Logo, string Name, string Code);
public record class Country(int ID, string Name, string Code);
