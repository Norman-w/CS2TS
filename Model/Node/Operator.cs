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

	/// <summary>
	///     返回类型,如果是implicit或者explicit,那么这个属性就是目标类型,也就是()前面的类型如:
	///     public static implicit operator Dollar(Rmb rmb)中的Dollar
	///     否则就是返回类型,也就是 operator前面的类型如:
	///     public static Dollar operator +(Dollar dollar, Rmb rmb)中的Dollar
	/// </summary>
	public string ReturnType { get; set; } = string.Empty;

	/// <summary>
	///     操作符名称,如果是implicit或者explicit,那么这个属性就是null
	///     因为implicit和explicit的操作符可以理解为目标类型的构造函数
	///     如不是implicit或者explicit,那么这个属性就是操作符的名称比如"+"
	/// </summary>
	public string? OperatorName { get; set; }
}