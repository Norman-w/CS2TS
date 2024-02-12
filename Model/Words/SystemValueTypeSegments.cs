/*



 一些系统内置类型的关键字的片段

 //TODO 这个还需要整理一下,有些类型定义以外的也放进来了



*/


namespace CS2TS.Model.Words;

public class SystemValueTypeSegments
{
	/// <summary>
	///     系统内置的string类型
	/// </summary>
	public static SystemValueTypeSegment String = new()
	{
		Content = "string"
	};

	/// <summary>
	///     系统内置的int类型
	/// </summary>
	public static SystemValueTypeSegment Int = new()
	{
		Content = "int"
	};

	/// <summary>
	///     系统内置的long类型
	/// </summary>
	public static SystemValueTypeSegment Long = new()
	{
		Content = "long"
	};

	public static SystemValueTypeSegment Uint = new()
	{
		Content = "uint"
	};

	public static SystemValueTypeSegment Ulong = new()
	{
		Content = "ulong"
	};

	public static SystemValueTypeSegment Short = new()
	{
		Content = "short"
	};

	public static SystemValueTypeSegment Ushort = new()
	{
		Content = "ushort"
	};

	public static SystemValueTypeSegment Byte = new()
	{
		Content = "byte"
	};

	public static SystemValueTypeSegment Sbyte = new()
	{
		Content = "sbyte"
	};

	public static SystemValueTypeSegment Char = new()
	{
		Content = "char"
	};

	public static SystemValueTypeSegment Float = new()
	{
		Content = "float"
	};

	public static SystemValueTypeSegment Double = new()
	{
		Content = "double"
	};

	public static SystemValueTypeSegment Decimal = new()
	{
		Content = "decimal"
	};

	public static SystemValueTypeSegment Bool = new()
	{
		Content = "bool"
	};

	public static SystemValueTypeSegment Object = new()
	{
		Content = "object"
	};

	public static SystemValueTypeSegment Void = new()
	{
		Content = "void"
	};

	public static SystemValueTypeSegment Dynamic = new()
	{
		Content = "dynamic"
	};

	#region 引用值

	public static SystemValueTypeSegment Null = new()
	{
		Content = "null"
	};

	public static SystemValueTypeSegment Typeof = new()
	{
		Content = "typeof"
	};

	public static SystemValueTypeSegment Sizeof = new()
	{
		Content = "sizeof"
	};

	public static SystemValueTypeSegment Default = new()
	{
		Content = "default"
	};

	public static SystemValueTypeSegment This = new()
	{
		Content = "this"
	};

	public static SystemValueTypeSegment Base = new()
	{
		Content = "base"
	};

	#endregion

	#region 参数类型限定

	public static SystemValueTypeSegment Params = new()
	{
		Content = "params"
	};

	public static SystemValueTypeSegment Ref = new()
	{
		Content = "ref"
	};

	public static SystemValueTypeSegment Out = new()
	{
		Content = "out"
	};

	#endregion

	#region 参数类型判断/确定/使用, 但是这个in 要考虑是不是要跟 for之类的单独作为什么逻辑

	//TODO: 这个in 要考虑是不是要跟 for之类的单独作为什么逻辑

	public static SystemValueTypeSegment Is = new()
	{
		Content = "is"
	};

	public static SystemValueTypeSegment As = new()
	{
		Content = "as"
	};

	public static SystemValueTypeSegment In = new()
	{
		Content = "in"
	};

	#endregion
}