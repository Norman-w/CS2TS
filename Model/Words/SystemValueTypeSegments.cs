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
}