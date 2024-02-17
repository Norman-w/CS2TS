/*


 类型名称的关键字,如class, struct, enum, namespace等等


*/


using CS2TS.Model.Node;

namespace CS2TS.Model.Words;

public class CodeNodeTypeSegments
{
	#region 对外提供的对象 StaticSegments

	private static List<CodeNodeTypeSegment>? _all;

	/// <summary>
	///     所有的静态字段,也就是所有的Segment
	/// </summary>
	/// <exception cref="Exception"></exception>
	public static List<CodeNodeTypeSegment> All
	{
		get
		{
			//如果已经初始化过了,那么直接返回
			if (_all != null) return _all;
			_all = Segments.GetAllStaticSegments<CodeNodeTypeSegment, CodeNodeTypeSegments>();
			return _all;
		}
	}

	#endregion

	#region 类似于class, struct, enum, namespace等这样的关键字,这几种都叫做"类型关键字"

	public static readonly CodeNodeTypeSegment Class = new() { Content = "class", CodeNodeType = typeof(Class) };
	public static readonly CodeNodeTypeSegment Struct = new() { Content = "struct", CodeNodeType = typeof(Struct) };
	public static readonly CodeNodeTypeSegment Enum = new() { Content = "enum", CodeNodeType = typeof(EnumDefine) };

	public static readonly CodeNodeTypeSegment Namespace = new()
		{ Content = "namespace", CodeNodeType = typeof(Namespace) };

	public static readonly CodeNodeTypeSegment Interface = new()
		{ Content = "interface", CodeNodeType = typeof(Interface) };

	public static readonly CodeNodeTypeSegment Delegate = new()
		{ Content = "delegate", CodeNodeType = typeof(Delegate) };

	public static readonly CodeNodeTypeSegment Record = new() { Content = "record", CodeNodeType = typeof(Record) };
	public static readonly CodeNodeTypeSegment Event = new() { Content = "event", CodeNodeType = typeof(Event) };
	public static readonly CodeNodeTypeSegment Var = new() { Content = "var", CodeNodeType = typeof(Var) };

	public static readonly CodeNodeTypeSegment Operator = new()
		{ Content = "operator", CodeNodeType = typeof(Operator) };

	public static readonly CodeNodeTypeSegment Using = new() { Content = "using", CodeNodeType = typeof(Using) };

	#endregion
}