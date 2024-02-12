/*

 访问修饰符
 例如:
 public
 private
 protected
 internal

可以用来修饰的类型:
class/struct/enum/interface/delegate/record/event

*/

namespace CS2TS.Model.Words;

public class AccessModifierWordSegment : WordSegment
{
	/// <summary>
	///     可以用于哪种类型的CodeNode,比如public等可用于
	///     public: class/struct/enum/interface/delegate/record/event
	/// </summary>
	public List<Type> UseForCodeNodeTypes { get; set; } = new();
}