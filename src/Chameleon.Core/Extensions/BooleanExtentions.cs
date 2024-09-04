using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Core.Extensions;
public static class BooleanExtentions
{
    public static string ToLowerStrring(this bool value)
    {
        return value.ToString().ToLower();
    }
}
