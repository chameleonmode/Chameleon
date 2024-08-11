using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.ThirdParty.SMSPool.Models;

public class SuccessfullOrder
{
    public int success { get; set; }
    public int number { get; set; }
    public string cc { get; set; }
    public string phonenumber { get; set; }
    public string order_id { get; set; }
    public string country { get; set; }
    public string service { get; set; }
    public int pool { get; set; }
    public int expires_in { get; set; }
    public int expiration { get; set; }
    public string message { get; set; }
    public string cost { get; set; }
    public int cost_in_cents { get; set; }
}

public class OUT_OF_STOCK
{
    public string message { get; set; }
    public int success { get; set; }
    public Pools pools { get; set; }
    public Error1[] errors { get; set; }
    public string type { get; set; }
}

public class Pools
{
    public Foxtrot Foxtrot { get; set; }
}

public class Foxtrot
{
    public int success { get; set; }
    public string message { get; set; }
    public Error[] errors { get; set; }
    public string type { get; set; }
}

public class Error
{
    public string message { get; set; }
}

public class Error1
{
    public string message { get; set; }
}

