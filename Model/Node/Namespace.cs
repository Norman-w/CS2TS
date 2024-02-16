using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

/// <summary>
///     命名空间,里面可以包含命名空间,类,接口
/// </summary>
public class Namespace : ContainerCodeNode,
	IContainer4DefineNamespace, IContainer4DefineClass, IContainer4DefineInterface, IContainer4DefineEnum
{
	public Namespace(string name)
	{
		Name = name;
	}

	/// <summary>
	///     在命名空间下可以写的儿子CodeNode的类型
	///     包含了Class,Interface,Enum等,不能直接包含function等
	/// </summary>
	public override List<Type> SonCodeNodeValidTypes => new()
	{
		typeof(Namespace),
		typeof(Class),
		typeof(Interface),
		typeof(EnumDefine)
	};

	public List<Class> Classes => Children.OfType<Class>().ToList();
	public List<EnumDefine> Enums => Children.OfType<EnumDefine>().ToList();
	public List<Interface> Interfaces => Children.OfType<Interface>().ToList();

	public string Name { get; set; }

	public List<Namespace> Namespaces => Children.OfType<Namespace>().ToList();
}