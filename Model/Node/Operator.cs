using CS2TS.Model.Words;

namespace CS2TS.Model.Node;

public class Operator : CodeNode
{
	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment CodeNodeTypeSegment => CodeNodeTypeSegments.Operator;

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

	/// <summary>
	///     一定不是空的,因为操作符的参数至少有一个
	///     第一个参数的类型,如果是implicit或者explicit,那么这个属性就是唯一的参数类型(源类型),如:
	///     public static implicit operator Dollar(Rmb rmb)中的Rmb
	/// </summary>
	public string FirstParameterType { get; set; } = string.Empty;

	/// <summary>
	///     一定不是空的,因为操作符的参数至少有一个
	///     第一个参数的名称,如果是implicit或者explicit,那么这个属性就是唯一的参数名称(源类型),如:
	///     public static implicit operator Dollar(Rmb rmb)中的rmb
	/// </summary>
	public string FirstParameterName { get; set; } = string.Empty;

	/// <summary>
	///     第二个参数的类型,如果是implicit或者explicit,那么这个属性就是null,因为implicit和explicit的操作符只有一个参数也就是源类型
	///     否则就是第二个参数的类型,如:
	///     public static Dollar operator +(Dollar dollar, Rmb rmb)中的Rmb
	/// </summary>
	public string? SecondParameterType { get; set; }

	/// <summary>
	///     第二个参数的名称,如果是implicit或者explicit,那么这个属性就是null,因为implicit和explicit的操作符只有一个参数也就是源类型
	///     否则就是第二个参数的名称,如:
	///     public static Dollar operator +(Dollar dollar, Rmb rmb)中的rmb
	/// </summary>
	public string? SecondParameterName { get; set; }

	/// <summary>
	///     中间内容的起始符号,结构类似于
	///     public static Dollar operator +(Dollar dollar, int rmb)
	///     {
	///     return new Dollar { Value = dollar.Value + rmb / Constant.DollarToRmb };
	///     }
	///     中的{
	/// </summary>
	/// <returns></returns>
	public SymbolSegment BodyStartSegment => SymbolSegments.BracesStartSymbol;

	/// <summary>
	///     中间内容的结束符号,结构类似于
	///     public static Dollar operator +(Dollar dollar, int rmb)
	///     {
	///     return new Dollar { Value = dollar.Value + rmb / Constant.DollarToRmb };
	///     }
	///     中的}
	/// </summary>
	public SymbolSegment BodyEndSegment => SymbolSegments.BracesEndSymbol;
}