/*


 类型名称的关键字,如class, struct, enum, namespace等等


*/


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

	public static readonly CodeNodeTypeSegment Class = new() { Content = "class" };
	public static readonly CodeNodeTypeSegment Struct = new() { Content = "struct" };
	public static readonly CodeNodeTypeSegment Enum = new() { Content = "enum" };
	public static readonly CodeNodeTypeSegment Namespace = new() { Content = "namespace" };
	public static readonly CodeNodeTypeSegment Interface = new() { Content = "interface" };
	public static readonly CodeNodeTypeSegment Delegate = new() { Content = "delegate" };
	public static readonly CodeNodeTypeSegment Record = new() { Content = "record" };
	public static readonly CodeNodeTypeSegment Event = new() { Content = "event" };
	public static readonly CodeNodeTypeSegment Var = new() { Content = "var" };

	#endregion
}