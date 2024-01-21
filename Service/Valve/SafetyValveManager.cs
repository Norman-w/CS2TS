/*


 安全阀,用于限流,防止恶意攻击
 当被视为同一个来源的端请求过多的错误subProtocol时,将会被视为恶意攻击,并被拒绝连接.
 或当有效来源的多个无效请求过多时,限流多久以后才能再次请求.
 没有错误但是同一时间请求多少时,执行限流,再超过另外多少时,拒绝连接或者踢出连接.


*/


using System.Net.WebSockets;
using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service.Valve;

/// <summary>
///     安全阀,用于限流,防止恶意攻击等
/// </summary>
public class SafetyValveManager
{
	private SafetyValveConfig? _config;

	public SafetyValveConfig Config => _config ??= LoadConfig();

	private SafetyValveConfig LoadConfig()
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
						new TotalValueExceedValve<double> { Threshold = 100 },
						new InTimeExceedValve<double> { TimeSpan = TimeSpan.FromSeconds(1), Threshold = 100 }
					}
				},
				{
					"ShowSegments", new List<IValve>
					{
						new TotalValueExceedValve<double> { Threshold = 100 },
						new InTimeExceedValve<double> { TimeSpan = TimeSpan.FromSeconds(1), Threshold = 100 }
					}
				},
				{
					"AddSegments", new List<IValve>
					{
						new TotalValueExceedValve<double> { Threshold = 100 },
						new InTimeExceedValve<double> { TimeSpan = TimeSpan.FromSeconds(1), Threshold = 100 }
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
	/// <param name="systemErrorMessage">当发生系统错误的时候的系统错误消息.</param>
	/// <param name="noticeMessage">当触发了阀门的时候给用户的通知消息</param>
	/// <returns>代表该方法调用是否成功</returns>
	public bool NoticeInvalidRequest(WebSocketContext webSocketContext, out string systemErrorMessage,
		out string noticeMessage)
	{
		// var msg = "安全阀消息NoticeInvalidRequest,apiName:" + (apiName ?? "空");
		//找到方法对应的阀门集合
		var matchedValves = Config.FunctionValves.TryGetValue(nameof(NoticeInvalidRequest), out var valves)
			? valves
			: new List<IValve>();
		//依次报告给阀门,并且判断是否应该触发
		foreach (var matchedValve in matchedValves)
			if (matchedValve is InTimeExceedTimesValve inTimeExceedTimesValve)
			{
				var result = inTimeExceedTimesValve.TryToTrigger();
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