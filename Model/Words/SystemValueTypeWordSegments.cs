/*



 一些系统内置类型的关键字的片段

 //TODO 这个还需要整理一下,有些类型定义以外的也放进来了



*/


namespace CS2TS.Model.Words;

public class SystemValueTypeWordSegments
{
	/// <summary>
	///     系统内置的string类型
	/// </summary>
	public static SystemValueTypeWordSegment String = new()
	{
		Content = "string"
	};

	/// <summary>
	///     系统内置的int类型
	/// </summary>
	public static SystemValueTypeWordSegment Int = new()
	{
		Content = "int"
	};

	/// <summary>
	///     系统内置的long类型
	/// </summary>
	public static SystemValueTypeWordSegment Long = new()
	{
		Content = "long"
	};

	public static SystemValueTypeWordSegment Uint = new()
	{
		Content = "uint"
	};

	public static SystemValueTypeWordSegment Ulong = new()
	{
		Content = "ulong"
	};

	public static SystemValueTypeWordSegment Short = new()
	{
		Content = "short"
	};

	public static SystemValueTypeWordSegment Ushort = new()
	{
		Content = "ushort"
	};

	public static SystemValueTypeWordSegment Byte = new()
	{
		Content = "byte"
	};

	public static SystemValueTypeWordSegment Sbyte = new()
	{
		Content = "sbyte"
	};

	public static SystemValueTypeWordSegment Char = new()
	{
		Content = "char"
	};

	public static SystemValueTypeWordSegment Float = new()
	{
		Content = "float"
	};

	public static SystemValueTypeWordSegment Double = new()
	{
		Content = "double"
	};

	public static SystemValueTypeWordSegment Decimal = new()
	{
		Content = "decimal"
	};

	public static SystemValueTypeWordSegment Bool = new()
	{
		Content = "bool"
	};

	public static SystemValueTypeWordSegment Object = new()
	{
		Content = "object"
	};

	public static SystemValueTypeWordSegment Void = new()
	{
		Content = "void"
	};

	public static SystemValueTypeWordSegment Dynamic = new()
	{
		Content = "dynamic"
	};

	#region 引用值

	public static SystemValueTypeWordSegment Null = new()
	{
		Content = "null"
	};

	public static SystemValueTypeWordSegment Typeof = new()
	{
		Content = "typeof"
	};

	public static SystemValueTypeWordSegment Sizeof = new()
	{
		Content = "sizeof"
	};

	public static SystemValueTypeWordSegment Default = new()
	{
		Content = "default"
	};

	public static SystemValueTypeWordSegment This = new()
	{
		Content = "this"
	};

	public static SystemValueTypeWordSegment Base = new()
	{
		Content = "base"
	};

	#endregion

	#region 参数类型限定

	public static SystemValueTypeWordSegment Params = new()
	{
		Content = "params"
	};

	public static SystemValueTypeWordSegment Ref = new()
	{
		Content = "ref"
	};

	public static SystemValueTypeWordSegment Out = new()
	{
		Content = "out"
	};

	#endregion

	#region 参数类型判断/确定/使用, 但是这个in 要考虑是不是要跟 for之类的单独作为什么逻辑

	//TODO: 这个in 要考虑是不是要跟 for之类的单独作为什么逻辑

	public static SystemValueTypeWordSegment Is = new()
	{
		Content = "is"
	};

	public static SystemValueTypeWordSegment As = new()
	{
		Content = "as"
	};

	public static SystemValueTypeWordSegment In = new()
	{
		Content = "in"
	};

	#endregion
}