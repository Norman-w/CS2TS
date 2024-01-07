// using System.Reflection;
// using System.Text.Json.Nodes;
//
// namespace CS2TS;
//
// public static class Utils
// {
// 	/// <summary>
// 	/// 将static类序列化成一个json对象,static类里面如果还有static类,也会被序列化(使用递归)
// 	/// </summary>
// 	/// <param name="type"></param>
// 	/// <returns></returns>
// 	public static JsonNode StaticClassToJsonObject(Type type)
// 	{
// 		var result = new JsonNode();
// 		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
// 		foreach (var property in properties)
// 		{
// 			var value = property.GetValue(null);
// 			if (value == null)
// 			{
// 				result.Add(property.Name, new JsonNullNode());
// 				continue;
// 			}
// 			var valueType = value.GetType();
// 			if (valueType.IsPrimitive || valueType == typeof(string))
// 			{
// 				result.Add(property.Name, new JsonStringNode($"{value}"));
// 				continue;
// 			}
// 			if (valueType.IsEnum)
// 			{
// 				result.Add(property.Name, new JsonStringNode($"{value}"));
// 				continue;
// 			}
// 			if (valueType.IsClass)
// 			{
// 				result.Add(property.Name, StaticClassToJsonObject(valueType));
// 				continue;
// 			}
// 			throw new Exception($"未知的类型:{valueType.FullName}");
// 		}
// 		return result;
// 	}
// }

