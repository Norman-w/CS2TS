using System.Reflection;

namespace CS2TS;

public static class RequestParser
{
	/// <summary>
	///     请求名称到请求类型的字典,通过
	///     <see>
	///         <cref>BaseRequest.GetApiName</cref>
	///     </see>
	///     来获取请求名称
	///     比如想获取字符串中保存的api name 为 process.list 获取到 ProcessListRequest 类型
	/// </summary>
	public static readonly Dictionary<string, Type> RequestNameToTypeDic = new();

	//初始化时,异步的执行InitDic
	static RequestParser()
	{
		Task.Run(InitDic);
	}

	//初始化时,使用反射来获取所有继承自BaseRequest的类,并将其添加到RequestNameToType中
	private static void InitDic()
	{
		//记录初始化列表时间
		var startTime = DateTime.Now;
		var assembly = Assembly.GetExecutingAssembly();
		// var types = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(BaseRequest)));
		// 获取所有继承自BaseRequest的类(BaseRequest<T>)
		var types = assembly.GetTypes()
			.Where(
				x => x.BaseType is { IsGenericType: true }
				     && x.BaseType.GetGenericTypeDefinition() == typeof(BaseRequest<>)
			);
		foreach (var type in types)
		{
			//实例化一个对象,并通过调用它的GetApiName方法来获取请求的名称
			//  实例化对象
			object? obj;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				Console.WriteLine("在RequestParser初始化获取所有Api的名称和类型字典时,实例化对象失败:" + e.Message);
				throw;
			}

			//  获取GetApiName方法
			var method = type.GetMethod("GetApiName");
			//检验是否有GetApiName方法
			if (method == null)
			{
				var msg = "在RequestParser初始化获取所有Api的名称和类型字典时,获取GetApiName方法失败:" + type.FullName;
				Console.WriteLine(msg);
				throw new Exception(msg);
			}

			//  调用GetApiName方法
			var requestName = method.Invoke(obj, null);
			//  检查获取到的名称是否为空
			if (requestName == null)
			{
				var msg = "在RequestParser初始化获取所有Api的名称和类型字典时,获取到的名称为空:" + type.FullName;
				Console.WriteLine(msg);
				throw new Exception(msg);
			}

			var requestNameStr = $"{requestName}";
			if (RequestNameToTypeDic.ContainsKey(requestNameStr))
			{
				var msg = "在RequestParser初始化获取所有Api的名称和类型字典时,获取到的名称重复:" + type.FullName;
				Console.WriteLine(msg);
				throw new Exception(msg);
			}

			RequestNameToTypeDic.Add($"{requestName}", type);
		}

		//记录结束初始化列表时间
		var endTime = DateTime.Now;
		//高亮
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"初始化RequestNameToTypeDic完成,耗时:{(endTime - startTime).TotalMilliseconds}ms");
		Console.ResetColor();
	}
}