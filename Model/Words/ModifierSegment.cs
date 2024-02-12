namespace CS2TS.Model.Words;

public class ModifierSegment : WordSegment
{
	/// <summary>
	///     可以用于哪种类型的CodeNode,比如public等可用于
	///     public: class/struct/enum/interface/delegate/record/event
	/// </summary>
	public List<Type> UseForCodeNodeTypes { get; set; } = new();
}