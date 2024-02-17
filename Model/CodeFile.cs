using CS2TS;
using CS2TS.Model;
using CS2TS.Model.Node;

public static class Define
{
	/// <summary>
	///     权限的集合,确定有没有权限定义找这里
	/// </summary>
	public static List<string> Permissions = new() { "public", "private", "protected", "internal" };
}

public class CodeFile : ContainerCodeNode,
	IContainer4DefineNamespace,
	IContainer4DefineClass,
	IContainer4DefineEnum,
	IContainer4DefineFunction,
	IContainer4DefineUsing
{
	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;

	public override List<Type> SonCodeNodeValidTypes => new()
	{
		typeof(Namespace),
		typeof(Class),
		typeof(EnumDefine),
		typeof(Function),
		typeof(Using)
	};

	public List<Class> Classes => Children.OfType<Class>().ToList();
	public List<EnumDefine> Enums => Children.OfType<EnumDefine>().ToList();
	public List<Function> Functions => Children.OfType<Function>().ToList();

	public string Name { get; set; }

	public List<Namespace> Namespaces => Children.OfType<Namespace>().ToList();

	public List<Using> Usings => Children.OfType<Using>().ToList();
}