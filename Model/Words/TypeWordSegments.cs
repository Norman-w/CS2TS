/*


 类型名称的关键字,如class, struct, enum, namespace等等


*/


namespace CS2TS.Model.Words;

public class TypeWordSegments
{
	#region 类似于class, struct, enum, namespace等这样的关键字,这几种都叫做"类型关键字"

	public static readonly TypeWordSegment Class = new() { Content = "class" };
	public static readonly TypeWordSegment Struct = new() { Content = "struct" };
	public static readonly TypeWordSegment Enum = new() { Content = "enum" };
	public static readonly TypeWordSegment Namespace = new() { Content = "namespace" };
	public static readonly TypeWordSegment Interface = new() { Content = "interface" };
	public static readonly TypeWordSegment Delegate = new() { Content = "delegate" };
	public static readonly TypeWordSegment Record = new() { Content = "record" };
	public static readonly TypeWordSegment Event = new() { Content = "event" };
	public static readonly TypeWordSegment Var = new() { Content = "var" };

	#endregion
}