using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS;

/// <summary>
///     命名空间,里面可以包含命名空间,类,接口
/// </summary>
public class Namespace :
	ContainerCodeNode,
	IContainer4DefineNamespace,
	IContainer4DefineClass,
	IContainer4DefineInterface,
	IContainer4DefineEnum,
	INameChainCodeNode
{
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

	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment CodeNodeTypeSegment => CodeNodeTypeSegments.Namespace;

	public List<Class> Classes => Children.OfType<Class>().ToList();
	public List<EnumDefine> Enums => Children.OfType<EnumDefine>().ToList();
	public List<Interface> Interfaces => Children.OfType<Interface>().ToList();
	public List<Namespace> Namespaces => Children.OfType<Namespace>().ToList();

	/// <summary>
	///     命名空间的完整名字
	/// </summary>
	public string Name
	{
		get => NameChain.ToString();
		// set => NameChain = NameChain.FromString(value);
		[Obsolete("不支持设置Name,请设置NameChain")]
		//设置的时候还还是设置NameChain吧,不然可能会有一些问题,所以这里的set调用抛出异常
		set => throw new NotImplementedException("不支持设置Name,请设置NameChain");
	}

	/// <summary>
	///     命名空间的名字的链, 比如命名空间的名字是A.B.C, 那么这个链就是A->B->C
	/// </summary>
	public NameChain NameChain { get; set; } = new();
}