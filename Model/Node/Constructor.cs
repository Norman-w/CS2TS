using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

public class Constructor : ContainerCodeNode
{
	/// <summary>
	///     构造函数不像其他的CodeNode,它没有CodeNode类型名称,所以这里返回null
	///     比如 public class 这里的class就是本参数对于类的值,而构造函数没有这个值
	/// </summary>
	/// <returns></returns>
	public override Segment? CodeNodeTypeSegment => null;

	public override List<Type> SonCodeNodeValidTypes => new()
	{
		//TODO
	};
}