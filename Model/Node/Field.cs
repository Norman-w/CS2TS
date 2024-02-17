using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

public class Field : CodeNode
{
	/// <summary>
	///     字段并不会像class那样的定义的时候写一下 public field之类的类型修饰符,所以这里不需要这个属性,返回null.
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;
}