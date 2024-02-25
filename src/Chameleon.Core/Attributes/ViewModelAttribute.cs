namespace Chameleon.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ViewModelAttribute
    : Attribute
{
    public Type Type { get; }

    public ViewModelAttribute(Type type)
    {
        Type = type;
    }
}
