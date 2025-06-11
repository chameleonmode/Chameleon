namespace Chameleon.client.MvvM;
[AttributeUsage(AttributeTargets.Class)]
public class ViewModelAttribute(Type type) : Attribute {
	public Type Type { get; private set; } = type;
}