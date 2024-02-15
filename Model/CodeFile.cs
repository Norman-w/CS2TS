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

public class CodeFile : CodeNode,
	IContainer4DefineNamespace,
	IContainer4DefineClass,
	IContainer4DefineEnum,
	IContainer4DefineFunction,
	IContainer4DefineUsing
{
	public List<Class> Classes => Chirldren.OfType<Class>().ToList();
	public List<EnumDefine> Enums => Chirldren.OfType<EnumDefine>().ToList();
	public List<Function> Functions => Chirldren.OfType<Function>().ToList();

	public string Name { get; set; }

	public List<Namespace> Namespaces => Chirldren.OfType<Namespace>().ToList();
	public List<Using> Usings => Chirldren.OfType<Using>().ToList();
}