using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS;

public class Interface :
	ContainerCodeNode,
	INamedCodeNode
{
	/// <summary>
	///     继承的接口(若是类,则包含最多一个父类和0+个接口)
	/// </summary>
	public List<string> Extends { get; set; } = new();

	public List<Function> Functions => Children.OfType<Function>().ToList();
	public List<Interface> Interfaces => Children.OfType<Interface>().ToList();
	public List<Variable> Variables => Children.OfType<Variable>().ToList();

	public override Segment CodeNodeTypeSegment => CodeNodeTypeSegments.Interface;

	/// <summary>
	///     直接在这下面可容纳的子节点的类型
	/// </summary>
	public override List<Type> SonCodeNodeValidTypes => new()
	{
		typeof(Interface),
		typeof(Function),
		typeof(Field),
		typeof(Property)
	};

	public AccessModifierPermissionEnum AccessPermission
	{
		get => AccessModifierPermissionEnum.Public;
		set => throw new NotImplementedException("接口不支持设置访问权限,默认为public");
	}

	public List<ModifierSegment> AvailableModifiers => ModifierSegments.GetAvailableModifiers<Interface>();

	/// <summary>
	///     接口的名称,若是类,则为类的名称
	/// </summary>
	public string Name { get; set; } = string.Empty;

	public List<Interface> GetInterfaces()
	{
		return GetNodes<Interface>();
	}
}