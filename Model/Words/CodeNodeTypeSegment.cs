namespace CS2TS.Model.Words;

public class CodeNodeTypeSegment : WordSegment
{
	/// <summary>
	///     代码节点类型.比如比如当前content为class, 那么CodeNodeType就是typeof(ClassNode).
	/// </summary>
	public Type CodeNodeType { get; init; }
}