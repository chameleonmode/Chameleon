using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.ThirdParty;
public interface IPVAInstance
{
    string Name { get; }
    string ApiKey { get; set; }
    List<RCountry> Countries { get; }
    List<RService> Services { get; }
    Task Init();
    Task Save();
    Task<Tuple<string, string>> GetNumberAsync(RCountry country, RService app);
    Task<Tuple<string, string>> GetCodeAsync(RCountry country, RService app, string numberData);
    Task<Tuple<string, string>> CancelOrderAsync(string orderId);
}
