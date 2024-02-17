using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

public class Method : ContainerCodeNode
{
	/// <summary>
	///     函数并不会像class那样的定义的时候写一下 public method之类的类型修饰符,所以这里不需要这个属性,返回null.
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;

	/// <summary>
	///     TODO, 函数体内部可以放什么类型的CodeNode
	/// </summary>
	public override List<Type> SonCodeNodeValidTypes => new()
	{
		typeof(Method),
		typeof(Variable)
	};
}