/*

 名称链,比如 namespace A.B.C; 这个名称链就是A.B.C
 用于定义:
  用于定义命名空间的候,表示命名空间的具体层级的名称.
 用于使用:
  也可以在使用的时候,表示一个类型的全名,比如 A.B.C.D

*/

namespace CS2TS.Model;

/// <summary>
///     名称链,比如 namespace A.B.C; 这个名称链就是A.B.C
///     使用ToString方法可以得到完整的名称链(C#代码中的表示方式)
/// </summary>
public class NameChain
{
	/// <summary>
	///     名称链,比如 namespace A.B.C; 这个名称链就是A.B.C
	/// </summary>
	public List<string> Names { get; set; } = new();

	/// <summary>
	///     获得完整的名称链(C#代码中的表示方式)如 namespace A.B.C; 这个名称链就是A.B.C
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return string.Join(SymbolSegments.DotSymbol.Content, Names);
	}

	/// <summary>
	///     从字符串中创建一个名称链,比如 输入参数是A.B.C,那么返回的名称链就是A->B->C
	/// </summary>
	/// <param name="nameChain"></param>
	/// <returns></returns>
	public static NameChain FromString(string nameChain)
	{
		var names = nameChain.Split(SymbolSegments.DotSymbol.Content);
		return new NameChain { Names = names.ToList() };
	}
}