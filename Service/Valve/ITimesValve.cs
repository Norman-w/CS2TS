using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service.Valve;

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