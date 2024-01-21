using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service.Valve;

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