using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS;

/// <summary>
///     类,内部可以包含 变量/字段/属性 方法,类   不能包含接口,命名空间
/// </summary>
public class Class : Interface,
	IUseStaticModifierCodeNode,
	IUseAccessModifierCodeNode,
	IContainer4DefineClass, IContainer4DefineVariable
{
	public new List<ModifierSegment> AvailableModifiers => ModifierSegments.GetAvailableModifiers<Class>();

	public List<Class> Classes => Children.OfType<Class>().ToList();

	public List<Variable> Variables => Children.OfType<Variable>().ToList();
	// public Class(string name, List<string> extends) : base(name, extends)
	// {
	// }

	/// <summary>
	///     访问权限,默认为public
	/// </summary>
	public new AccessModifierPermissionEnum AccessPermission { get; set; } = AccessModifierPermissionEnum.Public;

	public bool IsStatic { get; set; }
}