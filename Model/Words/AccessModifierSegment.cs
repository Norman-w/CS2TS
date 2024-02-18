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

/// <summary>
///     访问修饰符,例如public,private,protected,internal
/// </summary>
public class AccessModifierSegment : ModifierSegment
{
	/// <summary>
	///     将访问修饰符转换为枚举
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public AccessModifierPermissionEnum ToAccessModifierPermissionEnum()
	{
		return Content switch
		{
			"public" => AccessModifierPermissionEnum.Public,
			"private" => AccessModifierPermissionEnum.Private,
			"protected" => AccessModifierPermissionEnum.Protected,
			"internal" => AccessModifierPermissionEnum.Internal,
			_ => throw new Exception($"不支持的访问修饰符{Content}")
		};
	}
}