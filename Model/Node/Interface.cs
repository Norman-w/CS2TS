using CS2TS.Model;

namespace CS2TS;

public class Interface : VariableWithStructure,
	IContainer4DefineInterface, IContainer4DefineFunction, IContainer4DefineVariable
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

	public List<Function> Functions => Children.OfType<Function>().ToList();
	public List<Interface> Interfaces => Children.OfType<Interface>().ToList();
	public List<Variable> Variables => Children.OfType<Variable>().ToList();

	public List<Interface> GetInterfaces()
	{
		return GetNodes<Interface>();
	}
}