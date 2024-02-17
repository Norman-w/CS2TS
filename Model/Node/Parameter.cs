using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

/// <summary>
///     参数,用于表示函数的入参 出参
/// </summary>
public class Parameter : CodeNode
{
	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;

	// public int Index { get; set; }
	public string Name { get; set; }
	public TypeDefine Type { get; set; }
	public bool IsRef { get; set; }
	public bool IsOut { get; set; }

  /// <summary>
  ///     是否可以不传入或者传入null. 也就是在参数类型后面加入问号
  /// </summary>
  public bool Nullable { get; set; }

  /// <summary>
  ///     参数(入参)默认值,也就是在参数类型后面加入等号和默认值
  /// </summary>
  public string? DefaultValue { get; set; }
}