namespace CS2TS.Model.Node;

public class Operator : CodeNode
{
	/// <summary>
	///     是否是隐式转换
	///     固定搭配: public static implicit operator 目标类型(源类型 变量名)
	/// </summary>
	public bool IsImplicit { get; set; }

	/// <summary>
	///     是否是显式转换
	///     固定搭配: public static explicit operator 目标类型(源类型 变量名)
	/// </summary>
	public bool IsExplicit { get; set; }

	/// <summary>
	///     是否是一元操作符 //TODO, 这个属性的意义是什么?
	/// </summary>
	public bool IsUnary { get; set; }

	/// <summary>
	///     是否是重载
	/// </summary>
	public bool IsOverride { get; set; }

	/// <summary>
	///     是否是静态的,如果是implicit或者explicit,那么这个属性就一定是true
	/// </summary>
	public bool IsStatic { get; set; }
}