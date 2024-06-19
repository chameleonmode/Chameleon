namespace Chameleon.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ViewModelAttribute
    : Attribute
{
    public Type Type { get; private set; }

    public ViewModelAttribute(Type type)
    {
        Type = type;
    }
}
