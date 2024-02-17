using CS2TS.Model;
using CS2TS.Model.Words;

namespace CS2TS;

/// <summary>
///     对命名空间的引用,看做是using类型的 xxx变量.这个变量作为全局变量可以被使用/调用
/// </summary>
public class Using : Variable
{
	public Using(string name)
		: base(name, new TypeDefine { Name = "using" }, null, null, null, null, null, name)
	{
	}

	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment CodeNodeTypeSegment => CodeNodeTypeSegments.Using;
}