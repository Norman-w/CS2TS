/*


 安全阀,用于限流,防止恶意攻击
 当被视为同一个来源的端请求过多的错误subProtocol时,将会被视为恶意攻击,并被拒绝连接.
 或当有效来源的多个无效请求过多时,限流多久以后才能再次请求.
 没有错误但是同一时间请求多少时,执行限流,再超过另外多少时,拒绝连接或者踢出连接.


*/


using System.Net.WebSockets;

namespace CS2TS.Service.WebSocketServer;

/// <summary>
///     限制器类型
/// </summary>
public enum ValveType
{
	/*
	 包含总值超过多少类型,总值低于多少类型,规定时间内超过多少类型,规定时间内低于多少类型
	 规定时间内上升幅度类型,规定时间内下降幅度类型等
	 */
	/// <summary>
	///     总值超过多少类型
	/// </summary>
	TotalValueExceed,

	/// <summary>
	///     总值低于多少类型
	/// </summary>
	TotalValueLowerThan,

	/// <summary>
	///     规定时间内超过多少类型
	/// </summary>
	InTimeExceed,

	/// <summary>
	///     规定时间内低于多少类型
	/// </summary>
	InTimeLowerThan,

	/// <summary>
	///     规定时间内超过多少次类型
	/// </summary>
	InTimeExceedTimes,

	/// <summary>
	///     规定时间内低于多少次类型
	/// </summary>
	InTimeLowerThanTimes,

	/// <summary>
	///     规定时间内上升幅度类型
	/// </summary>
	InTimeRise,

	/// <summary>
	///     规定时间内下降幅度类型
	/// </summary>
	InTimeFall
}

/// <summary>
///     值阀
/// </summary>
public interface IValueValve : IValve
{
	/// <summary>
	///     值,当前值
	/// </summary>
	public double Value { get; set; }

	//阈值
	public double Threshold { get; set; }
}

/// <summary>
///     次数阀
/// </summary>
public interface ITimesValve : IValve
{
	/// <summary>
	///     次数阈值
	/// </summary>
	public int Threshold { get; set; }
}

/// <summary>
///     时间阀
/// </summary>
public interface ITimeSpanValve : IValve
{
	/// <summary>
	///     时间间隔
	/// </summary>
	public TimeSpan TimeSpan { get; set; }
}

public class NoticeValveResult
{
	public string? ErrorMessage { get; set; }
	public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);

	public OverloadHandleType? OverloadHandleType { get; set; }
}

/// <summary>
///     总值超过多少阀
/// </summary>
public class TotalValueExceedValve : IValueValve
{
	public Guid Id => Guid.NewGuid();
	public string Name => "总值超过多少阀";
	public DateTime? LastTriggerTime { get; set; }
	public int TriggerCount { get; set; }
	public OverloadHandleType OverloadHandleType { get; set; }
	public TimeSpan? SelfResetTimeSpan { get; set; }
	public int SelfResetCount { get; set; }

	public void Reset()
	{
		TriggerCount = 0;
		LastTriggerTime = DateTime.MinValue;
	}

	public double Value { get; set; }
	public double Threshold { get; set; }

	public NoticeValveResult TryToTrigger<T>(Func<T, T>? recordValueCallback)
	{
		var result = new NoticeValveResult();
		try
		{
			if (recordValueCallback != null) Value = (double)(object)recordValueCallback((T)(object)Value);
		}
		catch (Exception e)
		{
			Console.WriteLine("SafetyValve:TotalValueExceedValve.Record()发生错误");
			result.ErrorMessage = e.Message;
			return result;
		}

		if (Value <= Threshold) return result;
		//触发了
		TriggerCount++;
		LastTriggerTime = DateTime.Now;
		result.OverloadHandleType = OverloadHandleType;
		return result;
	}
}

/// <summary>
///     规定时间内超过多少值阀
/// </summary>
public class InTimeExceedValve : ITimeSpanValve, IValueValve
{
	public Guid Id => Guid.NewGuid();
	public string Name => "规定时间内超过多少值阀";
	public DateTime? LastTriggerTime { get; set; }
	public int TriggerCount { get; set; }
	public OverloadHandleType OverloadHandleType { get; set; }
	public TimeSpan? SelfResetTimeSpan { get; set; }
	public int SelfResetCount { get; set; }

	public NoticeValveResult TryToTrigger<T>(Func<T, T>? recordValueCallback)
	{
		var result = new NoticeValveResult();
		try
		{
			Value = (double)(object)recordValueCallback((T)(object)Value);
		}
		catch (Exception e)
		{
			Console.WriteLine("SafetyValve:InTimeExceedValve.Record()发生错误");
			result.ErrorMessage = e.Message;
			return result;
		}

		if (Value <= Threshold) return result;
		//触发了
		TriggerCount++;
		LastTriggerTime = DateTime.Now;
		result.OverloadHandleType = OverloadHandleType;
		return result;
	}

	public void Reset()
	{
		TriggerCount = 0;
		LastTriggerTime = DateTime.MinValue;
	}

	public TimeSpan TimeSpan { get; set; }
	public double Value { get; set; }
	public double Threshold { get; set; }
}

/// <summary>
///     规定时间超过多少次阀
/// </summary>
public class InTimeExceedTimesValve : ITimeSpanValve, ITimesValve
{
	public Guid Id => Guid.NewGuid();
	public string Name => "规定时间超过多少次阀";
	public DateTime? LastTriggerTime { get; set; }
	public int TriggerCount { get; set; }
	public OverloadHandleType OverloadHandleType { get; set; }
	public TimeSpan? SelfResetTimeSpan { get; set; }
	public int SelfResetCount { get; set; }

	public void Reset()
	{
		TriggerCount = 0;
		LastTriggerTime = DateTime.MinValue;
	}

	public TimeSpan TimeSpan { get; set; }

	public NoticeValveResult TryToTrigger<T>(Func<T, T>? recordValueCallback)
	{
		var result = new NoticeValveResult();
		try
		{
			if (recordValueCallback != null)
				throw new Exception("InTimeExceedTimesValve不支持记录值");
		}
		catch (Exception e)
		{
			Console.WriteLine("SafetyValve:InTimeExceedTimesValve.Record()发生错误");
			result.ErrorMessage = e.Message;
			return result;
		}

		//触发了
		TriggerCount++;
		Console.WriteLine("SafetyValve:InTimeExceedTimesValve触发了,当前次数:" + TriggerCount + ",阈值:" + Threshold);
		LastTriggerTime = DateTime.Now;
		if (TriggerCount <= Threshold) return result;
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.WriteLine("SafetyValve:InTimeExceedTimesValve达到阈值,当前次数:" + TriggerCount + ",阈值:" + Threshold);
		Console.ResetColor();
		//如果触发了,则赋值OverloadHandleType,表示为触发了
		result.OverloadHandleType = OverloadHandleType;
		return result;
	}

	public int Threshold { get; set; }
}

public class SafetyValveConfig
{
	/// <summary>
	///     api对应的阀门
	/// </summary>
	public Dictionary<string, List<IValve>> ApiValves { get; set; } = new();

	/// <summary>
	///     在SafetyValveManager中的方法对应的阀门
	/// </summary>
	public Dictionary<string, List<IValve>> FunctionValves { get; set; } = new();
}

/// <summary>
///     过载后的处理方式(用flag来表示,可以组合) 包含限流(等待),拒绝连接,提出连接等
/// </summary>
[Flags]
public enum OverloadHandleType
{
	/// <summary>
	///     限流(等待)
	/// </summary>
	Overload = 0x1,

	/// <summary>
	///     拒绝连接
	/// </summary>
	Refuse = 0x2,

	/// <summary>
	///     提出连接
	/// </summary>
	KickOut = 0x4
}

/// <summary>
///     安全阀,用于限流,防止恶意攻击等
/// </summary>
public static class SafetyValveManager
{
	private static SafetyValveConfig? _config;

	public static SafetyValveConfig Config => _config ??= LoadConfig();

	private static SafetyValveConfig LoadConfig()
	{
		if (_config != null)
			return _config;
		//TODO:实现,当前是测试用的
		_config = new SafetyValveConfig
		{
			ApiValves = new Dictionary<string, List<IValve>>
			{
				{
					"ShowCsCodeString", new List<IValve>
					{
						new TotalValueExceedValve { Threshold = 100 },
						new InTimeExceedValve { TimeSpan = TimeSpan.FromSeconds(1), Threshold = 100 }
					}
				},
				{
					"ShowSegments", new List<IValve>
					{
						new TotalValueExceedValve { Threshold = 100 },
						new InTimeExceedValve { TimeSpan = TimeSpan.FromSeconds(1), Threshold = 100 }
					}
				},
				{
					"AddSegments", new List<IValve>
					{
						new TotalValueExceedValve { Threshold = 100 },
						new InTimeExceedValve { TimeSpan = TimeSpan.FromSeconds(1), Threshold = 100 }
					}
				}
			},
			FunctionValves = new Dictionary<string, List<IValve>>
			{
				{
					"NoticeInvalidRequest", new List<IValve>
					{
						new InTimeExceedTimesValve
						{
							TimeSpan = TimeSpan.FromSeconds(60), Threshold = 5,
							OverloadHandleType = OverloadHandleType.Refuse | OverloadHandleType.KickOut
						}
					}
				}
			}
		};
		return _config;
	}

	/// <summary>
	///     报告给安全阀,某个客户端的某个api请求是无效的
	///     发生错误时(没有正确处理),则返回false,并且返回错误信息
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <param name="apiName"></param>
	/// <param name="systemErrorMessage">当发生系统错误的时候的系统错误消息.</param>
	/// <param name="noticeMessage">当触发了阀门的时候给用户的通知消息</param>
	/// <returns>代表该方法调用是否成功</returns>
	public static bool NoticeInvalidRequest(WebSocketContext webSocketContext, string? apiName,
		out string systemErrorMessage,
		out string noticeMessage)
	{
		// var msg = "安全阀消息NoticeInvalidRequest,apiName:" + (apiName ?? "空");
		apiName ??= "未知的api";
		//找到方法对应的阀门集合
		var matchedValves = Config.FunctionValves.TryGetValue(nameof(NoticeInvalidRequest), out var valves)
			? valves
			: new List<IValve>();
		//依次报告给阀门,并且判断是否应该触发
		foreach (var matchedValve in matchedValves)
			if (matchedValve is InTimeExceedTimesValve inTimeExceedTimesValve)
			{
				var result = inTimeExceedTimesValve.TryToTrigger<int>(null);
				if (result.IsError)
				{
					systemErrorMessage = result.ErrorMessage!;
					noticeMessage = "发生系统错误,请联系管理员,错误信息:" + systemErrorMessage;
					return false;
				}

				if (result.OverloadHandleType == null) continue;
				if (result.OverloadHandleType.Value.HasFlag(OverloadHandleType.Refuse))
				{
					systemErrorMessage = "";
					noticeMessage = "请求过于频繁,已拒绝连接";
					matchedValve.Reset();
				}

				if (result.OverloadHandleType.Value.HasFlag(OverloadHandleType.KickOut))
				{
					systemErrorMessage = "";
					noticeMessage = "请求过于频繁,已踢出连接";
					matchedValve.Reset();
					try
					{
						webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "请求过于频繁,已踢出连接",
							CancellationToken.None).Wait();
					}
					catch (Exception e)
					{
						Console.WriteLine("SafetyValve:NoticeInvalidRequest()发生错误:" + e.Message);
					}
				}
			}
			else
			{
				//这个阀门不需要在这里处理
				Console.WriteLine("SafetyValve:NoticeInvalidRequest()发现不支持的阀门类型:" + matchedValve.GetType().Name);
			}

		systemErrorMessage = "";
		noticeMessage = "";
		return true;
	}
}