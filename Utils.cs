using System.Reflection;
using Newtonsoft.Json.Linq;

namespace CS2TS;

public static class Utils
{
	/// <summary>
	///     序列化一个静态类
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static string SerializeStaticClass(Type type)
	{
		var jObject = new JObject();
		_toJson(jObject, type);
		var ret = new JObject { { type.Name, jObject } };
		var json = ret.ToString();
		// Console.WriteLine(json);
		return json;
	}

	/// <summary>
	///     递归方法,将一个静态类序列化成一个JObject对象
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="type"></param>
	private static void _toJson(JObject parent, Type type)
	{
		var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
		foreach (var memberInfo in members)
		{
			var memberName = memberInfo.Name;
			var memberType = memberInfo.MemberType;
			//添加内容,继续递归
			if (memberType == MemberTypes.NestedType)
			{
				var jObject = new JObject();
				parent.Add(memberName, jObject);
				var memberTypeInfo = memberInfo.ReflectedType?.GetNestedType(memberName) ??
				                     throw new Exception($"获取嵌套类型失败:{memberName}");
				_toJson(jObject, memberTypeInfo);
				continue;
			}
			//如果是property或者是field,则使用反射获取值

			if (memberType is MemberTypes.Property or MemberTypes.Field)
			{
				var value = memberInfo switch
				{
					PropertyInfo propertyInfo => propertyInfo.GetValue(null),
					FieldInfo fieldInfo => fieldInfo.GetValue(null),
					_ => throw new Exception($"未知的成员类型:{memberType}")
				};
				//如果值是null,则直接添加null
				if (value == null)
				{
					parent.Add(memberName, null);
					continue;
				}

				//如果值是基础类型,则直接添加
				if (value.GetType().IsPrimitive || value is string)
				{
					parent.Add(memberName, value.ToString());
					continue;
				}

				//如果值是枚举类型,则直接添加
				if (value.GetType().IsEnum)
				{
					parent.Add(memberName, value.ToString());
					continue;
				}

				//如果值是class类型,则递归
				if (value.GetType().IsClass)
				{
					var jObject = new JObject();
					parent.Add(memberName, jObject);
					_toJson(jObject, value.GetType());
					continue;
				}

				throw new Exception($"未知的类型:{value.GetType().FullName}");
			}
		}
	}
}