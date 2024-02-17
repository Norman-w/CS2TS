using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS;

public class Delegate : CodeNode
{
	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment CodeNodeTypeSegment => CodeNodeTypeSegments.Delegate;
}