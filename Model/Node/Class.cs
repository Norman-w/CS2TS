using CS2TS.Model;

namespace CS2TS;

/// <summary>
///     类,内部可以包含 变量/字段/属性 方法,类   不能包含接口,命名空间
/// </summary>
public class Class : Interface,
	IContainer4DefineClass, IContainer4DefineVariable
{
	public Class(string name, List<string> extends) : base(name, extends)
	{
	}

	public PermissionEnum Permission { get; set; }

	public List<string> Extends { get; set; }

	public List<Class> Classes => Chirldren.OfType<Class>().ToList();
	public string Name { get; set; }
	public List<Variable> Variables => Chirldren.OfType<Variable>().ToList();
}