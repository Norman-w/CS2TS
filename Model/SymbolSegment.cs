namespace CS2TS.Model;

/*
 *
 *
 * 语义单元和CodeNode之间的关系
 *
 *
 */

public class SymbolSegment : Segment
{
	/// <summary>
	///     可以作为什么类型CodeNode的体终止符号
	///     大括回: class/struct/enum/interface/delegate/record/event/property/field/method/constructor/operator/getter/setter等
	/// </summary>
	/// <returns></returns>
	public List<Type> CanFinishCodeNodeBodyTypes { get; set; } = new();

	/// <summary>
	///     可以作为什么类型CodeNode的体起始符号
	///     大括号:
	///     namespace/enum/class/struct/interface/delegate/record/event/property/field/method/constructor/operator/getter/setter等
	/// </summary>
	public List<Type> CanStartCodeNodeBodyTypes { get; set; } = new();

	/// <summary>
	///     可以作为什么类型CodeNode的头终止符号
	///     分号;
	///     using/namespace/method/property/field/enum/var等
	/// </summary>
	public List<Type> CanFinishCodeNodeHeaderTypes { get; set; } = new();
}