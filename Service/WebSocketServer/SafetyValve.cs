/*


 安全阀,用于限流,防止恶意攻击
 当被视为同一个来源的端请求过多的错误subProtocol时,将会被视为恶意攻击,并被拒绝连接.
 或当有效来源的多个无效请求过多时,限流多久以后才能再次请求.
 没有错误但是同一时间请求多少时,执行限流,再超过另外多少时,拒绝连接或者踢出连接.


*/


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
	///     规定时间内上升幅度类型
	/// </summary>
	InTimeRise,

	/// <summary>
	///     规定时间内下降幅度类型
	/// </summary>
	InTimeFall
}

/// <summary>
///     基础限流阀类
/// </summary>
public interface IValve
{
	/// <summary>
	///     阀门Id
	/// </summary>
	public Guid Id => Guid.NewGuid();

	/// <summary>
	///     阀门类型
	/// </summary>
	public ValveType ValveType { get; }
}

/// <summary>
///     值阀
/// </summary>
public interface IValueValve : IValve
{
	public double Value { get; set; }
}

/// <summary>
///     时间阀(包含值)
/// </summary>
public interface IValueTimeSpanValve : IValueValve
{
	public TimeSpan TimeSpan { get; set; }
}

/// <summary>
///     总值超过多少阀
/// </summary>
public class TotalValueExceedValve : IValueValve
{
	public ValveType ValveType => ValveType.TotalValueExceed;
	public double Value { get; set; }
}

/// <summary>
///     总值低于多少阀
/// </summary>
public class TotalValueLowerThanValve : IValueValve
{
	public ValveType ValveType => ValveType.TotalValueLowerThan;
	public double Value { get; set; }
}

/// <summary>
///     规定时间内超过多少阀
/// </summary>
public class InTimeExceedValve : IValueTimeSpanValve
{
	public ValveType ValveType => ValveType.InTimeExceed;
	public double Value { get; set; }
	public TimeSpan TimeSpan { get; set; }
}

/// <summary>
///     规定时间内低于多少阀
/// </summary>
public class InTimeLowerThanValve : IValueTimeSpanValve
{
	public ValveType ValveType => ValveType.InTimeLowerThan;
	public double Value { get; set; }
	public TimeSpan TimeSpan { get; set; }
}

/// <summary>
///     规定时间内上升幅度阀
/// </summary>
public class InTimeRiseValve : IValueTimeSpanValve
{
	public ValveType ValveType => ValveType.InTimeRise;
	public double Value { get; set; }
	public TimeSpan TimeSpan { get; set; }
}

/// <summary>
///     规定时间内下降幅度阀
/// </summary>
public class InTimeFallValve : IValueTimeSpanValve
{
	public ValveType ValveType => ValveType.InTimeFall;
	public double Value { get; set; }
	public TimeSpan TimeSpan { get; set; }
}

public class SafetyValveConfig
{
	/// <summary>
	///     阀门列表
	/// </summary>
	private List<IValve> Valves { get; set; } = new();
}

/// <summary>
///     安全阀,用于限流,防止恶意攻击等
/// </summary>
public static class SafetyValveManager
{
	/// <summary>
	///     报告给安全阀,某个客户端的某个api请求是无效的
	///     发生错误时(没有正确处理),则返回false,并且返回错误信息
	/// </summary>
	/// <param name="client"></param>
	/// <param name="apiName"></param>
	/// <param name="errorMessage"></param>
	/// <returns>代表该方法调用是否成功</returns>
	public static bool NoticeInvalidRequest(Client client, string? apiName, out string errorMessage)
	{
		// client.WebSocketConnection.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "无效的请求,没有匹配到apiName",
		// 	CancellationToken.None).Wait();
		//TODO:实现
		errorMessage = "";
		return true;
	}
}