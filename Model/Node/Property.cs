using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

public class Property : ContainerCodeNode,
	INamedCodeNode
{
	/// <summary>
	///     属性并不会像class那样的定义的时候写一下 public property之类的类型修饰符,所以这里不需要这个属性,返回null.
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;

	/// <summary>
	///     TODO, 属性的内部是不是跟函数体一样?内部可以放什么类型的CodeNode?
	/// </summary>
	public override List<Type> SonCodeNodeValidTypes => new()
	{
		typeof(Method),
		typeof(Variable)
	};

	/// <summary>
	///     属性的名字
	/// </summary>
	public string Name { get; set; } = null!;
}