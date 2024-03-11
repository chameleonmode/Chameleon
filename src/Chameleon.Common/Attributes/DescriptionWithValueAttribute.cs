using System.ComponentModel;

namespace Chameleon.Common.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class DescriptionWithValueAttribute : DescriptionAttribute
{
    public DescriptionWithValueAttribute(string description, string value) : base(description)
    {
        Value = value;
    }

    public string Value { get; private set; }
}
