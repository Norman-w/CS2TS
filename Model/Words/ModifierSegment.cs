using CS2TS.Model.Node;

namespace CS2TS.Model.Words;

public class ModifierSegment : WordSegment
{
	private List<Type> _useForCodeNodeTypes = new();

	/// <summary>
	///     可以用于哪种类型的CodeNode,比如public等可用于
	///     public: class/struct/enum/interface/delegate/record/event
	/// </summary>
	public List<Type> UseForCodeNodeTypes
	{
		get => _useForCodeNodeTypes;
		set
		{
			//检查type是否是CodeNode的子类
			foreach (var type in value.Where(type => !type.IsSubclassOf(typeof(CodeNode))))
				throw new Exception(
					$"类型{type}不是CodeNode的子类,所以不能被修饰符{Content}修饰");
			_useForCodeNodeTypes = value;
		}
	}

	/// <summary>
	///     使用于哪个名称的字段,比如static可用于IsStatic
	/// </summary>
	public string UseForFieldOfCodeNode { get; set; } = string.Empty;
}