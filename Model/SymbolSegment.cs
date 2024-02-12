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
	///     可以作为什么类型CodeNode的终止符号
	///     大括回: class/struct/enum/interface/delegate/record/event/property/field/method/constructor/operator/getter/setter等
	/// </summary>
	/// <returns></returns>
	public List<Type> CanFinishCodeNodeTypes { get; set; } = new();
}