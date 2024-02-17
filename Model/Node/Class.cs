using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

/// <summary>
///     类,内部可以包含 变量/字段/属性 方法,类   不能包含接口,命名空间
/// </summary>
public class Class : Interface,
	IContainer4DefineClass, IContainer4DefineVariable,
	IUseModifierCodeNode<Class>
{
	// public Class(string name, List<string> extends) : base(name, extends)
	// {
	// }

	public PermissionEnum Permission { get; set; }

	public List<Class> Classes => Children.OfType<Class>().ToList();
	public List<Variable> Variables => Children.OfType<Variable>().ToList();
}