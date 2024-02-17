namespace CS2TS.Model.Node;

public class Var : CodeNode
{
	/// <summary>
	///     代码节点的类型的Segment表示,因为并不会像 public class那样要写一个"class"的关键字,所以这里返回null
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;
}