using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

/// <summary>
///     命名空间,里面可以包含命名空间,类,接口
/// </summary>
public class Namespace : CodeNode,
	IContainer4DefineNamespace, IContainer4DefineClass, IContainer4DefineInterface, IContainer4DefineEnum
{
	public Namespace(string name)
	{
		Name = name;
	}

	public List<Class> Classes => Children.OfType<Class>().ToList();
	public List<EnumDefine> Enums => Children.OfType<EnumDefine>().ToList();
	public List<Interface> Interfaces => Children.OfType<Interface>().ToList();

	public string Name { get; set; }

	public List<Namespace> Namespaces => Children.OfType<Namespace>().ToList();
}