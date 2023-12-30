using CS2TS.Model;

namespace CS2TS;

/// <summary>
///     命名空间,里面可以包含命名空间,类,接口
/// </summary>
public class Namespace : CodeNode,
	IContainer4Namespace, IContainer4Class, IContainer4Interface, IContainer4Enum
{
	public Namespace(string name)
	{
		Name = name;
	}

	public List<Class> Classes => Chirldren.OfType<Class>().ToList();
	public List<EnumDefine> Enums => Chirldren.OfType<EnumDefine>().ToList();
	public List<Interface> Interfaces => Chirldren.OfType<Interface>().ToList();

	public string Name { get; set; }

	public List<Namespace> Namespaces => Chirldren.OfType<Namespace>().ToList();
}