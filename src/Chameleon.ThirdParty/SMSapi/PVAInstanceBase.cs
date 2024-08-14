using Chameleon.Interfaces.ThirdParty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chameleon.ThirdParty.SMSapi;
public abstract class PVAInstanceBase : IPVAInstance
{
    public readonly JsonSerializerOptions JSOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public string Name { get; }
    public string ApiKey { get; set; }

    public List<RCountry> Countries { get; set; }
    public List<RService> Services { get; set; }

    protected PVAInstanceBase(string name)
    {
        Name = name;
    }

    public abstract Task Init();
    public abstract Task Save();
    public abstract Task<Tuple<string, string>> GetNumberAsync(RCountry country, RService app);
    public abstract Task<Tuple<string, string>> GetCodeAsync(RCountry country, RService app, string numberData);
    public abstract Task<Tuple<string, string>> CancelOrderAsync(string orderId);
}
