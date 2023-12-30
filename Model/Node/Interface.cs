using CS2TS.Model;

namespace CS2TS;

public class Interface : VariableWithStructure,
	IContainer4Interface, IContainer4Function, IContainer4Variable
{
	// public List<Function> GetFunctions()
	// {
	// return GetNodes<Function>();
	// }

	// public List<Variable> GetVariables()
	// {
	// return GetNodes<Variable>();
	// }

	public Interface(string name,
		List<string> extends) : base(name, null, PermissionEnum.Public, null, null, null, null, null, extends, null,
		null)
	{
	}

	public List<Function> Functions => Chirldren.OfType<Function>().ToList();
	public List<Interface> Interfaces => Chirldren.OfType<Interface>().ToList();
	public List<Variable> Variables => Chirldren.OfType<Variable>().ToList();

	public List<Interface> GetInterfaces()
	{
		return GetNodes<Interface>();
	}
}